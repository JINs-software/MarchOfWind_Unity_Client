using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
 * UnitSectionManager�� ������ �ڵ����� ��ϵǵ��� �Ѵ�.
 */

public class Unit
{
    public GameObject m_GameObject = null;

    public int m_id;
    public int m_type;
    public int m_team;
    public float m_speed;

    public GameObject m_UIObject = null;
    public bool isMoving = false;
    Vector2 m_UiNorm = Vector2.zero;

    //public int m_MaxHP;
    //public int m_HP;

    public float m_AttackDistance;
    public float m_AttackRate;

    public float m_RotationSpeed = 5f;      // 임시 
    
    Animator m_Animator;
    NavMeshAgent m_NavMeshAgent;
    //HealthTracker m_HearthTracker;
    HealthController m_HearthController;

    public Vector3 Destination { get { return m_NavMeshAgent.destination; } }

    public Unit(GameObject gameObject, int id, int type, int team, Vector3 position, Vector3 direction, float speed, int maxHP, float attackDist, float attackRate)
    {
        m_GameObject = gameObject;  
        m_NavMeshAgent = m_GameObject.GetComponent<NavMeshAgent>();
        m_NavMeshAgent.speed = speed;
        //m_HearthTracker = m_GameObject.GetComponent<HealthTracker>();
        m_Animator = m_GameObject.GetComponent<Animator>();
        m_HearthController = m_GameObject.GetComponent<HealthController>();
        m_HearthController.InitHealth(maxHP);

        m_id = id;
        m_type = type;  
        m_team = team;  
        m_speed = speed;
        ///m_position = position;  
        //m_Dir = dir;    
        //m_Destination = Vector3.zero;
        
        //m_MaxHP = m_HP = maxHP;
        // => Health Controller에서 관리

        m_AttackDistance = attackDist;
        m_AttackRate = attackRate;  

        m_NavMeshAgent.speed = m_speed; 
        m_NavMeshAgent.Warp(position); 
        m_GameObject.transform.rotation = Quaternion.LookRotation(direction.normalized);
    }


