using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovState : StateMachineBehaviour
{
    private float unitAttackDistance;

    AttackController    atkController;
    NavMeshAgent        navMeshAgent;

    bool bTranstion;
    const float TRANSITION_WAIT_TIME = 0.1F;
    float fTransitionWaitTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<NavMeshAgent>().avoidancePriority = 99;

        if (animator.gameObject.GetComponent<AttackController>() != null)
        {
            //Debug.Log("UnitMoveState, OnStateEnter@@@");

            bTranstion = false;
            fTransitionWaitTime = TRANSITION_WAIT_TIME;

            atkController = animator.transform.GetComponent<AttackController>();
            navMeshAgent = animator.transform.GetComponent<NavMeshAgent>();
            //atkController.SetMovingMaterial();

            if(animator.gameObject.GetComponent<UnitController>() == null)
            {
                //Debug.Log("animator.gameObject.GetComponent<UnitController>() == null");
                return;
            }
            if(animator.gameObject.GetComponent<UnitController>().Unit == null)
            {
                //Debug.Log("animator.gameObject.GetComponent<UnitController>().Unit == null");
                return;
            }

            unitAttackDistance = animator.gameObject.GetComponent<UnitController>().Unit.m_AttackDistance;

            animator.gameObject.GetComponent<UnitController>().State = enUNIT_STATUS.MOVE;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<AttackController>() == null)
        {
            return;
        }

        // 상태 무변화에 대한 임시 조치
        if(bTranstion)
        {
            if(fTransitionWaitTime < 0f)
            {
                bTranstion = false;
                fTransitionWaitTime = TRANSITION_WAIT_TIME;
            }
            else
            {
                fTransitionWaitTime -= Time.deltaTime;
            }
        }
        else
        {
            if (navMeshAgent.hasPath == false || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || ShouldStop(animator))
            {
                animator.transform.GetComponent<UnitMovement>().isCommandedToMove = false;

                //Debug.Log("UnitMovState, SendMoveStopMsg 송신!");
                SendMoveStopMsg(animator);
            }

            if (atkController != null && atkController.m_TargetObject != null && animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                float distanceFromTarget = Vector3.Distance(atkController.m_TargetObject.transform.position, animator.transform.position);
                if (distanceFromTarget <= unitAttackDistance)
                {
                    //Debug.Log("UnitMovState, SendAttackMsg 송신!");
                    SendAttackMsg(animator);
                }
            }
        }
    }

    private bool ShouldStop(Animator animator)
    {
        if (navMeshAgent.pathPending == true) 
        {
            // navMeshAgent.remainingDistance이 계산 전(기존 도착지, 새로운 도착지 반영x)면서 DistanceFromCenter는 반영된 상태라면 그대로 멈추는 현상 발생
            return false;
        }

        if (IsNearByUnit(animator) && navMeshAgent.remainingDistance < animator.gameObject.GetComponent<UnitMovement>().DistanceFromCenter)
        {
            animator.gameObject.GetComponent<UnitMovement>().DistanceFromCenter = 0;
            return true;
        }

        return false;
    }

    private bool IsNearByUnit(Animator animator)
    {
        // 현재 위치를 기준으로 주변에 장애물(유닛 등)이 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(animator.transform.position, animator.gameObject.GetComponent<UnitMovement>().DistanceFromCenter);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != animator.gameObject && collider.gameObject.GetComponent<NavMeshAgent>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void SendMoveStopMsg(Animator animator){
        MSG_UNIT_S_MOVE movMsg = new MSG_UNIT_S_MOVE();
        movMsg.type = (ushort)enPacketType.UNIT_S_MOVE;  
        movMsg.moveType = (byte)(enUnitMoveType.Move_Stop);    
        movMsg.posX = animator.transform.position.x;
        movMsg.posZ = animator.transform.position.z;
        movMsg.normX = animator.transform.forward.normalized.x;
        movMsg.normZ = animator.transform.forward.normalized.z;

        animator.gameObject.GetComponent<UnitController>().UnitSession.SendPacket<MSG_UNIT_S_MOVE>(movMsg);
    }
    private void SendAttackMsg(Animator animator)
    {
        MSG_UNIT_S_ATTACK atkMsg = new MSG_UNIT_S_ATTACK();
        atkMsg.type = (ushort)enPacketType.UNIT_S_ATTACK;
        atkMsg.posX = animator.transform.position.x;
        atkMsg.posZ = animator.transform.position.z;
        Vector3 dirVec = (atkController.m_TargetObject.transform.position - animator.transform.position).normalized;
        atkMsg.normX = dirVec.x;
        atkMsg.normZ = dirVec.z;
        atkMsg.targetID = atkController.m_TargetObject.GetComponent<Enemy>().ID;
        atkMsg.attackType = (int)enUnitAttackType.ATTACK_NORMAL;

        animator.gameObject.GetComponent<UnitController>().UnitSession.SendPacket<MSG_UNIT_S_ATTACK>(atkMsg);
    }
}
