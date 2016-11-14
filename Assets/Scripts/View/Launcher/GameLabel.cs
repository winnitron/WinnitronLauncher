using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLabel : MonoBehaviour {
    
    public float tweenTime;
	public float alpha = 1;

    GoTween currentTween;

    public GameLabelManager gameLabelManager { get; set; }    


    public void move(Vector3 pos, Vector3 scale) {

        if (gameLabelManager.moving) {

            currentTween.destroy();
        }

        currentTween = Go.to(transform, tweenTime, new GoTweenConfig()
            .position(pos)            
            .scale(scale)            
            .setEaseType(GoEaseType.ExpoOut));


        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });
    }

    public void onMoveComplete() {

        gameLabelManager.moving = false;
    }

	public void setAlpha(float newAlpha) {
		GetComponent<Text>().color = new Color(1, 1, 1, newAlpha);
	}

    public void stopTween() {

        currentTween.destroy();
    }
}
