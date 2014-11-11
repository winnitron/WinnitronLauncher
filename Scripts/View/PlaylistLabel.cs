using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlaylistLabel : MonoBehaviour {


    public List<Text> textList;                 // List of the different text objects that make up the pulsing label
    float[] alphaList;              // Starting alpha levels for each individual text object

    GoTween currentTween;

    public PlaylistNavigationManager playlistNavigationManager { get; set; }


    public void Awake() {

        alphaList = new float[textList.Count];

        for (int i = 0; i < textList.Count; i++) {

            alphaList[i] = textList[i].color.a;
        }
    }

    public void move(Vector3 pos, Vector3 scale, float tweenTime) {

        if (playlistNavigationManager.moving) {

            currentTween.destroy();
        }

        currentTween = Go.to(transform, tweenTime, new GoTweenConfig()
            .position(pos)            
            .scale(scale)            
            .setEaseType(GoEaseType.ExpoOut));


        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });
    }

    public void onMoveComplete() {

        playlistNavigationManager.moving = false;
    }

    // Sets the alpha of each individual text object, 0 means faded, 1 is original alpha
	public void setAlpha(float newAlpha) {

        if (newAlpha == 1) {

            for (int i = 0; i < textList.Count; i++) { textList[i].color = new Color(1, 1, 1, alphaList[i]); }
        }
        else {

            for (int i = 0; i < textList.Count; i++) { textList[i].color = new Color(1, 1, 1, alphaList[i] * newAlpha); }
        }
	}

    public void initializeName(string name) {

        foreach (Text text in GetComponentsInChildren<Text>()) {

            text.text = name;
        }

    }
}
