using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {

    Process gameRunner;
    Process keyTranslator;

    void Awake()
    {
        string ahk = Path.Combine(Application.dataPath, "Options/RunGame.exe");

        if (!System.IO.File.Exists(ahk))
        {
            GM.dbug.Log("Could not find rungame exe: " + ahk);
            GM.Oops(GM.Text("error", "noRunGameExe"), true);
        }

        keyTranslator = new Process();
        keyTranslator.StartInfo.FileName = ahk;
        GM.dbug.Log(this, "keyTranslator path " + keyTranslator.StartInfo.FileName);
    }

	public void Run(Game game) {

        GM.dbug.Log(this, "Running Game " + game.name);

        gameRunner = new Process();
        gameRunner.StartInfo.FileName = game.executable;

        LoadAHKScript(game);
        game.PreRun();

        StartCoroutine(RunGame(gameRunner, keyTranslator));
	}

	IEnumerator RunGame(Process game, Process translator) {

		if (GM.jukebox) GM.jukebox.Stop();

		GM.state.ChangeState(StateManager.WorldState.Idle);
		Screen.fullScreen = false;

		yield return new WaitForSeconds(1.0f);

        GM.dbug.Log(this, "RUNNER: Running game " + game.StartInfo.FileName);

        translator.Start();

        game.Start();
        game.WaitForExit();

        translator.Kill();

        GM.dbug.Log(this, "RUNNER: Finished running game " + game.StartInfo.FileName);
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

