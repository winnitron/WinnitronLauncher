using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AttractState : State {

    public int numberOfItems;
    public int currentItem;

    private float playDelay;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        base.OnStateEnter(animator, info, layerIndex);

        numberOfItems = GM.Instance.data.attractFiles.Count;
        currentItem = 0;

        PlayVideo(0);   
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        if (Input.anyKeyDown)
            animator.SetTrigger("NextState");

        playDelay -= Time.deltaTime;

        if (playDelay < 0 && !GM.Instance.video.player.isPlaying)
            PlayNextVideo();
    }

    private void PlayVideo(int number)
    {
        playDelay = 1;
        GM.Instance.video.PlayVideo(GM.Instance.data.attractFiles[number], false, null);
    }

    private void PlayNextVideo()
    {
        currentItem++;

        if (currentItem > numberOfItems - 1)
            currentItem = 0;

        PlayVideo(currentItem);
    }
}
