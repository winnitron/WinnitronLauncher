using UnityEngine;
using System.Collections;

public class Tweenable : MonoBehaviour {

    GoTween currentTween;
    private bool destroyOnTweenEnd = false;

    public void TweenPosition(Vector3 position)
    {
        if (currentTween != null) StopTween();

        currentTween = Go.to(transform, GM.Instance.options.tweenTime, new GoTweenConfig()
            .position(position)
            .setEaseType(GoEaseType.ExpoOut));

        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });
    }

    public void TweenLocalPosition(Vector3 position, float tweenTime, bool destroyOnEnd)
    {
        if (currentTween != null) StopTween();

        currentTween = Go.to(transform, tweenTime, new GoTweenConfig()
            .localPosition(position)
            .setEaseType(GoEaseType.ExpoOut));

        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });

        destroyOnTweenEnd = destroyOnEnd;
    }

    public void Tween(Vector3 position, Vector3 scale)
    {
        Tween(position, scale, GM.Instance.options.tweenTime);
    }

    public void Tween(Vector3 position, Vector3 scale, float time)
    {
        if (currentTween != null) StopTween();

        currentTween = Go.to(transform, time, new GoTweenConfig()
            .localPosition(position)
            .scale(scale)
            .setEaseType(GoEaseType.ExpoOut));

        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });
    }

    public void TweenLocal(Vector3 position, Quaternion rotation, Vector3 scale, float time)
    {
        if (currentTween != null) StopTween();

        currentTween = Go.to(transform, time, new GoTweenConfig()
            .localPosition(position)
            .localRotation(rotation)
            .scale(scale)
            .setEaseType(GoEaseType.ExpoOut));

        currentTween.setOnCompleteHandler(c => { onMoveComplete(); });
    }

    private void onMoveComplete()
    {
        //ResetTempTransform();
        if (destroyOnTweenEnd) Destroy(gameObject);
    }

    public void StopTween()
    {
        currentTween.destroy();
    }
}