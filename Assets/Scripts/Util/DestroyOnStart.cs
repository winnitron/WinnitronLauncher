using UnityEngine;
using System.Collections;

public class DestroyOnStart : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		Destroy(gameObject);
	}
}
