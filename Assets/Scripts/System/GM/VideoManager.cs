using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// This class handles all the full-screen videos in the launcher.
/// </summary>
public class VideoManager : MonoBehaviour {

    public enum VideoState { Loading, Playing, Stopped }
    public VideoState state = VideoState.Stopped;

    public VideoPlayer player;
    public AudioSource audioSource;
    public string currentVideo;

    /// <summary>
    /// Plays a new video in the background.
    /// </summary>
    /// <param name="newVideo">A VideoClip to play in the background.</param>
    /// <param name="loop">Whether the video should loop.</param>
    /// <param name="audioClip">If there is a audio component to the video.</param>
    /// <returns>Whether it was successful.</returns>
    public void PlayVideo(string url, bool loop, bool playAudio)
    {
        StopVideo();

        if (url != "" && url != null)
        {
            state = VideoState.Loading;

            currentVideo = url;
            player.isLooping = loop;
            audioSource.enabled = playAudio;

            StartCoroutine(playVideo(url));
        }
    }

    /// <summary>
    /// Overloaded method where it assumes that there is no audio with the video.
    /// </summary>
    /// <param name="newVideo"></param>
    /// <param name="loop"></param>
    public void PlayVideo(VideoClip newVideo, bool loop, bool playAudio)
    {
        PlayVideo(newVideo.originalPath, loop, playAudio);
    }

    IEnumerator playVideo(string url)
    {
        //Disable Play on Awake for both Video and Audio
        player.playOnAwake = true;
        audioSource.playOnAwake = true;

        //We want to play from video clip not from url
        player.source = VideoSource.Url;
        player.url = currentVideo;

        //Set video To Play then prepare Audio to prevent Buffering
        player.Prepare();

        GM.Instance.logger.Debug("Preparing Video " + url);

        //Wait until video is prepared
        while (!player.isPrepared)
        {

            yield return null;
        }

        GM.Instance.logger.Debug("Done Preparing Video " + url);

        //Play Video
        state = VideoState.Playing;
        player.Play();

        GM.Instance.logger.Debug("Playing Video " + url);
        while (player.isPlaying)
        {
            // Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)player.time));
            yield return null;
        }

        GM.Instance.logger.Debug("Done Playing Video " + url);

        state = VideoState.Stopped;
    }

    /// <summary>
    /// Just stops any video that's currently playing.
    /// </summary>
    public void StopVideo()
    {
        player.Stop();
        state = VideoState.Stopped;
    }

    public VideoState GetState()
    {
        return state;
    }

    /// <summary>
    /// Quickly checks to see if the background is playing, and restarts it if it's not.
    /// </summary>
    public void PlayBackground()
    {
        if (player.url != GM.Instance.data.backgroundVideo || !player.isPlaying)
            PlayVideo(GM.Instance.data.backgroundVideo, true, false);
    }
}
