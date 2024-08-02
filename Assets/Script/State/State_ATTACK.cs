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
        // �÷��̾� �������� UnitController ������Ʈ ������ �ĺ�
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

            // ���� �Ÿ��� 1.2�谡 ���� �ߴ� �Ÿ�
            unitAttackStopDistance = unitController.Unit.m_AttackDistance * 1.2f;
            unitAttackRate = unitController.Unit.m_AttackRate;

            // ���� ���� �� ���� �޽��� �۽� �ڷ�ƾ ����
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
