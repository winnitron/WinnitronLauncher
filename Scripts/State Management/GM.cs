using UnityEngine;
using System.Collections;

public class GM : Singleton<GM> {

	public enum WorldState{Intro, Launcher, Attract, Idle};

	public static WorldState worldState = WorldState.Intro;
	private WorldState prevWorldState = WorldState.Launcher;

	public StateManager intro;
	public StateManager launcher;
	public StateManager attract;
	public StateManager idle;
	
	void Awake() {
		Screen.showCursor = false;
	}

	//Ideally these wouldn't be called every frame, probably not optimized
	void Update () {

		if (Input.GetKey (KeyCode.Alpha1)) worldState = WorldState.Intro;
		if (Input.GetKey (KeyCode.Alpha2)) worldState = WorldState.Launcher;
		if (Input.GetKey (KeyCode.Alpha3)) worldState = WorldState.Attract;
		if (Input.GetKey (KeyCode.Alpha4)) worldState = WorldState.Idle;

		if(worldState != prevWorldState)
		{
			switch(worldState)
			{
				case WorldState.Intro:
					intro.Activate();
					idle.Deactivate();
					launcher.Deactivate();
					attract.Deactivate();
					Debug.Log ("Intro State");
				break;

				case WorldState.Launcher:
					intro.Deactivate();
					idle.Deactivate();
					launcher.Activate();
					attract.Deactivate();
					Debug.Log ("Launcher State");
				break;

				case WorldState.Idle:
					intro.Deactivate();
					idle.Activate();
					launcher.Deactivate();
					attract.Deactivate();
					Debug.Log ("Idle State");
				break;

				case WorldState.Attract:
					intro.Deactivate();
					idle.Deactivate();
					launcher.Deactivate();
					attract.Activate();
					Debug.Log ("Attract State");
				break;

			}
		}

		prevWorldState = worldState;
	}

	static public void ChangeState(WorldState ws) {
		worldState = ws;
	}
}
