using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class DebugManager
{
    public bool DebugMode = false;

    bool DummyToggleOn = false;
    bool EnemyToggleOn = false;

    private Camera m_Camera;
    private LayerMask LClickLayerMask;

    private int CrtCode = 0;

    public void Update()
    {
        if(m_Camera == null)
        {
            m_Camera = Camera.main;
        }
        if(LClickLayerMask.value == 0)
        {
            LClickLayerMask = LayerMask.GetMask("GroundLayer");
        }

        if (DummyToggleOn)
        {
            // 마우스 좌 클릭
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LClickLayerMask))
                {
                    CreateDummySample(hit.point, enUnitType.Terran_Marine);
                    //Debug.Log("DummyToggleOn & L Button Click!");
                }
            }
        }
        else if (EnemyToggleOn)
        {
            // 마우스 좌 클릭
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LClickLayerMask))
                {
                    CreateEnemySample(hit.point);
                    //Debug.Log("EnemyToggleOn & L Button Click!");
                }
            }
        }
    }

    public bool OnDummyToggle()
    {
        if (EnemyToggleOn)
        {
            return false;
        }

        DummyToggleOn = true;
        Manager.UnitSelection.DeSelectAll();    
        return true;
    }
    public bool OnEnemyToggle()
    {
        if (DummyToggleOn)
        {
            return false;
        }

        EnemyToggleOn = true;
        Manager.UnitSelection.DeSelectAll();
        return true;
    }

    public void OffDummyToggle()
    {
        DummyToggleOn = false;
    }
    public void OffEnemyToggle()
    {
        EnemyToggleOn = false;
    }

    private void CreateDummySample(Vector3 position, enUnitType unitType)
    {
        NetworkManager dummySession = new NetworkManager();
        dummySession.Connect(Manager.GamePlayer.GameServerIP);

        MSG_UNIT_S_CONN_BATTLE_FIELD connMsg = new MSG_UNIT_S_CONN_BATTLE_FIELD();
        connMsg.type = (ushort)enPacketType.UNIT_S_CONN_BATTLE_FIELD;
        connMsg.fieldID = Manager.GamePlayer.BattleFieldID;
        if (!dummySession.SendPacket<MSG_UNIT_S_CONN_BATTLE_FIELD>(connMsg))
        {
            //Debug.Log("MSG_UNIT_S_CONN_BATTLE_FIELD 메시지 송신 실패");
            return;
        }

        MSG_UNIT_S_CREATE_UNIT crtMsg = new MSG_UNIT_S_CREATE_UNIT();
        int crtCode = Manager.GamePlayer.MakeCrtMessage(crtMsg, unitType);
        crtMsg.team = (int)enPlayerTeamInBattleField.Team_Dummy;
        crtMsg.posX = position.x;   
        crtMsg.posZ = position.z;       
        crtMsg.normX = 0;
        crtMsg.normZ = -1;

        Manager.GamePlayer.NewUnitSessionsDummy.Add(crtCode, dummySession);

        if (!dummySession.SendPacket<MSG_UNIT_S_CREATE_UNIT>(crtMsg))
        {
            //Debug.Log("테스트 유닛 생성 메시지 송신 실패");
            return;
        }
    }

    private void CreateEnemySample(Vector3 position)
    {
        NetworkManager network = new NetworkManager();
        network.Connect(Manager.GamePlayer.GameServerIP);

        // BattleField ID 지정
        MSG_UNIT_S_CONN_BATTLE_FIELD connMsg = new MSG_UNIT_S_CONN_BATTLE_FIELD();
        connMsg.type = (ushort)enPacketType.UNIT_S_CONN_BATTLE_FIELD;
        connMsg.fieldID = Manager.GamePlayer.BattleFieldID;
        if (!network.SendPacket<MSG_UNIT_S_CONN_BATTLE_FIELD>(connMsg))
        {
            //Debug.Log("MSG_UNIT_S_CONN_BATTLE_FIELD 메시지 송신 실패");
            return;
        }

        MSG_UNIT_S_CREATE_UNIT crtMsg = new MSG_UNIT_S_CREATE_UNIT();
        crtMsg.type = (ushort)enPacketType.UNIT_S_CREATE_UNIT;
        crtMsg.crtCode = 9999;
        crtMsg.unitType = (int)enUnitType.Terran_Marine;
        crtMsg.team = (int)enPlayerTeamInBattleField.Team_Test;
        crtMsg.posX = position.x;
        crtMsg.posZ = position.z;
        crtMsg.normX = 0;
        crtMsg.normZ = -1;

        if (!network.SendPacket<MSG_UNIT_S_CREATE_UNIT>(crtMsg))
        {
            //Debug.Log("테스트 유닛 생성 메시지 송신 실패");
            return;
        }
    }
}
