using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The behaviour attached to the Idle state in the Animator component of the GM GameObject.
/// 
/// The functions of this script get called using the Animator transitions and conditions
/// set within Unity's built in Animator.  Please see the Animator component of the GM 
/// GameObject to find out more.
/// </summary>
public class IdleState : State {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //turn everything off to save those cycles!
        helper.mainCanvas.SetActive(false);
        GM.Instance.video.StopVideo();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        helper.mainCanvas.SetActive(true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {

    }
}
