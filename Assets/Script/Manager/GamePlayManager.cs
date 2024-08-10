using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

    public string GameServerIP;

    ///////////////////////////////////////////////////////
    /// Collider Element Mark (Debug)
    ///////////////////////////////////////////////////////
    GameObject ColliderMarkObject = null;
    public void SetColliderMarkObject(GameObject obj)
    {
        ColliderMarkObject = obj;
    }


    public List<GameObject> ColliderElementMarks = new List<GameObject>();
    public void ResetColliderMarks()
    {
        foreach(var mark in ColliderElementMarks)
        {
            if(mark != null )
            {
                mark.SetActive(false);
                GameObject.Destroy(mark);
            }
        }
        ColliderElementMarks.Clear();
    }
    public void SetColliderMarks(MSG_S_MONT_COLLIDER_MAP msg)
    {
        if(ColliderMarkObject != null)
        {
            for(int i=0; i<msg.numOfElements; i++)
            {
                Position pos = msg.colliders[i];
                GameObject elemMark =  GameObject.Instantiate(ColliderMarkObject, new Vector3(pos.X / 10f, 0f, pos.Z / 10f), Quaternion.identity);
                elemMark.SetActive(true);
                ColliderElementMarks.Add(elemMark); 
            }
        }
    }

}
