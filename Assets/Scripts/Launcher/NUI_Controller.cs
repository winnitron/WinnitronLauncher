using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NUI_Controller : MonoBehaviour {

    public GameObject playlistLabelContainer;

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

    //Need a flag to update in LateUpdate
    public bool refreshUI = true;

    //Delegates for the tweening UI elements to hook into
    public delegate void MoveUp();
    public MoveUp moveUp;

    public delegate void MoveDown();
    public MoveDown moveDown;

    public delegate void RefreshLauncherUI();
    public RefreshLauncherUI refreshLauncherUI;

    private void OnEnable()
    {
        UpdateLauncherUI();
    }

    // Update is called once per frame
    void Update ()
    {

        if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(1, "up")) && gameSelector > 0)
            MoveGames(-1);

        if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(1, "down")) && gameSelector < GetCurrentPlaylist().games.Count - 1)
            MoveGames(1);

        if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(1, "left")) && playlistSelector > 0)
            MovePlaylist(-1);

        if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(1, "right")) && playlistSelector < GM.Instance.data.playlists.Count - 1)
            MovePlaylist(1);

        if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(1, "button1")) ||
            Input.GetKeyDown(GM.Instance.options.keys.GetKey(1, "button2")) ||
            Input.GetKeyDown(KeyCode.Escape))
            LaunchGame();

        if (Input.GetKeyDown(KeyCode.Space))
            UpdateLauncherUI();
    }

    private void LaunchGame()
    {
        Debug.Log("Attempting to run game");
        GM.Instance.runner.Run(GetCurrentGame());
    }


    private void MoveGames(int direction)
    {
        //Apply the direciton
        gameSelector += direction;

        //Call delegates.  Used in the Game Labels
        if (direction == 1) moveDown();
        if (direction == -1) moveUp();

        //Update the list
        UpdateLauncherUI();
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

        UpdateLauncherUI();
    }


    public void UpdateLauncherUI()
    {
        //Set the toggle so it updates in LateUpdate
        refreshUI = true;
    }


    void LateUpdate()
    {
        if (refreshUI)
        {
            
            if (refreshLauncherUI != null)
            {
                //Updates the list of games here.
                UpdateGameList();

                //Fire the delegate in case anything's linked in
                //Game Info uses this so far.
                refreshLauncherUI();

                //Reset that flag.
                refreshUI = false;
            }
        }
    }

    /// <summary>
    /// Makes sure that all the games are in the right spots.
    /// </summary>
    private void UpdateGameList()
    {
        //Update the playlist name
        playlistLabelContainer.GetComponent<Text>().text = GetCurrentPlaylist().name.ToUpper();

        //Move the Game Labels to their right spots and feed them the right text
        foreach (NUI_GameLabel label in gameLabels)
        {
            if (label.position + gameSelector >= 0 && label.position + gameSelector < GetCurrentPlaylist().games.Count)
                label.text.text = GetCurrentPlaylist().games[gameSelector + label.position].name.ToUpper();

            else
            {
                label.text.text = "";
            }
        }

        //Move the Game Images to the right spots and feed them the image they need
        gameImageToFade.sprite = gameImageSelected.sprite;
        gameImageToFade.GetComponent<ImageFade>().SetOpaqueAndFade();
        gameImageSelected.sprite = GetCurrentGame().screenshot;
    }


    public Game GetCurrentGame()
    {
        return GetCurrentPlaylist().games[gameSelector];
    }

    public Playlist GetCurrentPlaylist()
    {
        return GM.Instance.data.playlists[playlistSelector];
    }
}
