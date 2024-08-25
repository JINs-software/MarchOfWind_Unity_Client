using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager s_Instance;   // 유일성 보장
    static Manager Instance { get { Init(); return s_Instance; } }

    static bool IsGameMode = false;
    public static bool GameMode { get { return IsGameMode; } set { IsGameMode = value; } }

    NetworkManager          m_NetworkMgr = new NetworkManager();
    UnitSelectionManager    m_UnitSelectionMgr = new UnitSelectionManager();
    GamePlayManager         m_GamePlayMgr = new GamePlayManager();  
    SceneTransfer           m_SceneTransfer = new SceneTransfer();  
    DebugManager            m_DebugManager = new DebugManager();

    public static NetworkManager Network { get { return Instance.m_NetworkMgr; } }
    public static UnitSelectionManager UnitSelection { get { return Instance.m_UnitSelectionMgr; } }    
    public static GamePlayManager GamePlayer { get { return Instance.m_GamePlayMgr; } } 
    public static SceneTransfer SceneTransfer { get { return Instance.m_SceneTransfer; } }

    public static DebugManager DebugManager { get { return Instance.m_DebugManager; } }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //if (IsGameMode)
        //{
        //    m_UnitSelectionMgr.Update();
        //    m_GamePlayMgr.Update();
        //}

        m_DebugManager.Update();    
    }

    static void Init()
    {
        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@Manager");
            if (go == null)
            {
                go = new GameObject { name = "@Manager" };
                go.AddComponent<Manager>();
            }

            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<Manager>();

            //s_Instance.m_UnitSelectionMgr.Init();   
            // => 실제 게임이 시작되면 Init 되도록함

        }
    }
}
