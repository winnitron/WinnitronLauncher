using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// The behaviour attached to the Launcher state in the Animator component of the GM GameObject.
/// 
/// The functions of this script get called using the Animator transitions and conditions
/// set within Unity's built in Animator.  Please see the Animator component of the GM 
/// GameObject to find out more.
/// </summary>
public class LauncherState : State {

    public LauncherController launcher;
    public VideoClip videoClip;
    public float timeToAttractMode;
    public float currentIdleTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //Start the lawncher!
        helper.launcher.SetActive(true);
        helper.jukebox.SetActive(true);

        if (GM.Instance.data.launcherBackground != null)
            GM.Instance.video.PlayVideo(GM.Instance.data.launcherBackground, true, null);

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
