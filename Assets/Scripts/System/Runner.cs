using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;


public class Runner : MonoBehaviour {
    
    Jukebox jukebox;

    void Awake()
    {
        //Not 100% sure why the jukebox is here. :S
        if (GameObject.Find("Jukebox"))
            jukebox = GameObject.Find("Jukebox").GetComponent<Jukebox>();
    }

    /*
	public void RunSync()
    {
        GM.dbug.Log(this, "RUNNER: Running sync program.");
        GM.sync.execute();
	}
    */


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

		GM.state.ChangeState(StateManager.WorldState.Idle);
		Screen.fullScreen = false;

		//TO DO - stuff that is a transition
		yield return new WaitForSeconds(1.0f);
        GM.dbug.Log(this, "RUNNER: Running game " + process.StartInfo.FileName);

        process.Start();
		process.WaitForExit();

        GM.dbug.Log(this, "RUNNER: Finished running game " + process.StartInfo.FileName);

		GM.state.ChangeState(StateManager.WorldState.Intro);

        if (jukebox) jukebox.play();
	}
}

