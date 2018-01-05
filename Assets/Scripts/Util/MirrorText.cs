using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorText : MonoBehaviour {

    private Text myText;
    public Text targetText;

    private void Start()
    {
        myText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        myText.text = targetText.text;
	}
}
