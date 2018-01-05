using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// This class handles all the full-screen videos in the launcher.
/// </summary>
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
    /// <param name="loop">Whether the video should loop.</param>
    /// <param name="audioClip">If there is a audio component to the video.</param>
    /// <returns>Whether it was successful.</returns>
    public void PlayVideo(VideoClip newVideo, bool loop, AudioClip audioClip)
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

        //There's a bug in Unity 2017.1 where the audio doesn't play from a
        //VideoPlayer, so this is a hacky way of fixing that by playing the
        //sound separately for now. :/
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        player.Play();
    }

    /// <summary>
    /// Overloaded method where it assumes that there is no audio with the video.
    /// </summary>
    /// <param name="newVideo"></param>
    /// <param name="loop"></param>
    public void PlayVideo(VideoClip newVideo, bool loop)
    {
        PlayVideo(newVideo, loop, null);
    }

    /// <summary>
    /// Just stops any video that's currently playing.
    /// </summary>
    public void StopVideo()
    {
        player.Stop();
    }
}
