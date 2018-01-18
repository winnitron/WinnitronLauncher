using UnityEngine;
using System.Collections;

public class ConstantMove : MonoBehaviour {

	public bool relative = false;
	public Vector3 force = new Vector3(0, 0, 0);
	public Vector3 spin = new Vector3(0, 0, 0);

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update () {
		if (GetComponent<Rigidbody>()) 
		{
			if (relative)
					GetComponent<Rigidbody>().AddRelativeForce (force);
			else
					GetComponent<Rigidbody>().AddForce (force);
		}

		else 
		{
			//No rigidbody, just move the transform
			transform.Translate(force * Time.deltaTime);
			transform.localEulerAngles += spin * Time.deltaTime;
		}
	}

    private void OnEnable()
    {
        transform.position = startPosition;
    }
}
