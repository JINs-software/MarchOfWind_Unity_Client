using System;
using System.Collections;
using System.Collections.Generic;
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
    MOVE_SPATH,
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

    public bool ServerPathFindingReq = false;
    public bool ServerPathFinding = false;
    public bool ServerPathPending = false;
    public Queue<Tuple<int, Vector3>> ServerSPathQueue = new Queue<Tuple<int, Vector3>>();
    public int SpathID = 0;

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
        else if(moveType == enUnitState.MOVE_SPATH)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByServerPathFindingCoroutine());
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
                        else
                        {
                            // 공격 불가 && 유닛의 제자리 걸음
                            if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                            {
                                if (unchangedCount++ > 3)
                                {
                                    // 제자리 걸음 유지
                                    // => UNIT_S_REQ_TRACE_PATH_FINDING 메시지 전송을 통해 서버 측 JPS 경로 계산 유도

                                    ServerSPathQueue.Clear();
                                    ServerPathFindingReq = true;
                                    ServerPathFinding = true;
                                    ServerPathPending = true;
                                    Send_PathFindingReqMessage(m_NavMeshAgent.destination, ++SpathID);

                                    //yield return new WaitForSeconds(1f);
                                    yield break;
                                }
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

            if (!m_NavMeshAgent.pathPending && !ServerPathFindingReq)
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

                    if(m_AttackController.m_TargetObject == null)
                    {
                        // 타겟 해제 시
                        Send_MoveStopMessage();
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                    else
                    {
                        // 타겟 이동 시
                        if(Vector3.Distance(beforeTargetPosition, m_AttackController.m_TargetObject.transform.position) > 1)
                        {
                            Send_MoveStartMessage(m_AttackController.m_TargetObject.transform.position);
                        }
                    }

                    // 공격 범위 체크
                    float distanceToTarget = Vector3.Distance(transform.position, m_AttackController.m_TargetObject.transform.position);
                    float unitRadius = m_Unit.m_radius;
                    float targetRadius = m_AttackController.m_TargetObject.GetComponent<Unit>().m_radius;
                    distanceToTarget -= targetRadius + unitRadius;
                    if (distanceToTarget <= m_AttackController.m_AttackDistance)
                    {
                        // 공격 가능
                        yield return new WaitForSeconds(m_AttackController.m_AttackDelay);
                        if (!m_UnitMovement.isCommandedToMove && m_AttackController.m_TargetObject != null)
                        {
                            Send_AttackMessage(m_AttackController.m_TargetObject);
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    else
                    {
                        // 공격 불가 && 유닛의 제자리 걸음
                        if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                        {
                            if(unchangedCount++ > 3)
                            {
                                // 제자리 걸음 유지
                                // => UNIT_S_REQ_TRACE_PATH_FINDING 메시지 전송을 통해 서버 측 JPS 경로 계산 유도

                                ServerSPathQueue.Clear();
                                ServerPathFindingReq = true;
                                ServerPathFinding = false;
                                ServerPathPending = true;
                                Send_PathFindingReqMessage(m_NavMeshAgent.destination, ++SpathID);

                                yield return new WaitForSeconds(1f);
                            }
                        }
                    }
                }
                else
                {
                    // 타겟 없음
                    Send_MoveStopMessage();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator MoveStateByServerPathFindingCoroutine()
    {
        int unchangedCount = 0;
        Vector3 beforePosition, nextPostion;
        beforePosition = nextPostion = gameObject.transform.position;    

        while (true)
        {
            if(State != enUnitState.MOVE_SPATH || m_UnitMovement.isCommandedToMove || !ServerPathFinding)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            if (!m_NavMeshAgent.pathPending && !ServerPathPending)
            {
                // 타겟이 유효하고, 공격 범위 내 존재
                if (m_AttackController.m_TargetObject != null)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, m_AttackController.m_TargetObject.transform.position);
                    float unitRadius = m_Unit.m_radius;
                    float targetRadius = m_AttackController.m_TargetObject.GetComponent<Unit>().m_radius;
                    distanceToTarget -= targetRadius + unitRadius;
                    if (distanceToTarget <= m_AttackController.m_AttackDistance)
                    {
                        // 공격 가능
                        yield return new WaitForSeconds(m_AttackController.m_AttackDelay);
                        if (!m_UnitMovement.isCommandedToMove && m_AttackController.m_TargetObject != null)
                        {
                            Send_AttackMessage(m_AttackController.m_TargetObject);
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    else
                    {
                        float distance = Vector3.Distance(nextPostion, gameObject.transform.position);
                        if (distance < m_NavMeshAgent.stoppingDistance)
                        {
                            bool hasPath = false;
                            while(ServerSPathQueue.Count > 0)
                            {
                                Tuple<int, Vector3> spath = ServerSPathQueue.Dequeue();
                                if (spath.Item1 == SpathID)
                                {
                                    Send_MoveStartMessage(spath.Item2);
                                    nextPostion = spath.Item2;
                                    beforePosition = gameObject.transform.position;
                                    hasPath = true;
                                    break;
                                }
                            }

                            if (hasPath)
                            {
                                yield return new WaitForSeconds(0.1f);
                            }
                            else
                            {
                                // 최종 spath 목적지까지 도착하였는데, 타겟 공격이 없다는 뜻
                                // => idle 상태로 전이...
                                Send_MoveStopMessage();
                                yield return new WaitForSeconds(1f);
                            }
                        }
                        else
                        {
                            // 이동 유지..
                            // 그러나 여기서도 제자리 걸음이 유지된다면?
                            if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                            {
                                if (unchangedCount++ > 3)
                                {
                                    // 제자리 걸음 유지
                                    // => UNIT_S_REQ_TRACE_PATH_FINDING 메시지 전송을 통해 서버 측 JPS 경로 계산 유도
                                    //
                                    //Send_PathFindingReqMessage(m_NavMeshAgent.destination, ++SpathID);
                                    //
                                    //ServerPathFinding = true;
                                    //ServerPathPending = true;

                                    //Send_MoveStopMessage();

                                    yield return new WaitForSeconds(1f);
                                }
                            }
                            else
                            {
                                beforePosition = gameObject.transform.position;
                                yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }
                }
                else
                {
                    // 타겟 없음 -> 중지
                    Send_MoveStopMessage();
                    yield return new WaitForSeconds(1f);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

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

    public bool Send_PathFindingReqMessage(Vector3 destination, int spathID)
    {
        MSG_UNIT_S_REQ_TRACE_PATH_FINDING pathFindingReqMsg = new MSG_UNIT_S_REQ_TRACE_PATH_FINDING();
        pathFindingReqMsg.type = (ushort)enPacketType.UNIT_S_REQ_TRACE_PATH_FINDING;
        pathFindingReqMsg.spathID = spathID;
        pathFindingReqMsg.posX = gameObject.transform.position.x;
        pathFindingReqMsg.posZ = gameObject.transform.position.z;
        pathFindingReqMsg.normX = gameObject.transform.forward.normalized.x;
        pathFindingReqMsg.normZ = gameObject.transform.forward.normalized.z;
        pathFindingReqMsg.destX = destination.x;
        pathFindingReqMsg.destZ = destination.z;

        Debug.Log("Send_PathFindingReqMessage");
        return m_UnitSession.SendPacket<MSG_UNIT_S_REQ_TRACE_PATH_FINDING>(pathFindingReqMsg);  
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
