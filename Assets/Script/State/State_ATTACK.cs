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
        // => �ִϸ��̼� Ŭ���� �̺�Ʈ �Լ��� ������ ��!, ���� ���� ���ɸ� �Ǵ�(Ÿ�� null üũ, �Ÿ� üũ)

        // UnitController ������Ʈ ���� ���ο� ���� �÷��̾��� ������ ����
        //unitController = animator.gameObject.GetComponent<UnitController>();
        // if (unitController != null)
        // => ���� ���� �߰��� ���� ��Ʈ�ѷ��� �ĺ�
        attackController = animator.gameObject.GetComponent<AttackController>();    
        if (attackController != null && animator.tag == GamaManager.TEAM_TAG) 
        {
            // AttackController�� Ÿ�� ���� �� ���� üũ �ڷ�ƾ ���� 
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
        // => �ִϸ��̼� Ŭ���� �̺�Ʈ �Լ��� ������ ��!
        if (attackController != null && animator.tag == GamaManager.TEAM_TAG)
        {
            // AttackController�� Ÿ�� ���� �� ���� üũ �ڷ�ƾ ���� 

            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            attackController.StopAttackJudgmentCoroutine();
        }

        // ���� Muzzle�� �̺�Ʈ �Լ��� ���� ���Ͽ� ��� �����Ǵ� ���� �߻�(Ư�� Enemy)
        // UnitAnimEventHandler�� ���� ���������� SetActive(False)
        UnitAnimEventHandler animEventHnd = animator.gameObject.GetComponent<UnitAnimEventHandler>();
        if (animEventHnd != null)
        {
            animEventHnd.OnAnimAttackEndEvent();
        }
    }
}
