using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        animator.SetTrigger("NextState");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {

    }
}
