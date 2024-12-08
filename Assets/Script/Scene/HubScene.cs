using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HubScene : BaseScene
{
    MOW_HUB stup_MOW_HUB;

    public InitUI initUI;
    public CreateMatchUI createMatchUI;
    public LobbyUI lobbyUI;
    public MatchRoomUI matchRoomUI;

    public UInt16 PlayerID;
    public UInt16 RoomID;
    public bool IsHost = false;

    protected override void Init()
    {
        //////////////////////////////////////
        // temp, 채팅 서버 입장
        //if(!ChattingManager.Instance.Connect())
        //{
        //    Debug.Log("채팅 서버 접속 실패");
        //    return;
        //}
        
        //string tokenStr = "12345";
        //byte[] token = Encoding.Unicode.GetBytes(tokenStr);
        //ChattingManager.Instance.Login(GamaManager.Instance.AccountNo, token, token.Length, null);
        //////////////////////////////////////

        base.Init();

        // 사용 Stub 컴포넌트 부착
        stup_MOW_HUB = gameObject.GetComponent<MOW_HUB>();
        if (stup_MOW_HUB == null)
        {
            stup_MOW_HUB = gameObject.AddComponent<MOW_HUB>();
        }

        // RPC 등록
        //RPC.Instance.AttachStub(stup_MOW_HUB);
        //=> stup_MOW_HUB Start에서 자동으로 Attach

        GameObject initUIObj =  Manager.Resource.Instantiate("UI/InitUI");
        if(initUIObj != null)
        {
            initUI = initUIObj.GetComponent<InitUI>();
            initUI.CreateBtnHandler -= OnCreateBtnClicked;
            initUI.CreateBtnHandler += OnCreateBtnClicked;
            initUI.JoinBtnHandler-= OnJoinBtnClicked;
            initUI.JoinBtnHandler += OnJoinBtnClicked;
            initUI.SettingBtnHandler -= OnSettingBtnClicked;
            initUI.SettingBtnHandler += OnSettingBtnClicked;
            initUI.QuitBtnHandler -= OnQuitBtnClicked;
            initUI.QuitBtnHandler += OnQuitBtnClicked;
        }
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }


    // Hub Stub -> Connect Reply 
    public void OnRecv_ConnectReply(Byte reply) 
    {
        switch ((enCONNECTION_REPLY_CODE)reply)
        {
            case enCONNECTION_REPLY_CODE.SUCCESS:
                initUI.SetUI_ConnSuccess("Connetion Completed!");
                break;
            case enCONNECTION_REPLY_CODE.PLAYER_CAPACITY_EXCEEDED:
                initUI.SetUI_ConnInvalid("SERVER: PLAYER_CAPACITY_EXCEEDED!");
                break;
            case enCONNECTION_REPLY_CODE.INVALID_MSG_FIELD_VALUE:
                initUI.SetUI_ConnFail("SERVER: INVALID_MSG_FIELD_VALUE!");
                break;
            case enCONNECTION_REPLY_CODE.PLAYER_NAME_ALREADY_EXIXTS:
                initUI.SetUI_ConnFail("SERVER: PLAYER_NAME_ALREADY_EXIXTS!");
                break;
            default:
                initUI.SetUI_ConnInvalid("SERVER ERR: INVALID REPLY CODE!");
                break;
        }
    }

    // Hub Stub -> Create Room Success Reply 
    public void OnRecv_CreateRoomSuccess(UInt16 matchID)
    {
        // 매치룸 입장
        if (createMatchUI != null)
        {
            Manager.Resource.Destroy(createMatchUI.gameObject);
            createMatchUI = null;

            GameObject matchRoomUIObj = Manager.Resource.Instantiate("UI/MatchRoomUI");
            if (matchRoomUIObj != null)
            {
                matchRoomUI = matchRoomUIObj.GetComponent<MatchRoomUI>();
                matchRoomUI.StartReadyBtnClickHandler -= OnStartReadyRoomClicked;
                matchRoomUI.StartReadyBtnClickHandler += OnStartReadyRoomClicked;
                matchRoomUI.CancelBtnClickHandler -= OnMatchRoomCancelBtnClicked;
                matchRoomUI.CancelBtnClickHandler += OnMatchRoomCancelBtnClicked;
            }
        }

        // 채팅 서버
        ChattingManager.Instance.EnterMatch(matchID);
    }

    public void OnRecv_JoinRoomSuccess()
    {
        if (lobbyUI != null)
        {
            Manager.Resource.Destroy(lobbyUI.gameObject);
            lobbyUI = null;

            GameObject matchRoomUIObj = Manager.Resource.Instantiate("UI/MatchRoomUI");
            if (matchRoomUIObj != null)
            {
                matchRoomUI = matchRoomUIObj.GetComponent<MatchRoomUI>();
                matchRoomUI.StartReadyBtnClickHandler -= OnStartReadyRoomClicked;
                matchRoomUI.StartReadyBtnClickHandler += OnStartReadyRoomClicked;
                matchRoomUI.CancelBtnClickHandler -= OnMatchRoomCancelBtnClicked;
                matchRoomUI.CancelBtnClickHandler += OnMatchRoomCancelBtnClicked;
            }
        }
    }

    public void OnRecv_PlayerReady(UInt16 playerID)
    {
        if(matchRoomUI != null)
        {
            matchRoomUI.SetPlayerReady(playerID, true);
        }
    }

    public void OnRecv_LaunchMatch()
    {
        // => 로딩 씬 전환
        Manager.Scene.Clear();
        Manager.Scene.LoadScene(Define.Scene.LoadScene);
    }

    private void OnCreateBtnClicked()
    {
        if(initUI != null)
        {
            Manager.Resource.Destroy(initUI.gameObject);
            initUI = null;
            GameObject crtMatchUIObj = Manager.Resource.Instantiate("UI/CreateMatchUI");
            if(crtMatchUIObj != null)
            {
                createMatchUI = crtMatchUIObj.GetComponent<CreateMatchUI>();
                createMatchUI.CancelBtnHandler -= OnCrtMatchCancelBtnClicked;
                createMatchUI.CancelBtnHandler += OnCrtMatchCancelBtnClicked;
            }
        }
    }

    private void OnJoinBtnClicked()
    {
        if (initUI != null)
        {
            // 로비 입장 메시지 전송
            RPC.proxy.ENTER_TO_ROBBY();

            Manager.Resource.Destroy(initUI.gameObject);
            initUI = null;
            GameObject lobbyUIObj = Manager.Resource.Instantiate("UI/LobbyUI");
            if (lobbyUIObj != null)
            {
                lobbyUI = lobbyUIObj.GetComponent<LobbyUI>();
                lobbyUI.MatchRoomBtnClickHandler -= OnMatchRoomBtnClick;
                lobbyUI.MatchRoomBtnClickHandler += OnMatchRoomBtnClick;
                lobbyUI.CancelBtnHandler -= OnLobbyCancelBtnClicked;
                lobbyUI.CancelBtnHandler += OnLobbyCancelBtnClicked;
            }
        }
    }

    private void OnCrtMatchCancelBtnClicked()
    {
        if (createMatchUI != null)
        {
            Manager.Resource.Destroy(createMatchUI.gameObject);
            createMatchUI = null;
            GameObject initUIObj = Manager.Resource.Instantiate("UI/InitUI");
            if (initUIObj != null)
            {
                initUI = initUIObj.GetComponent<InitUI>();
                initUI.CreateBtnHandler -= OnCreateBtnClicked;
                initUI.CreateBtnHandler += OnCreateBtnClicked;
                initUI.JoinBtnHandler -= OnJoinBtnClicked;
                initUI.JoinBtnHandler += OnJoinBtnClicked;
                initUI.SettingBtnHandler -= OnSettingBtnClicked;
                initUI.SettingBtnHandler += OnSettingBtnClicked;
                initUI.QuitBtnHandler -= OnQuitBtnClicked;
                initUI.QuitBtnHandler += OnQuitBtnClicked;
            }
        }
    }

    private void OnLobbyCancelBtnClicked()
    {
        if(lobbyUI != null)
        {
            Manager.Resource.Destroy(lobbyUI.gameObject);
            lobbyUI = null;
            GameObject initUIObj = Manager.Resource.Instantiate("UI/InitUI");
            if (initUIObj != null)
            {
                initUI = initUIObj.GetComponent<InitUI>();
                initUI.CreateBtnHandler -= OnCreateBtnClicked;
                initUI.CreateBtnHandler += OnCreateBtnClicked;
                initUI.JoinBtnHandler -= OnJoinBtnClicked;
                initUI.JoinBtnHandler += OnJoinBtnClicked;
                initUI.SettingBtnHandler -= OnSettingBtnClicked;
                initUI.SettingBtnHandler += OnSettingBtnClicked;
                initUI.QuitBtnHandler -= OnQuitBtnClicked;
                initUI.QuitBtnHandler += OnQuitBtnClicked;
            }
        }
    }

    private void OnMatchRoomBtnClick(UInt16 matchRoomID)
    {
        lobbyUI.SetOnlyCancelBtn();
        RPC.proxy.JOIN_TO_MATCH_ROOM(matchRoomID);

        // 채팅 서버
        ChattingManager.Instance.EnterMatch(matchRoomID);
    }

    private void OnLobbyCancelBtnClick()
    {
        RPC.proxy.QUIT_FROM_ROBBY();
        if (lobbyUI != null)
        {
            Manager.Resource.Destroy(lobbyUI.gameObject);
            lobbyUI = null;
            GameObject initUIObj = Manager.Resource.Instantiate("UI/InitUI");
            if (initUIObj != null)
            {
                initUI = initUIObj.GetComponent<InitUI>();
                initUI.CreateBtnHandler -= OnCreateBtnClicked;
                initUI.CreateBtnHandler += OnCreateBtnClicked;
                initUI.JoinBtnHandler -= OnJoinBtnClicked;
                initUI.JoinBtnHandler += OnJoinBtnClicked;
                initUI.SettingBtnHandler -= OnSettingBtnClicked;
                initUI.SettingBtnHandler += OnSettingBtnClicked;
                initUI.QuitBtnHandler -= OnQuitBtnClicked;
                initUI.QuitBtnHandler += OnQuitBtnClicked;
            }
        }
    }

    private void OnStartReadyRoomClicked()
    {
        // Start/Ready Btn 클릭
        if (IsHost)
        {
            RPC.proxy.MATCH_START();
        }
        else
        {
            RPC.proxy.MATCH_READY();
        }
    }

    private void OnMatchRoomCancelBtnClicked()
    {
        RPC.proxy.QUIT_FROM_MATCH_ROOM();   
        if (matchRoomUI != null)
        {
            Manager.Resource.Destroy(matchRoomUI.gameObject);
            matchRoomUI = null;
            GameObject lobbyUIObj = Manager.Resource.Instantiate("UI/LobbyUI");
            if (lobbyUIObj != null)
            {
                lobbyUI = lobbyUIObj.GetComponent<LobbyUI>();
                lobbyUI.MatchRoomBtnClickHandler -= OnMatchRoomBtnClick;
                lobbyUI.MatchRoomBtnClickHandler += OnMatchRoomBtnClick;
                lobbyUI.CancelBtnHandler -= OnLobbyCancelBtnClicked;
                lobbyUI.CancelBtnHandler += OnLobbyCancelBtnClicked;
            }
        }
    }

    public void OnSettingBtnClicked()
    {

    }

    public void OnQuitBtnClicked()
    {

    }
}
