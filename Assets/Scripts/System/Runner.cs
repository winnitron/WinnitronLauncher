using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {
    
    Jukebox jukebox;
    Process gameRunner;

    public int secondsToWaitForIdle;

    TextAsset tmp;
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

        tmp = Resources.Load("RunGameTemplate") as TextAsset;
        legacy = Resources.Load("WinnitronLegacy") as TextAsset;
    }


	//Run those games!

	public void Run(Game game) {

        if (System.IO.File.Exists(game.executable))
        {
            GM.dbug.Log(this, "Running Game " + game.name + " legacy: " + game.useLegacyControls);

            CreateAHKScript(game);
            StartCoroutine(RunProcess(gameRunner));
        }

        else
        {
            GM.Oops(GM.Text("error", "couldNotFindGame"));
        }
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
        GM.dbug.Log(this, "RUNNER: Creat script game " + game.executable);

        //Replace variables
        string newString = tmp.text.Replace("{GAME_PATH}", game.executable);
        newString = newString.Replace("{IDLE_TIME}", "" + secondsToWaitForIdle);

        //Replace Keymaps
        if (game.useLegacyControls) newString = newString.Replace("{KEYMAPS}", legacy.text);
        else newString = newString.Replace("{KEYMAPS}", "");

        //Delete old file and write to new one
        File.Delete(Application.dataPath + "/Options/RunGame.ahk");
        System.IO.File.WriteAllText(Application.dataPath + "/Options/RunGame.ahk", newString);
    }
}

