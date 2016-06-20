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
		SyncProcess.StartInfo.CreateNoWindow = false;
		SyncProcess.StartInfo.UseShellExecute = true;
		//SyncProcess.StartInfo.FileName = Path.Combine (Application.dataPath, "Sync") + "/winnitron-sync.bat";//"C:\\WINNITRON\\Games\\Canabalt\\Canabalt.exe";
		SyncProcess.StartInfo.FileName = "C:/WINNITRON/Winnitron_Data/Sync/winnitron-sync.bat";
		UnityEngine.Debug.Log("SYNC: Checking for Sync program in " + Path.Combine (Application.dataPath, "Sync") + "\\node index");
		SyncProcess.EnableRaisingEvents = true;
		StartCoroutine(RunSyncProcess(SyncProcess));
	}

	IEnumerator RunSyncProcess(Process process) {

		UnityEngine.Debug.Log("SYNC: Running sync program.");

		if (jukebox) jukebox.stop();

		GM.ChangeState(GM.WorldState.Sync);

		yield return new WaitForSeconds(1.0f);
		process.Start();
		process.WaitForExit();

		UnityEngine.Debug.Log("SYNC: Sync program complete!  Do intro.");

		//Screen.SetResolution (1024, 768, true);
		//GM.ResetScreen();

		GM.ChangeState(GM.WorldState.Intro);
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

		GM.ChangeState(GM.WorldState.Idle);
		Screen.fullScreen = false;
		//TO DO - stuff that is a transition
		yield return new WaitForSeconds(1.0f);
		process.Start();
		process.WaitForExit();

		//Screen.SetResolution (1024, 768, true);
		//GM.ResetScreen();

		//GM.ChangeState(GM.WorldState.Intro);
		RunSync();
        if (jukebox) jukebox.play();
	}
}

