using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NUI_GameLabel : MonoBehaviour {

    public Text text;
    public int position;
    public GameObject targetAbove;
    public GameObject target;
    public GameObject targetBelow;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveUp()
    {
        SetToTarget(targetBelow);
        TweenToTarget();
    }

    public void MoveDown()
    {
        SetToTarget(targetAbove);
        TweenToTarget();
    }

    private void SetToTarget(GameObject obj)
    {
        transform.localPosition = obj.transform.localPosition;
        transform.localRotation = obj.transform.localRotation;
    }

    private void TweenToTarget()
    {
        //Will be tweened but for now snap
        transform.localPosition = target.transform.localPosition;
        transform.localRotation = target.transform.localRotation;
    }
}
