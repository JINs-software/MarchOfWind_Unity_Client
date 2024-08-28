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

    public UInt16 PlayerID;
    public UInt16 RoomID;   

    protected override void Init()
    {
        base.Init();

        // ��� Stub ������Ʈ ����
        stup_MOW_HUB = gameObject.GetComponent<MOW_HUB>();
        if (stup_MOW_HUB == null)
        {
            stup_MOW_HUB = gameObject.AddComponent<MOW_HUB>();
        }

        // RPC ���
        //RPC.Instance.AttachStub(stup_MOW_HUB);
        //=> stup_MOW_HUB Start���� �ڵ����� Attach

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
        throw new System.NotImplementedException();
    }

    public void OnReceiveCreateRoomSuccess()
    {

    }

    public void OnReceiveJoinRoomSuccess()
    {

    }

    public void OnCreateBtnClicked()
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

    public void OnJoinBtnClicked() {
        if (initUI != null)
        {
            Manager.Resource.Destroy(initUI.gameObject);
            initUI = null;
            GameObject lobbyUIObj = Manager.Resource.Instantiate("UI/LobbyUI");
            if (lobbyUIObj != null)
            {
                lobbyUI= lobbyUI.GetComponent<LobbyUI>();
                lobbyUI.MatchRoomBtnClickHandler -= OnMatchRoomBtnClick;
                lobbyUI.MatchRoomBtnClickHandler += OnMatchRoomBtnClick;
                lobbyUI.CancelBtnHandler -= OnLobbyCancelBtnClick;
                lobbyUI.CancelBtnHandler += OnLobbyCancelBtnClick;
            }
        }
    }

    public void OnCrtMatchCancelBtnClicked()
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

    private void OnMatchRoomBtnClick(UInt16 matchRoomID)
    {
        lobbyUI.SetOnlyCancelBtn();
        RPC.proxy.JOIN_TO_MATCH_ROOM(matchRoomID);
    }

    private void OnLobbyCancelBtnClick()
    {
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

    public void OnSettingBtnClicked()
    {

    }

    public void OnQuitBtnClicked()
    {

    }
}
