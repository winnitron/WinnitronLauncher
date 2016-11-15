using UnityEngine;
using System.Collections;

public class Tweenable : MonoBehaviour {

    GoTween currentTween;

    //public LauncherUIController playlistNavigationManager { get; set; }

    public void move(Vector3 pos, Vector3 scale, float tweenTime)
    {

        currentTween = Go.to(transform, tweenTime, new GoTweenConfig()
            .position(pos)
            .scale(scale)
            .setEaseType(GoEaseType.ExpoOut));


        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });
    }

    public void onMoveComplete()
    {

        
    }

    public void stopTween()
    {
        currentTween.destroy();
    }
}
