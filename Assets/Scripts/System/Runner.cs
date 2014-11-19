using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;

public class Runner : MonoBehaviour {
    
    Jukebox jukebox;

    void Awake() {

        if (GameObject.Find("Jukebox"))
            jukebox = GameObject.Find("Jukebox").GetComponent<Jukebox>();
    }

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
		Screen.fullScreen = true;
		GM.ChangeState(GM.WorldState.Intro);
        if (jukebox) jukebox.play();
	}
}

