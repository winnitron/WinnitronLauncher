using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;

[System.Serializable]
public class KeyBindings {

    public static string[] CONTROLS = new string[6] {
        "up",
        "down",
        "left",
        "right",
        "button1",
        "button2"
    };

    private Dictionary<string, KeyCode>[] keymap = new Dictionary<string, KeyCode>[4];

    public KeyBindings() {
        // Just set up the defaults
        for (int i = 0; i < keymap.Length; i++) {
            keymap[i] = new Dictionary<string, KeyCode>();
        }

        SetKey(1, "up", KeyCode.UpArrow);
        SetKey(1, "down", KeyCode.DownArrow);
        SetKey(1, "left", KeyCode.LeftArrow);
        SetKey(1, "right", KeyCode.RightArrow);
        SetKey(1, "button1", KeyCode.Period);
        SetKey(1, "button2", KeyCode.Slash);

        SetKey(2, "up", KeyCode.W);
        SetKey(2, "down", KeyCode.S);
        SetKey(2, "left", KeyCode.A);
        SetKey(2, "right", KeyCode.D);
        SetKey(2, "button1", KeyCode.BackQuote);
        SetKey(2, "button2", KeyCode.Alpha1);

        SetKey(3, "up", KeyCode.I);
        SetKey(3, "down", KeyCode.K);
        SetKey(3, "left", KeyCode.J);
        SetKey(3, "right", KeyCode.L);
        SetKey(3, "button1", KeyCode.G);
        SetKey(3, "button2", KeyCode.H);

        SetKey(4, "up", KeyCode.Keypad8);
        SetKey(4, "down", KeyCode.Keypad5);
        SetKey(4, "left", KeyCode.Keypad4);
        SetKey(4, "right", KeyCode.Keypad6);
        SetKey(4, "button1", KeyCode.Keypad1);
        SetKey(4, "button2", KeyCode.Keypad2);
    }


    public void SetKey(int playerNum, string control, KeyCode key) {

        try {
            if (keymap[playerNum - 1][control] == key) {
                return; // nothing to do here.
            }

            if (allKeys().Contains(key)) {
                GM.logger.Error("Duplicate key found: " + key);
                GM.Oops(GM.Text("error", "invalidKeymap"), true);
            }
        } catch (KeyNotFoundException) {
            // NOP. This (only) gets thrown on initialization. We can ignore it.
        }

        keymap[playerNum - 1][control] = key;
    }

    public KeyCode GetKey(int playerNum, string control) {
        return keymap[playerNum - 1][control];
    }

    private List<KeyCode> allKeys() {
        List<KeyCode> keys = new List<KeyCode>();

        for (int p = 1; p <= 4; p++) {
            foreach (string control in CONTROLS) {
                keys.Add(GetKey(p, control));
            }
        }
        return keys;
    }

}

public class OptionsManager : MonoBehaviour {

    //Launcher Options
    public bool widescreen = true;
    public int launcherIdleTimeBeforeAttract;

    public bool initializing = true;

    public float tweenTime = 0.7f;

    //Default Folders
    public string dataPath;
    public string defaultOptionsPath;
    public string optionsPath;
    public string playlistsPath = "/Playlists";
    public string musicPath = "/Music";
    public string attractPath = "/Attract";

    //Utilities
    //public string legacyControlsPath = "/Util/WinnitronLegacy.exe";

    //Runner Settings
    public int runnerSecondsIdle = 10;
    public int runnerSecondsESCHeld = 3;
    public int runnerSecondsIdleInitial = 30;

    //Keys
    public KeyBindings keys;
    public KeyTranslator keyTranslator;

    //language
    public JSONNode language;

    //Sync Settings
    public JSONNode sync;

    public JSONNode logger;

    //Options JSON
    public JSONNode O;

