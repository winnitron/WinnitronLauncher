using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {
    
    Jukebox jukebox;

    void Awake() {

        if (GameObject.Find("Jukebox"))
            jukebox = GameObject.Find("Jukebox").GetComponent<Jukebox>();
		
    }

	public void RunSync() {
		Process SyncProcess = new Process();
		//SyncProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
		SyncProcess.StartInfo.FileName = GM.options.syncExe;
		SyncProcess.StartInfo.UseShellExecute = true;
        SyncProcess.StartInfo.CreateNoWindow = true;
        SyncProcess.StartInfo.WorkingDirectory = GM.options.syncPath;
		GM.dbug.Log(this, "RUNNER: Checking for Sync program in " + SyncProcess.StartInfo.FileName);
		StartCoroutine(RunSyncProcess(SyncProcess));
	}

	IEnumerator RunSyncProcess(Process process) {

		GM.dbug.Log(this, "RUNNER: Running sync program.");

		if (jukebox) jukebox.stop();

		GM.state.Change(StateManager.WorldState.Sync);

		yield return new WaitForSeconds(1.0f);
		process.Start();
		GM.dbug.Log(this, "RUNNER: Sync program started.");
		process.WaitForExit();

		GM.dbug.Log(this, "RUNNER: Sync program complete!  Do intro.");

		GM.state.Change(StateManager.WorldState.Intro);
		if (jukebox) jukebox.play();
	}


	//Run those games!

	public void Run(Game game) {
		Process myProcess = new Process();
		myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		myProcess.StartInfo.CreateNoWindow = true;
		myProcess.StartInfo.UseShellExecute = false;
		myProcess.StartInfo.FileName = game.executablePath;//"C:\\WINNITRON\\Games\\Canabalt\\Canabalt.exe";
		myProcess.EnableRaisingEvents = true;
		StartCoroutine(RunProcess(myProcess));
	}

	IEnumerator RunProcess(Process process){
        
		if (jukebox) jukebox.stop();

		GM.state.Change(StateManager.WorldState.Idle);
		Screen.fullScreen = false;

		//TO DO - stuff that is a transition
		yield return new WaitForSeconds(1.0f);
        GM.dbug.Log(this, "RUNNER: Running game " + process.StartInfo.FileName);

        process.Start();
		process.WaitForExit();

        GM.dbug.Log(this, "RUNNER: Finished running game " + process.StartInfo.FileName);

		GM.state.Change(StateManager.WorldState.Intro);

        if (jukebox) jukebox.play();
	}
}

