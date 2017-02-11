using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {
    
    Jukebox jukebox;
    Process gameRunner;

    string templatesFolder = "AHK_templates/";

    TextAsset ExeAHKTemplate;
    TextAsset Pico8AHKTemplate;
    TextAsset Pico8JS;

    TextAsset legacy; 

    void Awake()
    {
        if (!System.IO.File.Exists(Application.dataPath + "/Options/RunGame.exe"))
            GM.Oops(GM.Text("error", "noRunGameExe"), true);

        //Store the legacyController in the Awake
        gameRunner = new Process();
        gameRunner.StartInfo.FileName = Application.dataPath + "/Options/RunGame.exe";
        GM.dbug.Log(this, "gameRunner path " + gameRunner.StartInfo.FileName);

        if(gameRunner == null)
            GM.Oops(GM.Text("error", "noLegacyControlExe"));

        ExeAHKTemplate = Resources.Load(templatesFolder + "ExeGameTemplate") as TextAsset;
        Pico8AHKTemplate = Resources.Load(templatesFolder + "Pico8GameTemplate") as TextAsset;
        Pico8JS = Resources.Load("Pico8Launcher") as TextAsset;
        legacy = Resources.Load(templatesFolder + "WinnitronLegacy") as TextAsset;
    }


	//Run those games!

	public void Run(Game game) {

        GM.dbug.Log(this, "Running Game " + game.name + " legacy: " + game.useLegacyControls);

        CreateAHKScript(game);
        StartCoroutine(RunProcess(gameRunner));
	}

	IEnumerator RunProcess(Process process){
        
		if (jukebox) jukebox.Stop();

		GM.state.ChangeState(StateManager.WorldState.Idle);
		Screen.fullScreen = false;

		yield return new WaitForSeconds(1.0f);

        GM.dbug.Log(this, "RUNNER: Running game " + process.StartInfo.FileName);

        process.Start();
        process.WaitForExit();

        GM.dbug.Log(this, "RUNNER: Finished running game " + process.StartInfo.FileName);

        GM.Restart();
	}

    private void CreateAHKScript(Game game)
    {
        GM.dbug.Log(this, "RUNNER: Create script game " + game.executable);

        string newAHKfile = "";

        switch (game.gameType)
        {
            case Game.GameType.EXE:
                newAHKfile = ExeAHKTemplate.text.Replace("{GAME_PATH}", game.executable);
                
                //Replace Keymaps
                if (game.useLegacyControls) newAHKfile = newAHKfile.Replace("{KEYMAPS}", legacy.text);
                else newAHKfile = newAHKfile.Replace("{KEYMAPS}", "");

                break;

            case Game.GameType.PICO8:
                CreatePico8LauncherJS(game);
                newAHKfile = Pico8AHKTemplate.text.Replace("{GAME_PATH}", GM.options.dataPath + "/Options/Pico8/nw.exe");
                break;
        }

        //Replace variables
        newAHKfile = newAHKfile.Replace("{IDLE_TIME}", "" + GM.options.runnerSecondsIdle);
        newAHKfile = newAHKfile.Replace("{IDLE_INITIAL}", "" + GM.options.runnerSecondsIdleInitial);
        newAHKfile = newAHKfile.Replace("{ESC_HOLD}", "" + GM.options.runnerSecondsESCHeld);

        //Delete old file and write to new one
        File.Delete(Application.dataPath + "/Options/RunGame.ahk");
        System.IO.File.WriteAllText(Application.dataPath + "/Options/RunGame.ahk", newAHKfile);
    }

    private void CreatePico8LauncherJS(Game game)
    {
        string newJS = "";

        newJS = Pico8JS.text.Replace("{{{PATH_TO_HTML}}}", game.executable.Replace("\\", "\\\\"));

        //Delete old file and write to new one
        File.Delete(GM.options.dataPath + "/Options/Pico8/Pico8Launcher.js");
        System.IO.File.WriteAllText(GM.options.dataPath + "/Options/Pico8/Pico8Launcher.js", newJS);
    }
}

