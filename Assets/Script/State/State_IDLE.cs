using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class State_IDLE : StateMachineBehaviour
{
    UnitController unitController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // UnitController 컴포넌트 존재 여부에 따라 플레이어의 유닛을 구분
        unitController = animator.gameObject.GetComponent<UnitController>();
        if (unitController != null)
        {
            // AttackController의 타겟 추적 및 공격 체크 코루틴 수행 
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            animator.gameObject.GetComponent<AttackController>().StartCheckTargetCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (unitController != null)
        {
            //Debug.Log("State_IDLE.OnStateExit*********************************************");
            animator.gameObject.GetComponent<AttackController>().StopCheckTargetCoroutine();
        }
    }
}
