using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraEffectsController : MonoBehaviour {

	public SunShafts sunshafts;

	public float intensity = 0;

	// Update is called once per frame
	void Update () {
		sunshafts.sunShaftIntensity = intensity;
	}
}
