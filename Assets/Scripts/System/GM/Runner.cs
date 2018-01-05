using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {

    private Game game;
    Process gameRunner;

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

    public void Run(Game game) {
        this.game = game;

        GM.Instance.logger.Info(this, "Running Game: " + game.name);

        LoadAHKScript(game);
        game.PreRun();
        StartCoroutine(RunProcess(gameRunner));
    }

    IEnumerator RunProcess(Process process){

        GM.Instance.state.SetTrigger("NextState");

        Screen.fullScreen = false;

        yield return new WaitForSeconds(1.0f);

        GM.Instance.logger.Info(this, "RUNNER: Running game " + process.StartInfo.FileName);

        GM.Instance.network.startGame(game.slug);

        process.Start();
        process.WaitForExit();
        process.Close();

        GM.Instance.network.stopGame(game.slug);

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

