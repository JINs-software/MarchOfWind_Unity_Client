using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class State_MOVE : StateMachineBehaviour
{
    UnitController unitController;
    UnitMovement unitMovement;
    NavMeshAgent navMeshAgent;

    Coroutine CheckColliderCoroutine;

    enUnitState unitState;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navMeshAgent = animator.gameObject.GetComponent<NavMeshAgent>();    
        unitController = animator.gameObject.GetComponent<UnitController>();
        unitMovement = animator.gameObject.GetComponent<UnitMovement>();    

        //navMeshAgent.avoidancePriority = 50;

        //animator.gameObject.GetComponent<NavMeshObstacle>().enabled = false;
        //animator.gameObject.GetComponent<NavMeshAgent>().enabled = true;

        // 플레이어 유닛임을 UnitController 컴포넌트 소유로 식별
        if (unitController != null)
        {
            Debug.Log("State_MOVE.OnStateEnter@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            //animator.gameObject.GetComponent<UnitController>().State = enUnitState.MOVE;/
            //unitController.StartMoveStateCoroutine();
            if (unitMovement.isCommandedToMove)
            {
                animator.gameObject.GetComponent<UnitController>().State = enUnitState.MOVE_COMMAND;
                unitState = enUnitState.MOVE_COMMAND;
            }
            else
            {
                animator.gameObject.GetComponent<UnitController>().State = enUnitState.MOVE_TRACING;
                unitState = enUnitState.MOVE_TRACING;
            }

            unitController.StartMoveStateCoroutine(unitState);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // => CommadToMove
        if((unitState == enUnitState.MOVE_TRACING || unitState == enUnitState.MOVE_SPATH) && unitMovement.isCommandedToMove)
        {
            if(unitController.ServerPathFinding)
            {
                unitController.ServerPathFinding = false;
                unitController.ServerPathPending = false;
                unitController.ServerSPathQueue.Clear();    
            }
            unitState = animator.gameObject.GetComponent<UnitController>().State = enUnitState.MOVE_COMMAND;
            unitController.StopMoveStateCoroutine();
            unitController.StartMoveStateCoroutine(unitState);
        }

        if((unitState == enUnitState.MOVE_COMMAND || unitState == enUnitState.MOVE_TRACING) && unitController.ServerPathFinding)
        {
            unitState = animator.gameObject.GetComponent<UnitController>().State = enUnitState.MOVE_SPATH;
            unitController.StopMoveStateCoroutine();
            unitController.StartMoveStateCoroutine(unitState);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (unitController != null)
        {
            Debug.Log("State_MOVE.OnStateExit*********************************************");
            unitController.StopMoveStateCoroutine();

            if (unitController.ServerPathFinding)
            {
                unitController.ServerPathFinding = false;
                unitController.ServerPathPending = false;
                unitController.ServerSPathQueue.Clear();
            }
        }
    }

    //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // 현재 위치를 기준으로 주변에 장애물(유닛 등)이 있는지 확인
    //    Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, m_NavMeshAgent.radius * gameObject.transform.localScale.x + 1);
    //
    //    foreach (Collider collider in colliders)
    //    {
    //        GameObject nearObject = collider.gameObject;
    //        // 자신 오브젝트가 아니면서 && 선택된 유닛이면서 && NavMeshAgent 컴포넌트를 가지면서, 해당 컴포넌트가 isStopped 상태일 때 
    //        if (nearObject != gameObject && Manager.UnitSelection.m_UnitsSelected.Contains(nearObject) && nearObject.GetComponent<NavMeshAgent>() != null && nearObject.GetComponent<NavMeshAgent>().isStopped)
    //        {
    //            return true;
    //        }
    //    }
    //}

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
