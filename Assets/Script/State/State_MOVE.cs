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
        // �÷��̾� �������� UnitController ������Ʈ ������ �ĺ�
        unitController = animator.gameObject.GetComponent<UnitController>();
        if (unitController != null)
        {
            unitController.StartMoveStateCoroutine();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (unitController != null)
        {
            //Debug.Log("State_IDLE.OnStateEnter*********************************************");
            unitController.StopMoveStateCoroutine();
        }
    }
}
