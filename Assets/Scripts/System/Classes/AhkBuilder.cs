using UnityEngine;
using System.IO;
using System.Collections;
using SimpleJSON;

class AhkBuilder {

    private string template;
    private string compiledAHK;
    private Game game;

    public AhkBuilder(Game game) {
        this.game = game;
        chooseTemplate();
    }

    public void compile() {
        compiledAHK = template;

        if (game.gameType == Game.GameType.PICO8) {
            compiledAHK = Resources.Load<TextAsset>("AHK_templates/Pico8GameTemplate").text;
            compiledAHK = compiledAHK.Replace("{GAME_PATH}", GM.Instance.options.dataPath + "/Options/Pico8/nw.exe");
        } else {
            compiledAHK = compiledAHK.Replace("{GAME_PATH}", game.executable);
            compiledAHK = compiledAHK.Replace("{GAME_NAME}", game.name);
        }

        compiledAHK = compiledAHK.Replace("{GAME_FILE}", Path.GetFileName(game.executable));
        compiledAHK = compiledAHK.Replace("{DEBUG_OUTPUT}", "true"); // TODO make this configurable
        compiledAHK = compiledAHK.Replace("{IDLE_TIME}", "" + GM.Instance.options.runnerSecondsIdle);
        compiledAHK = compiledAHK.Replace("{IDLE_INITIAL}", "" + GM.Instance.options.runnerSecondsIdleInitial);
        compiledAHK = compiledAHK.Replace("{ESC_HOLD}", "" + GM.Instance.options.runnerSecondsESCHeld);

        compiledAHK = insertKeyMapping(compiledAHK);
    }

    public void write() {
        game.WriteStringToFile(compiledAHK, "RunGame.ahk");
    }




    private void chooseTemplate() {
        switch (game.gameType) {

            case Game.GameType.PICO8:
                string newJS = Resources.Load<TextAsset>("Pico8Launcher").text;
                newJS = newJS.Replace("{{{PATH_TO_HTML}}}", game.executable.Replace("\\", "\\\\"));
                game.WriteStringToFile(newJS, "Pico8Launcher.js");
                break;

            case Game.GameType.FLASH:
                template = Resources.Load<TextAsset>("AHK_templates/FlashGameTemplate").text;
                break;

            default:
                template = Resources.Load<TextAsset>("AHK_templates/ExeGameTemplate").text;
                break;
        }
    }

    private string insertKeyMapping(string ahkFile) {
        string keymap = "";
        JSONNode parsedBindings = getKeyBindings();
        ArrayList gameKeys = allGameKeys(parsedBindings);

        // Write keys for players we have.
        for(int pNum = 1; pNum <= game.savedMetadata["max_players"].AsInt; pNum++) {
            JSONNode playerKeys;

            try {
                playerKeys = parsedBindings[pNum.ToString()];
            } catch (System.NullReferenceException) {
                break;
            }

            foreach(string control in KeyBindings.CONTROLS) {
                KeyCode key = GM.Instance.options.keys.GetKey(pNum, control);
                string launcherKey = GM.Instance.options.keyTranslator.toAHK(key);
                string gameKey = playerKeys[control];

                if (gameKey == null) { // this shouldn't happen, but just in case.
                    gameKey = "return";
                }

                keymap += (launcherKey + "::" + gameKey + "\n");
            }
        }

        // Write keys for players we don't have (e.g., players 3 & 4 on 2-player game)
        for(int pNum = game.savedMetadata["max_players"].AsInt + 1; pNum <= 4; pNum++) {
            foreach(string control in KeyBindings.CONTROLS) {
                KeyCode key = GM.Instance.options.keys.GetKey(pNum, control);
                string launcherKey = GM.Instance.options.keyTranslator.toAHK(key);

                if (!gameKeys.Contains(launcherKey))
                    keymap += (launcherKey + "::return\n");
            }
        }

        return ahkFile.Replace("{KEYMAP}", keymap);
    }

    private ArrayList allGameKeys(JSONNode bindings) {
        ArrayList keys = new ArrayList();

        for(int pNum = 1; pNum <= 4; pNum++) {
            foreach(string control in KeyBindings.CONTROLS) {
                string key = bindings[pNum.ToString()][control];

                if (key != null)
                    keys.Add(key);
            }
        }

        return keys;
    }

    private JSONNode getKeyBindings() {
        string tmpl = game.savedMetadata["keys"]["template"];
        JSONNode bindings = null;

        string tmplFile = Path.Combine(GM.Instance.options.defaultOptionsPath, "keymap_templates.json");
        JSONNode bindingTemplates = GM.Instance.data.LoadJson(tmplFile);

        if (tmpl == null) {
            if (game.savedMetadata["keys"]["bindings"] == null) {
                GM.Instance.logger.Debug("No key binding info provided for " + game.name + ". Using defaults.");
                tmpl = "default";
            } else {
                tmpl = "custom";
            }
        }

        switch(tmpl) {
            case "custom":
                GM.Instance.logger.Debug("Loading custom key bindings for " + game.name);
                bindings = game.savedMetadata["keys"]["bindings"];
                break;

            case "default":
            case "legacy":
            case "flash":
            case "pico8":
                GM.Instance.logger.Debug("Loading " + tmpl + " bindings from tmplFile: " + tmplFile);
                bindings = bindingTemplates[tmpl];
                break;

            default:
                GM.Instance.logger.Error("Invalid key template type '" + tmpl + "' for " + game.name + ". (Using default.) Valid templates are 'default', 'legacy', 'pico8', 'custom'.");
                GM.Instance.logger.Debug("Loading " + tmpl + " bindings from tmplFile: " + tmplFile);
                bindings = bindingTemplates["default"];
                break;
        }

        // Remove controls for players that don't exist.
        for (int p = game.savedMetadata["max_players"].AsInt + 1; p <= 4; p++) {
            bindings.Remove(p.ToString());
        }

        return bindings;
    }
}