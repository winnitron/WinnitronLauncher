using UnityEngine;
using System.Collections;

public class TextPulse : MonoBehaviour {
	
	public float zEnd = 1;
	public float yEnd = 1;
	public float xEnd = 1;

	public bool xPulse = false;
	public bool yPulse = false;
	public bool zPulse = true;

	public float speed = 1;

	Vector3 vStart;
	Vector3 vEnd;

	// Use this for initialization
	void Start () {
		if (!xPulse) xEnd = transform.localScale.x;
		if (!yPulse) yEnd = transform.localScale.y;
		if (!zPulse) zEnd = transform.localScale.z;

		vStart = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		vEnd = new Vector3(xEnd, yEnd, zEnd);
	}
	
	// Update is called once per frame
	void Update () {
		//transform.localScale = Auto.Wave(1f, Vector3(1f, 1f, 0f), Vector3(1f, 1f, 1f));
		transform.localScale = Auto.Wave(speed, vStart, vEnd);
	}
}
