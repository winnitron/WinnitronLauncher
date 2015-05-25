using UnityEngine;
using System.Collections;

public class TesselatedPlane : MonoBehaviour {

	public Transform resetPoint;

	public float endPoint;

	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.z > endPoint) {
			gameObject.transform.position = resetPoint.position;
		}
	}
}
