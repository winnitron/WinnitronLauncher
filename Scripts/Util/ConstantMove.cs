using UnityEngine;
using System.Collections;

public class ConstantMove : MonoBehaviour {

	public bool relative = false;
	public Vector3 force = new Vector3(0, 0, 0);
	public Vector3 spin = new Vector3(0, 0, 0);
	
	// Update is called once per frame
	void Update () {
		if (rigidbody) 
		{
			if (relative)
					rigidbody.AddRelativeForce (force);
			else
					rigidbody.AddForce (force);
		}

		else 
		{
			//No rigidbody, just move the transform
			transform.Translate(force);
			transform.localEulerAngles += spin;
		}
	}
}
