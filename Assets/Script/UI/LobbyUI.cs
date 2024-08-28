using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyUI : UI_Base
{
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

    private List<MatchRoomBtn> matchRoomBtns = new List<MatchRoomBtn>();

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
            btn.gameObject.GetComponent<Button>().interactable = false;
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

    public void RegistMatchRoom(UInt16 matchRoomID, string matchRoomName, UInt16 roomIndex, UInt16 totalRoom)
    {
        GameObject matchRoomBtnObj = Manager.Resource.Instantiate("UI/MatchRoomBtn", matchRoomContent.transform);
        MatchRoomBtn matchRoomBtn = matchRoomBtnObj.GetComponent<MatchRoomBtn>();
        matchRoomBtn.MatchRoomID = matchRoomID;
        matchRoomBtn.MatchRoomName = matchRoomName;
        matchRoomBtn.ClickHandler -= OnMatchRoomBtnClick;
        matchRoomBtn.ClickHandler += OnMatchRoomBtnClick;
        matchRoomBtns.Add(matchRoomBtn);
    }

    public void DeleteMatchRoom(UInt16 matchRoomID)
    {
        foreach(var btn in matchRoomBtns)
        {
            if(btn.MatchRoomID == matchRoomID)
            {
                matchRoomBtns.Remove(btn);
                Manager.Resource.Destroy(btn.gameObject);
                break;
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
