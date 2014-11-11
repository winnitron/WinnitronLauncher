using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Duplicator : MonoBehaviour {

	public int numberOfCopies = 0;

	public bool isClone = false;

	public Vector3 rotationOffset;
	public Vector3 scaleOffset;
	public Vector3 positionOffset;

	// Use this for initialization
	void Start () {

		if(name.Contains("Clone")) 
		{
			Debug.Log ("IS CLONE");

			if(transform.parent.name.Contains ("Clone")) Destroy(gameObject);

			Destroy (this);
		}

		else
		{

			for(var i = 0; i < numberOfCopies; i++)
			{
				//Create the copy
				GameObject me = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;

				//Assignt it to parent this object
				me.transform.parent = transform;

				//Assign offsets to the new component
				DuplicatorChild dc = me.AddComponent("DuplicatorChild") as DuplicatorChild;

				dc.rotationOffset = rotationOffset;
				dc.scaleOffset = scaleOffset;
				dc.positionOffset = positionOffset;

				dc.copyNumber = i + 1;

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
