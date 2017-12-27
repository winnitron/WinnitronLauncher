using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NUI_GameLabel : MonoBehaviour {

    public Text text;
    public int position;
    private Tweenable tween;
    public GameObject targetAbove;
    public GameObject target;
    public GameObject targetBelow;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        tween = GetComponent<Tweenable>();
	}
	
	// Update is called once per frame
	void Update () {
		
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
        transform.localRotation = obj.transform.localRotation;
        transform.localScale = obj.transform.localScale;
        TweenToTarget();
    }

    private void TweenToTarget()
    {
        tween.TweenLocal(target.transform.localPosition, target.transform.localRotation, target.transform.localScale, 0.5f);

        //Will be tweened but for now snap
        //transform.localPosition = target.transform.localPosition;
        //transform.localRotation = target.transform.localRotation;
    }
}
