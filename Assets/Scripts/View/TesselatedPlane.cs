using UnityEngine;
using System.Collections;

public class TesselatedPlane : MonoBehaviour {

	public Transform closeResetPoint;
	public Transform farResetPoint;

	public float closePoint;
	public float farPoint;

	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.z < closePoint) {
			gameObject.transform.position = farResetPoint.position;
		}

		if (gameObject.transform.position.z > farPoint) {
			gameObject.transform.position = closeResetPoint.position;
		}
	}
}
