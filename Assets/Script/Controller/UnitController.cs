using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum enUnitState
{
    IDLE,
    MOVE,
    MOVE_COMMAND,
    MOVE_TRACING,
    ATTACK,
    DIE,
    CTR_WAIT
}

public class UnitController : MonoBehaviour
{
    NetworkManager m_UnitSession = null;
    public NetworkManager UnitSession { get { return m_UnitSession; }  set { m_UnitSession = value; } }

    public Unit m_Unit;
    public Unit Unit { get { return m_Unit; } set { m_Unit = value; } }

    AttackController m_AttackController = null; 
    UnitMovement m_UnitMovement = null;
    NavMeshAgent m_NavMeshAgent = null; 

    Vector3 m_PosBefore = Vector3.zero;
    Vector3 m_NormBefore = Vector3.zero;

    public bool OnMoving = false;

    public enUnitState State = enUnitState.IDLE;

    private Coroutine MoveStateCoroutine;
   

    // Start is called before the first frame update
    void Start()
    {
        Manager.UnitSelection.UnitList.Add(gameObject);

        m_AttackController = gameObject.GetComponent<AttackController>();   
        m_UnitMovement = gameObject.GetComponent<UnitMovement>();
        m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();   

        m_PosBefore = gameObject.transform.position;
        m_NormBefore = gameObject.transform.forward.normalized;
    }

    void OnDestroy()
    {
        Manager.UnitSelection.UnitList.Remove(gameObject);
    }


    private void Update()
    {
        // check direction changed(by AI.Nav)
        if (OnMoving)
        {
            if (CheckChangeDirection())
            {
                Debug.Log("CheckChangeDirection => Send_DirChangeMessage");
                Send_SyncDirectionMessage();
            }
            else if (CheckFowardByRadius())
            {
                Debug.Log("CheckFowardByRadius => Send_SyncPosMessage");
                Send_SyncPosMessage();
            }

            // => temp
        }
    }

    private bool CheckFowardByRadius()
    {
        if(Vector3.Distance(m_PosBefore, gameObject.transform.position) > m_Unit.m_radius)
        {
            m_PosBefore = gameObject.transform.position;
            return true;        
        }

        return false;
    }

    private bool CheckChangeDirection()
    {
        //bool ret = false;
        //
        //float angle = Vector3.Angle(m_NormBefore, gameObject.transform.forward);
        //float distance = Vector3.Distance(m_PosBefore, gameObject.transform.position);
        //
        //if (angle > 1f && distance > 1f)
        //{
        //    ret = true;
        //    m_NormBefore = gameObject.transform.forward.normalized;
        //    m_PosBefore = gameObject.transform.position;
        //}
        //
        //return ret;

        float angle = Vector3.Angle(m_NormBefore, gameObject.transform.forward);
        if(angle > 1f)
        {
            m_NormBefore = gameObject.transform.forward.normalized;
            return true;
        }

        return false;   
    }