    public void Move_Warp(Vector3 position)
    {
        m_NavMeshAgent.Warp(position);
    }
    public void Move_Start(Vector3 position, Vector3 destPosition, float Speed)
    {
        Debug.Log("Recv Move_Start---------------------");

        m_NavMeshAgent.isStopped = false; 
        if (!m_NavMeshAgent.SetDestination(destPosition))
        {
            Debug.Log("Move_Start, SetDestination returns Fail..");
        }

        //m_Animator.SetBool("bMove", true);
        //m_Animator.SetBool("bAttack", false);

        m_Animator.ResetTrigger("trIdle");
        m_Animator.ResetTrigger("trAttack");
        m_Animator.SetTrigger("trMove");
        m_NavMeshAgent.avoidancePriority = 99;       // 우선순위 변경?

        if(m_GameObject.GetComponent<UnitController>() != null) 
        {
            m_GameObject.GetComponent<UnitController>().OnMoving = true;
        }
    }
    public void Move_Stop(Vector3 position)
    {
        Debug.Log("Recv Move_Stop---------------------");  

        if (!m_NavMeshAgent.Warp(position))
        {
            Debug.Log("Move_Stop, Warp returns Fail..");
        }
        m_NavMeshAgent.isStopped = true;

        //m_Animator.SetBool("bMove", false);

        m_Animator.ResetTrigger("trMove");
        m_Animator.ResetTrigger("trAttack");
        m_Animator.SetTrigger("trIdle");
        m_NavMeshAgent.avoidancePriority = 0;       // 우선순위 변경?

        if (m_GameObject.GetComponent<UnitController>() != null) 
        {
            m_GameObject.GetComponent<UnitController>().OnMoving = false;
        }
    }
    public void Attack(Vector3 position, Vector3 dir, int attkType)
    {
        Debug.Log("Recv Atack---------------------");  

        m_GameObject.transform.forward = dir;
        // => 급격한 방향 전환이 어색함, Quaternion.Slerp 함수 사용
        
        //Quaternion targetRotation = Quaternion.LookRotation(dir);
        //m_GameObject.transform.rotation = Quaternion.Slerp(m_GameObject.transform.rotation, targetRotation, Time.deltaTime * m_RotationSpeed);
        // => AttackState에서 차차 돌려줘야 할듯
        

        if (!m_NavMeshAgent.Warp(position))
        {
            Debug.Log("Move_Stop, Warp returns Fail..");
        }
        m_NavMeshAgent.isStopped = true;

        //m_Animator.SetBool("bAttack", true);
        //m_Animator.SetBool("bMove", false);

        m_Animator.ResetTrigger("trIdle");
        m_Animator.ResetTrigger("trMove");
        m_Animator.SetTrigger("trAttack");
        m_NavMeshAgent.avoidancePriority = 50;       // 우선순위 변경?
        //m_NavMeshAgent.qual

        if (m_GameObject.GetComponent<UnitController>() != null) 
        {
            m_GameObject.GetComponent<UnitController>().OnMoving = false;
        }
    }
    public void Attack_Invalid(Vector3 position, Vector3 dir)
    {
        Debug.Log("Recv Atack Invalid---------------------");

        m_GameObject.transform.forward = dir;
        // => 급격한 방향 전환이 어색함, Quaternion.Slerp 함수 사용

        //Quaternion targetRotation = Quaternion.LookRotation(dir);
        //m_GameObject.transform.rotation = Quaternion.Slerp(m_GameObject.transform.rotation, targetRotation, Time.deltaTime * m_RotationSpeed);
        // => AttackState에서 차차 돌려줘야 할듯


        if (!m_NavMeshAgent.Warp(position))
        {
            Debug.Log("Move_Stop, Warp returns Fail..");
        }
        m_NavMeshAgent.isStopped = true;


        //m_Animator.SetBool("bAttack", true);
        //m_Animator.SetBool("bMove", false);

        m_Animator.ResetTrigger("trIdle");
        m_Animator.ResetTrigger("trMove");
        m_Animator.SetTrigger("trAttack");
        m_NavMeshAgent.avoidancePriority = 0;       // 우선순위 변경?

        if (m_GameObject.GetComponent<UnitController>() != null)
        {
            m_GameObject.GetComponent<UnitController>().OnMoving = false;
        }
    }


    public void AttackStop(Vector3 position)
    {
        Debug.Log("Recv Atack Stop---------------------");  

        if (!m_NavMeshAgent.Warp(position))
        {
            Debug.Log("Move_Stop, Warp returns Fail..");
        }

        //m_Animator.SetBool("bAttack", false);
        m_Animator.ResetTrigger("trMove");
        m_Animator.ResetTrigger("trAttack");
        m_Animator.SetTrigger("trIdle");
        m_NavMeshAgent.avoidancePriority = 0;       // 우선순위 변경?
    }

    //private void LookAtTarget(Vector3 dir)
    //{
    //    Vector3 direction = atkController.m_TargetObject.transform.position - navMeshAgent.transform.position;  
    //    navMeshAgent.transform.rotation = Quaternion.LookRotation(direction);
//
    //    var yRotation = navMeshAgent.transform.eulerAngles.y;
    //    navMeshAgent.transform.rotation = Quaternion.Euler(0, yRotation, 0);    
    //}

    public void RenewHP(int renewHP)
    {
        m_GameObject.GetComponent<HealthController>().UpdateHealth(renewHP);
    }

    public void Die() 
    {
        m_Animator.SetTrigger("trDie");
        m_UIObject.SetActive(false);    
    }

    // �������� �̵� ����ȭ �׽�Ʈ �� UI �̵�
    public void Move_Start_UI(Vector3 position, float Speed, float normX, float normZ)
    {
        Vector2 uiPosition = new Vector3(position.x - 100f, position.z - 100f);
        m_UIObject.GetComponent<RectTransform>().anchoredPosition = uiPosition;

        m_UiNorm.x = normX;
        m_UiNorm.y = normZ;
        m_UiNorm = m_UiNorm.normalized;

        isMoving = true;
    }
    public void Move_Stop_UI(Vector3 position)
    {
        isMoving = false;

        Vector2 uiPosition = new Vector2(position.x - 100f, position.z - 100f);
        m_UIObject.GetComponent<RectTransform>().anchoredPosition = uiPosition;
    }
    public void Update_UI(float deltaTime)
    {
        if(isMoving)
        {
            Vector2 move = m_UiNorm * m_speed * deltaTime;
            m_UIObject.GetComponent<RectTransform>().anchoredPosition += move;  
        }
    }
}

