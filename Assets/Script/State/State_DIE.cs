using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class State_DIE : StateMachineBehaviour
{
    UnitController unitController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unitController = animator.gameObject.GetComponent<UnitController>();

        if (unitController != null)
        {
            //Debug.Log("State_DIE.OnStateEnter@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            unitController.State = enUNIT_STATUS.DIE;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
