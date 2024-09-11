using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static System.Collections.Specialized.BitVector32;
using Random = UnityEngine.Random;

public enum enUNIT_STATUS
{
    IDLE,
    MOVE,
    MOVE_COMMAND,
    MOVE_TRACING,
    MOVE_SPATH_PENDING,
    MOVE_SPATH,
    ATTACK,
    DIE,
    CTR_WAIT
}

public class UnitController : MonoBehaviour
{
    // Unit Network Session
    NetworkManager m_UnitSession = null;
    public NetworkManager UnitSession { get { return m_UnitSession; }  set { m_UnitSession = value; } }

    // Unit info
    public Unit m_Unit;
    public Unit Unit { get { return m_Unit; } set { m_Unit = value; } }

    // Components
    AttackController m_AttackController = null; 
    UnitMovement m_UnitMovement = null;
    NavMeshAgent m_NavMeshAgent = null;

    // MoveCommand
    float m_DistanceFromCenterOfSelections;

    // Sync
    Vector3 m_PosBefore = Vector3.zero;
    Vector3 m_NormBefore = Vector3.zero;

    // Unit Status
    public bool OnMoving = false;
    public enUNIT_STATUS State = enUNIT_STATUS.IDLE;

    public Queue<Tuple<int, Vector3>> ServerSPathQueue = new Queue<Tuple<int, Vector3>>();
    public int SpathID = 0;

    private Coroutine MoveStateSyncCoroutine = null;
    private Coroutine MoveStateCoroutine = null;


    // Start is called before the first frame update
    void Start()
    {
        GamaManager.UnitSelection.UnitList.Add(gameObject);

        m_AttackController = gameObject.GetComponent<AttackController>();   
        m_UnitMovement = gameObject.GetComponent<UnitMovement>();
        m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();   

        m_PosBefore = gameObject.transform.position;
        m_NormBefore = gameObject.transform.forward.normalized;
    }

    void OnDestroy()
    {
        GamaManager.UnitSelection.UnitList.Remove(gameObject);
    }

    public void OnMoveCmd(Vector3 rayHitPoint, float distanceFromCenterOfSelections)
    {
        State = enUNIT_STATUS.MOVE_COMMAND;
        m_DistanceFromCenterOfSelections = distanceFromCenterOfSelections;
        //Send_MoveStartMessage(rayHitPoint);
        SEND_MOVE_START(rayHitPoint);
    }

    public void MOVE_START(Vector3 destination, enUNIT_STATUS status)
    {
        State = status;
        //Send_MoveStartMessage(destination);
        SEND_MOVE_START(destination);
    }

    public void MOVE_STOP()
    {
        State = enUNIT_STATUS.IDLE;
        //Send_MoveStopMessage();
        SEND_MOVE_STOP();
    }

    public void SPATH_REQ()
    {
        State = enUNIT_STATUS.MOVE_SPATH_PENDING;
        //Send_SyncPosMessage();
        SEND_SYNC();
        ServerSPathQueue.Clear();
        //Send_PathFindingReqMessage(m_NavMeshAgent.destination, ++SpathID);
        SEND_TRACE_PATH_FINDING_REQ(m_NavMeshAgent.destination, ++SpathID);
    }

    public void SPATH_REPLY(int spathID)
    {
        if (State == enUNIT_STATUS.MOVE_SPATH_PENDING && SpathID == spathID)
        {
            Debug.Log("@SPATH_REPLY@");
        }
    }

    public void SPATH(Int32 SPATH_ID, float POS_X, float POS_Z, byte SPATH_OPT)
    {
        RecvSPath(SPATH_ID, SPATH_OPT, new Vector3(POS_X, 0, POS_Z));
    }

    public void LAUNCH_ATTACK(Vector3 TargetPosition)
    {
        State = enUNIT_STATUS.ATTACK;
        //SendAttackLaunch();
        SEND_LAUNCH_ATTACK(TargetPosition);
    }

