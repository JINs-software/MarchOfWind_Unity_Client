using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class UnitAttackState : StateMachineBehaviour
{
    private float unitAttackStopDistance;
    private float unitAttackRate = 1f;

    AttackController atkController;
    NavMeshAgent navMeshAgent;

    GameObject muzzleEffect;

    private float attackTimer;

    bool bTranstion;
    const float TRANSITION_WAIT_TIME = 0.1F;
    float fTransitionWaitTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Common
        muzzleEffect = animator.gameObject.GetComponent<MuzzleEffect>().MuzzleObject;
        if (muzzleEffect != null)
        {
            muzzleEffect.SetActive(true);
        }
        else 
        {
            //Debug.Log("animator.gameObject.GetComponent<MuzzleEffect>().MuzzleObject == NULL");
        }

        animator.gameObject.GetComponent<NavMeshAgent>().avoidancePriority = 10;


        if (animator.gameObject.GetComponent<AttackController>() != null)
        {
            bTranstion = false;
            fTransitionWaitTime = TRANSITION_WAIT_TIME;

            atkController = animator.GetComponent<AttackController>();
            navMeshAgent = animator.GetComponent<NavMeshAgent>();

            unitAttackStopDistance = animator.gameObject.GetComponent<UnitController>().Unit.m_AttackDistance + 1f;
            unitAttackRate = animator.gameObject.GetComponent<UnitController>().Unit.m_AttackRate;
            attackTimer = 1f / unitAttackRate;

            //animator.gameObject.GetComponent<UnitController>().OnAttacking = true;

            animator.gameObject.GetComponent<UnitController>().State = enUNIT_STATUS.ATTACK;
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

        if(atkController == null || bTranstion || animator.transform.GetComponent<UnitMovement>().isCommandedToMove)
        {
            return;    
        }

        if (atkController.m_TargetObject == null)
        {
            SendAttackStopMsg(animator);
            bTranstion = true;
        }
        else 
        {
            // 공격 범위 벗어남 -> 공격 중단 메시지 송신
            float distanceFromTarget = Vector3.Distance(atkController.m_TargetObject.transform.position, animator.transform.position);
            if (distanceFromTarget > unitAttackStopDistance)
            {
                SendAttackStopMsg(animator);
                bTranstion = true;
            }
            else
            {
                //LookAtTarget();
                // => Unit Attack 함수로 이동
    
                if(attackTimer <= 0f)
                {
                    SendAttackMsg(animator);
                    attackTimer = 1f / unitAttackRate;
                }
                else
                {
                    attackTimer -= Time.deltaTime;  
                }                
            }
        }

/*
        if (atkController != null && !bTranstion)
        {
            if (atkController.m_TargetObject == null)
            {
                if(!SendAttackStopMsg(animator)) 
                {
                    //Debug.Log("SendAttackStopMsg Fail..");
                }
                else{
                    bTranstion = true;
                }
            }
            else
            {
                if (animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
                {
                    LookAtTarget();

                    if(attackTimer <= 0f)
                    {

                        if (!SendAttackMsg(animator))
                        {
                            //Debug.Log("SendAttackMsg Fail..");
                        }

                        attackTimer = 1f / unitAttackRate;
                    }
                    else
                    {
                        attackTimer -= Time.deltaTime;  
                    }

                    float distanceFromTarget = Vector3.Distance(atkController.m_TargetObject.transform.position, animator.transform.position);
                    if (distanceFromTarget > unitAttackStopDistance)
                    {
                        if(!SendAttackStopMsg(animator)) 
                        {
                            //Debug.Log("SendAttackStopMsg Fail..");
                            animator.SetBool("bAttack", false);
                        }
                        else{
                            bTranstion = true;
                        }
                    }
                }
            }
        }
*/
    }
    
    private void SendAttackMsg(Animator animator)
    {
        MSG_UNIT_S_ATTACK atkMsg = new MSG_UNIT_S_ATTACK();
        atkMsg.type = (ushort)enPacketType.UNIT_S_ATTACK;
        atkMsg.posX = animator.transform.position.x;
        atkMsg.posZ = animator.transform.position.z;
        Vector3 dirVec = (atkController.m_TargetObject.transform.position - navMeshAgent.transform.position).normalized;
        atkMsg.normX = dirVec.x;
        atkMsg.normZ = dirVec.z;
        atkMsg.targetID = atkController.m_TargetObject.GetComponent<Enemy>().ID;
        atkMsg.attackType = (int)enUnitAttackType.ATTACK_NORMAL;

        animator.gameObject.GetComponent<UnitController>().UnitSession.SendPacket<MSG_UNIT_S_ATTACK>(atkMsg);
    }
    private void SendAttackStopMsg(Animator animator)
    {
        MSG_UNIT_S_ATTACK_STOP atkStopMsg = new MSG_UNIT_S_ATTACK_STOP();
        atkStopMsg.type = (ushort)enPacketType.UNIT_S_ATTACK_STOP;
        atkStopMsg.posX = animator.transform.position.x;    
        atkStopMsg.posZ= animator.transform.position.z;
        atkStopMsg.normX = animator.transform.forward.normalized.x;
        atkStopMsg.normZ = animator.transform.forward.normalized.z;

        animator.gameObject.GetComponent<UnitController>().UnitSession.SendPacket<MSG_UNIT_S_ATTACK_STOP>(atkStopMsg);
    }

    //private void LookAtTarget()
    //{
    //    Vector3 direction = atkController.m_TargetObject.transform.position - navMeshAgent.transform.position;  
    //    navMeshAgent.transform.rotation = Quaternion.LookRotation(direction);
//
    //    var yRotation = navMeshAgent.transform.eulerAngles.y;
    //    navMeshAgent.transform.rotation = Quaternion.Euler(0, yRotation, 0);    
    //}
    // => Unit Attack 함수로 이동

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (muzzleEffect != null)
        {
            muzzleEffect.SetActive(false);
        }

        if(animator.gameObject.GetComponent<UnitController>() != null) {
            //animator.gameObject.GetComponent<UnitController>().OnAttacking = false;
        }
    }
}
