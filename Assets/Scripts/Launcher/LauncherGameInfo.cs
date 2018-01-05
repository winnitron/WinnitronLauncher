using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Takes two targets and tweens to the endTarget 
/// </summary>
public class LauncherGameInfo : Tweenable {

    public LauncherController controller;
    public Text text;

    public Vector3 endTarget;
    public Vector3 startTarget;
    public float tweenTime;

    public void Start()
    {
        endTarget = transform.localPosition;

        //Hook in the delegate so Trigger fires each time the UI is refreshed
        controller.refreshLauncherUI += Trigger;

        ResetPosition();
    }

    public void Trigger()
    {
        ResetPosition();
        CompileString();
        TweenLocalPosition(endTarget, tweenTime, false);
    }

    public void ResetPosition()
    {
        transform.localPosition = startTarget;        
    }

    public void CompileString()
    {
        var game = controller.GetCurrentGame();
        var newString = "";

        if (game.author != "") newString += "Developed by: " + game.author + "\n";
        newString += game.description + "\n";

        text.text = newString;
    }
}
