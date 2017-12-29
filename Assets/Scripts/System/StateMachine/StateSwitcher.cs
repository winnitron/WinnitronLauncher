using UnityEngine;
using System.Collections;

public class StateSwitcher : MonoBehaviour {

	public bool switchToIntro = false;
	public bool switchToLauncher = false;

	void Update()
	{
		if (switchToIntro)
			GM.state.ChangeState(StateManager.WorldState.Intro);
		if (switchToLauncher)
			GM.state.ChangeState (StateManager.WorldState.Launcher);

		switchToIntro = false;
		switchToLauncher = false;
	}
}
