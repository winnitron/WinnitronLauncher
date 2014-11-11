using UnityEngine;
using System.Collections;

public class DuplicatorChild : MonoBehaviour {

	public int copyNumber = 0;

	public Vector3 rotationOffset;
	public Vector3 scaleOffset;
	public Vector3 positionOffset;
	
	// Update is called once per frame
	void Update () {

		transform.localEulerAngles = Vector3.zero + (rotationOffset * copyNumber);
		transform.localScale = Vector3.one + (scaleOffset * copyNumber);
		transform.localPosition = Vector3.zero + (positionOffset * copyNumber);
	}
}
