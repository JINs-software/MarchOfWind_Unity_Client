using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class State_IDLE : StateMachineBehaviour
{
    AttackController attackController;  

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // UnitController ������Ʈ ���� ���ο� ���� �÷��̾��� ������ ����
        //unitController = animator.gameObject.GetComponent<UnitController>();
        // => ���� ���� �߰�, ���� ��Ʋ�ѷ��� �ƴ� ���� ��Ʈ�ѷ��� �ĺ�
        attackController = animator.gameObject.GetComponent<AttackController>();    
        if (attackController != null)
        {
            // AttackController�� Ÿ�� ���� �� ���� üũ �ڷ�ƾ ���� 
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            animator.gameObject.GetComponent<AttackController>().StartCheckTargetCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController != null)
        {
            //Debug.Log("State_IDLE.OnStateExit*********************************************");
            animator.gameObject.GetComponent<AttackController>().StopCheckTargetCoroutine();
        }
    }
}
