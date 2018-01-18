using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script just holds a number of GameObjects that the State scripts can then reference in code.
/// This is because you cannot assign in scene objects to public variables of a state behaviour.
/// </summary>
public class StateMachineHelper : MonoBehaviour {

    public GameObject launcher;
    public GameObject topBar;
    public GameObject jukebox;
    public GameObject info;
    public GameObject mainCanvas;
    public GameObject attract;
    public Image attractImage;
    public Text attractText;

    public bool oopsIsCritical = false;

    /// <summary>
    /// Just a quick way to deactivate all the specific objects used by various states.
    /// </summary>
    public void DeactivateAll()
    {
        info.SetActive(false);
        launcher.SetActive(false);
        jukebox.SetActive(false);
        attract.SetActive(false);
    }
}
