using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {
    
    Jukebox jukebox;
    Process legacyController;

    void Awake()
    {
        //Store the legacyController in the Awake
        legacyController = new Process();
        legacyController.StartInfo.FileName = GM.options.legacyControlsPath;

        if(legacyController == null)
            GM.Oops(GM.Text("error", "noLegacyControlExe"));

        //Not 100% sure why the jukebox is here. :S
        if (GameObject.Find("Jukebox"))
            jukebox = GameObject.Find("Jukebox").GetComponent<Jukebox>();
    }


	//Run those games!

	public void Run(Game game) {

        GM.dbug.Log(this, "Running Game " + game.name + " legacy: " + game.useLegacyControls);

        if (game.useLegacyControls)
            StartLegacyControls();

		Process myProcess = new Process();
		myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		myProcess.StartInfo.CreateNoWindow = true;
		myProcess.StartInfo.UseShellExecute = false;
		myProcess.StartInfo.FileName = game.executable;//"C:\\WINNITRON\\Games\\Canabalt\\Canabalt.exe";
		myProcess.EnableRaisingEvents = true;
		StartCoroutine(RunProcess(myProcess, legacyController));
	}

	IEnumerator RunProcess(Process process, Process legacyController){
        
		if (jukebox) jukebox.Stop();

		GM.state.ChangeState(StateManager.WorldState.Idle);
		Screen.fullScreen = false;

		//TO DO - stuff that is a transition
		yield return new WaitForSeconds(1.0f);
        GM.dbug.Log(this, "RUNNER: Running game " + process.StartInfo.FileName);

        process.Start();
		process.WaitForExit();

        GM.dbug.Log(this, "RUNNER: Finished running game " + process.StartInfo.FileName);

        StopLegacyControls();

		GM.state.ChangeState(StateManager.WorldState.Intro);

        if (jukebox) jukebox.PlayRandom();
	}

    public void StartLegacyControls()
    {
        GM.dbug.Log(this, "Runner: Starting Legacy Controls");
        legacyController.Start();
    }

    public void StopLegacyControls()
    {
        try
        {
            GM.dbug.Log(this, "Runner: Starting Legacy Controls");
            legacyController.CloseMainWindow();
        }

        catch { }
    }
}

