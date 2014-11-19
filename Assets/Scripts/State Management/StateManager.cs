using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour {

	public List<GameObject> objectsToToggleOnForState;
	public List<GameObject> objectsToToggleOffForState;
	public List<Canvas> canvasesToToggleActivation;

	//On Activate
	public List<Animation> animationsToPlayOnStart;

	//On deactivate
	public List<Animation> animationsToRewind;

	public bool pauseEngine = false;

	public void Deactivate() {
		if (pauseEngine) Time.timeScale = 1;

		foreach(GameObject gameObject in objectsToToggleOnForState) gameObject.SetActive(false);
		foreach(GameObject gameObject in objectsToToggleOffForState) gameObject.SetActive(true);
		foreach(Animation anim in animationsToRewind) {
			anim.Stop();
			anim.Rewind();
			Debug.Log ("rewinding: " + anim.name);
		}
		foreach (Canvas canvas in canvasesToToggleActivation) {
			Debug.Log ("deactivating: " + canvas.name);
			canvas.enabled = false;
		}
	}

	public void Activate() {
		foreach(Animation anim in animationsToPlayOnStart) anim.Play();
		foreach(Canvas canvas in canvasesToToggleActivation) canvas.enabled = true;
		foreach(GameObject gameObject in objectsToToggleOnForState) gameObject.SetActive(true);
		foreach(GameObject gameObject in objectsToToggleOffForState) gameObject.SetActive(false);

		if (pauseEngine) Time.timeScale = 0;
	}
}
