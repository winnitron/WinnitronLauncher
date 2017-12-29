using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherState : State {

    public NUI_Controller launcher;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //Start the lawncher!
        helper.launcher.SetActive(true);

        //Make sure the Jukebox is on
        helper.jukebox.SetActive(true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        
    }

}
