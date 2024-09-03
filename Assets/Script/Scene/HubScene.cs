using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void OnReceiveCreateRoomSuccess()
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
    }

    public void OnReceiveJoinRoomSuccess()
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

    public void OnReceivePlayerReady(UInt16 playerID)
    {
        if(matchRoomUI != null)
        {
            matchRoomUI.SetPlayerReady(playerID, true);
        }
    }

    public void OnReceiveLaunchMatch()
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

    private void OnJoinBtnClicked() {
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
