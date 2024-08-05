using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class BattleField : MonoBehaviour
{
    static string TeamTagStr = "TEAM_UNIT";

    private static Vector3 SpawnPointA = new Vector3(150f, 0, 150f);
    private static Vector3 SpawnPointB = new Vector3(250f, 0, 150f);
    private static Vector3 SpawnPointC = new Vector3(250f, 0, 250f);
    private static Vector3 SpawnPointD = new Vector3(150f, 0, 250f);

    Dictionary<int, Unit> m_Units = new Dictionary<int, Unit>();
    //Dictionary<int, NetworkManager> m_Sessions = new Dictionary<int, NetworkManager>();

    static bool InitPosFlag = true;
    static Vector3 InitPos = Vector3.zero;
    static float PosAngle;
    static float PosRadius;

    public static void InitCreatePostion(){
        InitPosFlag = true;
        if(Manager.GamePlayer.m_Team == (int)enPlayerTeamInBattleField.Team_A)
        {
            InitPos = SpawnPointA;
        }
        else if (Manager.GamePlayer.m_Team == (int)enPlayerTeamInBattleField.Team_B)
        {
            InitPos = SpawnPointB;
        }
        else if (Manager.GamePlayer.m_Team == (int)enPlayerTeamInBattleField.Team_C)
        {
            InitPos = SpawnPointC;
        }
        else if (Manager.GamePlayer.m_Team == (int)enPlayerTeamInBattleField.Team_D)
        {
            InitPos = SpawnPointD;
        }
        else
        {
            InitPos = Vector3.zero;
        }
    }
    public static Vector3 GetRandomCreatePosition(enUnitType unitType)
    {
        Vector3 position = Vector3.zero;

        if(InitPosFlag) 
        {
            InitPosFlag = false;
            PosAngle = 0f;
            PosRadius = GetUnitRadius(unitType);
            position = InitPos;
        }
        else{
            position = InitPos + new Vector3(
                Mathf.Cos(PosAngle) * (PosRadius + GetUnitRadius(unitType)), 
                0.0f, 
                Mathf.Sin(PosAngle) * (PosRadius + GetUnitRadius(unitType))
            );

            PosAngle += 45f;
            PosRadius += GetUnitRadius(unitType);   
        }

        return position;
    }
    public static float GetUnitRadius(enUnitType unitType) 
    {
        //Terran_Marine,
	    //Terran_Firebat,
	    //Terran_Tank,
	    //Terran_Robocop,
	    //Zerg_Zergling,
	    //Zerg_Hydra,
	    //Zerg_Golem,
	    //Zerg_Tarantula,
        float radius = 0f; 
        if (unitType == enUnitType.Terran_Marine)
        {
            radius = 1.5f;
        }
        else if (unitType == enUnitType.Terran_Firebat)
        {
            radius = 1.5f;
        }
        else if (unitType == enUnitType.Terran_Tank)
        {
            //
        }
        else if (unitType == enUnitType.Terran_Robocop)
        {
            //
        }
        else if (unitType == enUnitType.Zerg_Zergling)
        {
            radius = 4f;
        }
        else if (unitType == enUnitType.Zerg_Hydra)
        {
            radius = 4.5f;
        }
        else if (unitType == enUnitType.Zerg_Golem)
        {
            radius = 7.5f;
        }
        else if (unitType == enUnitType.Zerg_Tarantula)
        {
            radius = 10f;
        }

        return radius;
    }

    private void Start()
    {
        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_MOVE_SELECT_FIELD_TO_BATTLE_FIELD);
        if (!Manager.Network.SendPacket<MSG_COM_REQUEST>(req))
        {
            Debug.Log("REQ_SELECT_FIELD_TO_BATTLE_FIELD �۽� ����");
        }

        foreach (var crt in Manager.GamePlayer.CrtMessageList)
        {
            NetworkManager session = crt.Item1;
            MSG_UNIT_S_CREATE_UNIT msg = crt.Item2;
            if (!session.SendPacket<MSG_UNIT_S_CREATE_UNIT>(msg))
            {
                Debug.Log("���� ���� �޽��� �۽� ����");
            }
        }
        Manager.GamePlayer.CrtMessageList.Clear();
    }

    private void OnDestroy()
    {
        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_MOVE_BATTLE_FIELD_TO_SELECT_FIELD);
        if (!Manager.Network.SendPacket<MSG_COM_REQUEST>(req))
        {
            Debug.Log("Send REQ_MOVE_BATTLE_FIELD_TO_SELECT_FIELD fail.....");
        }
        else
        {
            // REQ_MOVE_BATTLE_FIELD_TO_SELECT_FIELD 에 대한 응답을 받은 후 Battle 씬 파괴
            while (true)
            {
                if (Manager.Network.ReceiveDataAvailable())
                {
                    byte[] payload = Manager.Network.ReceivePacket();
                    if (payload == null)
                    {
                        Debug.Log("ReceiveDataAvailable, but ReceivePacket() => null");
                        return;
                    }
                    else
                    {
                        enPacketType packetType = Manager.Network.GetMsgTypeInBytes(payload);
                        if (packetType == enPacketType.COM_REPLY)
                        {
                            MSG_COM_REPLY msg = Manager.Network.BytesToMessage<MSG_COM_REPLY>(payload);
                            if (msg.replyCode == (int)enProtocolComReply.REPLY_MOVE_BATTLE_FIELD_TO_SELECT_FIELD)
                            {
                                break;
                            }
                        }
                    }

                    Debug.Log("REPLY_MOVE_BATTLE_FIELD_TO_SELECT_FIELD 메시지 수신 대기................");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            foreach(var obj in m_Units)
            {
                Unit unit = obj.Value;
                if (unit.m_team == Manager.GamePlayer.m_Team)
                {
                    UnitController unitController = unit.m_GameObject.GetComponent<UnitController>();

                    MSG_MGR_UNIT_DIE_REQUEST msg = new MSG_MGR_UNIT_DIE_REQUEST();
                    msg.type = (ushort)enPacketType.MGR_UNIT_DIE_REQUEST;
                    msg.unitID = unit.m_id;

                    Manager.UnitSelection.SelectableUnitDestroyed(unit.m_GameObject);

                    unitController.UnitSession.SendPacket<MSG_MGR_UNIT_DIE_REQUEST>(msg);
                }
            }
            Manager.GamePlayer.MyTeamUnitCnt = 0;
            Manager.SceneTransfer.TransferToSelectField();
        }

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

                switch (packetType)
                {
                    case enPacketType.S_MGR_CREATE_UNIT:
                        {
                            MSG_S_MGR_CREATE_UNIT msg = Manager.Network.BytesToMessage<MSG_S_MGR_CREATE_UNIT>(payload);
                            Proc_CREATE_UNIT(msg);
                        }
                        break;
                    case enPacketType.S_MGR_MOVE:
                        {
                            MSG_S_MGR_MOVE msg = Manager.Network.BytesToMessage<MSG_S_MGR_MOVE>(payload);
                            Proc_MOVE(msg);
                        }
                        break;
                    case enPacketType.S_MGR_ATTACK:
                        {
                            MSG_S_MGR_ATTACK msg = Manager.Network.BytesToMessage<MSG_S_MGR_ATTACK>(payload);
                            Proc_ATTACK(msg);
                        }
                        break;
                    case enPacketType.S_MGR_ATTACK_INVALID:
                        {
                            MSG_S_MGR_ATTACK_INVALID msg = Manager.Network.BytesToMessage<MSG_S_MGR_ATTACK_INVALID>(payload);
                            Proc_ATTACK_INVALID(msg);
                        }
                        break;
                    case enPacketType.S_MGR_ATTACK_STOP:
                        {
                            MSG_S_MGR_ATTACK_STOP msg = Manager.Network.BytesToMessage<MSG_S_MGR_ATTACK_STOP>(payload); 
                            Proc_ATTACK_STOP(msg);
                        }
                        break;
                    case enPacketType.S_MGR_UINT_DAMAGED:
                        {
                            MSG_S_MGR_UINT_DAMAGED msg = Manager.Network.BytesToMessage<MSG_S_MGR_UINT_DAMAGED>(payload);
                            Proc_UINT_DAMAGED(msg);
                        }
                        break;
                    case enPacketType.S_MGR_UNIT_DIED:
                        {
                            MSG_S_MGR_UNIT_DIED msg = Manager.Network.BytesToMessage<MSG_S_MGR_UNIT_DIED>(payload);
                            Proc_UNIT_DIED(msg);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        Update_UI_Unit(Time.deltaTime);
    }

    private void Update_UI_Unit(float deltaTime)
    {
        foreach(var unit in m_Units)
        {
            unit.Value.Update_UI(deltaTime);
        }
    }

    private void Proc_CREATE_UNIT(MSG_S_MGR_CREATE_UNIT msg)
    {
        Unit newUnit = CreateUnitObjectInScene(msg);

        newUnit.m_GameObject.GetComponent<NavMeshAgent>().avoidancePriority = 0;

        if(Manager.GamePlayer.m_Team == msg.team)
        {
            UnitMovement unitMovement = newUnit.m_GameObject.AddComponent<UnitMovement>();
            newUnit.m_GameObject.GetComponent<UnitMovement>().enabled = false;

            newUnit.m_GameObject.AddComponent<UnitController>();
            newUnit.m_GameObject.GetComponent<UnitController>().m_Unit = newUnit;
            newUnit.m_GameObject.GetComponent<UnitController>().UnitSession = Manager.GamePlayer.NewUnitSessions[msg.crtCode];
            Manager.GamePlayer.NewUnitSessions.Remove(msg.crtCode); 
           
            newUnit.m_GameObject.AddComponent<AttackController>();
            newUnit.m_GameObject.GetComponent<AttackController>().m_AttackDistance = newUnit.m_AttackDistance;
            newUnit.m_GameObject.GetComponent<AttackController>().m_StopAttackDistance = newUnit.m_AttackDistance + 1f;
            newUnit.m_GameObject.GetComponent<AttackController>().m_AttackRate = newUnit.m_AttackRate;
            newUnit.m_GameObject.GetComponent<AttackController>().m_AttackDelay = msg.attackDelay;

            newUnit.m_GameObject.tag = Manager.GamePlayer.TeamTagStr;
        }
        else
        {
            newUnit.m_GameObject.AddComponent<Enemy>();
            newUnit.m_GameObject.GetComponent<Enemy>().m_Unit = newUnit;
            newUnit.m_GameObject.tag = Manager.GamePlayer.EnemyTagStr;
            newUnit.m_GameObject.AddComponent<Rigidbody>();
        }

        if(newUnit.m_team == (int)enPlayerTeamInBattleField.Team_Test)
        {
            newUnit.m_GameObject.GetComponent<SphereCollider>().enabled = false;
        }

        newUnit.m_GameObject.GetComponent<MuzzleEffect>().MuzzleRate = newUnit.m_AttackRate;

        newUnit.m_GameObject.SetActive(true);

        newUnit.m_UIObject = CreateUIUnit(newUnit);
        newUnit.m_UIObject.SetActive(true);

        //newUnit.m_GameObject.GetComponent

        m_Units.Add(newUnit.m_id, newUnit);
    }


    private void Proc_MOVE(MSG_S_MGR_MOVE msg)
    {
        if(!m_Units.ContainsKey(msg.unitID)) {
            return;
        }

        Unit unit = m_Units[msg.unitID];
        
        if (msg.moveType == (byte)enUnitMoveType.Move_Start)
        {
            unit.Move_Start(new Vector3(msg.posX, 0, msg.posZ), new Vector3(msg.destX, 0, msg.destZ), msg.speed);
        }
        else if (msg.moveType == (byte)enUnitMoveType.Move_Stop)
        {
            unit.Move_Stop(new Vector3(msg.posX, 0, msg.posZ));
        }
        

        if (msg.moveType == (byte)enUnitMoveType.Move_Start)
        {
            m_Units[msg.unitID].Move_Start_UI(new Vector3(msg.posX, 0, msg.posZ), msg.speed, msg.normX, msg.normZ);
        }
        else
        {
            m_Units[msg.unitID].Move_Stop_UI(new Vector3(msg.posX, 0, msg.posZ));
        }
    }

    private void Proc_ATTACK(MSG_S_MGR_ATTACK msg)
    {
        if(!m_Units.ContainsKey(msg.unitID)) {
            return;
        }
        
        Unit unit = m_Units[msg.unitID];

        unit.Attack(new Vector3(msg.posX, 0, msg.posZ), new Vector3(msg.normX, 0, msg.normZ), msg.attackType);

        unit.Move_Stop_UI(new Vector3(msg.posX, 0, msg.posZ));
    }

    private void Proc_ATTACK_INVALID(MSG_S_MGR_ATTACK_INVALID msg)
    {
        if (!m_Units.ContainsKey(msg.unitID))
        {
            return;
        }

        Unit unit = m_Units[msg.unitID];

        unit.Attack_Invalid(new Vector3(msg.posX, 0, msg.posZ), new Vector3(msg.normX, 0, msg.normZ));

        unit.Move_Stop_UI(new Vector3(msg.posX, 0, msg.posZ));
    }

    private void Proc_ATTACK_STOP(MSG_S_MGR_ATTACK_STOP msg)
    {
        if(!m_Units.ContainsKey(msg.unitID)) {
            return;
        }

        Unit unit = m_Units[msg.unitID];
        unit.AttackStop(new Vector3(msg.posX, 0, msg.posZ));
    }

    private void Proc_UINT_DAMAGED(MSG_S_MGR_UINT_DAMAGED msg)
    {
        if(!m_Units.ContainsKey(msg.unitID)) {
            return;
        }
        Unit unit = m_Units[msg.unitID];

        unit.RenewHP(msg.renewHP);
    }

    private void Proc_UNIT_DIED(MSG_S_MGR_UNIT_DIED msg)
    {
        if(!m_Units.ContainsKey(msg.unitID)) {
            return;
        }

        Unit unit = m_Units[msg.unitID];
        unit.Die();

        // 선택된 유닛이라면 제거
        Manager.UnitSelection.SelectableUnitDestroyed(unit.m_GameObject);

        int team = unit.m_team;
        GameObject.Destroy(unit.m_GameObject);
        m_Units.Remove(unit.m_id);
        
        if(team == Manager.GamePlayer.m_Team) 
        {
            Manager.GamePlayer.MyTeamUnitCnt -= 1;
            if(Manager.GamePlayer.MyTeamUnitCnt == 0) 
            {
                Manager.SceneTransfer.TransferToSelectField();
            }    
        }
    }

    private GameObject CreateUIUnit(Unit unit)
    {
        GameObject gameObj = null;
        string prefabName = string.Empty;
        if (unit != null)
        {
            switch (unit.m_team)
            {
                case (int)enPlayerTeamInBattleField.Team_A:
                    prefabName = "UIUnit_A";
                    break;
                case (int)enPlayerTeamInBattleField.Team_B:
                    prefabName = "UIUnit_B";
                    break;
                case (int)enPlayerTeamInBattleField.Team_C:
                    prefabName = "UIUnit_C";
                    break;
                case (int)enPlayerTeamInBattleField.Team_D:
                    prefabName = "UIUnit_D";
                    break;
                case (int)enPlayerTeamInBattleField.Team_Test:
                    prefabName = "UIUnit_D";
                    break;
            }

            if (prefabName != string.Empty)
            {
                GameObject prefab = Resources.Load<GameObject>("prefab/" + prefabName);
                //Vector3 position = new Vector3(unit.m_GameObject.transform.position.x - 100f, unit.m_GameObject.transform.position.z - 100f, 0);
                gameObj = Instantiate(prefab);
                gameObj.GetComponent<RectTransform>().SetParent(GameObject.Find("MiniMap").transform, false);
                gameObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(unit.m_GameObject.transform.position.x - 100f, unit.m_GameObject.transform.position.z - 100f, 0);
            }
        }

        return gameObj; 
    }

    private Unit CreateUnitObjectInScene(MSG_S_MGR_CREATE_UNIT crtMsg)
    {
        GameObject gameObj = null;
        string prefabName = string.Empty;

        enUnitType unitType = (enUnitType)crtMsg.unitType;
        switch (unitType)
        {
            case enUnitType.Terran_Marine:
                {
                    prefabName = "Marine";
                }
                break;
            case enUnitType.Terran_Firebat:
                {
                    prefabName = "Firebat";
                }
                break;
            case enUnitType.Terran_Tank:
                {
                    prefabName = "Tank";
                }
                break;
            case enUnitType.Terran_Robocop:
                {
                    prefabName = "Robocop";
                }
                break;
            case enUnitType.Zerg_Zergling:
                {
                    prefabName = "Zergling";
                }
                break;
            case enUnitType.Zerg_Hydra:
                {
                    prefabName = "Hydra";
                }
                break;
            case enUnitType.Zerg_Golem:
                {
                    prefabName = "Golem";
                }
                break;
            case enUnitType.Zerg_Tarantula:
                {
                    prefabName = "Tarantula";
                }
                break;
            default:
                break;
        }

        Vector3 position = new Vector3(crtMsg.posX, 0, crtMsg.posZ);
        Vector3 dir = new Vector3(crtMsg.normX, 0, crtMsg.normZ);

        if (prefabName != string.Empty)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/" + prefabName);
            gameObj = Instantiate(prefab, position, Quaternion.LookRotation(dir));
        }

        if (gameObj == null)
        {
            return null;
        }

        Unit newUnit = new Unit(gameObj, crtMsg.unitID, crtMsg.unitType, crtMsg.team, position, dir, crtMsg.speed, crtMsg.maxHP, crtMsg.attackDistance, crtMsg.attackRate);
        return newUnit;
    }
}
