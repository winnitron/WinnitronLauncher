using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : State
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        //Wait until GM is completed initializing.

        //Then either go to intro, Sync, or Oops depending on what happens.

        animator.SetTrigger("NextState");
    }
}
