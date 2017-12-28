using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

[System.Serializable]
public class State : StateMachineBehaviour {

    public StateMachineHelper helper;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        helper = GM.stateMachineHelper;
    }
}
