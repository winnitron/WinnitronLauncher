using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {

    Process gameRunner;

    void Awake()
    {
        string rungame = Path.Combine(Application.dataPath, "Options/RunGame.exe");

        if (!System.IO.File.Exists(rungame))
            GM.Oops(GM.Text("error", "noRunGameExe"), true);

        //Store the legacyController in the Awake
        gameRunner = new Process();
        gameRunner.StartInfo.FileName = rungame;
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
        string source = Path.Combine(game.directory.FullName, "RunGame.ahk");
        string dest   = Path.Combine(Application.dataPath, "Options/RunGame.ahk");
        File.Copy(source, dest, true);
    }
}

