using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameScreenshot : Tweenable {

    public Image screenshot;

    public void SetImage(Sprite sprite)
    {
        screenshot.sprite = sprite;
    }
}
