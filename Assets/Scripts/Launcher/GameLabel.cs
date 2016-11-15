using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLabel : Tweenable {

	public float alpha = 1;

	public void setAlpha(float newAlpha) {
		GetComponent<Text>().color = new Color(1, 1, 1, newAlpha);
	}
}
