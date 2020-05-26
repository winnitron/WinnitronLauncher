using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

/// <summary>
/// This handles launching and running games.
/// </summary>
public class Runner : MonoBehaviour {

    private Game game;
    Process gameRunner;

    /// <summary>
    /// Called by GM during the Init state.
    /// </summary>
    public void Init()
    {
        string rungame = Path.Combine(Application.dataPath, "Options/RunGame.exe");

        if (!System.IO.File.Exists(rungame))
            GM.Instance.Oops(GM.Instance.Text("error", "noRunGameExe"), true);

        //Store the legacyController in the Awake
        gameRunner = new Process();
        gameRunner.StartInfo.FileName = rungame;
        GM.Instance.logger.Info(this, "gameRunner path " + gameRunner.StartInfo.FileName);

        if(gameRunner == null)
            GM.Instance.Oops(GM.Instance.Text("error", "noLegacyControlExe"));
    }

    /// <summary>
    /// Called by the Launcher Controller, this attempts to run the game that's passed to it.
    /// </summary>
    /// <param name="game">The Game object to attempt to run.</param>
    public void Run(Game game) {
        this.game = game;

        GM.Instance.logger.Info(this, "Running Game: " + game.name);

        LoadAHKScript(game);
        game.PreRun();
        StartCoroutine(RunProcess(gameRunner));
    }

    /// <summary>
    /// The coroutine that runs while the game's executable is running.  It puts the launcher
    /// into the Idle state and waits for the game process to end before continuing.
    /// </summary>
    /// <param name="process">The process to check if it's running.</param>
    /// <returns></returns>
    IEnumerator RunProcess(Process process){

        GM.Instance.state.SetTrigger("NextState");

        Screen.fullScreen = false;

        yield return new WaitForSeconds(1.0f);

        GM.Instance.logger.Info(this, "RUNNER: Running game " + process.StartInfo.FileName);

        System.DateTime started = DateTime.Now;

        process.Start();
        process.WaitForExit();
        process.Close();

        GM.Instance.network.RecordPlaySession(game.slug, started);

        GM.Instance.logger.Info(this, "RUNNER: Finished running game " + process.StartInfo.FileName);

        GM.Instance.state.SetTrigger("NextState");
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

