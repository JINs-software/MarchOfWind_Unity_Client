using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class AttackController : MonoBehaviour
{
    public GameObject       m_TargetObject;
    public List<GameObject> m_NearTargets = new List<GameObject>();

    public float m_AttackDistance;
    public float m_StopAttackDistance;
    public float m_AttackRate;
    public float m_AttackDelay;

    public float m_TracingRange;

    private Coroutine CheckTargetCoroutine;
    private Coroutine AttackJudgmentCoroutine;

    UnitController m_UnitController;
    UnitMovement m_UnitMovement;

    public void StartCheckTargetCoroutine()
    {
        //UnityEngine.Debug.Log("StartCheckTargetCoroutine");
        if (CheckTargetCoroutine == null)
        {
            CheckTargetCoroutine = StartCoroutine(CheckTarget());
        }
    }
    public void StopCheckTargetCoroutine()
    {
        //UnityEngine.Debug.Log("StopCheckTargetCoroutine");
        if (CheckTargetCoroutine != null)
        {
            StopCoroutine(CheckTargetCoroutine);
            CheckTargetCoroutine = null;
        }
    }
    private IEnumerator CheckTarget()
    {
        while (m_UnitController.State == enUnitState.IDLE && m_UnitMovement.isCommandedToMove == false)
        {
            if (m_TargetObject != null)
            {
                // 타겟 존재 확인
                float distanceToTarget = Vector3.Distance(transform.position, m_TargetObject.transform.position);
                distanceToTarget -= m_TargetObject.GetComponent<Unit>().m_radius + m_UnitController.Unit.m_radius;
                if (distanceToTarget <= m_AttackDistance)
                {
                    // 공격 
                    gameObject.GetComponent<UnitController>().Send_AttackMessage(m_TargetObject);
                }
                else
                {
                    // 추적
                    m_UnitController.Send_MoveStartMessage(m_TargetObject.transform.position);
                    //UnityEngine.Debug.Log("@@@@@@@@@@@@@@@ in Idle state -> trace @@@@@@@@@@@@@@@@@@ ");
                    //Vector3 direction = (m_TargetObject.transform.position - gameObject.transform.position);
                    //float diff = (m_TargetObject.transform.position - gameObject.transform.position).magnitude - m_AttackDistance;
                    //Vector3 destination = gameObject.transform.position + direction.normalized * diff;
                    //m_UnitController.Send_MoveStartMessage(destination);
                }
            }

           yield return new WaitForSeconds(1f);
        }

        //yield break;    
        // 현재 움직임이 없는 유닛 존재 발생
        // AttackJudgmentCoroutine의 yield break와 함께 의심해 볼것
    }

    public void StartAttackJudgmentCoroutine()
    {
        //UnityEngine.Debug.Log("StartAttackJudgmentCoroutine");
        if(AttackJudgmentCoroutine == null)
        {
            AttackJudgmentCoroutine = StartCoroutine(AttackJudgment());
        }
    }
    public void StopAttackJudgmentCoroutine()
    {
        //UnityEngine.Debug.Log("StopAttackJudgmentCoroutine");
        if (AttackJudgmentCoroutine != null )
        {
            StopCoroutine(AttackJudgmentCoroutine); 
            AttackJudgmentCoroutine = null; 
        }
    }

    private IEnumerator AttackJudgment()
    {
        int raceCnt = 0;
        float attackWaitTime = 0f;

        while (m_UnitController.State == enUnitState.ATTACK && m_UnitMovement.isCommandedToMove == false)
        {
            if (m_TargetObject != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, m_TargetObject.transform.position);
                distanceToTarget -= m_TargetObject.GetComponent<Unit>().m_radius + m_UnitController.Unit.m_radius;
                if (distanceToTarget <= m_AttackDistance)
                {
                    if(attackWaitTime <= 0f)
                    {
                        // 공격
                        yield return new WaitForSeconds(m_AttackDelay);
                        if (!m_UnitMovement.isCommandedToMove && m_TargetObject != null)
                        {
                            m_UnitController.Send_AttackMessage(m_TargetObject);
                        }

                        attackWaitTime = (1f / m_AttackRate) - m_AttackDelay;
                        yield return null;
                    }
                    else
                    {
                        attackWaitTime -= Time.deltaTime;

                        Vector3 normToTarget = (m_TargetObject.transform.position - gameObject.transform.position).normalized;
                        float angle = Vector3.Angle(normToTarget, gameObject.transform.forward);
                        if (angle > 1f)
                        {
                            //UnityEngine.Debug.Log("Send_MoveDirChangeMessage");
                            m_UnitController.Send_MoveDirChangeMessage(normToTarget);
                        }

                        yield return null;
                    }
                }
                else
                {
                    gameObject.GetComponent<UnitController>().Send_AttackStopMessage();
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                gameObject.GetComponent<UnitController>().Send_AttackStopMessage();
                yield return new WaitForSeconds(1f);
            }
            //yield return new WaitForSeconds((1f / m_AttackRate) - m_AttackDelay);   // attack delay는 (1f / m_AttackRate) 보다 작아야 함.
            yield return null;
        }

        yield break;
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
        if(other.CompareTag("Enemy"))
        {
            m_NearTargets.Add(other.gameObject);
            if(m_TargetObject == null)
            {
                m_TargetObject = other.gameObject;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && m_TargetObject == null)
        {
            GameObject nearestEnemy = GetNearestTarget();
            if (nearestEnemy == other.gameObject)
            {
                m_TargetObject = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (m_NearTargets.Contains(other.gameObject))
            {
                m_NearTargets.Remove(other.gameObject);   
            }

            if (m_TargetObject != null && m_TargetObject == other.gameObject)
            {
                m_TargetObject = null;
                float minDistance = float.MaxValue;
                foreach(var target in m_NearTargets)
                {
                    if(minDistance > Vector3.Distance(gameObject.transform.position, target.transform.position))
                    {
                        minDistance = Vector3.Distance(gameObject.transform.position, target.transform.position);
                        m_TargetObject = target;
                    }
                }
            }
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
        if (m_NearTargets.Contains(enemy))
        {
            m_NearTargets.Remove(enemy);    
        }
        if(m_TargetObject == enemy)
        {
            m_TargetObject = GetNearestTarget();
        }
    }
                   
    public GameObject GetNearestTarget()
    {
        GameObject nearestTarget = null;
        float minDistance = float.MaxValue;
        foreach (var target in m_NearTargets)
        {
            if(target == null) continue;

            if (minDistance > Vector3.Distance(gameObject.transform.position, target.transform.position))
            {
                minDistance = Vector3.Distance(gameObject.transform.position, target.transform.position);
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }

    public GameObject GetOtherTarget()
    {
        GameObject nearestOtherTarget = null;
        float minDistance = float.MaxValue;
        foreach (var target in m_NearTargets)
        {
            if (target == null || target == m_TargetObject) continue;

            if (minDistance > Vector3.Distance(gameObject.transform.position, target.transform.position))
            {
                minDistance = Vector3.Distance(gameObject.transform.position, target.transform.position);
                nearestOtherTarget = target;
            }
        }

        return nearestOtherTarget;
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