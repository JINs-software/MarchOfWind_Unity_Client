using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatchRoomUI : UI_Base
{
    const byte MAX_OF_PLAYERS = 4;

    enum Buttons
    {
        StartReadyBtn,
        CancelBtn,
    }

    enum Texts
    {
        StatusText,
    }

    public Action StartReadyBtnClickHandler;
    public Action CancelBtnClickHandler;
    Button startReadyBtn;
    Button cancelBtn;
    Text statusText;
    GameObject playerListContent;

    Dictionary<UInt16, Tuple<PlayerBtn, byte>> playerMap = new Dictionary<ushort, Tuple<PlayerBtn, byte>>();
    PlayerBtn[] playerBtns = new PlayerBtn[MAX_OF_PLAYERS];


    //private void Start()
    //{
    //    Init();
    //}
    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        startReadyBtn = GetButton((int)Buttons.StartReadyBtn);
        cancelBtn = GetButton((int)Buttons.CancelBtn);
        BindEvent(startReadyBtn.gameObject, OnStartReadyBtnClick);
        BindEvent(cancelBtn.gameObject, OnCancelBtnClick);

        statusText = Get<Text>((int)Texts.StatusText);
        playerListContent = Util.FindChild(gameObject, "Content", true);
    }

    public void SetStatusText(string text)
    {
        statusText.text = text;
    }

    public void SetPlayerInMatchRoom(UInt16 playerID, string playerName, byte playerIndex, byte totalPlayer)
    {
        if(playerIndex == totalPlayer)
        {
            // �÷��̾� ������ �ǹ�
            if (playerMap.ContainsKey(playerID))
            {
                PlayerBtn playerBtn = playerMap[playerID].Item1;
                byte orgnIndex = playerMap[playerID].Item2;
                if (playerBtns[orgnIndex] == playerBtn)
                {
                    playerBtns[orgnIndex] = null;
                }
                playerMap.Remove(playerID);
                Manager.Resource.Destroy(playerBtn.gameObject);
            }
            return;
        }

        if(!playerMap.ContainsKey(playerID))
        {
            // ���ο� �÷��̾� ����
            GameObject playerBtnObj = Manager.Resource.Instantiate("UI/PlayerBtn", playerListContent.transform);
            PlayerBtn playerBtn = playerBtnObj.GetComponent<PlayerBtn>();
            playerBtn.PlayerID = playerID;
            playerBtn.PlayerName = playerName;
            playerBtn.PlayerNameText.text = playerName;
            playerBtn.ReadyToggle.isOn = false;

            playerMap.Add(playerID, new Tuple<PlayerBtn, byte>(playerBtn, playerIndex));
            playerBtns[playerIndex] = playerBtn;

            // Reset Player List View
            ResetPlayerListView();
        }
        else
        {
            PlayerBtn playerBtn = playerMap[playerID].Item1;
            byte indexOrgn = playerMap[playerID].Item2;
            if (playerBtns[indexOrgn] != playerBtn)
            {
                // ������ �ڸ��� �P�� ���
                // - indexOrgn != playerIndex ���� ���ǹ� (�̹� orgn �ε����� �������� ����)
                // - indexOrgn �ڸ��� null�� ��ü�� �ʿ� ���� (�ٸ� �ڸ��� �̹� ������ ��ư ����)
                playerBtns[playerIndex] = playerBtn;
                playerMap[playerID] = new Tuple<PlayerBtn, byte>(playerBtn, playerIndex);

                // Reset Player List View
                ResetPlayerListView();
            }
            else
            {
                // .. x
                if(indexOrgn != playerIndex)
                {
                    // �ڸ� ����
                    playerBtns[indexOrgn] = null;
                    playerBtns[playerIndex] = playerBtn;
                    playerMap[playerID] = new Tuple<PlayerBtn, byte>(playerBtn, playerIndex);

                    // Reset Player List View
                    ResetPlayerListView();
                }
            }
        }
    }

    public void SetPlayerReady(UInt16 playerID, bool beReady)
    {
        foreach (var btn in playerBtns)
        {
            if (btn.PlayerID == playerID)
            {
                btn.ReadyToggle.isOn = beReady;    
                break;
            }
        }
    }

    private void OnStartReadyBtnClick(PointerEventData data)
    {
        StartReadyBtnClickHandler.Invoke();
    }

    private void OnCancelBtnClick(PointerEventData data)
    {
        CancelBtnClickHandler.Invoke();
    }

    private void ResetPlayerListView()
    {
        for(int i=0; i<playerBtns.Length; i++)
        {
            if(playerBtns[i] != null)
            {
                playerBtns[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
