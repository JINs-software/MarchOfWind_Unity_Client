using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera m_Camera;
    NavMeshAgent m_NavMeshAgent;
    UnitController m_UnitController;    

    [SerializeField]
    LayerMask m_LayerMask;

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

        m_UnitController = gameObject.GetComponent<UnitController>();
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

                if(gameObject.tag == "Selector") 
                {
                    m_NavMeshAgent.isStopped = false;
                    m_NavMeshAgent.SetDestination(hit.point);
                }
                else
                {
                    //m_UnitController.State = enUnitState.CTR_WAIT;

                    //DistanceFromCenter = (Manager.UnitSelection.CenterOfUnitSelected - transform.position).magnitude;
                    DistanceFromCenter = Manager.UnitSelection.UnitSelectedCircumscriber * 2;
                    if (!gameObject.GetComponent<UnitController>().Send_MoveStartMessage(hit.point))
                    {
                        Debug.Log("UnitMovenent, Send_MoveStartMessage 송신 실패");
                    }
                }
            }
        }
    }

    //IEnumerator WaitForPath()
    //{
    //    isPathFinding = true;
    //    Debug.Log("path finding..");
    //    yield return new WaitUntil(() => m_NavMeshAgent.pathPending == false);
    //    Debug.Log("path finidhed!");
//
    //    if (!m_NavMeshAgent.hasPath)
    //    {
    //        isCommandedToMove = false;
    //        m_NavMeshAgent.isStopped = true;
    //        Debug.LogWarning("No valid path found for the agent.");
    //    }
    //}
}
