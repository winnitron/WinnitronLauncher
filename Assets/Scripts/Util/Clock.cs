using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    private Text text;

    //sets something to record the date and time
    private System.DateTime date;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        //tells the var to look for the precise moment you are at
        var date = System.DateTime.Now;
        //converts the above var to a readable string
        text.text = date.ToString("h:mmtt");
    }
}
