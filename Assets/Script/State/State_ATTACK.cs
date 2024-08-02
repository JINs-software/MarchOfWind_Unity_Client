using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_ATTACK : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<UnitController>() != null)
        {
            animator.gameObject.GetComponent<AttackController>().StartAttackJudgmentCoroutine();
        }

        animator.gameObject.GetComponent<MuzzleEffect>().StartMuzzleEffectCoroutine();  
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<UnitController>() != null)
        {
            animator.gameObject.GetComponent<MuzzleEffect>().MuzzleObject.SetActive(false);
            animator.gameObject.GetComponent<AttackController>().StopAttackJudgmentCoroutine();

            animator.gameObject.GetComponent<MuzzleEffect>().StoptMuzzleEffectCoroutine();
        }
    }


    /*
    UnitController unitController;
    AttackController attackController;
    NavMeshAgent navMeshAgent;
    UnitMovement unitMovement;

    float unitAttackStopDistance;
    float unitAttackRate;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 플레이어 유닛임을 UnitController 컴포넌트 소유로 식별
        if (animator.gameObject.GetComponent<UnitController>() != null)
        {
            // UnitController
            unitController = animator.gameObject.GetComponent<UnitController>();
            unitController.State = enUnitState.MOVE;

            // AttackController
            attackController = animator.gameObject.GetComponent<AttackController>();

            // NavMeshAgent
            navMeshAgent = animator.gameObject.GetComponent<NavMeshAgent>();

            // UnitMovement
            unitMovement = animator.gameObject.GetComponent<UnitMovement>();

            // 공격 거리의 1.2배가 공격 중단 거리
            unitAttackStopDistance = unitController.Unit.m_AttackDistance * 1.2f;
            unitAttackRate = unitController.Unit.m_AttackRate;

            // 공격 판정 및 공격 메시지 송신 코루틴 시작
            attackController.StartAttackJudgmentCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    IEnumerator AttackJudgment()
    {

    }
    */
}
