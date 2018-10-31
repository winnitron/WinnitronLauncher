using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeToWindowSize : MonoBehaviour {

    public RectTransform rectTransform;
	
	// Update is called once per frame
	void Update () {
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
	}
}
