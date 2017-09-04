using UnityEngine;
using System.Collections;

public class TesselatedPlane : MonoBehaviour {

	public Transform closeResetPoint;
	public Transform farResetPoint;

	void OnTriggerEnter(Collider other) {
		GM.dbug.Debug(this, "Collided with " + other.name);

		if (other.name == "Close Plane Trigger") {
			gameObject.transform.position = farResetPoint.position;
		}

		if (other.name == "Far Plane Trigger") {
			gameObject.transform.position = closeResetPoint.position;
		}
	}
}
