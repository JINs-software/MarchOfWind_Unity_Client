using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class State_IDLE : StateMachineBehaviour
{
    UnitController unitController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // UnitController ������Ʈ ���� ���ο� ���� �÷��̾��� ������ ����
        unitController = animator.gameObject.GetComponent<UnitController>();
        if (unitController != null)
        {
            // AttackController�� Ÿ�� ���� �� ���� üũ �ڷ�ƾ ���� 
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            animator.gameObject.GetComponent<AttackController>().StartCheckTargetCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (unitController != null)
        {
            //Debug.Log("State_IDLE.OnStateExit*********************************************");
            animator.gameObject.GetComponent<AttackController>().StopCheckTargetCoroutine();
        }
    }
}
