using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour {

    //Enum for changing states
	public enum WorldState{Intro, Launcher, Attract, Idle, Sync, Oops}
	public WorldState worldState = WorldState.Intro;

    //Just contains all the GameObjects that act as States
	public List<State> states;

    //Oops
	public OopsScreenController oopsController;
	public string oops = "";

	public void Init() {
        //Need to make sure that the launcherUI can hook in it's delegate so it knows when the data
        //GameObject.Find("LauncherUI").GetComponent<LauncherUIController>().Init();

		//foreach (State state in states)
		//	state.gameObject.SetActive (true);

        //This just changes the state to the first state that's in the inspector
        //This will primarily be for testing, generally you want to sync when things start up
        if (worldState == WorldState.Sync || GM.sync.syncOnStartup) GM.sync.execute();
        else
        {
            GM.data.ReloadData();
            ChangeState(worldState);
        }
	}

	void Update() {

		//DEBUG KEYS
		//Switch states for testing.  These keys aren't used on Winnitrons yet
		if (Input.GetKeyDown (KeyCode.F1))
			ChangeState(WorldState.Intro);
        if (Input.GetKeyDown(KeyCode.F2))
            ChangeState(WorldState.Launcher);
		if (Input.GetKeyDown (KeyCode.F3))
			ChangeState(WorldState.Attract);
		if (Input.GetKeyDown (KeyCode.F4))
			ChangeState(WorldState.Idle);
        if (Input.GetKeyDown(KeyCode.F5) || Input.GetKeyDown(KeyCode.Return))
            ChangeState(WorldState.Sync);
        if (Input.GetKeyDown(KeyCode.F6))
            GM.Oops(GM.Text("error", "test"));
	}

    /// <summary>
    /// Change the state of the Launcher.
    /// </summary>
    /// <param name="newState">Uses StateManager.WorldState enums.</param>
    public void ChangeState(WorldState newState) {

		foreach (var state in states) {
			if (newState == state.worldState) {
				GM.logger.Debug(this, "STATE: activating state " + state.worldState);
				state.Activate();
				worldState = state.worldState;
			}
			else
				state.Deactivate();
		}

        GM.logger.Debug(this, "STATE: new state is " + worldState);

        GM.ResetScreen();
	}

    /// <summary>
    /// Causes and Oops screen to appear.
    /// </summary>
    /// <param name="text">Error text to display</param>
    /// <param name="isCritical">Critical oopses will quit the launcher</param>
    public void Oops(string text, bool isCritical)
    {
        ChangeState(WorldState.Oops);
        oopsController.SetErrorText(text);
        oopsController.isCritical = isCritical;
    }
}
