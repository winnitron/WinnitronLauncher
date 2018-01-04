using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherState : State {

    public NUI_Controller launcher;
    public float timeToAttractMode;
    public float currentIdleTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //Start the lawncher!
        helper.launcher.SetActive(true);

        //Make sure the Jukebox is on
        helper.jukebox.SetActive(true);

        ResetIdleTime();
}

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        //Turn off that which you have turned on
        helper.launcher.SetActive(false);
        helper.jukebox.SetActive(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        currentIdleTime += Time.deltaTime;

        if(Input.anyKeyDown)
            ResetIdleTime();

        if (currentIdleTime > timeToAttractMode)
            animator.SetTrigger("Attract");
    }

    void ResetIdleTime()
    {
        currentIdleTime = 0;
    }
}
