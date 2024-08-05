using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class State_IDLE : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.gameObject.GetComponent<NavMeshAgent>().avoidancePriority = 0;
        //animator.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        //animator.gameObject.GetComponent<NavMeshObstacle>().enabled = true;    

        // UnitController ������Ʈ ���� ���ο� ���� �÷��̾��� ������ ����
        if (animator.gameObject.GetComponent<UnitController>() != null)
        {
            Debug.Log("State_IDLE.OnStateEnter@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            animator.gameObject.GetComponent<UnitController>().State = enUnitState.IDLE;
            animator.gameObject.GetComponent<AttackController>().StartCheckTargetCoroutine();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<UnitController>() != null)
        {
            Debug.Log("State_IDLE.OnStateExit*********************************************");
            animator.gameObject.GetComponent<AttackController>().StopCheckTargetCoroutine();
        }
    }
}
