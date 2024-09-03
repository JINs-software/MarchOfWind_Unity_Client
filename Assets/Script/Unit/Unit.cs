using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public int m_id;
    public byte m_type;
    public byte m_team;
    public float m_speed;
    public int m_initHP;
    public int m_maxHP;
    public float m_radius;
    public float m_AttackDistance;
    public float m_AttackRate;

    Vector3 m_InitPostion;
    Vector3 m_InitDirection;

    Animator m_Animator;
    NavMeshAgent m_NavMeshAgent;
    HealthController m_HearthController;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();  
        if(m_Animator == null)
        {
            m_Animator = gameObject.AddComponent<Animator>();   
        }
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        if(m_NavMeshAgent == null)
        {
            m_NavMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }
        m_HearthController = GetComponent<HealthController>();
        if(m_HearthController == null)
        {
            m_HearthController = gameObject.AddComponent<HealthController>();       
        }
    }

    private void Start()
    {
        m_NavMeshAgent.Warp(m_InitPostion);
        m_NavMeshAgent.avoidancePriority = 0;
        gameObject.transform.rotation = Quaternion.LookRotation(m_InitDirection.normalized);
        m_NavMeshAgent.speed = m_speed;
        m_HearthController.InitHealth(m_initHP, m_maxHP);
    }

    public void Init(int id, byte type, byte team, Vector3 position, Vector3 direction, float speed, int nowHP, int maxHP, float radius, float attackDist, float attackRate)
    {
        m_id = id;
        m_type = type;
        m_team = team;
        m_speed = speed;
        m_initHP = nowHP;   
        m_maxHP = maxHP;    
        m_radius = radius;  
        m_AttackDistance = attackDist;
        m_AttackRate = attackRate;
        m_InitPostion = position;
        m_InitDirection = direction;

        if(gameObject.GetComponent<MuzzleEffect>() == null)
        {
            gameObject.AddComponent<MuzzleEffect>().MuzzleRate = m_AttackRate;
        }
    }


    public void Move_Warp(Vector3 position)
    {
        m_NavMeshAgent.Warp(position);
    }

    public void Move_Start(Vector3 position, Vector3 destPosition, float Speed)
    {
        //Debug.Log("Recv Move_Start---------------------");

        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.avoidancePriority = 99;
        //m_NavMeshAgent.avoidancePriority = Random.Range(20, 99);
        if (!m_NavMeshAgent.SetDestination(destPosition))
        {
            //Debug.Log("Move_Start, SetDestination returns Fail..");
        }

        // 애니메이터 변경
        m_Animator.ResetTrigger("trIdle");
        m_Animator.ResetTrigger("trAttack");
        m_Animator.SetTrigger("trMove");        
    }
    public void Move_Stop(Vector3 position)
    {
        //Debug.Log("Recv Move_Stop---------------------");

        if (!m_NavMeshAgent.Warp(position))
        {
            //Debug.Log("Move_Stop, Warp returns Fail..");
        }
        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.avoidancePriority = 0;
        
        m_Animator.ResetTrigger("trMove");
        m_Animator.ResetTrigger("trAttack");
        m_Animator.SetTrigger("trIdle");
    }

    //public void DIR_CHANGE(Vector3 norm)
    //{
    //    gameObject.transform.rotation = Quaternion.LookRotation(norm);
    //}

    public void LauchAttack()
    {
        // 공격 시작 시 정지!
        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.avoidancePriority = 10;

        m_Animator.ResetTrigger("trIdle");
        m_Animator.ResetTrigger("trMove");
        m_Animator.SetTrigger("trAttack");
    }

    public void StopAttack()
    {
        //Debug.Log("Recv Atack Stop---------------------");
        m_Animator.ResetTrigger("trMove");
        m_Animator.ResetTrigger("trAttack");
        m_Animator.SetTrigger("trIdle");
        m_NavMeshAgent.avoidancePriority = 0;       // 우선순위 변경?
    }

    public void Attack(Vector3 position, Vector3 dir, int attkType)
    {
        //Debug.Log("Recv Atack---------------------");

        gameObject.transform.forward = dir;
        // => 급격한 방향 전환이 어색함, Quaternion.Slerp 함수 사용

        //Quaternion targetRotation = Quaternion.LookRotation(dir);
        //gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * m_RotationSpeed);
        // => AttackState에서 차차 돌려줘야 할듯


        if (!m_NavMeshAgent.Warp(position))
        {
            //Debug.Log("Move_Stop, Warp returns Fail..");
        }
        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.avoidancePriority = 10;

        m_Animator.ResetTrigger("trIdle");
        m_Animator.ResetTrigger("trMove");
        m_Animator.SetTrigger("trAttack");
    }

    //public void Attack_Invalid(Vector3 position, Vector3 dir)
    //{
    //    //Debug.Log("Recv Atack Invalid---------------------");
    //
    //    gameObject.transform.forward = dir;
    //
    //    if (!m_NavMeshAgent.Warp(position))
    //    {
    //        //Debug.Log("Move_Stop, Warp returns Fail..");
    //    }
    //    m_NavMeshAgent.isStopped = true;
    //    m_NavMeshAgent.avoidancePriority = 10;
    //
    //    m_Animator.ResetTrigger("trIdle");
    //    m_Animator.ResetTrigger("trMove");
    //    m_Animator.SetTrigger("trAttack");
    //    //m_NavMeshAgent.avoidancePriority = 0;       // 우선순위 변경?
    //
    //    if (gameObject.GetComponent<UnitController>() != null)
    //    {
    //        gameObject.GetComponent<UnitController>().OnMoving = false;
    //    }
    //}


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
        gameObject.GetComponent<HealthController>().UpdateHealth(renewHP);
    }

    public void Die()
    {
        m_Animator.SetTrigger("trDie");
        //m_UIObject.SetActive(false);
        Destroy(gameObject); 
    }

    //public void Move_Start_UI(Vector3 position, float Speed, float normX, float normZ)
    //{
    //    Vector2 uiPosition = new Vector3(position.x - 100f, position.z - 100f);
    //    m_UIObject.GetComponent<RectTransform>().anchoredPosition = uiPosition;
    //
    //    m_UiNorm.x = normX;
    //    m_UiNorm.y = normZ;
    //    m_UiNorm = m_UiNorm.normalized;
    //
    //    isMoving = true;
    //}
    //public void Move_Stop_UI(Vector3 position)
    //{
    //    isMoving = false;
    //
    //    Vector2 uiPosition = new Vector2(position.x - 100f, position.z - 100f);
    //    m_UIObject.GetComponent<RectTransform>().anchoredPosition = uiPosition;
    //}
    //public void Update_UI(float deltaTime)
    //{
    //    if (isMoving)
    //    {
    //        Vector2 move = m_UiNorm * m_speed * deltaTime;
    //        m_UIObject.GetComponent<RectTransform>().anchoredPosition += move;
    //    }
    //}

    //public void RecvJpsReqReply()
    //{
    //    m_NavMeshAgent.isStopped = true;
    //    m_Animator.ResetTrigger("trIdle");
    //    m_Animator.ResetTrigger("trAttack");
    //    m_Animator.SetTrigger("trMove");
    //}

    /*public void RecvSPath(int spathID, byte spathOpt, Vector3 position)
    {
        UnitController unitController = gameObject.GetComponent<UnitController>();
        if (unitController != null && unitController.ServerPathFindingReq)
        {
            if (spathID == unitController.SpathID)
            {
                enSPathStateType spathType = (enSPathStateType)spathOpt;
                if (spathType != enSPathStateType.END_OF_PATH)
                {
                    Tuple<int, Vector3> spath = new Tuple<int, Vector3>(spathID, position);
                    unitController.ServerSPathQueue.Enqueue(spath);

                    GameObject prefab = Resources.Load<GameObject>("prefab/SPath");
                    Instantiate(prefab, position, Quaternion.identity).SetActive(true);
                }
                else
                {
                    unitController.ServerPathPending = false;
                }
            }
        }
    }*/
}