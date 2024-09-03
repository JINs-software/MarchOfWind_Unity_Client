using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_WAIT : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<UnitController>().State = enUNIT_STATUS.CTR_WAIT;
        animator.GetComponent<UnitMovement>().isCommandedToMove = true; 
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
