using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour {

	public enum WorldState{Intro, Launcher, Attract, Idle, Sync, Oops};

	public WorldState worldState = WorldState.Intro;

	public List<State> states;

	public float idleTime = 0;
	public float timeBeforeIdle = 5;

	public OopsScreenController oopsController;
	public string oops = "";

	public void Init() {

		foreach (State state in states) 
			state.gameObject.SetActive (true);

		Change (WorldState.Intro);
	}

	void Update () {

		//DEBUG KEYS
		//Switch states for testing.  These keys aren't used on Winnitrons yet
		if (Input.GetKeyDown (KeyCode.Alpha1))
			worldState = WorldState.Intro;
		if (Input.GetKeyDown (KeyCode.Alpha2))
			worldState = WorldState.Launcher;
		if (Input.GetKeyDown (KeyCode.Alpha3))
			worldState = WorldState.Attract;
		if (Input.GetKeyDown (KeyCode.Alpha4))
			worldState = WorldState.Idle;
		if (Input.GetKeyDown (KeyCode.Alpha5))
			GM.runner.RunSync ();
		if (Input.GetKeyDown (KeyCode.Alpha6))
			worldState = WorldState.Sync;
		if (Input.GetKeyDown (KeyCode.Alpha7))
			oops = GM.Text ("error", "test");

		//Things to do in Attract Mode
		if (worldState == WorldState.Attract) {
			//Relaunch launcher if any key is pressed
			if (Input.anyKeyDown)
				worldState = WorldState.Launcher;
		}

		//Things to do in Launcher Mode
		if (worldState == WorldState.Launcher) {
			//Increase idle time
			idleTime += Time.deltaTime;

			//Reset idle time if a key is pressed
			if (Input.anyKey)
				idleTime = 0;

			//Go into Attract mode is key isn't pressed for a while
			if (idleTime > timeBeforeIdle)
				worldState = WorldState.Attract;
		} else {
			//Reset idleTime if not in Launcher
			idleTime = 0;
		}


		//OOPS
		if (oops != "" && worldState != WorldState.Oops) {
			Change (WorldState.Oops);
			oopsController.SetErrorText (oops);
		}

		if (worldState == WorldState.Oops) {
			if (Input.GetKeyDown (KeyCode.Q))
				Application.Quit ();
		}
	}

	//Changes the worldstate
	public void Change(WorldState newState) {

		foreach (var state in states) {
			if (newState == state.worldState) {
				Debug.Log ("STATE: activating state " + state.worldState);
				state.Activate ();
				worldState = state.worldState;
			}
			else
				state.Deactivate ();
		}

		Debug.Log ("STATE: new state is " + worldState);
	}
}
