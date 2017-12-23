using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLabel : Tweenable {

    public Text labelText;

    public void SetText(string text)
    {
        labelText.text = text;
    }
}
