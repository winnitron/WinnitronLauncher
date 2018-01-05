using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LauncherState : State {

    public NUI_Controller launcher;
    public VideoClip videoClip;
    public float timeToAttractMode;
    public float currentIdleTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        //Start the lawncher!
        helper.launcher.SetActive(true);
        helper.jukebox.SetActive(true);

        GM.Instance.video.PlayVideo(videoClip, true);

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
