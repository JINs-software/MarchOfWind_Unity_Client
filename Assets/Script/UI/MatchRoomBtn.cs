using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchRoomBtn : MonoBehaviour
{
    public Action<UInt16> ClickHandler;
    public UInt16 MatchRoomID;
    public string MatchRoomName;
    public Text RoomNameTxt;
    public Text PlayerNameTxt;

    public void OnClicked()
    {
        ClickHandler.Invoke(MatchRoomID);
    }
}