    public void STOP_ATTACK()
    {
        State = enUNIT_STATUS.IDLE;
        //Unit.AttackStop();
        //Send_AttackStopMessage();
        SEND_STOP_ATTACK();
    }

    public void ATTACK()
    {
        if(m_AttackController.HasTarget())
        {
            State = enUNIT_STATUS.ATTACK;
            SEND_ATTACK(m_AttackController.GetTargetID(), (byte)enATTACK_TYPE.BASE);    // 임시, 공격 타입 추가 시 변경
        }
    }

    public void TRACE(GameObject target)
    {
        State = enUNIT_STATUS.MOVE_TRACING;
        //Send_MoveStartMessage(target.transform.position);
        SEND_MOVE_START(target.transform.position);
    }

    public void RecvSPath(int spathID, byte spathOpt, Vector3 position)
    {
        if (State == enUNIT_STATUS.MOVE_SPATH_PENDING && SpathID == spathID)
        {
            enSPathStateType spathType = (enSPathStateType)spathOpt;
            if (spathType != enSPathStateType.END_OF_PATH)
            {
                Tuple<int, Vector3> spath = new Tuple<int, Vector3>(spathID, position);
                ServerSPathQueue.Enqueue(spath);

                GameObject prefab = Resources.Load<GameObject>("prefab/SPath");
                Instantiate(prefab, position, Quaternion.identity).SetActive(true);
            }
            else
            {
                State = enUNIT_STATUS.MOVE_SPATH;
            }
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

    public void StartMoveStateCoroutine()
    {
        if(MoveStateSyncCoroutine == null)
        {
            StartCoroutine(MoveStateSyncCoroutineFunc());
        }
        if(MoveStateCoroutine != null)
        {
            StopCoroutine(MoveStateCoroutine);  
            MoveStateCoroutine = null;  
        }

        if(State == enUNIT_STATUS.MOVE_COMMAND)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByCommandCourtine());
        }
        else if(State == enUNIT_STATUS.MOVE_TRACING)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByTracingCoroutine());
        }
        else if(State == enUNIT_STATUS.MOVE_SPATH)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByServerPathFindingCoroutine());
        }
    }

    public void StopMoveStateCoroutine()
    {
        if(MoveStateSyncCoroutine != null)
        {
            StopCoroutine(MoveStateSyncCoroutine);
            MoveStateSyncCoroutine = null;  
        }
        if(MoveStateCoroutine != null)
        {
            StopCoroutine(MoveStateCoroutine);
            MoveStateCoroutine = null;

            m_PosBefore = gameObject.transform.position;
            m_NormBefore = gameObject.transform.forward.normalized;
        }
    }

    private void ResetMoveStateCoroutine()
    {
        if (MoveStateCoroutine != null)
        {
            StopCoroutine(MoveStateCoroutine);
        }

        if (State == enUNIT_STATUS.MOVE_COMMAND)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByCommandCourtine());
        }
        else if (State == enUNIT_STATUS.MOVE_TRACING)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByTracingCoroutine());
        }
        else if (State == enUNIT_STATUS.MOVE_SPATH || State == enUNIT_STATUS.MOVE_SPATH_PENDING)
        {
            MoveStateCoroutine = StartCoroutine(MoveStateByServerPathFindingCoroutine());
        }
    }

    private IEnumerator MoveStateSyncCoroutineFunc()
    {
        while (State == enUNIT_STATUS.MOVE_COMMAND || State == enUNIT_STATUS.MOVE_TRACING || State == enUNIT_STATUS.MOVE_SPATH)
        {
            //if (CheckChangeDirection())
            //{
            //    Send_SyncDirectionMessage();
            //}
            //else if (CheckFowardByRadius())
            //{
            //    Send_SyncPosMessage();
            //}

            if(CheckChangeDirection() || CheckFowardByRadius())
            {
                SEND_SYNC();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator MoveStateByCommandCourtine()
    {
        Debug.Log("MoveStateByCommandCourtine~~~");

        int unchangedCount = 0;

        while(true)
        {
            if(State == enUNIT_STATUS.MOVE_TRACING || State == enUNIT_STATUS.MOVE_SPATH_PENDING || State == enUNIT_STATUS.MOVE_SPATH) ResetMoveStateCoroutine();
            //else if(State != enUNIT_STATUS.MOVE_COMMAND) { yield return new WaitForSeconds(0.1f); continue; }
            
            if(!m_NavMeshAgent.pathPending)
            {
                // 적군 타겟 지정 상태
                if(m_UnitMovement.TargetOnEnemy)
                {
                    if(m_AttackController.HasTarget())
                    {
                        Vector3 beforeTargetPosition = m_AttackController.m_TargetObject.transform.position;
                        Vector3 beforePosition = gameObject.transform.position;
                        float expectedTime = m_NavMeshAgent.remainingDistance / m_NavMeshAgent.speed;
                        if (expectedTime > 0.1f) yield return new WaitForSeconds(0.1f);
                        else yield return new WaitForSeconds(expectedTime);

                        // some time passe.. check movin
                        if (gameObject == null || m_AttackController == null || m_NavMeshAgent == null || !m_NavMeshAgent.isActiveAndEnabled || !m_NavMeshAgent.isOnNavMesh || m_UnitMovement == null) yield break;

                        if (!m_AttackController.HasTarget())
                        {
                            m_UnitMovement.isCommandedToMove = false;
                            MOVE_STOP();
                            //yield return new WaitForSeconds(0.1f);
                        }
                        else
                        {
                            if (m_AttackController.GetDistanceFromTarget() <= m_AttackController.m_AttackDistance)
                            {
                                m_UnitMovement.isCommandedToMove = false;
                                LAUNCH_ATTACK(m_AttackController.m_TargetObject.transform.position);
                                //yield return new WaitForSeconds(1f);
                            }
                            else
                            {
                                // 타겟 위치 변경
                                if (Vector3.Distance(beforeTargetPosition, m_AttackController.m_TargetObject.transform.position) > 1)
                                {
                                    MOVE_START(m_AttackController.m_TargetObject.transform.position, enUNIT_STATUS.MOVE_COMMAND);
                                    //yield return new WaitForSeconds(0.1f);
                                }
                                // 목적지 도착
                                else if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                                {
                                    m_UnitMovement.isCommandedToMove = false;
                                    MOVE_STOP();
                                    //yield return new WaitForSeconds(1f);
                                }
                                // 제자리 걸음
                                else if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                                {
                                    unchangedCount++;
                                    if(unchangedCount > 3)
                                    {
                                        // 제자리 걸음 반복 -> spath 요청
                                        SPATH_REQ();
                                        //yield return new WaitForSeconds(1f);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        m_UnitMovement.isCommandedToMove = false;
                        MOVE_STOP();
                        //yield return new WaitForSeconds(1f);
                    }
                }
                else
                {
                    Vector3 beforePosition = gameObject.transform.position;
                    float expectedTime = m_NavMeshAgent.remainingDistance / m_NavMeshAgent.speed;
                    if (expectedTime > 0.1f) yield return new WaitForSeconds(0.1f);
                    else yield return new WaitForSeconds(expectedTime);

                    // some time passe.. check moving

                    if (gameObject == null || m_AttackController == null || m_NavMeshAgent == null || !m_NavMeshAgent.isActiveAndEnabled || !m_NavMeshAgent.isOnNavMesh || m_UnitMovement == null) yield break;

                    // 목적지 도착
                    if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                    {
                        m_UnitMovement.isCommandedToMove = false;
                        MOVE_STOP();
                        //yield return new WaitForSeconds(1f);
                    }
                    // 제자리 걸음
                    else if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                    {
                        if (m_NavMeshAgent.remainingDistance < m_DistanceFromCenterOfSelections || unchangedCount > 3)    // 조건 추가(제자리 걸음 횟수)
                        {
                            // 허용되는 정지 범위 내 -> 정지
                            m_UnitMovement.isCommandedToMove = false;
                            MOVE_STOP();
                            //yield return new WaitForSeconds(1f);
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
                                MOVE_START(stoppedUnitsCenter, enUNIT_STATUS.MOVE_COMMAND);
                                //yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("m_NavMeshAgent.pathPending!!!");
                yield return null;
            }
        }

        //yield break;
    }

    private IEnumerator MoveStateByTracingCoroutine()
    {
        Debug.Log("MoveStateByTracingCoroutine~~~");
        int unchangedCount = 0;

        while (true)
        {
            if (State == enUNIT_STATUS.MOVE_COMMAND || State == enUNIT_STATUS.MOVE_SPATH_PENDING || State == enUNIT_STATUS.MOVE_SPATH) ResetMoveStateCoroutine();
            //else if (State != enUNIT_STATUS.MOVE_TRACING) { yield return new WaitForSeconds(0.1f); continue; }

            if (!m_NavMeshAgent.pathPending)
            {
                if (m_AttackController.HasTarget())
                {
                    Vector3 beforeTargetPosition = m_AttackController.m_TargetObject.transform.position;  
                    Vector3 beforePosition = gameObject.transform.position;
                    float expectedTime = m_NavMeshAgent.remainingDistance / m_NavMeshAgent.speed;
                    if (expectedTime > 0.1f) yield return new WaitForSeconds(0.1f);
                    else yield return new WaitForSeconds(expectedTime);

                    // some time passe.. check moving
                    if (gameObject == null || m_AttackController == null || m_NavMeshAgent == null || !m_NavMeshAgent.isActiveAndEnabled || !m_NavMeshAgent.isOnNavMesh || m_UnitMovement == null) yield break;

                    if (!m_AttackController.HasTarget())
                    {
                        m_UnitMovement.isCommandedToMove = false;
                        MOVE_STOP();
                        //yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        if (m_AttackController.GetDistanceFromTarget() <= m_AttackController.m_AttackDistance)
                        {
                            m_UnitMovement.isCommandedToMove = false;
                            LAUNCH_ATTACK(m_AttackController.m_TargetObject.transform.position);
                            //yield return new WaitForSeconds(1f);
                        }
                        else
                        {
                            // 타겟 위치 변경
                            if (Vector3.Distance(beforeTargetPosition, m_AttackController.m_TargetObject.transform.position) > 1)
                            {
                                //MOVE_START(m_AttackController.m_TargetObject.transform.position, enUNIT_STATUS.MOVE_TRACING);
                                //yield return new WaitForSeconds(0.1f);
                            }
                            // 목적지 도착
                            else if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                            {
                                m_UnitMovement.isCommandedToMove = false;
                                MOVE_STOP();
                                //yield return new WaitForSeconds(1f);
                            }
                            // 제자리 걸음
                            else if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                            {
                                unchangedCount++;
                                if (unchangedCount > 3)
                                {
                                    // 제자리 걸음 반복 -> spath 요청
                                    SPATH_REQ();
                                    //yield return new WaitForSeconds(1f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MOVE_STOP();
                    //yield return new WaitForSeconds(1f);
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
        Debug.Log("Debug.Log(\"MoveStateByTracingCoroutine~~~\");~~~");

        int unchangedCount = 0;
        Vector3 beforePosition, nextPostion;
        beforePosition = nextPostion = gameObject.transform.position;
        Vector3 targetOrgnPosition = Vector3.zero;
        if (m_AttackController.m_TargetObject != null)
        {
            targetOrgnPosition = m_AttackController.m_TargetObject.transform.position;
        }

        while (true)
        {
            if (State == enUNIT_STATUS.MOVE_COMMAND || State == enUNIT_STATUS.MOVE_TRACING) ResetMoveStateCoroutine();
            // if (/*State != enUNIT_STATUS.MOVE_SPATH_PENDING && */ State != enUNIT_STATUS.MOVE_SPATH) { yield return new WaitForSeconds(0.1f); continue; }

            if (!m_NavMeshAgent.pathPending)
            {
                // 타겟이 유효하고, 공격 범위 내 존재
                if (m_AttackController.HasTarget())
                {
                    if (m_AttackController.GetDistanceFromTarget() <= m_AttackController.m_AttackDistance)
                    {
                        // 공격 가능
                        LAUNCH_ATTACK(m_AttackController.m_TargetObject.transform.position);
                        //yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        // 타겟 위치 변경
                        if (Vector3.Distance(targetOrgnPosition, m_AttackController.m_TargetObject.transform.position) > 1)
                        {
                            // PathPending == false, 즉 이전의 jps 추적을 통해 결과를 받은 상태에서 타겟의 위치가 변경되면 idle 상태로 복귀할 것
                            MOVE_STOP();
                            //yield return new WaitForSeconds(1f);
                        }
                        else
                        {
                            float distance = Vector3.Distance(nextPostion, gameObject.transform.position);
                            if (distance < m_NavMeshAgent.stoppingDistance)
                            {
                                Vector3 newPosition = Vector3.zero;
                                while (ServerSPathQueue.Count > 0)
                                {
                                    Tuple<int, Vector3> spath = ServerSPathQueue.Dequeue();
                                    if (spath.Item1 == SpathID)
                                    {
                                        // 방향성을 보고 방향이 
                                        //Send_MoveStartMessage(spath.Item2);
                                        MOVE_START(spath.Item2, enUNIT_STATUS.MOVE_SPATH);
                                        nextPostion = spath.Item2;
                                        newPosition = spath.Item2;
                                        beforePosition = gameObject.transform.position;
                                        break;
                                    }
                                }

                                if (newPosition == Vector3.zero)
                                {
                                    MOVE_STOP();
                                    //yield return new WaitForSeconds(1f);
                                }
                            }
                            else
                            {
                                // 이동 유지..
                                // 그러나 여기서도 제자리 걸음이 유지된다면?
                                if (Vector3.Distance(beforePosition, gameObject.transform.position) < 1f)
                                {
                                    // 1) 타겟 변경 시도
                                    GameObject otherTarget = m_AttackController.GetOtherTarget();
                                    if (otherTarget != null && otherTarget != m_AttackController.m_TargetObject)
                                    {
                                        // 타겟 변경...
                                        m_AttackController.m_TargetObject = otherTarget;
                                        MOVE_START(otherTarget.transform.position, enUNIT_STATUS.MOVE_TRACING);
                                       // yield return new WaitForSeconds(0.1f);
                                    }
                                    else
                                    {
                                        //// 타겟 유지, 경로 재계산
                                        //yield return new WaitForSeconds(1f);    // 1초 대기
                                        //Send_SyncPosMessage();
                                        //ServerSPathQueue.Clear();
                                        //ServerPathFindingReq = true;
                                        //ServerPathFinding = false;
                                        //ServerPathPending = true;
                                        //Send_PathFindingReqMessage(m_NavMeshAgent.destination, ++SpathID);
                                        //yield return new WaitForSeconds(1f);    // 1초 대기

                                        // => idle 상태 복귀
                                        MOVE_STOP();
                                        //yield return new WaitForSeconds(1f);
                                    }
                                }
                                else
                                {
                                    beforePosition = gameObject.transform.position;
                                   // yield return new WaitForSeconds(0.1f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 타겟 없음 -> 중지
                    MOVE_STOP();
                    //yield return new WaitForSeconds(1f);
                }
            }

            yield return null;
        }
    }

    /*****************************************************************************
     * Send Packet_new(proxy)
     *****************************************************************************/
    public void SEND_MOVE_START(Vector3 destionation)
    {
        Vector3 norm = (destionation - gameObject.transform.position).normalized;
        RPC.proxy.UNIT_S_MOVE((byte)enMOVE_TYPE.MOVE_START, transform.position.x, transform.position.z, norm.x, norm.z, destionation.x, destionation.z, UnitSession);
    }

    public void SEND_MOVE_STOP()
    {
        RPC.proxy.UNIT_S_MOVE((byte)enMOVE_TYPE.MOVE_STOP, transform.position.x, transform.position.z, transform.forward.x, transform.forward.z, transform.position.x, transform.position.z, UnitSession);
    }

    public void SEND_SYNC()
    {
        RPC.proxy.UNIT_S_SYNC_POSITION(transform.position.x, transform.position.z, transform.forward.x, transform.forward.z, UnitSession);
    }

    //UNIT_S_TRACE_PATH_FINDING_REQ(Int32 SPATH_ID, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float DEST_X, float DEST_Z,  NetworkManager sesion = null)
    //Vector3 destination, int spathID
    public void SEND_TRACE_PATH_FINDING_REQ(Vector3 destination, int spathID)
    {
        RPC.proxy.UNIT_S_TRACE_PATH_FINDING_REQ(spathID, transform.position.x, transform.position.z, transform.forward.x, transform.forward.z, destination.x, destination.z, UnitSession);
    }

    public void SEND_LAUNCH_ATTACK(Vector3 TargetPosition)
    {
        Vector3 dirToTarget = (TargetPosition - gameObject.transform.position).normalized;
        RPC.proxy.UNIT_S_LAUNCH_ATTACK(gameObject.transform.position.x, gameObject.transform.position.z, dirToTarget.x, dirToTarget.z, UnitSession);
    }

    public void SEND_STOP_ATTACK()
    {
        RPC.proxy.UNIT_S_STOP_ATTACK(UnitSession);
    }

    public void SEND_ATTACK(int targetID, byte attackType)
    {
        if(m_AttackController.m_TargetObject == null)
        {
            return;
        }

        Vector3 dirToTarget = (m_AttackController.m_TargetObject.transform.position - gameObject.transform.position).normalized;
        RPC.proxy.UNIT_S_ATTACK(transform.position.x, transform.position.z, dirToTarget.x, dirToTarget.z, targetID, attackType, UnitSession);
    }

    /*****************************************************************************
     * Send Packet_old
     *****************************************************************************/
    /*public void Send_MoveStartMessage(Vector3 destionation)
    {
        MSG_UNIT_S_MOVE moveMsg = new MSG_UNIT_S_MOVE();
        moveMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
        moveMsg.moveType = (byte)enUnitMoveType.Move_Start;
        moveMsg.posX = gameObject.transform.position.x;
        moveMsg.posZ = gameObject.transform.position.z; 
        Vector3 dirVec = (destionation - gameObject.transform.position).normalized;
        moveMsg.normX = dirVec.x;
        moveMsg.normZ = dirVec.z;
        moveMsg.destX = destionation.x;
        moveMsg.destZ = destionation.z;

        Debug.Log("Send_MoveStartMessage");
        m_UnitSession.SendPacket<MSG_UNIT_S_MOVE>(moveMsg);
    }

    public void Send_MoveStopMessage()
    {
        MSG_UNIT_S_MOVE stopMsg = new MSG_UNIT_S_MOVE();
        stopMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
        stopMsg.moveType = (byte)enUnitMoveType.Move_Stop;
        stopMsg.posX = gameObject.transform.position.x;
        stopMsg.posZ = gameObject.transform.position.z;
        stopMsg.normX = gameObject.transform.forward.normalized.x;
        stopMsg.normZ = gameObject.transform.forward.normalized.z;

        Debug.Log("Send_MoveStopMessage");
        m_UnitSession.SendPacket<MSG_UNIT_S_MOVE>(stopMsg);
    }

    public void Send_MoveDirChangeMessage(Vector3 normVec)
    {
        MSG_UNIT_S_MOVE dirCngMsg = new MSG_UNIT_S_MOVE();
        dirCngMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
        dirCngMsg.moveType = (byte)enUnitMoveType.Move_Change_Dir;
        dirCngMsg.posX = gameObject.transform.position.x;
        dirCngMsg.posZ = gameObject.transform.position.z;
        dirCngMsg.normX = normVec.x;
        dirCngMsg.normZ = normVec.z;

        m_UnitSession.SendPacket<MSG_UNIT_S_MOVE>(dirCngMsg);
    }

    public void Send_SyncPosMessage()
    {
        MSG_UNIT_S_SYNC_POSITION syncMsg = new MSG_UNIT_S_SYNC_POSITION();
        syncMsg.type = (ushort)enPacketType.UNIT_S_SYNC_POSITION;
        syncMsg.posX = gameObject.transform.position.x;
        syncMsg.posZ = gameObject.transform.position.z;
        syncMsg.normX = gameObject.transform.forward.normalized.x;
        syncMsg.normZ = gameObject.transform.forward.normalized.z;

        Debug.Log("Send_SyncPosMessage");
        m_UnitSession.SendPacket<MSG_UNIT_S_SYNC_POSITION>(syncMsg);
    }
    public void Send_SyncDirectionMessage()
    {
        MSG_UNIT_S_SYNC_DIRECTION dirMsg = new MSG_UNIT_S_SYNC_DIRECTION();
        dirMsg.type = (ushort)enPacketType.UNIT_S_SYNC_DIRECTION;
        dirMsg.normX = gameObject.transform.forward.normalized.x;
        dirMsg.normZ = gameObject.transform.forward.normalized.z;

        //Debug.Log("Send_DirChangeMessage");
        m_UnitSession.SendPacket<MSG_UNIT_S_SYNC_DIRECTION>(dirMsg);
    }

    public void Send_PathFindingReqMessage(Vector3 destination, int spathID)
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

        Debug.Log("Send_PathFindingReqMessage, spathID: " + spathID);
        m_UnitSession.SendPacket<MSG_UNIT_S_REQ_TRACE_PATH_FINDING>(pathFindingReqMsg);  
    }

    public void SendAttackLaunch()
    {

    }

    public void Send_AttackMessage(GameObject targetObject)
    {
        MSG_UNIT_S_ATTACK atkMsg = new MSG_UNIT_S_ATTACK();
        atkMsg.type = (ushort)enPacketType.UNIT_S_ATTACK;
        atkMsg.posX = gameObject.transform.position.x;
        atkMsg.posZ = gameObject.transform.position.z;
        Vector3 dirVec = (targetObject.transform.position - gameObject.transform.position).normalized;
        atkMsg.normX = dirVec.x;
        atkMsg.normZ = dirVec.z;
        atkMsg.targetID = targetObject.GetComponent<Enemy>().ID;
        atkMsg.attackType = (int)enUnitAttackType.ATTACK_NORMAL;

        //Debug.Log("Send_AttackMessage");
        m_UnitSession.SendPacket<MSG_UNIT_S_ATTACK>(atkMsg);
    }

    public void Send_AttackStopMessage()
    {
        MSG_UNIT_S_ATTACK_STOP atkStopMsg = new MSG_UNIT_S_ATTACK_STOP();
        atkStopMsg.type = (ushort)enPacketType.UNIT_S_ATTACK_STOP;
        atkStopMsg.posX = gameObject.transform.position.x;  
        atkStopMsg.posZ= gameObject.transform.position.z;   
        atkStopMsg.normX = gameObject.transform.forward.x;  
        atkStopMsg.normZ = gameObject.transform.forward.z;

        //Debug.Log("Send_AttackStopMessage");
        m_UnitSession.SendPacket<MSG_UNIT_S_ATTACK_STOP>(atkStopMsg);
    }*/
}
