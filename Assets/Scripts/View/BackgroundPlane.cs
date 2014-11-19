using UnityEngine;//
using System.Collections;

public class BackgroundPlane : MonoBehaviour {
    
    public GameObject currentPlane;
    public  GameObject otherPlane;

    Vector3 startPos;                       // Position to move the plane too when its out of view

    public float speed = 5;

    float diff;
    

    void Start() {

        diff = otherPlane.transform.position.z - currentPlane.transform.position.z;

        startPos = currentPlane.transform.position;        
    }

    public void scrollVertical() {
        
        animation.Stop();
        animation.Play();
    }

	void Update () {

        // Create a new plane when the current one has gone far enough
        if (otherPlane.transform.localPosition.z > 470) {

            otherPlane.transform.position = new Vector3(otherPlane.transform.position.x, otherPlane.transform.position.y, otherPlane.transform.position.z - (diff * 2));

            var temp = otherPlane;
            otherPlane = currentPlane;
            currentPlane = temp;
        }

        currentPlane.rigidbody.velocity = Vector3.forward * speed;
        otherPlane.rigidbody.velocity = Vector3.forward * speed;     
	}     
}
