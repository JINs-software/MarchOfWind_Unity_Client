using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_ATTACK : StateMachineBehaviour
{
    UnitController unitController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (animator.gameObject.GetComponent<UnitController>() != null)
        //{
        //    //Debug.Log("State_ATTACK.OnStateEnter@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        //    animator.gameObject.GetComponent<UnitController>().State = enUNIT_STATUS.ATTACK;
        //    animator.gameObject.GetComponent<AttackController>().StartAttackJudgmentCoroutine();
        //}
        // => 애니메이션 클립의 이벤트 함수에 의존할 것!, 단지 공격 가능만 판단(타겟 null 체크, 거리 체크)
        // UnitController 컴포넌트 존재 여부에 따라 플레이어의 유닛을 구분
        unitController = animator.gameObject.GetComponent<UnitController>();
        if (unitController != null)
        {
            // AttackController의 타겟 추적 및 공격 체크 코루틴 수행 
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            unitController.gameObject.GetComponent<AttackController>().StartAttackJudgmentCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (animator.gameObject.GetComponent<UnitController>() != null)
        //{
        //    animator.gameObject.GetComponent<AttackController>().StopAttackJudgmentCoroutine();
        //}
        // => 애니메이션 클립의 이벤트 함수에 의존할 것!
        if (unitController != null)
        {
            // AttackController의 타겟 추적 및 공격 체크 코루틴 수행 
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            unitController.gameObject.GetComponent<AttackController>().StopAttackJudgmentCoroutine();
            animator.gameObject.GetComponent<UnitAnimEventHandler>().OnAnimAttackEndEvent();
        }
    }
}
