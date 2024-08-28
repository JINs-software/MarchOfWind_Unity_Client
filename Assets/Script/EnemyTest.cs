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

    int randomCrtCode = 7777;
    private void OnEnable()
    {
        NetworkManager network = new NetworkManager();
        network.Connect(Manager.GamePlayer.GameServerIP);


        // BattleField ID ÁöÁ¤
        MSG_UNIT_S_CONN_BATTLE_FIELD connMsg = new MSG_UNIT_S_CONN_BATTLE_FIELD();
        connMsg.type = (ushort)enPacketType.UNIT_S_CONN_BATTLE_FIELD;
        connMsg.fieldID = Manager.GamePlayer.BattleFieldID;
        network.SendPacket<MSG_UNIT_S_CONN_BATTLE_FIELD>(connMsg);

        MSG_UNIT_S_CREATE_UNIT crtMsg = new MSG_UNIT_S_CREATE_UNIT();
        crtMsg.type = (ushort)enPacketType.UNIT_S_CREATE_UNIT;
        crtMsg.crtCode = randomCrtCode++;
        crtMsg.unitType = (int)enUnitType.Terran_Marine;
        crtMsg.team = (int)enPlayerTeamInBattleField.Team_Test;
        crtMsg.posX = 200;
        crtMsg.posZ = 250;
        crtMsg.normX = 0;
        crtMsg.normZ = -1;

        network.SendPacket<MSG_UNIT_S_CREATE_UNIT>(crtMsg);

        gameObject.SetActive(false);    
    }

}
