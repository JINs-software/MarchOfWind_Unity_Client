using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawn : MonoBehaviour
{
    [SerializeField]
    enUnitType UnitType;  

    private void OnEnable()
    {
        NetworkManager dummySession = new NetworkManager();
        dummySession.Connect(Manager.GamePlayer.GameServerIP);

        MSG_UNIT_S_CONN_BATTLE_FIELD connMsg = new MSG_UNIT_S_CONN_BATTLE_FIELD();
        connMsg.type = (ushort)enPacketType.UNIT_S_CONN_BATTLE_FIELD;
        connMsg.fieldID = Manager.GamePlayer.BattleFieldID;
        if (!dummySession.SendPacket<MSG_UNIT_S_CONN_BATTLE_FIELD>(connMsg))
        {
            Debug.Log("MSG_UNIT_S_CONN_BATTLE_FIELD 메시지 송신 실패");
            return;
        }

        MSG_UNIT_S_CREATE_UNIT crtMsg = new MSG_UNIT_S_CREATE_UNIT();   
        int crtCode = Manager.GamePlayer.MakeCrtMessage(crtMsg, UnitType);
        crtMsg.team = (int)enPlayerTeamInBattleField.Team_Dummy;

        Manager.GamePlayer.NewUnitSessionsDummy.Add(crtCode, dummySession);

        if (!dummySession.SendPacket<MSG_UNIT_S_CREATE_UNIT>(crtMsg))
        {
            Debug.Log("테스트 유닛 생성 메시지 송신 실패");
            return;
        }

        gameObject.SetActive(false);
    }
}