    public void StartMoveStateCoroutine(enUnitState moveType)
    {
        if(MoveStateCoroutine != null)
        {
            StopCoroutine(MoveStateCoroutine);  
            MoveStateCoroutine = null;  
        }

        if(moveType == enUnitState.MOVE_COMMAND)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByCommandCourtine());
        }
        else if(moveType == enUnitState.MOVE_TRACING)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByTracingCoroutine());
        }
    }
    public void StopMoveStateCoroutine()
    {
        if(MoveStateCoroutine != null)
        {
            StopCoroutine(MoveStateCoroutine);
            MoveStateCoroutine = null;

            m_PosBefore = gameObject.transform.position;
            m_NormBefore = gameObject.transform.forward.normalized;
        }
    }

    private IEnumerator MoveStateByCommandCourtine()
    {
        // 제자리 걸음 카운터
        int unchangedCount = 0;

        while(true)
        {
            if(State != enUnitState.MOVE_COMMAND || m_UnitMovement.isCommandedToMove == false)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if(!m_NavMeshAgent.pathPending)
            {
                if(m_UnitMovement.TargetOnEnemy)
                {
                    if(m_AttackController.m_TargetObject != null)
                    {
                        Vector3 beforeTargetPosition = m_AttackController.m_TargetObject.transform.position;
                        Vector3 beforePosition = gameObject.transform.position;
                        float expectedTime = m_NavMeshAgent.remainingDistance / m_NavMeshAgent.speed;
                        if (expectedTime > 0.1f)
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                        else
                        {
                            yield return new WaitForSeconds(expectedTime);
                        }

                        // some time passe.. check moving
                        if (gameObject == null || m_AttackController == null || m_NavMeshAgent == null || m_UnitMovement == null)
                        {
                            yield break;
                        }
                        else if (m_AttackController.m_TargetObject == null)
                        {
                            Send_MoveStopMessage();
                            m_UnitMovement.isCommandedToMove = false;
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        else
                        {
                            if (Vector3.Distance(beforeTargetPosition, m_AttackController.m_TargetObject.transform.position) > 1)
                            {
                                Send_MoveStartMessage(m_AttackController.m_TargetObject.transform.position);
                            }
                        }

                        float distanceToTarget = Vector3.Distance(transform.position, m_AttackController.m_TargetObject.transform.position);
                        float unitRadius = m_Unit.m_radius;
                        float targetRadius = m_AttackController.m_TargetObject.GetComponent<Unit>().m_radius;
                        distanceToTarget -= targetRadius + unitRadius;
                        if (distanceToTarget <= m_AttackController.m_AttackDistance)
                        {
                            yield return new WaitForSeconds(m_AttackController.m_AttackDelay);
                            if (m_AttackController.m_TargetObject != null)
                            {
                                Send_AttackMessage(m_AttackController.m_TargetObject);
                                m_UnitMovement.isCommandedToMove = false;
                                yield return new WaitForSeconds(1f);
                            }
                        }
                    }
                    else
                    {
                        Send_MoveStopMessage();
                        m_UnitMovement.isCommandedToMove = false;
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    Vector3 beforePosition = gameObject.transform.position;
                    float expectedTime = m_NavMeshAgent.remainingDistance / m_NavMeshAgent.speed;
                    if (expectedTime > 0.1f)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(expectedTime);
                    }

                    // some time passe.. check moving
                    if (gameObject == null || m_AttackController == null || m_NavMeshAgent == null || m_UnitMovement == null)
                    {
                        yield break;
                    }

                    // 도착 및 정지 확인
                    if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                    {
                        Send_MoveStopMessage();
                        m_UnitMovement.isCommandedToMove = false;
                    }
                    else
                    {
                        if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                        {
                            if (m_NavMeshAgent.remainingDistance < m_UnitMovement.DistanceFromCenter || unchangedCount > 3)    // 조건 추가(제자리 걸음 횟수)
                            {
                                // 허용되는 정지 범위 내 -> 정지
                                Send_MoveStopMessage();
                                m_UnitMovement.isCommandedToMove = false;
                            }
                            else
                            {
                                unchangedCount++;

                                // 경로 재계산
                                Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, m_NavMeshAgent.remainingDistance);
                                Vector3 stoppedUnitsCenter = Vector3.zero;
                                int unitCnt = 0;
                                foreach (Collider collider in colliders)
                                {
                                    if (collider.gameObject != gameObject && collider.gameObject.GetComponent<NavMeshAgent>() != null && collider.gameObject.GetComponent<NavMeshAgent>().isStopped)
                                    {
                                        stoppedUnitsCenter += collider.gameObject.transform.position;
                                        unitCnt++;
                                    }
                                }

                                if (unitCnt > 0)
                                {
                                    stoppedUnitsCenter /= unitCnt;
                                    Send_MoveStartMessage(stoppedUnitsCenter);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("m_NavMeshAgent.pathPending!!!");
                yield return null;
            }
        }

        //yield break;
    }

    private IEnumerator MoveStateByTracingCoroutine()
    {
        int unchangedCount = 0;

        while (true)
        {
            if(State != enUnitState.MOVE_TRACING || m_UnitMovement.isCommandedToMove)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if (!m_NavMeshAgent.pathPending)
            {
                if (m_AttackController.m_TargetObject != null)
                {
                    Vector3 beforeTargetPosition = m_AttackController.m_TargetObject.transform.position;  
                    Vector3 beforePosition = gameObject.transform.position;
                    float expectedTime = m_NavMeshAgent.remainingDistance / m_NavMeshAgent.speed;
                    if (expectedTime > 0.1f)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(expectedTime);
                    }

                    // some time passe.. check moving
                    if (gameObject == null || m_AttackController == null || m_NavMeshAgent == null || m_UnitMovement == null)
                    {
                        yield break;
                    }
                    else if(m_AttackController.m_TargetObject == null)
                    {
                        Send_MoveStopMessage();
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                    else
                    {
                        if(Vector3.Distance(beforeTargetPosition, m_AttackController.m_TargetObject.transform.position) > 1)
                        {
                            Send_MoveStartMessage(m_AttackController.m_TargetObject.transform.position);
                        }
                    }

                    float distanceToTarget = Vector3.Distance(transform.position, m_AttackController.m_TargetObject.transform.position);
                    float unitRadius = m_Unit.m_radius;
                    float targetRadius = m_AttackController.m_TargetObject.GetComponent<Unit>().m_radius;
                    distanceToTarget -= targetRadius + unitRadius;
                    if (distanceToTarget <= m_AttackController.m_AttackDistance)
                    {
                        yield return new WaitForSeconds(m_AttackController.m_AttackDelay);
                        if (!m_UnitMovement.isCommandedToMove && m_AttackController.m_TargetObject != null)
                        {
                            Send_AttackMessage(m_AttackController.m_TargetObject);
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                        {
                            if (unchangedCount > 3)
                            {
                                GameObject newTarget = m_AttackController.ResetTarget();
                                if(newTarget == null)
                                {
                                    Send_MoveStopMessage();
                                }
                                else
                                {
                                    Send_MoveStartMessage(newTarget.transform.position);    
                                    yield return new WaitForSeconds(0.1f);
                                    continue;
                                }
                            }
                            unchangedCount++;

                            // 경로 재계산
                            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, m_AttackController.m_AttackDistance);
                            float maxDistance = 0f;
                            GameObject maxDistColliderObject = null;
                            bool searchEnemy = false;

                            foreach (Collider collider in colliders)
                            {
                                //if (collider.gameObject != gameObject && collider.gameObject.GetComponent<Unit>() != null && collider.gameObject.GetComponent<NavMeshAgent>() != null && collider.gameObject.GetComponent<NavMeshAgent>().isStopped)
                                //{
                                //    if (maxDistance < Vector3.Distance(collider.transform.position, gameObject.transform.position))
                                //    {
                                //        maxDistance = Vector3.Distance(collider.transform.position, gameObject.transform.position);
                                //        //maxDistanceObjectPos = collider.transform.position;
                                //        maxDistColliderObject = collider.gameObject;
                                //    }
                                //}
                                // => 조건 수정, Physics.OverlapSphere는 주어진 반경 내에 있는 모든 콜라이더를 검색.
                                // 이 콜라이더가 포함된 게임 오브젝트가 활성화되어 있지 않더라도, 콜라이더 자체가 씬 내에 존재한다면 결과에 포함될 수 있습니다. 즉, 비활성화된 게임 오브젝트라 하더라도,
                                // 해당 오브젝트의 콜라이더가 활성화되어 있다면 OverlapSphere는 해당 콜라이더를 결과에 포함
                                // 따라서 활성화된 상태가 아닌 객체의 NavMeshAgent 컴포넌트의 isStopped에 접근하여 런타임 에러가 발생하였음
                                if (collider.gameObject != gameObject && collider.gameObject.GetComponent<Unit>() != null)
                                {
                                    NavMeshAgent agent = collider.gameObject.GetComponent<NavMeshAgent>();
                                    if (agent != null && agent.isOnNavMesh && agent.isStopped)
                                    {
                                        float distance = Vector3.Distance(collider.transform.position, gameObject.transform.position);
                                        if (maxDistance < distance)
                                        {
                                            maxDistance = distance;
                                            maxDistColliderObject = collider.gameObject;
                                        }
                                    }   
                                }
                            }

                            if (!searchEnemy && maxDistance > 0f)
                            {
                                Vector3 avoidancePosition = maxDistColliderObject.transform.position;
                                avoidancePosition += (maxDistColliderObject.transform.position - gameObject.transform.position).normalized * (maxDistColliderObject.GetComponent<Unit>().m_radius + gameObject.GetComponent<Unit>().m_radius);
                                Send_MoveStartMessage(avoidancePosition);
                            }
                        }   
                    }
                }
                else
                {
                    Send_MoveStopMessage();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                yield return null;
            }
        }

        //yield break;
    }

    /*
     
    public void StartMoveStateCoroutine()
    {
        Debug.Log("StartMoveStateCoroutine");
        if(MoveStateCoroutine == null)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateCoroutineFunc());
        }
    }
    public void StopMoveStateCoroutine()
    {
        Debug.Log("StopMoveStateCoroutine");
        if (MoveStateCoroutine != null)
        {
            StopCoroutine(MoveStateCoroutine);    
            MoveStateCoroutine = null;  
        }
    }

    private IEnumerator MoveStateCoroutineFunc()
    {
        float distanceFromDestination = m_NavMeshAgent.remainingDistance;

        while(State == enUnitState.MOVE)
        {
            Debug.Log("MoveStateCoroutineFunc_new");

            if (!m_NavMeshAgent.pathPending)
            {
                // 커맨드를 통한 이동 정지 판단
                if (m_UnitMovement.isCommandedToMove)
                {
                    if (m_NavMeshAgent.remainingDistance < m_UnitMovement.DistanceFromCenter)
                    {
                        if(m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                        {
                            // 정지
                            Send_MoveStopMessage();
                            m_UnitMovement.isCommandedToMove = false;
                            yield return new WaitForSeconds(0.01f);
                        }
                        else if(IsNearByUnit())
                        {
                            // 정지
                            Send_MoveStopMessage();
                            m_UnitMovement.isCommandedToMove = false;
                            yield return new WaitForSeconds(0.01f);
                        }
                        // 점점 더 멀어짐, 이 시점부터 어느정도 이동을 시도하다 멈추어야 함.
                        else 
                        {
                            if (m_NavMeshAgent.remainingDistance > distanceFromDestination)
                            {
                                // 1초 이동 기회 부여
                                yield return new WaitForSeconds(0.01f);

                                if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                                {
                                    // 정지
                                    Send_MoveStopMessage();
                                    m_UnitMovement.isCommandedToMove = false;
                                    yield return new WaitForSeconds(0.01f);
                                }
                                else if (IsNearByUnit())
                                {
                                    // 정지
                                    Send_MoveStopMessage();
                                    m_UnitMovement.isCommandedToMove = false;
                                    yield return new WaitForSeconds(0.01f);
                                }

                                // 다시 1초 이동 기회 부여

                                yield return new WaitForSeconds(0.01f);
                                if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                                {
                                    // 정지
                                    Send_MoveStopMessage();
                                    m_UnitMovement.isCommandedToMove = false;
                                    yield return new WaitForSeconds(0.01f);
                                }
                                else if (IsNearByUnit())
                                {
                                    // 정지
                                    Send_MoveStopMessage();
                                    m_UnitMovement.isCommandedToMove = false;
                                    yield return new WaitForSeconds(0.01f);
                                }

                                // 다시 1초 이동 기회 부여
                                // 정지
                                Send_MoveStopMessage();
                                m_UnitMovement.isCommandedToMove = false;
                                yield return new WaitForSeconds(0.01f);
                            }
                        }
                    }

                    distanceFromDestination = m_NavMeshAgent.remainingDistance;
                }
                // 추적을 통한 이동 정지 판단
                else
                {
                    // 충돌 판단
                    //Collider[] colliders;
                    //colliders = Physics.OverlapSphere(gameObject.transform.position, m_NavMeshAgent.radius * gameObject.transform.localScale.x + 1);
                    //foreach (Collider collider in colliders)
                    //{
                    //    GameObject nearObject = collider.gameObject;
                    //    NavMeshAgent nearNavMeshAgent = nearObject.GetComponent<NavMeshAgent>();
                    //    // 자신 오브젝트가 아니면서 && 선택된 유닛이면서 && NavMeshAgent 컴포넌트를 가지면서, 해당 컴포넌트가 isStopped 상태일 때 
                    //    if (nearObject != gameObject && nearNavMeshAgent != null && nearNavMeshAgent.hasPath)
                    //    {
                    //        if(m_NavMeshAgent.remainingDistance > nearNavMeshAgent.remainingDistance)
                    //        {
                    //            m_NavMeshAgent.ResetPath();
                    //            yield return new WaitForSeconds(0.1f);
                    //        }
                    //    }
                    //}

                    //Debug.Log("In Tracing...");
                    if (m_AttackController.m_TargetObject != null)
                    {
                        //Debug.Log("m_AttackController.m_TargetObject != null");
                        float distanceToTarget = Vector3.Distance(transform.position, m_AttackController.m_TargetObject.transform.position);
                        distanceToTarget -= m_AttackController.m_TargetObject.GetComponent<NavMeshAgent>().radius * m_AttackController.m_TargetObject.transform.localScale.x;
                        if (distanceToTarget <= m_AttackController.m_AttackDistance)
                        {
                            //Debug.Log("distanceToTarget <= m_AttackController.m_AttackDistance => SendAttackMsg");
                            Send_AttackMessage(m_AttackController.m_TargetObject);
                            //yield return new WaitForSeconds(0.01f);
                        }
                        else
                        {
                            //Debug.Log("distanceToTarget > m_AttackController.m_AttackDistance");
                            //Debug.Log("distanceToTarget: " + distanceToTarget);
                            //Debug.Log("AttackDistance: " + m_AttackController.m_AttackDistance);

                            Collider[] colliders;
                            colliders = Physics.OverlapSphere(gameObject.transform.position, m_NavMeshAgent.radius * gameObject.transform.localScale.x + 1);
                            foreach (Collider collider in colliders)
                            {
                                GameObject nearObject = collider.gameObject;
                                NavMeshAgent nearNavMeshAgent = nearObject.GetComponent<NavMeshAgent>();
                                // 자신 오브젝트가 아니면서 && 선택된 유닛이면서 && NavMeshAgent 컴포넌트를 가지면서, 해당 컴포넌트가 isStopped 상태일 때 
                                if (nearObject != gameObject && nearNavMeshAgent != null)
                                {
                                    if (m_NavMeshAgent.remainingDistance > nearNavMeshAgent.remainingDistance || m_NavMeshAgent.avoidancePriority > nearNavMeshAgent.avoidancePriority)
                                    {
                                        m_NavMeshAgent.ResetPath();
                                    }
                                }
                            }

                            Send_MoveStartMessage(m_AttackController.m_TargetObject.transform.position);
                            yield return new WaitForSeconds(0.01f);

                            //Vector3 direction = (m_AttackController.m_TargetObject.transform.position - gameObject.transform.position);
                            //float diff = (m_AttackController.m_TargetObject.transform.position - gameObject.transform.position).magnitude - m_AttackController.m_AttackDistance;
                            //Vector3 destination = gameObject.transform.position + direction.normalized * diff;
                            //Send_MoveStartMessage(destination);
                            //yield return new WaitForSeconds(0.01f);
                        }
                    }
                    else
                    {
                        //Debug.Log("m_AttackController.m_TargetObject != null");
                        //if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                        //{
                        //    if (!m_NavMeshAgent.hasPath || m_NavMeshAgent.velocity.sqrMagnitude == 0f)
                        //    {
                        //        Debug.Log("Send_MoveStopMessage~");
                        //        Send_MoveStopMessage();
                        //        yield return new WaitForSeconds(0.01f);
                        //    }
                        //}
                        // => 커맨드가 아닌 이동은 결국 추적, 추적 대상이 더 이상 존재하지 않는 다면 그냥 멈추는 것이 맞아 보임(?)
                        Send_MoveStopMessage();
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
            else
            {
                Debug.Log("m_NavMeshAgent.pathPending...");
            }

            yield return null;
        }
    }

    private bool ShouldStop()
    {
        if (IsNearByUnit() && m_NavMeshAgent.remainingDistance < m_UnitMovement.DistanceFromCenter)
        {
            m_UnitMovement.DistanceFromCenter = 0;
            return true;
        }

        return false;
    }
    private bool IsNearByUnit()
    {
        // 현재 위치를 기준으로 주변에 장애물(유닛 등)이 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, m_NavMeshAgent.radius * gameObject.transform.localScale.x + 1);
                                                                                    // 장애물 확인 범주는 유닛 Radius + 1로 지정(m_NavMeshAgent.radius * gameObject.transform.localScale.x + 1)

        foreach (Collider collider in colliders)
        {
            GameObject nearObject = collider.gameObject;
            // 자신 오브젝트가 아니면서 && 선택된 유닛이면서 && NavMeshAgent 컴포넌트를 가지면서, 해당 컴포넌트가 isStopped 상태일 때 
            if (nearObject != gameObject && Manager.UnitSelection.m_UnitsSelected.Contains(nearObject) && nearObject.GetComponent<NavMeshAgent>() != null && nearObject.GetComponent<NavMeshAgent>().isStopped)
            {
                return true;
            }
        }

        return false;
    }
    */


    /*****************************************************************************
     * Send Packet
     *****************************************************************************/
    public bool Send_MoveStartMessage(Vector3 destionation)
    {
        MSG_UNIT_S_MOVE moveMsg = new MSG_UNIT_S_MOVE();
        moveMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
        moveMsg.moveType = (byte)enUnitMoveType.Move_Start;
        moveMsg.posX = gameObject.transform.position.x;
        moveMsg.posZ = gameObject.transform.position.z; 
        //moveMsg.normX = gameObject.transform.forward.normalized.x;
        //moveMsg.normZ = gameObject.transform.forward.normalized.z;
        Vector3 dirVec = (destionation - gameObject.transform.position).normalized;
        moveMsg.normX = dirVec.x;
        moveMsg.normZ = dirVec.z;
        moveMsg.destX = destionation.x;
        moveMsg.destZ = destionation.z;

        Debug.Log("Send_MoveStartMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_MOVE>(moveMsg);
    }

    public bool Send_MoveStopMessage()
    {
        MSG_UNIT_S_MOVE stopMsg = new MSG_UNIT_S_MOVE();
        stopMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
        stopMsg.moveType = (byte)enUnitMoveType.Move_Stop;
        stopMsg.posX = gameObject.transform.position.x;
        stopMsg.posZ = gameObject.transform.position.z;
        stopMsg.normX = gameObject.transform.forward.normalized.x;
        stopMsg.normZ = gameObject.transform.forward.normalized.z;

        Debug.Log("Send_MoveStopMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_MOVE>(stopMsg);
    }

    public bool Send_MoveDirChangeMessage(Vector3 normVec)
    {
        MSG_UNIT_S_MOVE dirCngMsg = new MSG_UNIT_S_MOVE();
        dirCngMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
        dirCngMsg.moveType = (byte)enUnitMoveType.Move_Change_Dir;
        dirCngMsg.posX = gameObject.transform.position.x;
        dirCngMsg.posZ = gameObject.transform.position.z;
        dirCngMsg.normX = normVec.x;
        dirCngMsg.normZ = normVec.z;

        Debug.Log("Send_MoveStopMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_MOVE>(dirCngMsg);
    }

    public bool Send_SyncPosMessage()
    {
        MSG_UNIT_S_SYNC_POSITION syncMsg = new MSG_UNIT_S_SYNC_POSITION();
        syncMsg.type = (ushort)enPacketType.UNIT_S_SYNC_POSITION;
        syncMsg.posX = gameObject.transform.position.x;
        syncMsg.posZ = gameObject.transform.position.z;
        syncMsg.normX = gameObject.transform.forward.normalized.x;
        syncMsg.normZ = gameObject.transform.forward.normalized.z;

        Debug.Log("Send_SyncPosMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_SYNC_POSITION>(syncMsg);
    }
    public bool Send_SyncDirectionMessage()
    {
        MSG_UNIT_S_SYNC_DIRECTION dirMsg = new MSG_UNIT_S_SYNC_DIRECTION();
        dirMsg.type = (ushort)enPacketType.UNIT_S_SYNC_DIRECTION;
        dirMsg.normX = gameObject.transform.forward.normalized.x;
        dirMsg.normZ = gameObject.transform.forward.normalized.z;

        Debug.Log("Send_DirChangeMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_SYNC_DIRECTION>(dirMsg);
    }

    public bool Send_AttackMessage(GameObject targetObject)
    {
        MSG_UNIT_S_ATTACK atkMsg = new MSG_UNIT_S_ATTACK();
        atkMsg.type = (ushort)enPacketType.UNIT_S_ATTACK;
        atkMsg.posX = gameObject.transform.position.x;
        atkMsg.posZ = gameObject.transform.position.z;
        Vector3 dirVec = (targetObject.transform.position - gameObject.transform.position).normalized;
        atkMsg.normX = dirVec.x;
        atkMsg.normZ = dirVec.z;
        atkMsg.targetID = targetObject.GetComponent<Enemy>().m_Unit.m_id;
        atkMsg.attackType = (int)enUnitAttackType.ATTACK_NORMAL;

        Debug.Log("Send_AttackMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_ATTACK>(atkMsg);
    }

    public bool Send_AttackStopMessage()
    {
        MSG_UNIT_S_ATTACK_STOP atkStopMsg = new MSG_UNIT_S_ATTACK_STOP();
        atkStopMsg.type = (ushort)enPacketType.UNIT_S_ATTACK_STOP;
        atkStopMsg.posX = gameObject.transform.position.x;  
        atkStopMsg.posZ= gameObject.transform.position.z;   
        atkStopMsg.normX = gameObject.transform.forward.x;  
        atkStopMsg.normZ = gameObject.transform.forward.z;

        Debug.Log("Send_AttackStopMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_ATTACK_STOP>(atkStopMsg);
    }
}
