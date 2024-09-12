using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_ATTACK : StateMachineBehaviour
{
    //UnitController unitController;
    AttackController attackController;    

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
        //unitController = animator.gameObject.GetComponent<UnitController>();
        // if (unitController != null)
        // => 더미 유닛 추가로 공격 컨트롤러로 식별
        attackController = animator.gameObject.GetComponent<AttackController>();    
        if (attackController != null && animator.tag == GamaManager.TEAM_TAG) 
        {
            // AttackController의 타겟 추적 및 공격 체크 코루틴 수행 
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            attackController.StartAttackJudgmentCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (animator.gameObject.GetComponent<UnitController>() != null)
        //{
        //    animator.gameObject.GetComponent<AttackController>().StopAttackJudgmentCoroutine();
        //}
        // => 애니메이션 클립의 이벤트 함수에 의존할 것!
        if (attackController != null && animator.tag == GamaManager.TEAM_TAG)
        {
            // AttackController의 타겟 추적 및 공격 체크 코루틴 수행 

            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            attackController.StopAttackJudgmentCoroutine();
        }

        // 공격 Muzzle이 이벤트 함수를 받지 못하여 계속 유지되는 현상 발생(특히 Enemy)
        // UnitAnimEventHandler를 통해 직접적으로 SetActive(False)
        UnitAnimEventHandler animEventHnd = animator.gameObject.GetComponent<UnitAnimEventHandler>();
        if (animEventHnd != null)
        {
            animEventHnd.OnAnimAttackEndEvent();
        }
    }
}
