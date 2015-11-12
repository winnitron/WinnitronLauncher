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
    

    public PlaylistManager playlistPrefab;
    public PlaylistLabel playlistLabelPrefab;

    public BackgroundPlane backgroundPlane;

    public List<AudioClip> clipList;                    // List of sound effects : 0=Game label tween, 1=switch playlists, 2=launch game 

    public Animation arrowLeft;
    public Animation arrowRight;

    public float tweenTime;                             // Time for movement of playlist and playlist labels

    public float GRID_X_OFFSET_PLAYLIST = 60;
    public float GRID_X_OFFSET_LABEL = 200;

    public float smallScale = 0.7f;


    List<PlaylistLabel> playlistLabelList = new List<PlaylistLabel>();       // List of all playlist labels

    bool loading = true;                                       // Don't accept input for a period of time at the launch of the scene
  
    int selectedPlaylistIndex;

    public bool hmoving { get; set; }
	public bool vmoving { get; set; }
	public bool moving { get; set; }

    public List<PlaylistManager> playlistManagerList { get; set; }    


    void Start() {

        playlistManagerList = new List<PlaylistManager>();
        StartCoroutine("waitForLoad");
    }

    void Update() {
        
        if (!loading && GM.worldState == GM.WorldState.Launcher) {
		
	            // Cycle horizontally through playlists
                 if (playlistManagerList.Count > 1 && Input.GetKeyDown(KeyCode.RightArrow)) {

	                // Audio
	                GetComponent<AudioSource>().clip = clipList[1];
	                GetComponent<AudioSource>().Play();

	                // Update the playlist index
	                if (selectedPlaylistIndex == 0)
	                    selectedPlaylistIndex = playlistManagerList.Count - 1;
	                else
	                    selectedPlaylistIndex--;

	                // Stop all current tweens since they mess with the playlist movement
	                foreach (PlaylistManager playlist in playlistManagerList) { playlist.stopTween(); }


	                // Tween all the playlists to the proper position based on the updated index
	                sortPlaylists();

	                arrowRight.Rewind();
	                arrowRight.Play();
	            }
                 else if (playlistManagerList.Count > 1 && Input.GetKeyDown(KeyCode.LeftArrow)) {

	                // Audio
	                GetComponent<AudioSource>().clip = clipList[1];
	                GetComponent<AudioSource>().Play();

	                // Update the playlist index
	                if (selectedPlaylistIndex >= playlistManagerList.Count - 1)
	                    selectedPlaylistIndex = 0;
	                else
	                    selectedPlaylistIndex++;

	                // Stop all current tweens since they mess with the PlaylistManager movement
	                foreach (PlaylistManager playlist in playlistManagerList) { playlist.stopTween(); }

	                // Tween all the playlists to the proper position based on the updated index
	                sortPlaylists();

	                arrowLeft.Rewind();
	                arrowLeft.Play();
	            }

            // Cycle through games on current playlist
			//only do it if we're not moving horz
            if (!moving) {
                // Keyboard, move up and down through the current playlist
                if (Input.GetKeyDown(KeyCode.UpArrow)) {

                    // Audio
                    GetComponent<AudioSource>().clip = clipList[0];
                    GetComponent<AudioSource>().Play();

                    backgroundPlane.scrollVertical(1);
                    playlistManagerList[selectedPlaylistIndex].moveUpList();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow)) {

                    // Audio
                    GetComponent<AudioSource>().clip = clipList[0];
                    GetComponent<AudioSource>().Play();

                    backgroundPlane.scrollVertical(-1);
                    playlistManagerList[selectedPlaylistIndex].moveDownList();
                }

                // Launch game
                if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X)) {

                    // Audio
                    GetComponent<AudioSource>().clip = clipList[2];
                    GetComponent<AudioSource>().Play();

                    playlistManagerList[selectedPlaylistIndex].selectGame();
                }
            }
        }
    }

    public void BuildPlaylists() {

        foreach (var playlist in DataManager.Instance.playlists) {

            // Instantiate a new playlist and set the path to its directory
            PlaylistManager playlistManager = Instantiate(playlistPrefab) as PlaylistManager;
            playlistManager.playlistNavManager = this;
            playlistManager.playlist = playlist;
                        
            playlistManager.name = "Playlist: " + playlist.name;

            playlistManager.transform.SetParent(transform);            
            playlistManager.buildList();

            playlistManagerList.Add(playlistManager);

            PlaylistLabel playlistLabel = Instantiate(playlistLabelPrefab) as PlaylistLabel;
            playlistLabel.playlistNavigationManager = this;
            playlistLabel.transform.SetParent (GameObject.Find("PlaylistLabelHolder").transform);          // Place all playlist labels inside this object to insure their placement in hierarchy and thus sorting order
            playlistLabel.name = "PlaylistLabel: " + playlist.name;
            playlistLabel.initializeName(playlist.name);
            playlistLabelList.Add(playlistLabel);

            sortPlaylists();
        }

        // Check whether there is more than one playlist, if there is only one, deactivate the arrow graphics on either side of the label
        if (playlistManagerList.Count <= 1) {
            arrowRight.gameObject.SetActive(false);
            arrowLeft.gameObject.SetActive(false);
        }
    }

    public void sortPlaylists() {

        // Sort the playlists (which contain the game labels and screenshots
        for (int i = 0; i < playlistManagerList.Count; i++) {

            if (i < selectedPlaylistIndex) {

                playlistLabelList[i].move(new Vector3(playlistNamePosition.transform.position.x + (GRID_X_OFFSET_LABEL * (selectedPlaylistIndex - i)), playlistNamePosition.transform.position.y, playlistNamePosition.transform.position.z), new Vector3(smallScale, smallScale, smallScale), tweenTime);
                playlistLabelList[i].setAlpha(0.5f - (Mathf.Abs(selectedPlaylistIndex - i) * 0.1f));

                playlistManagerList[i].move(new Vector3(transform.position.x + (GRID_X_OFFSET_PLAYLIST * (selectedPlaylistIndex - i)), transform.position.y, transform.position.z), new Vector3(1, 1, 1), tweenTime);
            }
            else if (i == selectedPlaylistIndex) {

                playlistLabelList[i].move(new Vector3(playlistNamePosition.transform.position.x, playlistNamePosition.transform.position.y, playlistNamePosition.transform.position.z), new Vector3(1, 1, 1), tweenTime);
                playlistLabelList[i].setAlpha(1);

                playlistManagerList[i].move(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(1, 1, 1), tweenTime);
            }
            else if (i > selectedPlaylistIndex) {

                playlistLabelList[i].move(new Vector3(playlistNamePosition.transform.position.x - (GRID_X_OFFSET_LABEL * (i - selectedPlaylistIndex)), playlistNamePosition.transform.position.y, playlistNamePosition.transform.position.z), new Vector3(smallScale, smallScale, smallScale), tweenTime);
                playlistLabelList[i].setAlpha(0.5f - (Mathf.Abs(selectedPlaylistIndex - i) * 0.1f));

                playlistManagerList[i].move(new Vector3(transform.position.x - (GRID_X_OFFSET_PLAYLIST * (i - selectedPlaylistIndex)), transform.position.y, transform.position.z), new Vector3(1, 1, 1), tweenTime);
            }
        }

        moving = true;
    }

    IEnumerator waitForLoad() {

        yield return new WaitForSeconds(1.5f);

        loading = false;
    }
}
