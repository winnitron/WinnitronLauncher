using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the current game info.  Takes two targets and tweens to the endTarget. 
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

    private void ResetPosition()
    {
        transform.localPosition = startTarget;        
    }

    private void CompileString()
    {
        var game = controller.GetCurrentGame();
        var newString = "";

        if (game.author.Length > 2) newString += "Developed by: " + game.author + "\n";
        if (game.description.Length > 4)newString += game.description + "\n";

        text.text = newString;
    }
}
