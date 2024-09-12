
using System;
using System.Collections.Generic;
using UnityEngine;

public class GamaManager : MonoBehaviour 
{
    static GamaManager s_Instance;
    public static GamaManager Instance { get { init(); return s_Instance; } }

    UnitSelectionManager m_UnitSelectionMgr = new UnitSelectionManager();
    public static UnitSelectionManager UnitSelection { get { return Instance.m_UnitSelectionMgr; } }

    private static void init()
    {
        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@GameManager");
            if (go == null)
            {
                go = new GameObject { name = "@GameManager" };
                go.AddComponent<GamaManager>();
            }

            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<GamaManager>();

            //if(s_Instance.m_UnitSelectionMgr == null)
            //{
            //    s_Instance.m_UnitSelectionMgr = new UnitSelectionManager();
            //}
            //s_Instance.m_UnitSelectionMgr.Init();
            // => UnitSelectionMgr은 SelectField와 BattleField 초반부에 별도 호출이 필요함.
        }
    }

    private void Update()
    {
        if(s_Instance.m_UnitSelectionMgr != null)
        {
            s_Instance.m_UnitSelectionMgr.Update();
        }
    }

    //------------------------------------------------------
    public const string TEAM_TAG = "TeamUnit";
    public const string ENEMY_TAG = "Enemy";
    public const string DUMMY_TAG = "Dummy";

    public const string CLICKABLE_LAYER = "Clickable";
    public const string ATTACKABLE_LAYER = "Attackable";

    public string ServerIP;
    public UInt16 ServerPort;       

    public UInt16 BattleFieldID;
    Byte team;
    public Byte Team
    {
        get
        {
            return team;
        }
        set
        {
            team = value;
            switch (team)
            {
                case 0:
                    InitNorm = new Vector3(1, 0, 1);
                    InitNorm = InitNorm.normalized;
                    InitPosition = new Vector3(150f, 0, 150f);
                    break;
                case 1:
                    InitNorm = new Vector3(-1, 0, 1);
                    InitNorm = InitNorm.normalized;
                    InitPosition = new Vector3(250f, 0, 150f);
                    break;
                case 2:
                    InitNorm = new Vector3(-1, 0, -1);
                    InitNorm = InitNorm.normalized;
                    InitPosition = new Vector3(250f, 0, 250f);
                    break;
                case 3:
                    InitNorm = new Vector3(1, 0, -1);
                    InitNorm = InitNorm.normalized;
                    InitPosition = new Vector3(150f, 0, 150f);
                    break;
            }
        }
    }
    public UInt16 AliveUnitCnt = 0;
    public Byte SelectorCnt;
    public Vector3 InitNorm;
    public Vector3 InitPosition = Vector3.zero;
    private Dictionary<int, NetworkManager> UnitSessionMap = new Dictionary<int, NetworkManager>();
    public void SetUnitSession(int crtCode, NetworkManager unitSession)
    {
        UnitSessionMap.Add(crtCode, unitSession);
    }
    public NetworkManager GetUnitSession(int crtCode)
    {
        NetworkManager unitSession = null;

        if (UnitSessionMap.ContainsKey(crtCode))
        {
            unitSession = UnitSessionMap[crtCode];
            UnitSessionMap.Remove(crtCode);
        }
        
        return unitSession;
    }
}