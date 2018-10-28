using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LauncherGameLabel : MonoBehaviour {

    public TextMeshProUGUI text;
    public int position;

    public Tweenable tween;

    public GameObject targetAbove;
    public GameObject target;
    public GameObject targetBelow;

    public LauncherController controller;

	// Use this for initialization
	void Start () {
        //Hook in the delegates into the UI Controller
        controller.moveDown += MoveDown;
        controller.moveUp += MoveUp;
	}

    public void MoveDown()
    {
        SetToTarget(targetBelow);
    }

    public void MoveUp()
    {
        SetToTarget(targetAbove);
    }

    private void SetToTarget(GameObject obj)
    {
        transform.localPosition = obj.transform.localPosition;
        //transform.localRotation = obj.transform.localRotation;
        transform.localScale = obj.transform.localScale;
        TweenToTarget();
    }

    private void TweenToTarget()
    {
        tween.TweenLocal(target.transform.localPosition, target.transform.localScale, 0.5f);
        //tween.TweenLocalPosition(target.transform.localPosition, 0.5f);

        //Will be tweened but for now snap
        //transform.localPosition = target.transform.localPosition;
        //transform.localRotation = target.transform.localRotation;
    }

    private void OnDestroy()
    {
        controller.moveDown -= MoveDown;
        controller.moveUp -= MoveUp;
    }
}
