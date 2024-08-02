using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_INIT : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetTrigger("trIdle");
    }
}
