using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MirrorText : MonoBehaviour {

    private TextMeshProUGUI myText;
    public TextMeshProUGUI targetText;

    private void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update () {
        myText.text = targetText.text;
	}
}
