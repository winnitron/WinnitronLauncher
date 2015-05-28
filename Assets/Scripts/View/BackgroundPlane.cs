using UnityEngine;//
using System.Collections;

public class BackgroundPlane : MonoBehaviour {

	public float speedUpAmount = 10f;
	public float decayAmount = 0.1f;

	public float amountZ = 0;
	public float amountX = 0;

	public void scrollHorizontal() {
		amountX = speedUpAmount;
	}

    public void scrollVertical() {
		amountZ = speedUpAmount;
    }

	void Update() {
		transform.Translate(new Vector3(amountX, 0, amountZ));

		amountX *= decayAmount;
		amountZ *= decayAmount;
	}

}
