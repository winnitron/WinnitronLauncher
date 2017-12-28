using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

[System.Serializable]
public class State {

	public StateManager.WorldState worldState;

	public List<GameObject> objectsToToggleOnForState;
	public List<GameObject> objectsToToggleOffForState;
    public VideoClip clipToPlay;

	public bool pauseEngine = false;

	public void Deactivate() {
		if (pauseEngine) Time.timeScale = 1;

		foreach(GameObject gameObject in objectsToToggleOnForState) gameObject.SetActive(false);
		foreach(GameObject gameObject in objectsToToggleOffForState) gameObject.SetActive(true);
	}

	public void Activate() {
		foreach(GameObject gameObject in objectsToToggleOnForState) gameObject.SetActive(true);
		foreach(GameObject gameObject in objectsToToggleOffForState) gameObject.SetActive(false);

		if (pauseEngine) Time.timeScale = 0;
	}
}
