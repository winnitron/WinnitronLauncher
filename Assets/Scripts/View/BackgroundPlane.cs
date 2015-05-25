using UnityEngine;//
using System.Collections;

public class BackgroundPlane : MonoBehaviour {


    public void scrollVertical() {
        
        GetComponent<Animation>().Stop();
        GetComponent<Animation>().Play();
    }

}
