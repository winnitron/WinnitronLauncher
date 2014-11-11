using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Playlist : MonoBehaviour {

    public List<Game> gamesList;
    public string gamesDirectory;    

    public float fadeOutAlpha;

    public GameLabelManager gameLabelManagerPrefab;
    public ScreenshotManager screenshotManagerPrefab;

    public Animation UpArrow;
    public Animation DownArrow;


    GoTween currentTween;

    int selectedGameIndex = 0;
    bool waiting = false;

    #region Properties

    public GameLabelManager gameLabelManager { get; set; }
    public ScreenshotManager screenshotManager { get; set; }
    public PlaylistNavigationManager playlistNavigationManager { get; set; }
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
            selectedGameIndex = gamesList.Count - 1;
        else
            selectedGameIndex--;

        // Tell all labels and images to tween to the correct positions
        sort();

        UpArrow.Rewind();
        UpArrow.Play();
    }

    public void moveDownList() {

        if (selectedGameIndex >= gamesList.Count - 1)
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
            GameObject.Find("Runner").GetComponent<Runner>().Run(gamesList[selectedGameIndex]);
        }
    }

    // Builds a list of Game objects based on the game directory inside its main directory. Then instantiates the GameNavigationManager, which then instantiates the ScreenShotDisplayManager
    public void buildList() {
        
        var gamesDir = new DirectoryInfo(gamesDirectory);
        
        foreach (var dir in gamesDir.GetDirectories()) {

            gamesList.Add(CreateRepresentation(dir));
        }

        // Instantiate the game label manager and store the reference
        gameLabelManager = Instantiate(gameLabelManagerPrefab) as GameLabelManager;
        gameLabelManager.playlist = this;
        gameLabelManager.transform.parent = transform;
        gameLabelManager.transform.position = transform.position;        

        // Instantiate the screenshot manager and store the reference
        screenshotManager = Instantiate(screenshotManagerPrefab) as ScreenshotManager;
        screenshotManager.playlist = this;
        screenshotManager.transform.parent = transform;
        screenshotManager.transform.position = transform.position;        
    }

    Game CreateRepresentation(DirectoryInfo gameDirectory) {
        
        var directoryName = gameDirectory.Name;
        var name = directoryName.Replace('_', ' ');
        
        string author = null;

        // Load the screenshot from the games directory as a Texture2D
        var screenshot = new Texture2D(1024, 768);
        screenshot.LoadImage(File.ReadAllBytes(Path.Combine(gameDirectory.FullName, gameDirectory.Name + ".png")));

        // Turn the Texture2D into a sprite
        var screenshotSprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));

        var executablePath = Path.Combine(gameDirectory.FullName, gameDirectory.Name + ".exe");

        return new Game(name, author, screenshotSprite, executablePath);
    }

    public void move(Vector3 pos, Vector3 scale, float tweenTime) {

        if (playlistNavigationManager.moving) {

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

        playlistNavigationManager.moving = false;
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
