using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Jukebox : MonoBehaviour {

    public bool on;

    public Text artistName;
    public Text songName;
	
    public List<Song> songs;

    private int currentTrack;

	private string SONG_SUBDIRECTORY = "Music";
	private string songDirectory;
	private bool doneLoading = false;

	protected void Awake() {
		songDirectory = Path.Combine(Application.dataPath, SONG_SUBDIRECTORY);
		BuildSongList();
	}

    void Init() {
		Debug.Log("songnae: " + songs[1].clip.length);

        currentTrack = UnityEngine.Random.Range(0, songs.Count);
        audio.clip = songs[currentTrack].clip;

        if (on) audio.Play();

        initWdiget();

		doneLoading = true;
    }

    void Update() {
		if(doneLoading) {
	        // Player 2 Joystick controls song
	        if (Input.GetKeyUp(KeyCode.J))
	            lastTrack();
	        if (Input.GetKeyUp(KeyCode.L))
	            nextTrack();

	        // Stop & Play from keyboard
	        if (Input.GetKeyUp(KeyCode.P)) {

	            if (on) {
	                audio.Stop();
	                on = false;
	            }
	            else {
	                audio.Play();
	                on = true;
	            }
	        }

	        // Check for song end
	        if (!audio.isPlaying && on)
	            nextTrack();
		}
    }

    public void stop() {
        audio.Stop();
        on = false;
    }

    public void play() {

        currentTrack = UnityEngine.Random.Range(0, songs.Count);
        audio.clip = songs[currentTrack].clip;
        audio.Play();
        on = true;
    }

    public void nextTrack() {
        
        audio.Stop();

        if (currentTrack >= songs.Count - 1)
            currentTrack = 0;
        else
            currentTrack++;

        audio.clip = songs[currentTrack].clip;

        if (on) audio.Play();
        initWdiget();
    }

    public void lastTrack() {

        audio.Stop();

        if (currentTrack <= 1)
            currentTrack = songs.Count - 1;
        else
            currentTrack--;

        audio.clip = songs[currentTrack].clip;

        if (on) audio.Play();
        initWdiget();
    }

    public void initWdiget() {
        songName.text = songs[currentTrack].name;
        artistName.text = songs[currentTrack].author;
    }

	void BuildSongList()
	{
		// get all valid files
		var info = new DirectoryInfo(songDirectory);
		var songFiles = info.GetFiles();

		Debug.Log("Started Loading Song Files.");

		foreach(var song in songFiles)
		{
			var fileExt = song.FullName.Substring(Math.Max(0, song.FullName.Length - 4));

			Debug.Log ("Checking " + song.Name);
			if(song.Name.Substring(0, 1) != "." && fileExt != "meta") { 

				Debug.Log ("Started loading " + song.FullName);

				WWW audioLoader = new WWW("file://" + song.FullName);
				
				while( !audioLoader.isDone ) {}

				var name = song.Name;
				var author = song.Name;

				Debug.Log ("Done loading " + song.FullName);

				songs.Add(new Song(name, author, audioLoader.GetAudioClip(false)));
			} else {
				Debug.Log ("Skipped " + song.FullName);
			}
		} 

		Init ();
	}
}
