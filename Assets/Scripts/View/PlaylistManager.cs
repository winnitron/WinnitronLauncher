using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlaylistManager : MonoBehaviour {

    public float fadeOutAlpha;

    public GameLabelManager gameLabelManagerPrefab;
    public ScreenshotManager screenshotManagerPrefab;

    public Animation UpArrow;
    public Animation DownArrow;


    GoTween currentTween;

    int selectedGameIndex = 0;
    bool waiting = false;

    #region Properties

    public Playlist playlist { get; set; }                  // Data structure containing this playlists information

    public GameLabelManager gameLabelManager { get; set; }
    public ScreenshotManager screenshotManager { get; set; }
    public PlaylistNavigationManager playlistNavManager { get; set; }
    public bool activated { get; set; }

    public Vector3 positionProp {
        get { return transform.position; }
        set { transform.position = value; }
    }
    public Vector3 scaleProp {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }
    public float alphaProp { get; set; }

    #endregion


    public void moveUpList() {

        if (selectedGameIndex == 0)
            selectedGameIndex = playlist.games.Count - 1;
        else
            selectedGameIndex--;

        // Tell all labels and images to tween to the correct positions
        sort();

        UpArrow.Rewind();
        UpArrow.Play();
    }

    public void moveDownList() {

        if (selectedGameIndex >= playlist.games.Count - 1)
            selectedGameIndex = 0;
        else
            selectedGameIndex++;

        // Tell all labels and images to tween to the correct positions
        sort();

        DownArrow.Rewind();
        DownArrow.Play();
    }

    public void selectGame() {

        if (!waiting) {

            waiting = true; StartCoroutine("wait"); // So that the user can't launch multiple games at once
            GameObject.Find("Runner").GetComponent<Runner>().Run(playlist.games[selectedGameIndex]);
        }
    }

    // Takes in the list of games and creates the appropriate labels etc. for those games
    public void buildList() {
                
        // Instantiate the game label manager and store the reference
        gameLabelManager = Instantiate(gameLabelManagerPrefab) as GameLabelManager;
        gameLabelManager.playlistManager = this;
        gameLabelManager.transform.SetParent(transform);
        gameLabelManager.transform.position = transform.position;
        gameLabelManager.initialize();

        // Instantiate the screenshot manager and store the reference
        screenshotManager = Instantiate(screenshotManagerPrefab) as ScreenshotManager;
        screenshotManager.playlistManager = this;
        screenshotManager.transform.SetParent (transform);
        screenshotManager.transform.position = transform.position;
        screenshotManager.initialize();
    }

    public void move(Vector3 pos, Vector3 scale, float tweenTime) {

        if (playlistNavManager.moving && currentTween != null) {

            currentTween.destroy();
        }

        currentTween = Go.to(this, tweenTime, new GoTweenConfig()
            .vector3Prop("positionProp", pos)
            .vector3Prop("scaleProp", scale)
            .floatProp("alphaProp", fadeOutAlpha)
            .setEaseType(GoEaseType.ExpoOut));

        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });
    }

    public void onMoveComplete() {

        playlistNavManager.moving = false;
    }

    public void sort() {

        gameLabelManager.sortLabelList(selectedGameIndex);
        screenshotManager.sortImageList(selectedGameIndex);
    }


    public void stopTween() {

        gameLabelManager.stopTween();
        screenshotManager.stopTween();
    }

    IEnumerator wait() {

        yield return new WaitForSeconds(1);
        waiting = false;
    }
}
