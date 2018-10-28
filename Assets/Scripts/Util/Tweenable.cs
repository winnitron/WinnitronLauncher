using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Tweenable : MonoBehaviour {

    //GoTween currentTween;
    private bool destroyOnTweenEnd = false;
    private Sequence currentTween;

    public void TweenPosition(Vector3 position)
    {
        ResetTween();
        currentTween.Append(transform.DOMove(position, GM.Instance.options.tweenTime, false).OnComplete(MoveComplete));
        FinalizeTween();
    }

    public void TweenLocalPosition(Vector3 position, float tweenTime, bool destroyOnEnd)
    {
        ResetTween();
        currentTween.Append(transform.DOLocalMove(position, GM.Instance.options.tweenTime, false).OnComplete(MoveComplete));
        FinalizeTween(destroyOnEnd);
    }

    public void Tween(Vector3 position, Vector3 scale)
    {
        Tween(position, scale, GM.Instance.options.tweenTime);
    }

    public void Tween(Vector3 position, Vector3 scale, float time)
    {
        ResetTween();
        currentTween.Append(transform.DOMove(position, GM.Instance.options.tweenTime, false).OnComplete(MoveComplete));
        currentTween.Append(transform.DOScale(scale, GM.Instance.options.tweenTime));
        FinalizeTween();
    }

    public void TweenLocal(Vector3 position, Quaternion rotation, Vector3 scale, float time)
    {
        ResetTween();
        currentTween.Append(transform.DOLocalMove(position, GM.Instance.options.tweenTime, false).OnComplete(MoveComplete));
        currentTween.Append(transform.DOScale(scale, GM.Instance.options.tweenTime));
        FinalizeTween();

    }

    private void FinalizeTween(bool destroyOnEnd = false)
    {
        destroyOnTweenEnd = destroyOnEnd;
        currentTween.SetEase(DG.Tweening.Ease.OutExpo);
        currentTween.PlayForward();
    }

    private void MoveComplete()
    {
        if (destroyOnTweenEnd)
        {
            Destroy(gameObject);
        }
    }

    public void ResetTween()
    {
        currentTween.Kill();
        currentTween = DOTween.Sequence();
    }
}