    // Use this for initialization
    void Awake() {

        // we need to load default language here, so we can display errors
        language = GM.data.GetDefautLanguage();
        defaultOptionsPath = Path.Combine(Application.dataPath, "Options");
        optionsPath = defaultOptionsPath;
        keyTranslator = new KeyTranslator(defaultOptionsPath);

        GM.logger.Info("DEFAULT OPTIONS PATH:" + optionsPath);

        // Figure out where the Options are by reading the .json in Options file
        string userdataFile = Path.Combine(optionsPath, "winnitron_userdata_path.json");
        GM.logger.Info("reading userdata location from " + userdataFile);
        if (System.IO.File.Exists(userdataFile))
        {
            dataPath = GM.data.LoadJson(userdataFile)["userDataPath"];
        } else {
            GM.Oops(GM.Text("error", "cannotFindUserDataPathJson"), true);
        }

        GM.logger.Info("DATA PATH:" + dataPath);
        optionsPath = Path.Combine(dataPath, "Options");
        GM.logger.Info("CONFIGURED OPTIONS PATH:" + optionsPath);
        string optionsFile = Path.Combine(optionsPath, "winnitron_options.json");

        //Load that JSON
        GM.logger.Info("Loading options from " + optionsFile);

        if (System.IO.File.Exists(optionsFile))
        {
            O = GM.data.LoadJson(optionsFile);

            logger = O["logger"];
            switch(logger["level"]) {
                case "debug":
                    Logger.logLevel = Logger.LogLevels.Debug;
                    break;
                case "info":
                case null:
                    Logger.logLevel = Logger.LogLevels.Info;
                    break;
                case "warn":
                    Logger.logLevel = Logger.LogLevels.Warn;
                    break;
                case "error":
                    Logger.logLevel = Logger.LogLevels.Error;
                    break;
                default:
                    Logger.logLevel = Logger.LogLevels.Info;
                    GM.logger.Warn("Invalid log level '" + logger["level"] + "'. Setting to default 'info'. Valid values are 'debug', 'info', 'warn', 'error'.");
                    break;
            }

            SetKeys();

            //Launcher Settings
            GM.logger.Info(this, "OPTIONS:" + O.ToString());
            if (O["launcher"]["widescreen"] != null) widescreen = O["launcher"]["widescreen"].AsBool;
            if (O["launcher"]["idleTimeBeforeAttract"] != null) launcherIdleTimeBeforeAttract = O["launcher"]["idleTimeBeforeAttract"].AsInt;

            //Runner Settings
            if (O["runner"]["timeToHoldESCToQuit"] != null) runnerSecondsESCHeld = O["runner"]["timeToHoldESCToQuit"].AsInt;
            if (O["runner"]["idleTimeSeconds"] != null) runnerSecondsIdle = O["runner"]["idleTimeSeconds"].AsInt;
            if (O["runner"]["initialIdleTimeSeconds"] != null) runnerSecondsIdleInitial = O["runner"]["initialIdleTimeSeconds"].AsInt;


            playlistsPath = InitDataFolder("playlists");
            musicPath = InitDataFolder("music");
            attractPath = InitDataFolder("attract");

            //Load language file
            if (O["launcher"]["language"] != null)
            {
                string langFile = Path.Combine(optionsPath, "winnitron_text_" + O["launcher"]["language"] + ".json");
                GM.logger.Info(this, "LANGUAGE FILE: " + langFile);
                language = GM.data.LoadJson(langFile);
            }

            //Sync Options
            var mode = O["sync"]["type"].Value.ToLower();

            GM.logger.Info(this, "SYNCMODE " + mode);

            if (mode == "local" || mode == "localonly" || mode == "none")
                GM.sync.syncType = GameSync.SyncType.NONE;
            else if (mode == "daily")
                GM.sync.syncType = GameSync.SyncType.DAILY;
            else
                GM.sync.syncType = GameSync.SyncType.ALWAYS;

            GM.sync.timeToUpdate = new TimeSpan(O["sync"]["dailySyncTime"]["hour"].AsInt, O["sync"]["dailySyncTime"]["minute"].AsInt, 0);
            GM.logger.Info(this, "OPTIONS: Time to Update is " + GM.sync.timeToUpdate.ToString());

            GM.sync.syncOnStartup = O["sync"]["syncOnStartup"].AsBool;

            // Tell GM that Options is done with all the Init stuff
            initializing = false;
        }
        else
        {
            GM.Oops(GM.Text("error", "cannotFindJson"), true);
        }
    }

    public string GetText(string category, string text)
    {
        return language[category][text];
    }


    public JSONNode GetSyncSettings()
    {
        return O["sync"];
    }

    private string InitDataFolder(string jsonKey)
    {
        string path = O["folders"][jsonKey];
        string dirName = jsonKey[0].ToString().ToUpper() + jsonKey.Substring(1, jsonKey.Length - 1);

        if (path.ToString().Contains("default"))
            path = Path.Combine(dataPath, dirName);

        GM.logger.Info(this, "OPTIONS: " + dirName + " path is " + path);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private void SetKeys()
    {
        JSONNode[] players = new JSONNode[5];
        players[1] = O["keycodes"]["player1"];
        players[2] = O["keycodes"]["player2"];
        players[3] = O["keycodes"]["player3"];
        players[4] = O["keycodes"]["player4"];

        for(int pNum = 1; pNum <= 4; pNum++) {
            JSONNode player = players[pNum];

            foreach(string control in KeyBindings.CONTROLS) {
                if (player[control] != null) {
                    KeyCode customKey = keyTranslator.fromAHK(player[control]);
                    keys.SetKey(pNum, control, customKey);
                    GM.logger.Debug("CUSTOM KEY (player " + pNum + " " + control + "): " + customKey);
                }
            }
        }


    }
}
