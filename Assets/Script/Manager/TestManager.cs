using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    static TestManager s_Instance;
    public static TestManager Instance { get { Init(); return s_Instance; } }
    public static bool TestMode = false;
    
    bool OnDebug = false;
    TestUI testUI;
    MOW_HUB_TEST HubStub;
    bool readyToTest = false;

    UInt16 playerID = 7777;
    UInt16 matchRoomID = 7777;
    byte team;

    NetworkManager otherSession = null;
    UInt16 otherPlayerID;
    byte otherTeam;


    UInt16 battleFieldID;
    int CrtCode = 0;

    MOW_HUB_TEST hubStub = null;
    MOW_PRE_BATTLE_FIELD_TEST preBattleStub = null;

    enum Toggles {
        MarineToggle,
        FirebatToggle,
        ZerglingToggle,
        HydraToggle,
        MarineEnemyToggle,
        FirebatEnemyToggle,
        ZerglingEnemyToggle,
        HydraEnemyToggle,
        MarineDummyToggle,
        FirebatDummyToggle,
        ZerglingDummyToggle,
        HydraDummyToggle,
        NONE,
    }

    Toggles toggle = Toggles.NONE;

    static void Init()
    {
        TestMode = true;

        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@TestManager");
            if (go == null)
            {
                go = new GameObject { name = "@TestManager" };
                go.AddComponent<TestManager>();
            }

            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<TestManager>();

            if(s_Instance.testUI == null)
            {
                GameObject testUIObj = Manager.Resource.Instantiate("UI/TestUI");
                if(testUIObj != null)
                {
                    s_Instance.testUI = testUIObj.GetComponent<TestUI>();

                    s_Instance.testUI.DebugBtnHandler += s_Instance.DebugBtnHandler;
                    s_Instance.testUI.ConnectBtnHandler += s_Instance.ConnectBtnHandler;
                    s_Instance.testUI.MarineToggleHandler += s_Instance.MarineToggleHandler;
                    s_Instance.testUI.FirebatToggleHandler += s_Instance.FirebatToggleHandler;
                    s_Instance.testUI.ZerglingToggleHandler += s_Instance.ZerglingToggleHandler;
                    s_Instance.testUI.HydraToggleHandler += s_Instance.HydraToggleHandler;
                    s_Instance.testUI.MarineEnemyToggleHandler += s_Instance.MarineEnemyToggleHandler;
                    s_Instance.testUI.FirebatEnemyToggleHandler += s_Instance.FirebatEnemyToggleHandler;
                    s_Instance.testUI.ZerglingEnemyToggleHandler += s_Instance.ZerglingEnemyToggleHandler;
                    s_Instance.testUI.HydraEnemyToggleHandler += s_Instance.HydraEnemyToggleHandler;
                    s_Instance.testUI.MarineDummyToggleHandler += s_Instance.MarineDummyToggleHandler;
                    s_Instance.testUI.FirebatDummyToggleHandler += s_Instance.FirebatDummyToggleHandler;
                    s_Instance.testUI.ZerglingDummyToggleHandler += s_Instance.ZerglingDummyToggleHandler;
                    s_Instance.testUI.HydraDummyToggleHandler += s_Instance.HydraDummyToggleHandler;


                }
            }

            if(Instance.hubStub == null)
            {
                Instance.hubStub = Instance.gameObject.AddComponent<MOW_HUB_TEST>();
            }
            if(Instance.preBattleStub == null)
            {
                Instance.preBattleStub = Instance.gameObject.AddComponent<MOW_PRE_BATTLE_FIELD_TEST>();
            }
        }
    }


    private void OnEnable()
    {
        Debug.Log("TestManager.OnEnable");
        Init();
    }
    
    private void Update()
    {
        if (OnDebug)
        {

            if(Input.GetKey(KeyCode.Escape))
            {
                Instance.OnDebug = false;
                Instance.testUI.DebugOff();
                toggle = Toggles.NONE;
            }
            else
            {
                if(toggle != Toggles.NONE && Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("GroundLayer")))
                    {
                        NetworkManager unitSession = RPC.Instance.AllocNewClientSession();
                        RPC.proxy.UNIT_CONN_TO_BATTLE_FIELD(battleFieldID, unitSession);
                        int crtCode = CrtCode++;
                        byte unitType = (byte)enUnitType.Terran_Marine;
                        if (toggle == Toggles.MarineToggle || toggle == Toggles.MarineEnemyToggle) unitType = (byte)enUnitType.Terran_Marine;
                        else if (toggle == Toggles.FirebatToggle || toggle == Toggles.FirebatEnemyToggle) unitType = (byte)enUnitType.Terran_Firebat;
                        else if (toggle == Toggles.ZerglingToggle || toggle == Toggles.ZerglingEnemyToggle) unitType = (byte)enUnitType.Zerg_Zergling;
                        else if (toggle == Toggles.HydraToggle || toggle == Toggles.HydraEnemyToggle) unitType = (byte)enUnitType.Zerg_Hydra;
                        if(toggle == Toggles.MarineToggle || toggle == Toggles.ZerglingToggle || toggle == Toggles.FirebatToggle || toggle == Toggles.HydraToggle)
                        {
                            RPC.proxy.UNIT_S_CREATE(crtCode, unitType, team, hit.point.x, hit.point.z, 0, 0, unitSession);
                            GamaManager.Instance.SetUnitSession(crtCode, unitSession);
                        }
                        else if (toggle == Toggles.MarineEnemyToggle || toggle == Toggles.ZerglingEnemyToggle || toggle == Toggles.FirebatEnemyToggle || toggle == Toggles.HydraEnemyToggle)
                        {
                            RPC.proxy.UNIT_S_CREATE(crtCode, unitType, otherTeam, hit.point.x, hit.point.z, 0, 0, unitSession);
                            GamaManager.Instance.SetUnitSession(crtCode, unitSession);
                        }
                        else
                        {
                            RPC.proxy.UNIT_S_CREATE(crtCode, unitType, (byte)enPlayerTeamInBattleField.Team_Dummy, hit.point.x, hit.point.z, 0, 0, unitSession);
                            GamaManager.Instance.SetUnitSession(crtCode, unitSession);
                        }
                    }
                }
            }
        }
    }

    void DebugBtnHandler()
    {
        OnDebug = true;
        //testUI.TogglesOn();
        testUI.DebugOn();
    }
    void ConnectBtnHandler() {
        RPC.Instance.Initiate("127.0.0.1", 7777);
        string playerNameStr = "JIN";
        byte[] playerName = Encoding.ASCII.GetBytes(playerNameStr);
        RPC.proxy.CONNECTION(playerName, (byte)playerName.Length);


        otherSession = RPC.Instance.AllocNewClientSession();
        HashSet<UInt16> blockMessages = new HashSet<UInt16>();
        blockMessages.Add(3003);
        blockMessages.Add(3005);
        blockMessages.Add(3008);
        blockMessages.Add(3009);    
        blockMessages.Add(3011);
        blockMessages.Add(3013);
        blockMessages.Add(3015);
        blockMessages.Add(3016);
        blockMessages.Add(3017);
        RPC.Instance.AttachClientSession(otherSession, blockMessages);
        playerNameStr = "OTHER";
        playerName = Encoding.ASCII.GetBytes(playerNameStr);
        RPC.proxy.CONNECTION(playerName, (byte)playerName.Length, otherSession);
    }
    void MarineToggleHandler() { toggle = Toggles.MarineToggle; }
    void FirebatToggleHandler() { toggle = Toggles.FirebatToggle; }
    void ZerglingToggleHandler() { toggle = Toggles.ZerglingToggle; }
    void HydraToggleHandler() { toggle = Toggles.HydraToggle; }
    void MarineEnemyToggleHandler() { toggle = Toggles.MarineEnemyToggle; }
    void FirebatEnemyToggleHandler() { toggle = Toggles.FirebatEnemyToggle; }
    void ZerglingEnemyToggleHandler() { toggle = Toggles.ZerglingEnemyToggle; }
    void HydraEnemyToggleHandler() { toggle = Toggles.HydraEnemyToggle; }
    void MarineDummyToggleHandler() { toggle = Toggles.MarineDummyToggle; }
    void FirebatDummyToggleHandler() { toggle = Toggles.FirebatDummyToggle; }
    void ZerglingDummyToggleHandler() { toggle = Toggles.ZerglingDummyToggle; }
    void HydraDummyToggleHandler() { toggle = Toggles.HydraDummyToggle; }



    public void PROC_CONNECTION_REPLY(byte REPLY_CODE, UInt16 PLAYER_ID)
    {
        if(playerID == 7777)
        {
            playerID = PLAYER_ID;
            string roomNameStr = "my_room";
            byte[] roomName = Encoding.ASCII.GetBytes(roomNameStr);
            RPC.proxy.CREATE_MATCH_ROOM(roomName, (Byte)roomName.Length, 2);
        }
        else
        {
            otherPlayerID = PLAYER_ID;
        }
    }
    public void PROC_CREATE_MATCH_ROOM_REPLY(byte REPLY_CODE, UInt16 MATCH_ROOM_ID)
    {
        matchRoomID = MATCH_ROOM_ID;

        RPC.proxy.ENTER_TO_ROBBY(otherSession);
        RPC.proxy.JOIN_TO_MATCH_ROOM(matchRoomID, otherSession);    

    }
    public void PROC_JOIN_TO_MATCH_ROOM_REPLY(byte REPLY_CODE)
    {
        RPC.proxy.MATCH_READY(otherSession);
        RPC.proxy.MATCH_START();
    }

    bool launchMatchFlag = false;
    public void PROC_LAUNCH_MATCH()
    {
        if (!launchMatchFlag)
        {
            launchMatchFlag = true;
            RPC.proxy.READY_TO_BATTLE();
        }
        else
        {
            RPC.proxy.READY_TO_BATTLE(otherSession);
        }
    }

    bool readyToBattleFlag = false;
    public void READY_TO_BATTLE_REPLY(UInt16 BATTLE_FIELD_ID, byte TEAM)
    {
        if(!readyToBattleFlag)
        {
            readyToBattleFlag = true;
            battleFieldID = BATTLE_FIELD_ID;
            team = TEAM;
        }
        else
        {
            otherTeam = TEAM;
        }
    }

    bool allPlayerReadyFlag = false;
    public void PROC_ALL_PLAYER_READY()
    {
        if(!allPlayerReadyFlag)
        {
            allPlayerReadyFlag = true;
            RPC.proxy.ENTER_TO_SELECT_FIELD();
        }
        else
        {
            RPC.proxy.ENTER_TO_SELECT_FIELD(otherSession);
        }
    }

    bool enterToSelectFieldFlag = false;
    public void PROC_ENTER_TO_SELECT_FIELD_REPLY(byte SELECTOR_COUNT)
    {
        if (!enterToSelectFieldFlag)
        {
            enterToSelectFieldFlag = true;      
            Instance.readyToTest = true;
            Instance.testUI.TogglesOn();

            RPC.proxy.ENTER_TO_BATTLE_FIELD();  
        }
        else
        {
            RPC.proxy.ENTER_TO_BATTLE_FIELD(otherSession);
        }
    }
}
