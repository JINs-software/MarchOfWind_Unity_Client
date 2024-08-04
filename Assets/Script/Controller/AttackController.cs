using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AttackController : MonoBehaviour
{
    //public Transform m_TargetToAttack;
    public GameObject m_TargetObject;

    public float m_AttackDistance;
    public float m_StopAttackDistance;
    public float m_AttackRate;
    public float m_AttackDelay;

    private Coroutine CheckTargetCoroutine;
    private Coroutine AttackJudgmentCoroutine;

    UnitController m_UnitController;
    UnitMovement m_UnitMovement;

    public void StartCheckTargetCoroutine()
    {
        UnityEngine.Debug.Log("StartCheckTargetCoroutine");
        if (CheckTargetCoroutine == null)
        {
            CheckTargetCoroutine = StartCoroutine(CheckTarget());
        }
    }
    public void StopCheckTargetCoroutine()
    {
        UnityEngine.Debug.Log("StopCheckTargetCoroutine");
        if (CheckTargetCoroutine != null)
        {
            StopCoroutine(CheckTargetCoroutine);
            CheckTargetCoroutine = null;
        }
    }
    private IEnumerator CheckTarget()
    {
        while (m_UnitController.State == enUnitState.IDLE)
        {
            if (m_UnitMovement.isCommandedToMove)
            {
                yield return null;
            }

            //UnityEngine.Debug.Log("CheckTarget");
            if (m_TargetObject != null)
            {
                // 타겟 존재 확인
                float distanceToTarget = Vector3.Distance(transform.position, m_TargetObject.transform.position);
                distanceToTarget -= m_TargetObject.GetComponent<NavMeshAgent>().radius * m_TargetObject.transform.localScale.x;
                if (distanceToTarget <= m_AttackDistance)
                {
                    // 공격 
                    gameObject.GetComponent<UnitController>().Send_AttackMessage(m_TargetObject);
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    // 추적
                    UnityEngine.Debug.Log("@@@@@@@@@@@@@@@ in Idle state -> trace @@@@@@@@@@@@@@@@@@ ");
                    Vector3 direction = (m_TargetObject.transform.position - gameObject.transform.position);
                    float diff = (m_TargetObject.transform.position - gameObject.transform.position).magnitude - m_AttackDistance;
                    Vector3 destination = gameObject.transform.position + direction.normalized * diff;
                    m_UnitController.Send_MoveStartMessage(destination);
                    yield return new WaitForSeconds(0.01f);
                }
            }

           yield return null;
        }
    }

    public void StartAttackJudgmentCoroutine()
    {
        UnityEngine.Debug.Log("StartAttackJudgmentCoroutine");
        if(AttackJudgmentCoroutine == null)
        {
            AttackJudgmentCoroutine = StartCoroutine(AttackJudgment());
        }
    }
    public void StopAttackJudgmentCoroutine()
    {
        UnityEngine.Debug.Log("StopAttackJudgmentCoroutine");
        if (AttackJudgmentCoroutine != null )
        {
            StopCoroutine(AttackJudgmentCoroutine); 
            AttackJudgmentCoroutine = null; 
        }
    }

    private IEnumerator AttackJudgment()
    {
        while (m_UnitController.State == enUnitState.ATTACK)
        {
            if (m_UnitMovement.isCommandedToMove)
            {
                yield return null;
            }

            if (m_TargetObject != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, m_TargetObject.transform.position);
                distanceToTarget -= m_TargetObject.GetComponent<NavMeshAgent>().radius * m_TargetObject.transform.localScale.x;
                if (distanceToTarget <= m_AttackDistance)
                {
                    // 공격
                    yield return new WaitForSeconds(m_AttackDelay);
                    if (!m_UnitMovement.isCommandedToMove)
                    {
                        m_UnitController.Send_AttackMessage(m_TargetObject);
                    }
                }
                else
                {
                    // 추격
                    //m_UnitController.Send_MoveStartMessage(m_TargetObject.transform.position);
                    // => 타겟 방향으로 공격이 가능한 위치까지만 이동
                    Vector3 direction = (m_TargetObject.transform.position - gameObject.transform.position);
                    float diff = (m_TargetObject.transform.position - gameObject.transform.position).magnitude - m_AttackDistance;
                    Vector3 destination = gameObject.transform.position + direction.normalized * diff;
                    m_UnitController.Send_MoveStartMessage(destination);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                gameObject.GetComponent<UnitController>().Send_AttackStopMessage();
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds((1f / m_AttackRate) - m_AttackDelay);   // attack delay는 (1f / m_AttackRate) 보다 작아야 함.
        }
    }

    private void Start()
    {
        //gameObject.GetComponent<SphereCollider>().radius = gameObject.GetComponent<UnitController>().Unit.m_AttackDistance * 1.5f / gameObject.transform.localScale.x;
        gameObject.GetComponent<SphereCollider>().radius = 50f / gameObject.transform.localScale.x; // 모든 유닛 추적 범위 동일
        m_UnitController = gameObject.GetComponent<UnitController>();
        m_UnitMovement = gameObject.GetComponent<UnitMovement>();       
    }

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("OnTriggerEnter!");

        // �浹���� �±װ� "��"�̸鼭 ���ÿ� ���� Ÿ�� ����� ���� ��� Ÿ�� ������� ����
        if (other.CompareTag("Enemy") && m_TargetObject == null)
        {
            m_TargetObject = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && m_TargetObject == null)
        {
            m_TargetObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UnityEngine.Debug.Log("OnTriggerExit!");

        if (other.CompareTag("Enemy") && m_TargetObject != null)
        {
            m_TargetObject = null;
        }
    }

    private void OnEnable() 
    {
        Enemy.OnEnemyDestroyed += OnEnemyDestroyed;
    }
    private void OnDisable() 
    {
        Enemy.OnEnemyDestroyed -= OnEnemyDestroyed;
    }

    private void OnEnemyDestroyed(GameObject enemy) 
    {
        if(m_TargetObject == enemy)
        {
            UnityEngine.Debug.Log("Enemy Destroyed! : " + enemy.ToString());
            m_TargetObject = null;
        }
    }
   

    private void OnDrawGizmos()
    {
        // follow distance / area
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, 10f * 0.2f);    // spere collider's radius * unit scale)
        //Gizmos.DrawWireSphere(transform.position, transform.GetComponent<SphereCollider>().radius * transform.localScale.x);
        Gizmos.DrawWireSphere(transform.position, transform.GetComponent<SphereCollider>().radius * transform.localScale.x);


        // attack distance / area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackDistance);  // attack distance

        // stop attack distance / area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_StopAttackDistance);    // stop attack distance
    }
}