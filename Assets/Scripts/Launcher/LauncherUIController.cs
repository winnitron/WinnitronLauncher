using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls all aspects of the launcher UI.  Creates and sorts the playlists (which will have their own controllers)
/// </summary>
public class LauncherUIController : MonoBehaviour
{

    public List<Playlist> playlistData;

    public GameObject playlistsContainer;                 // Object that marks the position where the current selected playlist should be placed
    public GameObject playlistLabelsContainer;             // Object that marks the position where the playlist name labels will be placed

    public GameObject playlistUIControllerPrefab;
    public GameObject playlistLabelPrefab;

    public Animation arrowLeft;
    public Animation arrowRight;

    public float tweenTime;                             // Time for movement of playlist and playlist labels

    public Vector3 offsetPlaylists;
    public Vector3 offsetPlaylistLabels;

    public float smallScale = 0.7f;

    public List<PlaylistLabel> playlistLabelList;       // List of all playlist labels
    public List<PlaylistUIController> playlistControllers;

    public int selectedPlaylistIndex;

    public float launchTimeout = 0.25f;
    private float idleTime = 0;

    public void Init()
    {
        //Hook in the UpdateData function to the DataManager's
        //Update data function so we always get the data at the
        //right time
        GM.dbug.Log(this, "LauncherUIController: Hooking in OnDataUpdated delegate");
        GM.data.OnDataUpdated += UpdateData;
    }

    void UpdateData()
    {
        //Called when the DataManager finishes updating the data model
        GM.dbug.Log(this, "LauncherUIController: Updating Data!");
        playlistData = GM.data.playlists;
        BuildPlaylists();
    }

    void Update()
    {
        if (GM.state.worldState == StateManager.WorldState.Launcher)
        {
            if (Input.GetKeyDown(KeyCode.Equals))
                DeleteOldPlaylistStuff();

            if (Input.GetKeyDown(GM.options.keys.GetKey(1, "left")))
                PreviousPlaylist();

            if (Input.GetKeyDown(GM.options.keys.GetKey(1, "right")))
                NextPlaylist();

            if (Input.GetKeyDown(GM.options.keys.GetKey(1, "up")))
                PreviousGame();

            if (Input.GetKeyDown(GM.options.keys.GetKey(1, "down")))
                NextGame();

            if (idleTime >= 0)
            {
                if (Input.GetKeyDown(GM.options.keys.GetKey(1, "button1")) || Input.GetKeyDown(GM.options.keys.GetKey(1, "button2")) || Input.GetKeyDown(KeyCode.Escape))
                    SelectGame();
            }

            //Increase idle time
            idleTime += Time.deltaTime;

            //Reset idle time if a key is pressed
            if (Input.anyKey && idleTime >= 0) idleTime = 0;

            //Go into Attract mode is key isn't pressed for a while
            if (idleTime > GM.options.launcherIdleTimeBeforeAttract)
                GM.state.ChangeState(StateManager.WorldState.Attract);
        }

        else
        {
            //Since it's not the attract state, reset the idle timer
            //Make it negative so that there's some buffer before you can launch a game
            idleTime = -launchTimeout;
        }
    }

    private void BuildPlaylists()
    {
        DeleteOldPlaylistStuff();

        foreach (var playlist in playlistData)
        {

            // Instantiate a new playlist and set the path to its directory
            GameObject newPlaylistController = Instantiate(playlistUIControllerPrefab, playlistsContainer.transform, true) as GameObject;
            PlaylistUIController newPlaylist = newPlaylistController.GetComponent<PlaylistUIController>();
            newPlaylist.playlist = playlist;
            newPlaylist.Init();
            playlistControllers.Add(newPlaylist);

            GameObject newPlaylistLabel = Instantiate(playlistLabelPrefab, playlistLabelsContainer.transform, true) as GameObject;
            newPlaylistLabel.GetComponent<PlaylistLabel>().name = "PlaylistLabel: " + playlist.name;
            newPlaylistLabel.GetComponent<PlaylistLabel>().initializeName(playlist.name);
            playlistLabelList.Add(newPlaylistLabel.GetComponent<PlaylistLabel>());

        }

        // Check whether there is more than one playlist, if there is only one, deactivate the arrow graphics on either side of the label
        if (playlistData.Count <= 1)
        {
            arrowRight.gameObject.SetActive(false);
            arrowLeft.gameObject.SetActive(false);
        }

        RepositionPlaylists();
    }

    private void RepositionPlaylists()
    {
        var i = 0;
        foreach(var playlist in playlistControllers)
        {
            var relativeIndex = i - selectedPlaylistIndex;
            var thisOffset = offsetPlaylists * relativeIndex;

            //Position
            Vector3 newPosition = playlistsContainer.transform.position + thisOffset;

            //Scale
            playlist.transform.localScale = new Vector3(1, 1, 1);

            //Commit
            playlist.TweenPosition(newPosition);

            i++;
        }

        i = 0;
        foreach (var playlistLabel in playlistLabelList)
        {
            var relativeIndex = i - selectedPlaylistIndex;
            var thisOffset = offsetPlaylistLabels * relativeIndex;

            //Position
            Vector3 newPosition = thisOffset;

            //Scale
            playlistLabel.transform.localScale = new Vector3(1, 1, 1);

            //Commit
            playlistLabel.TweenLocalPosition(newPosition);

            i++;
        }
    }

    private void DeleteOldPlaylistStuff()
    {
        foreach (Transform child in playlistsContainer.transform)
            Destroy(child.gameObject);

        foreach (Transform child in playlistLabelsContainer.transform)
            Destroy(child.gameObject);

        playlistLabelList = new List<PlaylistLabel>();       // List of all playlist labels
        playlistControllers = new List<PlaylistUIController>();
}

    private void NextPlaylist()
    {
        selectedPlaylistIndex++;

        if (selectedPlaylistIndex >= playlistData.Count)
            selectedPlaylistIndex = 0;

        RepositionPlaylists();
    }

    private void PreviousPlaylist()
    {
        selectedPlaylistIndex--;

        if (selectedPlaylistIndex < 0)
            selectedPlaylistIndex = playlistData.Count - 1;

        RepositionPlaylists();
    }

    private void NextGame()
    {
        playlistControllers[selectedPlaylistIndex].NextGame();
    }

    private void PreviousGame()
    {
        playlistControllers[selectedPlaylistIndex].PreviousGame();
    }

    private void SelectGame()
    {
        GM.runner.Run(playlistControllers[selectedPlaylistIndex].GetCurrentGame());
    }
}
