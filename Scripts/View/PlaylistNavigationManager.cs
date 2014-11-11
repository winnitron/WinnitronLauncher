using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class PlaylistNavigationManager : MonoBehaviour {

    public GameObject playlistPosition;                 // Object that marks the position where the current selected playlist should be placed
    public GameObject playlistNamePosition;             // Object that marks the position where the playlist name labels will be placed

    public List<Playlist> playlistList;                 // List of all playlists
    public List<PlaylistLabel> playlistLabelList;       // List of all playlist labels

    public Playlist playlistPrefab;
    public PlaylistLabel playlistLabelPrefab;

    public BackgroundPlane backgroundPlane;

    public List<AudioClip> clipList;                    // List of sound effects : 0=Game label tween, 1=switch playlists, 2=launch game 

    public Animation arrowLeft;
    public Animation arrowRight;

    public float tweenTime;                             // Time for movement of playlist and playlist labels

    public float GRID_X_OFFSET_PLAYLIST = 60;
    public float GRID_X_OFFSET_LABEL = 200;

    public float smallScale = 0.7f;

    bool loading = true;                                       // Don't accept input for a period of time at the launch of the scene

    string playlistsDirectory;                          // Path to the directory containing all the playlist directories
    int selectedPlaylistIndex;

    public bool moving { get; set; }


    void Awake() {        

        playlistsDirectory = Path.Combine(Application.dataPath, "Playlists");

        BuildPlaylists(); if (playlistList.Count > 1) selectedPlaylistIndex = 1;
        SortPlaylists();
    }

    void Start() {

        StartCoroutine("waitForLoad");
    }

    void Update() {
        if (!loading && GM.worldState == GM.WorldState.Launcher) {

            // Cycle horizontally through playlists
            if (Input.GetKeyDown(KeyCode.RightArrow)) {

                // Audio
                audio.clip = clipList[1];
                audio.Play();

                // Update the playlist index
                if (selectedPlaylistIndex == 0)
                    selectedPlaylistIndex = playlistList.Count - 1;
                else
                    selectedPlaylistIndex--;

                // Stop all current tweens since they mess with the playlist movement
                foreach (Playlist playlist in playlistList) { playlist.stopTween(); }


                // Tween all the playlists to the proper position based on the updated index
                SortPlaylists();

                arrowRight.Rewind();
                arrowRight.Play();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) {

                // Audio
                audio.clip = clipList[1];
                audio.Play();

                // Update the playlist index
                if (selectedPlaylistIndex >= playlistList.Count - 1)
                    selectedPlaylistIndex = 0;
                else
                    selectedPlaylistIndex++;

                // Stop all current tweens since they mess with the playlist movement
                foreach (Playlist playlist in playlistList) { playlist.stopTween(); }

                // Tween all the playlists to the proper position based on the updated index
                SortPlaylists();

                arrowLeft.Rewind();
                arrowLeft.Play();
            }

            // Cycle through games on current playlist
            if (!moving) {
                // Keyboard, move up and down through the current playlist
                if (Input.GetKeyDown(KeyCode.UpArrow)) {

                    // Audio
                    audio.clip = clipList[0];
                    audio.Play();

                    backgroundPlane.scrollVertical();
                    playlistList[selectedPlaylistIndex].moveUpList();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow)) {

                    // Audio
                    audio.clip = clipList[0];
                    audio.Play();

                    backgroundPlane.scrollVertical();
                    playlistList[selectedPlaylistIndex].moveDownList();
                }

                // Launch game
                if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X)) {

                    // Audio
                    audio.clip = clipList[2];
                    audio.Play();

                    playlistList[selectedPlaylistIndex].selectGame();
                }
            }
        }
    }

    public void BuildPlaylists() {

        // Directory info for the directory containing all the playlist directories
        var playlistsDir = new DirectoryInfo(playlistsDirectory);

        foreach (var dir in playlistsDir.GetDirectories()) {

            // Instantiate a new playlist and set the path to its directory
            Playlist playlist = Instantiate(playlistPrefab) as Playlist;
            playlist.playlistNavigationManager = this;

            // Playlist name
            var directoryName = dir.Name;
            var name = directoryName.Replace('_', ' ');
            playlist.name = "Playlist: " + name;

            playlist.transform.parent = transform;
            playlist.gamesDirectory = Path.Combine(playlistsDirectory, directoryName);
            playlist.buildList();

            playlistList.Add(playlist);

            PlaylistLabel playlistLabel = Instantiate(playlistLabelPrefab) as PlaylistLabel;
            playlistLabel.playlistNavigationManager = this;
            playlistLabel.transform.parent = GameObject.Find("PlaylistLabelHolder").transform;          // Place all playlist labels inside this object to insure their placement in hierarchy and thus sorting order
            playlistLabel.name = "PlaylistLabel: " + name;
            playlistLabel.initializeName(name);
            playlistLabelList.Add(playlistLabel);
        }
    }

    public void SortPlaylists() {

        // Sort the playlists (which contain the game labels and screenshots
        for (int i = 0; i < playlistList.Count; i++) {

            if (i < selectedPlaylistIndex) {

                playlistLabelList[i].move(new Vector3(playlistNamePosition.transform.position.x + (GRID_X_OFFSET_LABEL * (selectedPlaylistIndex - i)), playlistNamePosition.transform.position.y, playlistNamePosition.transform.position.z), new Vector3(smallScale, smallScale, smallScale), tweenTime);
                playlistLabelList[i].setAlpha(0.5f - (Mathf.Abs(selectedPlaylistIndex - i) * 0.1f));

                playlistList[i].move(new Vector3(transform.position.x + (GRID_X_OFFSET_PLAYLIST * (selectedPlaylistIndex - i)), transform.position.y, transform.position.z), new Vector3(1, 1, 1), tweenTime);
            }
            else if (i == selectedPlaylistIndex) {

                playlistLabelList[i].move(new Vector3(playlistNamePosition.transform.position.x, playlistNamePosition.transform.position.y, playlistNamePosition.transform.position.z), new Vector3(1, 1, 1), tweenTime);
                playlistLabelList[i].setAlpha(1);

                playlistList[i].move(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(1, 1, 1), tweenTime);
            }
            else if (i > selectedPlaylistIndex) {

                playlistLabelList[i].move(new Vector3(playlistNamePosition.transform.position.x - (GRID_X_OFFSET_LABEL * (i - selectedPlaylistIndex)), playlistNamePosition.transform.position.y, playlistNamePosition.transform.position.z), new Vector3(smallScale, smallScale, smallScale), tweenTime);
                playlistLabelList[i].setAlpha(0.5f - (Mathf.Abs(selectedPlaylistIndex - i) * 0.1f));

                playlistList[i].move(new Vector3(transform.position.x - (GRID_X_OFFSET_PLAYLIST * (i - selectedPlaylistIndex)), transform.position.y, transform.position.z), new Vector3(1, 1, 1), tweenTime);
            }
        }

        moving = true;
    }

    IEnumerator waitForLoad() {

        yield return new WaitForSeconds(1.5f);

        loading = false;
    }
}
