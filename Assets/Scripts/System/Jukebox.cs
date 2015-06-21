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
	private AudioSource source;

	protected void Awake() {
		source = gameObject.GetComponent<AudioSource> ();
		songDirectory = Path.Combine(Application.dataPath, SONG_SUBDIRECTORY);
		BuildSongList();
	}

    void Init() {
		Debug.Log ("Initializing jukebox.");

		nextTrack ();

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
	                source.Stop();
	                on = false;
	            }
	            else {
	                source.Play();
	                on = true;
	            }
	        }

	        // Check for song end
	        if (!source.isPlaying && on)
	            nextTrack();

			//Debug.Log ("Jukebox is playing: " + source.isPlaying);
		}
    }

    public void stop() {
        source.Stop();
        on = false;
    }

    public void play() {

        currentTrack = UnityEngine.Random.Range(0, songs.Count);
        source.clip = songs[currentTrack].clip;
        source.Play();
        on = true;
    }

    public void nextTrack() {
        
        source.Stop();

        if (currentTrack >= songs.Count - 1)
            currentTrack = 0;
        else
            currentTrack++;

        source.clip = songs[currentTrack].clip;

        if (on) source.Play();
        initWdiget();
    }

    public void lastTrack() {

        source.Stop();

        if (currentTrack <= 1)
            currentTrack = songs.Count - 1;
        else
            currentTrack--;

        source.clip = songs[currentTrack].clip;

        if (on) source.Play();
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

			if(song.Name.Substring(0, 1) != "." && fileExt != "meta") { 

				Debug.Log ("Started loading " + song.FullName);

				WWW audioLoader = new WWW("file://" + song.FullName);

				while( !audioLoader.isDone ) {}

				//Figure out the song/author names
				//take out the file extension
				var fullName = song.Name.Replace(".ogg", "");

				//find the '-' and split the string
				string[] words = fullName.Split('-');

				//First half is the song title, second the author
				var name = words[0];
				var author = words[1];

				//We're done!
				Debug.Log ("Can load song: " + audioLoader.GetAudioClip(false));
				songs.Add(new Song(name, author, audioLoader.GetAudioClip(false)));
				Debug.Log ("Done loading " + song.FullName);
			} else {
				Debug.Log ("Skipped " + song.FullName + " // not and .ogg");
			}
		} 

		Init ();
	}
}
