using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {
    
    Process gameRunner;

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
    }


	

	public void Run(Game game) {

        GM.dbug.Log(this, "Running Game " + game.name);

        LoadAHKScript(game);
        game.PreRun();
        StartCoroutine(RunProcess(gameRunner));
	}

	IEnumerator RunProcess(Process process){
        
		if (GM.jukebox) GM.jukebox.Stop();

		GM.state.ChangeState(StateManager.WorldState.Idle);
		Screen.fullScreen = false;

		yield return new WaitForSeconds(1.0f);

        GM.dbug.Log(this, "RUNNER: Running game " + process.StartInfo.FileName);

        process.Start();
        process.WaitForExit();

        GM.dbug.Log(this, "RUNNER: Finished running game " + process.StartInfo.FileName);

        GM.Restart();
	}

    /// <summary>
    /// Places the proper AHK script named "RunGame.ahk" 
    /// that's found in the game's directory that was made during the GAME construction.
    /// </summary>
    /// <param name="game">The game that's being loaded.</param>
    private void LoadAHKScript(Game game)
    {
        File.Copy(game.directory.FullName + "/RunGame.ahk", Application.dataPath + "/Options/RunGame.ahk", true);
    }
}

