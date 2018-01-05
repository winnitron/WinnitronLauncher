using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroState : State
{
    public VideoClip introClip;
    public AudioClip audioClip;

    private bool introLoaded;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        introLoaded = false;
        GM.Instance.video.PlayVideo(introClip, false, audioClip);

        //Make sure the jukebox is off
        helper.jukebox.SetActive(false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        if (GM.Instance.video.player.isPrepared)
        {
            introLoaded = true;
        }

        if (introLoaded && !GM.Instance.video.player.isPlaying)
            animator.SetTrigger("NextState");
    }

}