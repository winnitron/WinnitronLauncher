using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NUI_Controller : MonoBehaviour {

    public GameObject gameLabelContainer;
    public NUI_GameLabel[] gameLabels;

    public GameObject playlist;
    public GameObject playlistTweenTargetRight;
    public GameObject playlistTweenTargetSelected;
    public GameObject playlistTweenTargetLeft;

    public Image gameImageSelected;
    public Image gameImageToFade;

    public int gameSelector = 0;
    public int playlistSelector = 0;

    public float tweenTimeBase = 0.25f;

    //Delegates for the tweening UI elements to hook into

    public delegate void MoveUp();
    public MoveUp moveUp;

    public delegate void MoveDown();
    public MoveDown moveDown;

	// Use this for initialization
	void Start () {
        gameLabels = gameLabelContainer.GetComponentsInChildren<NUI_GameLabel>();

        UpdateGameUI(0);
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.UpArrow) && gameSelector > 0)
            UpdateGameUI(-1);

        if (Input.GetKeyDown(KeyCode.DownArrow) && gameSelector < GetCurrentPlaylist().games.Count - 1)
            UpdateGameUI(1);

        if (Input.GetKeyDown(KeyCode.LeftArrow) && playlistSelector > 0)
            MovePlaylist(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) && playlistSelector < GM.data.playlists.Count - 1)
            MovePlaylist(1);
    }

    private void MovePlaylist(int direction)
    {
        //Save the current position of the playlist for later
        GetCurrentPlaylist().selectedIndex = gameSelector;

        //Move the playlist selector
        playlistSelector += direction;

        //Get the selector from the new playlist position
        gameSelector = GetCurrentPlaylist().selectedIndex;

        //Make a copy of the current playlist
        GameObject oldPlaylist = Instantiate(playlist);

        //Make sure it's exactly the same position (there's gotta be a better way to do this)
        oldPlaylist.transform.SetParent(playlist.transform.parent);
        oldPlaylist.transform.localPosition = playlist.transform.localPosition;
        oldPlaylist.transform.localScale = playlist.transform.localScale;
        oldPlaylist.transform.localRotation = playlist.transform.localRotation;

        //Move the origial playlist to the tween target
        if (direction == 1)
        {
            playlist.transform.localPosition = playlistTweenTargetRight.transform.localPosition;
            oldPlaylist.GetComponent<Tweenable>().TweenLocalPosition(playlistTweenTargetLeft.transform.localPosition, tweenTimeBase, true);
        }

        if (direction == -1)
        {
            playlist.transform.localPosition = playlistTweenTargetLeft.transform.localPosition;
            oldPlaylist.GetComponent<Tweenable>().TweenLocalPosition(playlistTweenTargetRight.transform.localPosition, tweenTimeBase, true);
        }

        //Tween the copy offscreen, and the original onscreen
        playlist.GetComponent<Tweenable>().TweenLocalPosition(playlistTweenTargetSelected.transform.localPosition, tweenTimeBase, false);

        UpdateGameUI(0);
    }

    private void UpdateGameUI(int direction)
    {
        gameSelector += direction;

        //Move the Game Labels to their right spots and feed them the right text
        foreach (NUI_GameLabel label in gameLabels)
        {
            if(label.position + gameSelector >= 0 && label.position + gameSelector < GetCurrentPlaylist().games.Count)
                label.text.text = GetCurrentPlaylist().games[gameSelector + label.position].name.ToUpper();

            else
                label.text.text = "";   
        }

        //Move the Game Images to the right spots and feed them the image they need
        gameImageToFade.sprite = gameImageSelected.sprite;
        gameImageToFade.GetComponent<ImageFade>().SetOpaqueAndFade();
        gameImageSelected.sprite = GetCurrentGame().screenshot;

        if (direction == 1) moveDown();
        if (direction == -1) moveUp();

        gameImageSelected.sprite = GetCurrentGame().screenshot;
    }

    private Game GetCurrentGame()
    {
        return GetCurrentPlaylist().games[gameSelector];
    }

    private Playlist GetCurrentPlaylist()
    {
        return GM.data.playlists[playlistSelector];
    }
}