/*
 * --------------------------------------------------------------------------------------------------
 * --------------------------------------------------------------------------------------------------
 * --------------------------------------------------------------------------------------------------
 */

public enum enUnitState
{
    IDLE,
    MOVE,
    ATTACK,
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

    public int m_FOW = 10;         // field of view (sphere collider radidus �� ����)

    public bool OnMoving = false;
    //public bool OnAttacking = false;

    public enUnitState State = enUnitState.IDLE;

    private Coroutine MoveStateCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Manager.UnitSelection.UnitList.Add(gameObject);

        // �þ� ������ ���� Sphere collider radius ����
        //GetComponent<SphereCollider>().radius = m_FOW;

        m_AttackController = gameObject.GetComponent<AttackController>();   
        m_UnitMovement = gameObject.GetComponent<UnitMovement>();
        m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();   

        m_PosBefore = gameObject.transform.position;
        m_NormBefore = gameObject.transform.forward.normalized;
    }

    private void Update()
    {
        // �̵� ���� ������ �۽�
        if (OnMoving && CheckChangeDirection())
        {
            Debug.Log("CheckChangeDirection => SendMoveStart msg");
            //Send_MoveStartMessage(m_Unit.Destination);
        }
    }

    private bool CheckChangeDirection()
    {
        bool ret = false;

        float distance = Vector3.Distance(m_PosBefore, gameObject.transform.position);
        float angle = Vector3.Angle(m_NormBefore, gameObject.transform.forward);    

        if(distance > 1f && angle > 1f)  
        {
            ret = true;
        }

        m_PosBefore = gameObject.transform.position;
        m_NormBefore = gameObject.transform.forward.normalized;
        return ret;
    }


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

                            //Send_MoveStartMessage(m_AttackController.m_TargetObject.transform.position);
                            Vector3 direction = (m_AttackController.m_TargetObject.transform.position - gameObject.transform.position);
                            float diff = (m_AttackController.m_TargetObject.transform.position - gameObject.transform.position).magnitude - m_AttackController.m_AttackDistance;
                            Vector3 destination = gameObject.transform.position + direction.normalized * diff;
                            Send_MoveStartMessage(destination);
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

    /*
    private IEnumerator MoveStateCoroutineFunc()
    {
        while(true)
        {
            if(!m_NavMeshAgent.pathPending) // 경로를 계산 중이라면 정지 조건 확인을 안하도록 한다.
            {
                //if (m_NavMeshAgent.hasPath == false || m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance || (m_UnitMovement.isCommandedToMove && ShouldStop()))
                //{
                //    Send_MoveStopMessage();
                //    m_UnitMovement.isCommandedToMove = false;
                //    yield return new WaitForSeconds(0.01f);
                //}
                // => 도착 조건 변경
                if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
                {
                    if (!m_NavMeshAgent.hasPath || m_NavMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        Send_MoveStopMessage();
                        m_UnitMovement.isCommandedToMove = false;
                        yield return new WaitForSeconds(0.01f);
                    }
                }
                else if (m_UnitMovement.isCommandedToMove && ShouldStop())
                {
                    Send_MoveStopMessage();
                    m_UnitMovement.isCommandedToMove = false;
                    yield return new WaitForSeconds(0.01f);
                }
            }

            if(!m_UnitMovement.isCommandedToMove)
            {
                // 커맨드를 통한 이동이 아닌 경우 공격 대상 확인
                if (m_AttackController.m_TargetObject != null)
                {
                    Send_AttackMessage(m_AttackController.m_TargetObject);
                    yield return new WaitForSeconds(0.01f);
                }
            }

            yield return null;
        }
    }
    */

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
