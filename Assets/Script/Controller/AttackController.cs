using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackController : MonoBehaviour
{
    //public Transform m_TargetToAttack;
    public GameObject m_TargetObject;

    public float m_AttackDistance;
    public float m_StopAttackDistance;
    public float m_AttackRate;

    private Coroutine CheckTargetCoroutine;
    private Coroutine AttackJudgmentCoroutine;

    public void StartCheckTargetCoroutine()
    {
        if (CheckTargetCoroutine == null)
        {
            CheckTargetCoroutine = StartCoroutine(CheckTarget());
        }
    }
    public void StopCheckTargetCoroutine()
    {
        if (CheckTargetCoroutine != null)
        {
            StopCoroutine(CheckTargetCoroutine);
            CheckTargetCoroutine = null;
        }
    }
    private IEnumerator CheckTarget()
    {
        while (true)
        {
            if (m_TargetObject != null)
            {
                // 타겟 존재 확인
                float distanceToTarget = Vector3.Distance(transform.position, m_TargetObject.transform.position);
                if (distanceToTarget <= m_AttackDistance)
                {
                    // 공격 
                    gameObject.GetComponent<UnitController>().SendAttackMsg(m_TargetObject);
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    // 추적
                    gameObject.GetComponent<UnitController>().Send_MoveStartMessage(m_TargetObject.transform.position); 
                    yield return new WaitForSeconds(0.1f);
                }
            }

           yield return null;
        }
    }

    public void StartAttackJudgmentCoroutine()
    {
        if(AttackJudgmentCoroutine == null)
        {
            AttackJudgmentCoroutine = StartCoroutine(AttackJudgment());
        }
    }
    public void StopAttackJudgmentCoroutine()
    {
        if(AttackJudgmentCoroutine != null )
        {
            StopCoroutine(AttackJudgmentCoroutine); 
            AttackJudgmentCoroutine = null; 
        }
    }

    private IEnumerator AttackJudgment()
    {
        while (true)
        {
            if (m_TargetObject != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, m_TargetObject.transform.position);
                if(distanceToTarget <= m_AttackDistance)
                {
                    gameObject.GetComponent<UnitController>().SendAttackMsg(m_TargetObject);
                }
            }
            yield return new WaitForSeconds(1f / m_AttackRate);
        }
    }

    private void Start()
    {
        gameObject.GetComponent<SphereCollider>().radius = gameObject.GetComponent<UnitController>().Unit.m_AttackDistance * 1.5f / gameObject.transform.localScale.x;
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