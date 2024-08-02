using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitIdleState : StateMachineBehaviour
{
    AttackController atkController;

    private float unitAttackDistance;

    bool bTranstion;
    const float TRANSITION_WAIT_TIME = 0.1F;
    float fTransitionWaitTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<AttackController>() != null)
        {
            bTranstion = false;
            fTransitionWaitTime = TRANSITION_WAIT_TIME;
            
            atkController = animator.transform.GetComponent<AttackController>();
            
            unitAttackDistance = animator.gameObject.GetComponent<UnitController>().Unit.m_AttackDistance;

            animator.gameObject.GetComponent<UnitController>().State = enUnitState.IDLE;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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

        if (!bTranstion && atkController != null && atkController.m_TargetObject != null && animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
        {
            float distanceFromTarget = Vector3.Distance(atkController.m_TargetObject.transform.position, animator.transform.position);
            if (distanceFromTarget <= unitAttackDistance)
            {
                if(!SendAttackMsg(animator)) 
                {
                    Debug.Log("SendAttackMsg Fail..");
                }
                else {
                    bTranstion = true;
                }
            }
            //if(distanceFromTarget > unitAttackDistance) 
            else 
            {
                if(!SendMoveMsg(animator)) 
                {
                    Debug.Log("SendMoveMsg Fail..");
                }
                else {
                    bTranstion = true;
                }
            }
        }
    }

    private bool SendMoveMsg(Animator animator){
        MSG_UNIT_S_MOVE movMsg = new MSG_UNIT_S_MOVE();
        movMsg.type = (ushort)enPacketType.UNIT_S_MOVE;  
        movMsg.moveType = (byte)(enUnitMoveType.Move_Start);    
        movMsg.posX = animator.transform.position.x;
        movMsg.posZ = animator.transform.position.z;
        movMsg.normX = animator.transform.forward.normalized.x;
        movMsg.normZ = animator.transform.forward.normalized.z;

        movMsg.destX = atkController.m_TargetObject.transform.position.x;
        movMsg.destZ = atkController.m_TargetObject.transform.position.z;

        animator.SetTrigger("trWait");
        return animator.gameObject.GetComponent<UnitController>().UnitSession.SendPacket<MSG_UNIT_S_MOVE>(movMsg);
    }
    private bool SendAttackMsg(Animator animator)
    {
        MSG_UNIT_S_ATTACK atkMsg = new MSG_UNIT_S_ATTACK();
        atkMsg.type = (ushort)enPacketType.UNIT_S_ATTACK;
        atkMsg.posX = animator.transform.position.x;
        atkMsg.posZ = animator.transform.position.z;
        Vector3 dirVec = (atkController.m_TargetObject.transform.position - animator.transform.position).normalized;
        atkMsg.normX = dirVec.x;
        atkMsg.normZ = dirVec.z;
        atkMsg.targetID = atkController.m_TargetObject.GetComponent<Enemy>().m_Unit.m_id;
        atkMsg.attackType = (int)enUnitAttackType.ATTACK_NORMAL;

        animator.SetTrigger("trWait");
        return animator.gameObject.GetComponent<UnitController>().UnitSession.SendPacket<MSG_UNIT_S_ATTACK>(atkMsg);
    }
}
