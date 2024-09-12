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
        // UnitController 컴포넌트 존재 여부에 따라 플레이어의 유닛을 구분
        //unitController = animator.gameObject.GetComponent<UnitController>();
        // => 더미 유닛 추가, 유닛 컨틀롤러가 아닌 공격 컨트롤러로 식별
        attackController = animator.gameObject.GetComponent<AttackController>();    
        if (attackController != null)
        {
            // AttackController의 타겟 추적 및 공격 체크 코루틴 수행 
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
