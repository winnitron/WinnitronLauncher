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
		SyncProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		SyncProcess.StartInfo.FileName = GM.options.syncBat;
		SyncProcess.StartInfo.UseShellExecute = true;
		UnityEngine.Debug.Log("RUNNER: Checking for Sync program in " + SyncProcess.StartInfo.FileName + " " + SyncProcess.StartInfo.Arguments);
		StartCoroutine(RunSyncProcess(SyncProcess));
	}

	IEnumerator RunSyncProcess(Process process) {

		UnityEngine.Debug.Log("RUNNER: Running sync program.");

		if (jukebox) jukebox.stop();

		GM.state.Change(StateManager.WorldState.Sync);

		yield return new WaitForSeconds(1.0f);
		process.Start();
		UnityEngine.Debug.Log("RUNNER: Sync program started.");
		process.WaitForExit();

		UnityEngine.Debug.Log("RUNNER: Sync program complete!  Do intro.");

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
        UnityEngine.Debug.Log("RUNNER: Running game " + process.StartInfo.FileName);

        process.Start();
		process.WaitForExit();

        UnityEngine.Debug.Log("RUNNER: Finished running game " + process.StartInfo.FileName);

		GM.state.Change(StateManager.WorldState.Intro);

        if (jukebox) jukebox.play();
	}
}

