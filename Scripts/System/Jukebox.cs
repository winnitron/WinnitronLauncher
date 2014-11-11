using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Jukebox : MonoBehaviour {

    public bool on;

    public Text artistName;
    public Text songName;


    private Song[] songs;

    private int currentTrack;


    void Start() {

        songs = GetComponentsInChildren<Song>();

        currentTrack = Random.Range(0, songs.Length);
        audio.clip = songs[currentTrack].clip;
        if (on) audio.Play();

        initWdiget();
    }

    void Update() {

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

    public void stop() {
        audio.Stop();
        on = false;
    }

    public void play() {

        currentTrack = Random.Range(0, songs.Length);
        audio.clip = songs[currentTrack].clip;
        audio.Play();
        on = true;
    }

    public void nextTrack() {
        
        audio.Stop();

        if (currentTrack >= songs.Length - 1)
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
            currentTrack = songs.Length - 1;
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
}
