using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameImage : MonoBehaviour {
    
    public float tweenTime;
    public float fadeOutAlpha;

    GoTween currentTween;

    #region Properties

    public ScreenshotManager screenshotManager { get; set; }		
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


    void Update() {

        GetComponent<Image>().color = new Color(1, 1, 1, alphaProp);
    }

    public void move(Vector3 pos, Vector3 scale) {

        if (screenshotManager.moving) {

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

        screenshotManager.moving = false;
    }

    public void stopTween() {

        currentTween.destroy();
    }
}
