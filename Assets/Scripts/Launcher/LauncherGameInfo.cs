using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays the current game info.  Takes two targets and tweens to the endTarget.
/// </summary>
public class LauncherGameInfo : Tweenable {

    public LauncherController controller;
    public TextMeshProUGUI text;

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
        var info = "";
        var game = controller.GetCurrentGame();

        if (game == null)
            return;

        if (game.Author.Length > 2) info += "Developed by: " + game.Author + "\n";
        if (game.Description.Length > 4) info += game.Description + "\n";

        text.text = info;
    }
}
