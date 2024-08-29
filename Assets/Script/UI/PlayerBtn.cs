using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBtn : MonoBehaviour
{
    [SerializeField]
    public Text PlayerNameText;
    [SerializeField]
    public Toggle ReadyToggle;

    public UInt16 PlayerID;
    public string PlayerName;
}
