using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour {

    public VideoPlayer player;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public VideoClip defaultVideo;
    public VideoClip currentVideo;


    /// <summary>
    /// Plays a new video in the background.
    /// </summary>
    /// <param name="newVideo">A VideoClip to play in the background.</param>
    /// <returns>Whether it was successful.</returns>
    public void PlayVideo(VideoClip newVideo, bool loop, AudioClip clip)
    {
        //Stop the video just in case
        player.Stop();

        //Set the looping stuffs
        player.isLooping = true;
        if (!loop) player.isLooping = false;
        
        if (newVideo != null)
            player.clip = newVideo;
        
        else
            player.clip = defaultVideo;

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        player.Play();
    }

    public void PlayVideo(VideoClip newVideo, bool loop)
    {
        PlayVideo(newVideo, loop, null);
    }
}
