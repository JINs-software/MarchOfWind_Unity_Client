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

    public Dictionary<int, NetworkManager> NewUnitSessionsDummy = new Dictionary<int, NetworkManager>();

    public string GameServerIP;

    private int CrtCode = 0;

    public void Update()
    {
        if (JpsObsMsgQueue.Count > 0)
        {
            MSG_S_MONT_JPS_OBSTACLE msg = JpsObsMsgQueue.Dequeue();
            SetJpsObstacle(msg);
        }
    }

    public int MakeCrtMessage(MSG_UNIT_S_CREATE_UNIT crtMsg, enUnitType unitType)
    {
        int crtCode = CrtCode++;

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

        crtMsg.unitType = (int)unitType;

        Vector3 crtPos = Vector3.zero;
        crtPos = BattleField.GetRandomCreatePosition((enUnitType)crtMsg.unitType);
        crtMsg.posX = crtPos.x;
        crtMsg.posZ = crtPos.z;

        return crtCode;
    }

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

    ///////////////////////////////////////////////////////
    /// JPS Obstacle Mark (Debug)
    ///////////////////////////////////////////////////////

    public Queue<MSG_S_MONT_JPS_OBSTACLE> JpsObsMsgQueue = new Queue<MSG_S_MONT_JPS_OBSTACLE>();
    Dictionary<Tuple<int, int>, GameObject> JpsObsMarksSet = new Dictionary<Tuple<int, int>, GameObject>();

    internal void SetJpsObstacle(MSG_S_MONT_JPS_OBSTACLE msg)
    {
        if ((enJpsObstacleSetting)msg.setting == enJpsObstacleSetting.CLEAR)
        {
            foreach(var obs in JpsObsMarksSet)
            {
                obs.Value.SetActive(false);
                GameObject.Destroy(obs.Value);
            }
            JpsObsMarksSet.Clear();
        }
        else if ((enJpsObstacleSetting)msg.setting == enJpsObstacleSetting.SET){
            if (!JpsObsMarksSet.ContainsKey(new Tuple<int, int>(msg.x, msg.y)))
            {
                GameObject obsMark = GameObject.Instantiate(ColliderMarkObject, new Vector3(msg.x, 0f, msg.y), Quaternion.identity);
                obsMark.SetActive(true);
                JpsObsMarksSet.Add(new Tuple<int, int>(msg.x, msg.y), obsMark);
            }
        }
        else if ((enJpsObstacleSetting)msg.setting == enJpsObstacleSetting.UNSET)
        {
            if(JpsObsMarksSet.ContainsKey(new Tuple<int, int>(msg.x, msg.y)))
            {
                GameObject gameObject = JpsObsMarksSet[new Tuple<int, int>(msg.x, msg.y)];
                gameObject.SetActive(false);    
                GameObject.Destroy(gameObject);
                JpsObsMarksSet.Remove(new Tuple<int, int>(msg.x, msg.y));
            }
        }
    }
}
