using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// The behaviour attached to the Intro state in the Animator component of the GM GameObject.
/// 
/// The functions of this script get called using the Animator transitions and conditions
/// set within Unity's built in Animator.  Please see the Animator component of the GM 
/// GameObject to find out more.
/// </summary>
public class IntroState : State
{
    public VideoClip introClip;
    public AudioClip audioClip;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //Make sure the jukebox is off
        helper.jukebox.SetActive(false);

        if (GM.Instance.data.introVideo != null && GM.Instance.data.introVideo != "")
            GM.Instance.video.PlayVideo(GM.Instance.data.introVideo, false, true);
        else
        {
            GM.Instance.video.StopVideo();
            animator.SetTrigger("NextState");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        //Makes sure no vid is playing when transitioning to the next state
        GM.Instance.video.StopVideo();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        if (GM.Instance.video.state == VideoManager.VideoState.Stopped)
            animator.SetTrigger("NextState");
    }

}