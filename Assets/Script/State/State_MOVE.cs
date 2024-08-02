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
        unitController = animator.gameObject.GetComponent<UnitController>();

        // 플레이어 유닛임을 UnitController 컴포넌트 소유로 식별
        if (unitController != null)
        {
            Debug.Log("State_MOVE.OnStateEnter@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            animator.gameObject.GetComponent<UnitController>().State = enUnitState.MOVE;
            unitController.StartMoveStateCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (unitController != null)
        {
            Debug.Log("State_MOVE.OnStateExit*********************************************");
            unitController.StopMoveStateCoroutine();
        }
    }

    /*
    UnitController unitController;  
    AttackController attackController;
    NavMeshAgent navMeshAgent;
    UnitMovement unitMovement;

    float unitAttackDistance;

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

            // unit attack distance
            unitAttackDistance = unitController.Unit.m_AttackDistance;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 플레이어 유닛임을 UnitController 컴포넌트 소유로 식별
        if (animator.gameObject.GetComponent<UnitController>() != null)
        {
            // 이동 정지 판단
            if (navMeshAgent.hasPath == false || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                unitController.Send_MoveStopMessage();
                animator.SetTrigger("trWait");
            }
            // 공격 판단 (플레이어 제어를 통한 유닛 이동 시에는 공격 불가)
            else if (unitMovement.isCommandedToMove == false && attackController.m_TargetObject != null)
            {
                float distanceFromTarget = Vector3.Distance(attackController.m_TargetObject.transform.position, animator.transform.position);
                if (distanceFromTarget <= unitAttackDistance)
                {
                    unitController.SendAttackMsg(attackController.m_TargetObject);
                    animator.SetTrigger("trWait");
                }
            }
        }
    }
    */
}
