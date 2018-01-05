using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : State
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //Make sure all states are off
        helper.DeactivateAll();

        helper.info.SetActive(true);

        GM.Instance.Init();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        helper.info.SetActive(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        //Wait until GM is completed initializing.

        //Then either go to intro, Sync, or Oops depending on what happens.

        //animator.SetTrigger("NextState");
    }
}
