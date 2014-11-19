using UnityEngine;
using System.Collections;

public class StateSwitcher : MonoBehaviour {

	public bool switchToIntro = false;
	public bool switchToLauncher = false;

	void Update() {
		if(switchToIntro) GM.ChangeState(GM.WorldState.Intro);
		if(switchToLauncher) GM.ChangeState(GM.WorldState.Launcher);

		switchToIntro = false;
		switchToLauncher = false;
	}
}
