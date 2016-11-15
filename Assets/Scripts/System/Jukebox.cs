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

    private int currentTrack;

	private bool doneLoading = false;
	private AudioSource source;

	protected void Awake() {
		source = gameObject.GetComponent<AudioSource> ();
        StartCoroutine("Init");
	}

    IEnumerator Init() {

        //Don't do anything until the paths have come through
        while (GM.options.initializing) yield return null;

		nextTrack ();

		doneLoading = true;
    }

    void Update() {
		if(doneLoading) {
	        // Player 2 Joystick controls song
	        if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Minus))
	            lastTrack();
	        if (Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.Equals))
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
		}
    }

    public void stop() {
        source.Stop();
        on = false;
    }

    public void play() {

        currentTrack = UnityEngine.Random.Range(0, GM.data.songs.Count);
        source.clip = GM.data.songs[currentTrack].clip;
        source.Play();
        on = true;
    }

    public void nextTrack() {
        
        source.Stop();

        if (currentTrack >= GM.data.songs.Count - 1)
            currentTrack = 0;
        else
            currentTrack++;

        source.clip = GM.data.songs[currentTrack].clip;

        if (on) source.Play();
        initWdiget();
    }

    public void lastTrack() {

        source.Stop();

        if (currentTrack <= 1)
            currentTrack = GM.data.songs.Count - 1;
        else
            currentTrack--;

        source.clip = GM.data.songs[currentTrack].clip;

        if (on) source.Play();
        initWdiget();
    }

    public void initWdiget() {
        songName.text = GM.data.songs[currentTrack].name;
        artistName.text = GM.data.songs[currentTrack].author;
    }
}
