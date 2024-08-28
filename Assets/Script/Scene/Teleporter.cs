using System;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Teleporter : MonoBehaviour {
    public int CreateUnitCount;

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

    
        for(int i = 0; i<CreateUnitCount; i++)
        {
            NetworkManager session = new NetworkManager();
            session.Connect(Manager.GamePlayer.GameServerIP);
            // BattleField ID 지정
            MSG_UNIT_S_CONN_BATTLE_FIELD connMsg = new MSG_UNIT_S_CONN_BATTLE_FIELD();
            connMsg.type = (ushort)enPacketType.UNIT_S_CONN_BATTLE_FIELD;
            connMsg.fieldID = Manager.GamePlayer.BattleFieldID;
            session.SendPacket<MSG_UNIT_S_CONN_BATTLE_FIELD>(connMsg);

            MSG_UNIT_S_CREATE_UNIT crtMsg = new MSG_UNIT_S_CREATE_UNIT();

            enUnitType unitType = enUnitType.None;
            if (transform.parent.name == "port_marine")
            {
                unitType = enUnitType.Terran_Marine;
            }
            else if (transform.parent.name == "port_firebat")
            {
                unitType = enUnitType.Terran_Firebat;
            }
            else if (transform.parent.name == "port_tank")
            {
                unitType = enUnitType.Terran_Tank;
            }
            else if (transform.parent.name == "port_robocop")
            {
                unitType = enUnitType.Terran_Robocop;
            }
            else if (transform.parent.name == "port_zergling")
            {
                unitType = enUnitType.Zerg_Zergling;
            }
            else if (transform.parent.name == "port_hydra")
            {
                unitType = enUnitType.Zerg_Hydra;
            }
            else if (transform.parent.name == "port_golem")
            {
                unitType = enUnitType.Zerg_Golem;
            }
            else if (transform.parent.name == "port_tarantula")
            {
                unitType = enUnitType.Zerg_Tarantula;
            }

            int crtCode = Manager.GamePlayer.MakeCrtMessage(crtMsg, unitType);

            Tuple<NetworkManager, MSG_UNIT_S_CREATE_UNIT> crtTuple = new Tuple<NetworkManager, MSG_UNIT_S_CREATE_UNIT>(session, crtMsg);
            Manager.GamePlayer.CrtMessageList.Add(crtTuple);
            Manager.GamePlayer.NewUnitSessions.Add(crtCode, session);
            Manager.GamePlayer.MyTeamUnitCnt++;
        }
    
        Manager.GamePlayer.m_SelectorCnt--;
        if (Manager.GamePlayer.m_SelectorCnt == 0)
        {
            Manager.SceneTransfer.TransferToBattleField();
        }
    }
}