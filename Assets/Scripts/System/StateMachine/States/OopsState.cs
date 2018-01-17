using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The behaviour attached to the Oops state in the Animator component of the GM GameObject.
///
/// The functions of this script get called using the Animator transitions and conditions
/// set within Unity's built in Animator.  Please see the Animator component of the GM
/// GameObject to find out more.
/// </summary>
public class OopsState : State
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //All we need for this state is the Info object and the text included with it
        helper.DeactivateAll();
        helper.info.SetActive(true);

        //Set the action text
        var actionText = GM.Instance.infoAction;
        actionText.gameObject.SetActive(true);

        if (helper.oopsIsCritical)
            actionText.text = "Press Home or ESC to Quit Application";

        else
            actionText.text = "Press Home to Continue";
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        GM.Instance.infoAction.gameObject.SetActive(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        //A critical Oops means that something happened that stops the basic functionality of
        //the launcher.  So the launcher must quit and the problem must be resolved before
        //restarting.  We check for that below.

        if(helper.oopsIsCritical)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GM.Instance.state.SetTrigger("NextState");
        }
    }

}