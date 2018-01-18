using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the main controller for the Launcher interface.
/// </summary>
public class LauncherController : MonoBehaviour {

    public GameObject playlistLabelContainer;

    public GameObject gameLabelContainer;
    public LauncherGameLabel[] gameLabels;

    public GameObject playlist;
    public GameObject playlistTweenTargetRight;
    public GameObject playlistTweenTargetSelected;
    public GameObject playlistTweenTargetLeft;

    public Image gameImageSelected;
    public Image gameImageToFade;

    public GameObject arrowRight;
    public GameObject arrowLeft;

    public GameObject whiteScreenFade;

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

    /// <summary>
    /// The LauncherState behaviour on the GM's Animator component will toggle this GameObject
    /// on and off depending on if it's the launcher state.  When it gets toggled on, we just
    /// need to make sure that the UI is properly updated with the latest data.
    /// </summary>
    private void OnEnable()
    {
        whiteScreenFade.SetActive(true);
        UpdateLauncherUI();
    }

    private void OnDisable()
    {
        whiteScreenFade.SetActive(false);
    }

    /// <summary>
    /// This is where we get all the inputs for the launcher.
    /// </summary>
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

    /// <summary>
    /// Called by the Inputs in the Update function, this will increment the game selector
    /// and make sure that the UI gets updated.
    /// </summary>
    /// <param name="direction">-1 for up, 1 for down, 0 for refreshing the UI.</param>
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

    /// <summary>
    /// Called by the Inputs in the Update function, this will increment the playlist selector
    /// and make sure the UI gets updated properly.
    /// </summary>
    /// <param name="direction">-1 for left, 1 for right, 0 to just refresh UI.</param>
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

    /// <summary>
    /// Makes sure the UI is displaying the correct info.  We do this by setting a toggle and having it update
    /// in LateUpdate so that we make sure that we have the latest information after button presses etc.
    /// </summary>
    public void UpdateLauncherUI()
    {
        //Set the toggle so it updates in LateUpdate
        refreshUI = true;
    }

    /// <summary>
    /// This is where we actually apply the Launcher refresh.
    /// </summary>
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

        if (playlistSelector == 0) arrowLeft.SetActive(false);
        else arrowLeft.SetActive(true);

        if (playlistSelector == GM.Instance.data.playlists.Count - 1) arrowRight.SetActive(false);
        else arrowRight.SetActive(true);

        //Move the Game Labels to their right spots and feed them the right text
        foreach (LauncherGameLabel label in gameLabels)
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

    /// <summary>
    /// Checks in with GM's Data Manager component and retrieves the currently selected Game class.
    /// </summary>
    /// <returns>Populated Game class</returns>
    public Game GetCurrentGame()
    {
        return GetCurrentPlaylist().games[gameSelector];
    }

    /// <summary>
    /// Checks in with GM's Data Manager component and retrieves the currently selected Playlist class.
    /// </summary>
    /// <returns>Populated Playlist class</returns>
    public Playlist GetCurrentPlaylist()
    {
        return GM.Instance.data.playlists[playlistSelector];
    }
}
