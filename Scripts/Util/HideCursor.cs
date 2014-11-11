using UnityEngine;
using System.Collections;

public class HideCursor : MonoBehaviour {
	void Awake() {
		Screen.showCursor = false;
	}
}
