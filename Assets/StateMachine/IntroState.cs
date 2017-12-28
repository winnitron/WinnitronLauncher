using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroState : StateMachineBehaviour
{
    public VideoClip introClip;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    { 
        GM.video.PlayVideo(introClip, false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        if(!GM.video.player.isPlaying)
            animator.SetTrigger("NextState");
    }

}