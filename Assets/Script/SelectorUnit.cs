using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SelectorUnit : MonoBehaviour
{
    Animator m_Animator;
    NavMeshAgent m_NavMeshAgent;

    public float m_Speed;

    // Start is called before the first frame update
    void Start()
    {
        Manager.UnitSelection.UnitList.Add(gameObject); 

        m_Animator = GetComponent<Animator>();  
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.speed = m_Speed;
    }

    // Update is called once per frame
    void Update()
    {
        // NavMeshAgent�� ������ ��ΰ� ���ų�, NavMeshAgent�� ��� ���̰� stop distance���� ª�� ���
        if (m_NavMeshAgent.remainingDistance > m_NavMeshAgent.stoppingDistance)
        {
            m_Animator.SetBool("isCmdToMove", true);
        }
        else
        {
            m_Animator.SetBool("isCmdToMove", false);
        }
    }

    private void OnDestroy()
    {
        if (Manager.UnitSelection.m_UnitsSelected.Contains(gameObject))
        {
            Manager.UnitSelection.m_UnitsSelected.Remove(gameObject);
        }
        Manager.UnitSelection.UnitList.Remove(gameObject);
    }
}
