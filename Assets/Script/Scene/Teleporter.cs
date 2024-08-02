using System;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    static int s_CrtCode = 0;

    void OnTriggerEnter(Collider collider) {
        Teleportable teleportable = collider.transform.GetComponent<Teleportable>();
        if (teleportable != null) {
            OnEnter(teleportable);
        }

        collider.gameObject.SetActive(false);
    }

    public void OnEnter(Teleportable teleportable) {
        if (!teleportable.canTeleport) {
            return;
        }
        teleportable.canTeleport = false;

    
        NetworkManager session = new NetworkManager();
        session.Connect();
        // BattleField ID 지정
        MSG_UNIT_S_CONN_BATTLE_FIELD connMsg = new MSG_UNIT_S_CONN_BATTLE_FIELD();  
        connMsg.type = (ushort)enPacketType.UNIT_S_CONN_BATTLE_FIELD;
        connMsg.fieldID = Manager.GamePlayer.BattleFieldID;
        if (!session.SendPacket<MSG_UNIT_S_CONN_BATTLE_FIELD>(connMsg))
        {
            Debug.Log("MSG_UNIT_S_CONN_BATTLE_FIELD 메시지 송신 실패");
            return;
        }
        MSG_UNIT_S_CREATE_UNIT crtMsg = new MSG_UNIT_S_CREATE_UNIT();
        int crtCode = s_CrtCode++;
        crtMsg.type = (ushort)enPacketType.UNIT_S_CREATE_UNIT;
        crtMsg.crtCode = crtCode;
        crtMsg.team = Manager.GamePlayer.m_Team;

        if (crtMsg.team == (int)enPlayerTeamInBattleField.Team_A)
        {
            crtMsg.normX = 1f;
            crtMsg.normZ = 1f;
        }
        else if (crtMsg.team == (int)enPlayerTeamInBattleField.Team_B)
        {
            crtMsg.normX = -1f;
            crtMsg.normZ = 1f;
        }
        else if (crtMsg.team == (int)enPlayerTeamInBattleField.Team_C)
        {
            crtMsg.normX = -1f;
            crtMsg.normZ = -1f;
        }
        else if (crtMsg.team == (int)enPlayerTeamInBattleField.Team_D)
        {
            crtMsg.normX = 1f;
            crtMsg.normZ = -1f;
        }

        if (transform.parent.name == "port_marine")
        {
            crtMsg.unitType = (int)enUnitType.Terran_Marine;
        }
        else if (transform.parent.name == "port_firebat")
        {
            crtMsg.unitType = (int)enUnitType.Terran_Firebat;
        }
        else if (transform.parent.name == "port_tank")
        {
            crtMsg.unitType = (int)enUnitType.Terran_Tank;
        }
        else if (transform.parent.name == "port_robocop")
        {
            crtMsg.unitType = (int)enUnitType.Terran_Robocop;
        }
        else if (transform.parent.name == "port_zergling")
        {
            crtMsg.unitType = (int)enUnitType.Zerg_Zergling;
        }
        else if (transform.parent.name == "port_hydra")
        {
            crtMsg.unitType = (int)enUnitType.Zerg_Hydra;
        }
        else if (transform.parent.name == "port_golem")
        {
            crtMsg.unitType = (int)enUnitType.Zerg_Golem;
        }
        else if (transform.parent.name == "port_tarantula")
        {
            crtMsg.unitType = (int)enUnitType.Zerg_Tarantula;
        }
        else
        {
            Debug.Log("strange port name....!");
        }
        Vector3 crtPos = Vector3.zero;
        crtPos = BattleField.GetRandomCreatePosition((enUnitType)crtMsg.unitType);
        crtMsg.posX = crtPos.x;
        crtMsg.posZ = crtPos.z;

        Tuple<NetworkManager, MSG_UNIT_S_CREATE_UNIT> crtTuple = new Tuple<NetworkManager, MSG_UNIT_S_CREATE_UNIT>(session, crtMsg);
        Manager.GamePlayer.CrtMessageList.Add(crtTuple);  
        Manager.GamePlayer.NewUnitSessions.Add(crtCode, session);
        Manager.GamePlayer.MyTeamUnitCnt++;
    
        Manager.GamePlayer.m_SelectorCnt--;
        if (Manager.GamePlayer.m_SelectorCnt == 0)
        {
            Manager.SceneTransfer.TransferToBattleField();
        }
    }
}
