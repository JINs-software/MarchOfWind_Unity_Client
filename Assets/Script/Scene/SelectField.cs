using UnityEngine;

public class SelectField : MonoBehaviour
{
    GameObject m_InitPoint;
    //float m_InitPointRadius;
    //public float m_SpawnHeight;
    public string prefabPath = "Prefab/";

   
    // Start is called before the first frame update
    void Start()
    {
        m_InitPoint = GameObject.Find("InitPoint");

        Manager.UnitSelection.Init();

        Manager.Network.ClearRecvBuffer();  

        // ������ ���� ��û
        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_NUM_OF_SELECTORS);
        
        if (!Manager.Network.SendPacket<MSG_COM_REQUEST>(req))
        {
            Debug.Log("���� ���� �ʵ�,  REQ_NUM_OF_SELECTORS ��û �۽� ����");
        }

        BattleField.InitCreatePostion();
    }

    private void Update()
    {
        if (Manager.Network.ReceiveDataAvailable())
        {
            byte[] payload = Manager.Network.ReceivePacket();
            if (payload == null)
            {
                Debug.Log("��ġ�� �޽��� ���� ����");
                return;
            }
            else
            {
                enPacketType packetType = Manager.Network.GetMsgTypeInBytes(payload);
                if (packetType == enPacketType.REPLY_NUM_OF_SELECTORS)
                {
                    MSG_REPLY_NUM_OF_SELECTORS msg = Manager.Network.BytesToMessage<MSG_REPLY_NUM_OF_SELECTORS>(payload);
                    Proc_Reply_NumOfSelector(msg);
                }
            }
        }   
    }

    void Proc_Reply_NumOfSelector(MSG_REPLY_NUM_OF_SELECTORS msg)
    {
        // �����÷��� �Ŵ����κ��� ���� �÷��̾��� ������ �޾� �����͸��� ��ȯ
        int team = Manager.GamePlayer.m_Team;
         Manager.GamePlayer.m_SelectorCnt = msg.numOfSelector;

        if (team == (int)enPlayerTeamInBattleField.Team_A)
        {
            GameObject selectorPrefab = Resources.Load<GameObject>(prefabPath + "SelectMan_A");
            CreateSelectorMan(selectorPrefab, msg.numOfSelector);
        }
        else if (team == (int)enPlayerTeamInBattleField.Team_B)
        {
            GameObject selectorPrefab = Resources.Load<GameObject>(prefabPath + "SelectMan_B");
            CreateSelectorMan(selectorPrefab, msg.numOfSelector);
        }
        else if (team == (int)enPlayerTeamInBattleField.Team_C)
        {
            GameObject selectorPrefab = Resources.Load<GameObject>(prefabPath + "SelectMan_C");
            CreateSelectorMan(selectorPrefab, msg.numOfSelector);
        }
        else if (team == (int)enPlayerTeamInBattleField.Team_D)
        {
            GameObject selectorPrefab = Resources.Load<GameObject>(prefabPath + "SelectMan_D");
            CreateSelectorMan(selectorPrefab, msg.numOfSelector);
        }
    }

    void CreateSelectorMan(GameObject selectorPrefab, int cnt)
    {
        int loopCnt = 0;
        for(int i=0; i<cnt; i++)
        {
            Vector3 spawnPos = GetRandomPositionOnCylinder(m_InitPoint);
            Instantiate(selectorPrefab, spawnPos, Quaternion.identity).SetActive(true); 
        }
    }

    Vector3 GetRandomPositionOnCylinder(GameObject spawnPoint)
    {
        Vector2 randomPos = Random.insideUnitCircle * m_InitPoint.transform.localScale.x / 2f;
        Vector3 spawnPosition = new Vector3(randomPos.x, 0, randomPos.y) + m_InitPoint.transform.position;
        return spawnPosition;
    }

}
