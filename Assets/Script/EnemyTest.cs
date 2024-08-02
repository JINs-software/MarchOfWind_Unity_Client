using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public int m_id;
    public int m_type;
    public int m_team;

    public int m_MaxHP;
    public int m_HP;

    private void Start()
    {
        NetworkManager network = new NetworkManager();
        network.Connect();


        // BattleField ID 지정
        MSG_UNIT_S_CONN_BATTLE_FIELD connMsg = new MSG_UNIT_S_CONN_BATTLE_FIELD();
        connMsg.type = (ushort)enPacketType.UNIT_S_CONN_BATTLE_FIELD;
        connMsg.fieldID = Manager.GamePlayer.BattleFieldID;
        if (!network.SendPacket<MSG_UNIT_S_CONN_BATTLE_FIELD>(connMsg))
        {
            Debug.Log("MSG_UNIT_S_CONN_BATTLE_FIELD 메시지 송신 실패");
            return;
        }

        MSG_UNIT_S_CREATE_UNIT crtMsg = new MSG_UNIT_S_CREATE_UNIT();
        crtMsg.type = (ushort)enPacketType.UNIT_S_CREATE_UNIT;
        crtMsg.crtCode = 7777;
        crtMsg.unitType = (int)enUnitType.Terran_Marine;
        crtMsg.team = (int)enPlayerTeamInBattleField.Team_Test;
        crtMsg.posX = 200;
        crtMsg.posZ = 250;
        crtMsg.normX = 0;
        crtMsg.normZ = -1;

        if (!network.SendPacket<MSG_UNIT_S_CREATE_UNIT>(crtMsg))
        {
            Debug.Log("테스트 유닛 생성 메시지 송신 실패");
            return;
        }

        gameObject.SetActive(false);    
    }

}
