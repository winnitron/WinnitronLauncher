using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //turn everything off to save those cycles!
        helper.mainCanvas.SetActive(false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        helper.mainCanvas.SetActive(true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {

    }
}
