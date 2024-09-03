using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class State_MOVE : StateMachineBehaviour
{
    UnitController unitController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 플레이어 유닛임을 UnitController 컴포넌트 소유로 식별
        unitController = animator.gameObject.GetComponent<UnitController>();
        if (unitController != null)
        {
            unitController.StartMoveStateCoroutine();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (unitController != null)
        {
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            unitController.StopMoveStateCoroutine();
        }
    }
}
