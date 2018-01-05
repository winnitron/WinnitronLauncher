using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script just holds a number of GameObjects that the State scripts can then reference in code.
/// </summary>
public class StateMachineHelper : MonoBehaviour {

    public GameObject launcher;
    public GameObject topBar;
    public GameObject oops;
    public GameObject jukebox;
    public GameObject info;
    public GameObject mainCanvas;

    public void DeactivateAll()
    {
        launcher.SetActive(false);
        jukebox.SetActive(false);
    }
}
