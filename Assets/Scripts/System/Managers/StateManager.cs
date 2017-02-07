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

    void Awake()
    {
        StartCoroutine("Init");
    }

	IEnumerator Init() {
        //Wait for options script to get all the folders before continuing
        while (GM.options.initializing) yield return null;

		foreach (State state in states) 
			state.gameObject.SetActive (true);

		ChangeState (worldState);
	}

	void Update () {

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
        if (Input.GetKeyDown (KeyCode.F6))
			oops = GM.Text ("error", "test");
	}

	//Changes the worldstate
	public void ChangeState(WorldState newState) {

		foreach (var state in states) {
			if (newState == state.worldState) {
				GM.dbug.Log (this, "STATE: activating state " + state.worldState);
				state.Activate ();
				worldState = state.worldState;
			}
			else
				state.Deactivate ();
		}

        GM.dbug.Log(this, "STATE: new state is " + worldState);

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
