using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherFlatUI : MonoBehaviour {

    public GameObject gameLabelContainer;
    public NUI_GameLabel[] gameLabels;

    public int gameSelector = 0;
    public int playlistSelector = 0;

	// Use this for initialization
	void Start () {
        gameLabels = gameLabelContainer.GetComponentsInChildren<NUI_GameLabel>();

        RefreshUI();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveUp();

        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveDown();
	}

    private void MoveUp()
    {
        //if(gameSelector > 0) 
        gameSelector--;
        
        RefreshUI();
    }

    private void MoveDown()
    {
        //if(gameSelector < GM.data.playlists[playlistSelector].games.Count) 
        gameSelector++;
        
        RefreshUI();
    }

    private void RefreshUI()
    {
        foreach (NUI_GameLabel label in gameLabels)
        {
            if(label.position + gameSelector >= 0 && label.position + gameSelector < GM.data.playlists[playlistSelector].games.Count)
            {
                label.text.text = GM.data.playlists[playlistSelector].games[gameSelector + label.position].name.ToUpper();
            }

            else
            {
                label.text.text = "";
            }
        }
    }
}
