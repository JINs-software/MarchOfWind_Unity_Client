using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera m_Camera;
    NavMeshAgent m_NavMeshAgent;
    //UnitController m_UnitController;   
    LayerMask m_LayerMask;

    public Action<Vector3, float> MoveCmdHandler;

    public bool isCommandedToMove;
    public float DistanceFromCenter;
    public bool TargetOnEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.isStopped = true;
        m_LayerMask = LayerMask.GetMask("GroundLayer");
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_LayerMask))
            {
                isCommandedToMove = true;
                // => WAIT 상태 활용

                if (gameObject.tag == "Selector")
                {
                    m_NavMeshAgent.isStopped = false;
                    m_NavMeshAgent.SetDestination(hit.point);
                }
                else if (gameObject.tag == "Dummy")
                {
                    gameObject.GetComponent<Dummy>().OnMoveCmd = true;
                    gameObject.GetComponent<Dummy>().Destination = hit.point;       
                }
                else
                {
                    DistanceFromCenter = GamaManager.UnitSelection.UnitSelectedCircumscriber * 2;
                    //gameObject.GetComponent<UnitController>().Send_MoveStartMessage(hit.point);
                    // => unitcontroller에서 수행 (아래 Invoke)

                    if (MoveCmdHandler != null)
                    {
                        MoveCmdHandler.Invoke(hit.point, DistanceFromCenter);
                    }
                }
            }
        }
    }
}
