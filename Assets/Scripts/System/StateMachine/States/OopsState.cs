using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OopsState : State
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        helper.DeactivateAll();
        helper.info.SetActive(true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        if(helper.oopsIsCritical)
        {
            if (Input.GetKey(KeyCode.Escape))
                Application.Quit();
        }

        else
        {
            if (Input.GetKey(KeyCode.Escape))
                GM.Instance.state.SetTrigger("NextState");
        }
    }

}