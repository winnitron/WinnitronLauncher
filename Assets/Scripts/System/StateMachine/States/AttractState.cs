using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AttractState : State {

    public VideoClip attractVideo;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        GM.Instance.video.PlayVideo(attractVideo, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        if (Input.anyKeyDown)
            animator.SetTrigger("NextState");
    }
}
