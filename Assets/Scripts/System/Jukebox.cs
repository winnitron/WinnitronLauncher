using UnityEngine;
using UnityEngine.UI;

public class Jukebox : MonoBehaviour {

    public bool isPlaying;

    public Text artistName;
    public Text songName;

    private int currentTrack;

	private AudioSource source;

    public GameObject container;
    public GameObject tweenTarget;
    public GameObject tweenOffScreenTarget;
    public float tweenTime;

    private bool init = true;

    void Start() {
		source = gameObject.GetComponent<AudioSource> ();
        Stop();
    }

    void OnEnable()
    {
        //Just making sure that the very first enable (when the prog starts) doesn't trigger an error
        if (!init)
            PlayRandom();
        else
            init = false;
    }

    void OnDisable()
    {
        Stop();
    }

    void Update() {

	    // Player 2 Joystick controls song
	    if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(2, "left")) || Input.GetKey(KeyCode.Minus))
	        LastTrack();
	    if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(2, "right")) || Input.GetKey(KeyCode.Equals))
	        NextTrack();

	    // Stop & Play from keyboard
	    if (Input.GetKeyDown(GM.Instance.options.keys.GetKey(2, "button1")) || Input.GetKeyDown(GM.Instance.options.keys.GetKey(2, "button2"))) {

	        if (isPlaying)
	            Stop();
	        else
	            PlayRandom();

	    }

	    // Check for song end
	    if (!source.isPlaying && isPlaying)
	        NextTrack();
    }

    private void Stop() {
        source.Stop();
        isPlaying = false;

        container.transform.localPosition = tweenOffScreenTarget.transform.localPosition;
    }

    private void PlayRandom() {
        if (GM.Instance.data.songs.Count > 0) {
            currentTrack = UnityEngine.Random.Range(0, GM.Instance.data.songs.Count);
            source.clip = GM.Instance.data.songs[currentTrack].clip;
            Play();
        }
    }

    private void NextTrack() {

        if (GM.Instance.data.songs.Count <= 0)
            return;

        Stop();

        currentTrack = (currentTrack + 1) % GM.Instance.data.songs.Count;
        source.clip = GM.Instance.data.songs[currentTrack].clip;

        Play();
    }

    private void LastTrack() {

        Stop();

        if (currentTrack <= 1)
            currentTrack = GM.Instance.data.songs.Count - 1;
        else
            currentTrack--;

        source.clip = GM.Instance.data.songs[currentTrack].clip;

        Play();
    }

    private void Play() {
        isPlaying = true;
        source.Play();
        songName.text = GM.Instance.data.songs[currentTrack].name + " - " + GM.Instance.data.songs[currentTrack].author;

        container.GetComponent<Tweenable>().TweenLocalPosition(tweenTarget.transform.localPosition, tweenTime, false);
    }
}
