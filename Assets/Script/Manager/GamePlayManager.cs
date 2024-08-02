using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager 
{
    public string TeamTagStr = "TeamUnit";
    public string EnemyTagStr = "Enemy";

    public bool TestMode = false;
    public Queue<MSG_S_MGR_CREATE_UNIT> TestCrtmsg = new Queue<MSG_S_MGR_CREATE_UNIT>();

    public string   m_PlayerID;
    public int      m_Team;

    public int m_Scene;
    public int m_SelectorCnt;

    //public Dictionary<int, NetworkManager> m_UnitSessions;

    public int BattleFieldID;

    public List<Tuple<NetworkManager, MSG_UNIT_S_CREATE_UNIT>> CrtMessageList = new List<Tuple<NetworkManager, MSG_UNIT_S_CREATE_UNIT>>();  
    public Dictionary<int, NetworkManager> NewUnitSessions = new Dictionary<int, NetworkManager>(); 
    public int MyTeamUnitCnt = 0;   

}
