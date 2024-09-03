using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyUI : UI_Base
{
    const UInt16 MAX_OF_MATCH_ROOMS = 300;
    enum Buttons
    {
        CancelBtn,
    }

    enum Texts
    {
        StatusText,
    }

    public Action<UInt16> MatchRoomBtnClickHandler;
    public Action CancelBtnHandler;
    Button cancelBtn;
    Text statusText;
    GameObject matchRoomContent;

    Dictionary<UInt16, Tuple<MatchRoomBtn, UInt16>> matchRoomMap = new Dictionary<ushort, Tuple<MatchRoomBtn, ushort>>();
    //List<MatchRoomBtn> matchRoomBtns = new List<MatchRoomBtn>();
    MatchRoomBtn[] matchRoomBtns = new MatchRoomBtn[MAX_OF_MATCH_ROOMS];

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        cancelBtn = GetButton((int)Buttons.CancelBtn);
        BindEvent(cancelBtn.gameObject, OnCancelBtnClicked);

        statusText = Get<Text>((int)Texts.StatusText);
        matchRoomContent = Util.FindChild(gameObject, "Content", true);
    }

    public void SetOnlyCancelBtn()
    {
        foreach (var btn in matchRoomBtns)
        {
            if(btn != null)
            {
                btn.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ResetUI()
    {
        foreach (var btn in matchRoomBtns)
        {
            btn.gameObject.GetComponent<Button>().interactable = true;
        }
    }

    public void SetStatusText(string text)
    {
        statusText.text = text;
    }

    public void SetMatchRoomInLobby(UInt16 matchRoomID, string matchRoomName, UInt16 roomIndex, UInt16 totalRoom)
    {
        if(matchRoomContent == null)
        {
            matchRoomContent = Util.FindChild(gameObject, "Content", true);
        }

        if(roomIndex == totalRoom)
        {
            if(matchRoomMap.ContainsKey(matchRoomID))
            {
                MatchRoomBtn matchRoomBtn = matchRoomMap[matchRoomID].Item1;
                UInt16 orgnIndex = matchRoomMap[matchRoomID].Item2;
                if (matchRoomBtns[orgnIndex] == matchRoomBtn)
                {
                    matchRoomBtns[orgnIndex] = null;
                }
                matchRoomMap.Remove(matchRoomID);
                Manager.Resource.Destroy(matchRoomBtn.gameObject);
            }
        }

        if (!matchRoomMap.ContainsKey(matchRoomID))
        {
            GameObject matchRoomBtnObj = Manager.Resource.Instantiate("UI/MatchRoomBtn", matchRoomContent.transform);
            MatchRoomBtn matchRoomBtn = matchRoomBtnObj.GetComponent<MatchRoomBtn>();
            matchRoomBtn.MatchRoomID = matchRoomID;
            matchRoomBtn.MatchRoomName = matchRoomName;
            matchRoomBtn.RoomNameTxt.text = matchRoomName;

            matchRoomBtn.ClickHandler -= OnMatchRoomBtnClick;
            matchRoomBtn.ClickHandler += OnMatchRoomBtnClick;

            matchRoomMap.Add(matchRoomID, new Tuple<MatchRoomBtn, UInt16>(matchRoomBtn, roomIndex));
            matchRoomBtns[roomIndex] = matchRoomBtn;

            ResetMatchRoomListView();
        }
        else
        {
            MatchRoomBtn matchRoomBtn = matchRoomMap[matchRoomID].Item1;
            UInt16 indexOrgn = matchRoomMap[matchRoomID].Item2;
            if (matchRoomBtns[indexOrgn] != matchRoomBtn)
            {
                // 이전에 자리를 뻇긴 경우
                // - indexOrgn != playerIndex 조건 무의미 (이미 orgn 인덱스에 존재하지 않음)
                // - indexOrgn 자리에 null을 대체할 필요 없음 (다른 자리를 이미 차지한 버튼 유지)
                matchRoomBtns[roomIndex] = matchRoomBtn;
                matchRoomMap[matchRoomID] = new Tuple<MatchRoomBtn, UInt16>(matchRoomBtn, roomIndex);

                ResetMatchRoomListView();
            }
            else
            {
                // .. x
                if (indexOrgn != roomIndex)
                {
                    // 자리 변경
                    matchRoomBtns[indexOrgn] = null;
                    matchRoomBtns[roomIndex] = matchRoomBtn;
                    matchRoomMap[matchRoomID] = new Tuple<MatchRoomBtn, UInt16>(matchRoomBtn, roomIndex);

                    ResetMatchRoomListView();
                }
            }
        }
    }

    private void ResetMatchRoomListView()
    {
        for (int i = 0; i < matchRoomBtns.Length; i++)
        {
            if (matchRoomBtns[i] != null)
            {
                matchRoomBtns[i].transform.SetSiblingIndex(i);
            }
        }
    }

    public void OnMatchRoomBtnClick(UInt16 matchRoomID)
    {
        MatchRoomBtnClickHandler.Invoke(matchRoomID);
    }

    private void OnCancelBtnClicked(PointerEventData data)
    {
        CancelBtnHandler.Invoke();
    }
}
