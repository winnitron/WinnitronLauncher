using UnityEngine;
using UnityEngine.UI;

public class Jukebox : MonoBehaviour {

    public bool isPlaying;

    public Text artistName;
    public Text songName;

    private int currentTrack;

	private AudioSource source;

	protected void Start() {
		source = gameObject.GetComponent<AudioSource> ();
        NextTrack();
    }

    void Update() {

	    // Player 2 Joystick controls song
	    if (Input.GetKeyDown(GM.options.keys.P2Left) || Input.GetKey(KeyCode.Minus))
	        LastTrack();
	    if (Input.GetKeyDown(GM.options.keys.P2Right) || Input.GetKey(KeyCode.Equals))
	        NextTrack();

	    // Stop & Play from keyboard
	    if (Input.GetKeyDown(GM.options.keys.P2Button1) || Input.GetKeyDown(GM.options.keys.P2Button2)) {

	        if (isPlaying)
	            Stop();
	        else
	            PlayRandom();

	    }

	    // Check for song end
	    if (!source.isPlaying && isPlaying)
	        NextTrack();

        //Make sure there's a track playing when the launcher is going
        if (GM.state.worldState == StateManager.WorldState.Launcher && !isPlaying)
            PlayRandom();
    }

    public void Stop() {
        source.Stop();
        isPlaying = false;
    }

    public void PlayRandom() {

        currentTrack = UnityEngine.Random.Range(0, GM.data.songs.Count);
        source.clip = GM.data.songs[currentTrack].clip;
        Play();
    }

    public void NextTrack() {

        if (GM.data.songs.Count <= 0)
            return;

        Stop();

        currentTrack = (currentTrack + 1) % GM.data.songs.Count;
        source.clip = GM.data.songs[currentTrack].clip;

        Play();
    }

    public void LastTrack() {

        Stop();

        if (currentTrack <= 1)
            currentTrack = GM.data.songs.Count - 1;
        else
            currentTrack--;

        source.clip = GM.data.songs[currentTrack].clip;

        Play();
    }

    public void Play() {
        isPlaying = true;
        source.Play();
        songName.text = GM.data.songs[currentTrack].name;
        artistName.text = GM.data.songs[currentTrack].author;
    }
}
