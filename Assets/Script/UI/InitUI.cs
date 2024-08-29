using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InitUI : UI_Base
{
    enum InputFields
    {
        ServerIpInput,
        ServerPortInput,
        PlayerNameInput,
    }

    enum Buttons
    {
        ConnectBtn,
        CreateBtn,
        JoinBtn,
        SettingBtn,
        QuitBtn,
    }

    enum Texts
    {
        StatusText,
    }

    public Action CreateBtnHandler;
    public Action JoinBtnHandler;
    public Action SettingBtnHandler;
    public Action QuitBtnHandler;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));  

        Button connectBtn = GetButton((int)Buttons.ConnectBtn);
        Button createBtn = GetButton((int)Buttons.CreateBtn);
        Button joinBtn = GetButton((int)Buttons.JoinBtn);
        Button settingBtn = GetButton((int)Buttons.SettingBtn);
        Button quitBtn = GetButton((int)Buttons.QuitBtn);

        createBtn.interactable = false;
        joinBtn.interactable = false;

        BindEvent(connectBtn.gameObject, OnConnectBtnClicked);
        BindEvent(createBtn.gameObject, OnCreateBtnClicked);
        BindEvent(joinBtn.gameObject, OnJoinBtnClicked);
        BindEvent(settingBtn.gameObject, OnSettingBtnClicked);
        BindEvent(quitBtn.gameObject, OnQuitBtnClicked);
    }

    public void OnReceiveConnectReply(Byte reply)
    {
       switch ((enCONNECTION_REPLY_CODE)reply)
        {
            case enCONNECTION_REPLY_CODE.SUCCESS:
                {
                    Get<Text>((int)Texts.StatusText).text = "Connetion Completed!";
                    Get<InputField>((int)InputFields.ServerIpInput).interactable = false;
                    Get<InputField>((int)InputFields.ServerPortInput).interactable = false;
                    Get<InputField>((int)InputFields.PlayerNameInput).interactable = false;
                    GetButton((int)Buttons.ConnectBtn).interactable = false;    

                    GetButton((int)Buttons.CreateBtn).interactable = true;
                    GetButton((int)Buttons.JoinBtn).interactable = true;
                }
                break;
            case enCONNECTION_REPLY_CODE.PLAYER_CAPACITY_EXCEEDED:
                {
                    // Quit 버튼만 활성화 
                    Get<Text>((int)Texts.StatusText).text = "SERVER: PLAYER_CAPACITY_EXCEEDED!";
                    GetButton((int)Buttons.SettingBtn).interactable = true;
                }
                break;
            case enCONNECTION_REPLY_CODE.INVALID_MSG_FIELD_VALUE:
                {
                    Get<Text>((int)Texts.StatusText).text = "SERVER: INVALID_MSG_FIELD_VALUE!";
                    GetButton((int)Buttons.ConnectBtn).interactable = true;
                    Get<InputField>((int)InputFields.PlayerNameInput).interactable = true;  
                }
                break;
            case enCONNECTION_REPLY_CODE.PLAYER_NAME_ALREADY_EXIXTS:
                {
                    Get<Text>((int)Texts.StatusText).text = "SERVER: PLAYER_NAME_ALREADY_EXIXTS!";
                    GetButton((int)Buttons.ConnectBtn).interactable = true;
                    Get<InputField>((int)InputFields.PlayerNameInput).interactable = true;
                }
                break;
            default:
                {
                    Get<Text>((int)Texts.StatusText).text = "SERVER ERR: INVALID REPLY CODE!";
                    GetButton((int)Buttons.SettingBtn).interactable = true;
                }
                break;
        }
    }

    private void OnConnectBtnClicked(PointerEventData data)
    {
        Button connBtn = GetButton((int)Buttons.ConnectBtn);
        InputField serverIpInput = Get<InputField>((int)InputFields.ServerIpInput);
        InputField serverPortInput = Get<InputField>((int)InputFields.ServerPortInput);
        InputField playerNameInput = Get<InputField>((int)InputFields.PlayerNameInput);

        IPAddress ip;
        if(!IPAddress.TryParse(serverIpInput.text, out ip))
        {
            // 올바르지 않은 IP 주소
            serverIpInput.text = "올바르지 않은 IP 주소!!"; 
            return;
        }

        if(string.IsNullOrEmpty(serverPortInput.text))
        {
            // 빈 port 입력 창
            serverPortInput.text = "Port 번호 입력!!";
            return;
        }

        if(string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.text = "플레이어 이름 입력!!";
            return;
        }

        if (!RPC.Network.Connected)
        {
            // 새로운 연결 요청
            UInt16 port = UInt16.Parse(serverPortInput.text);
            if (!RPC.Instance.Initiate(serverIpInput.text, port)) 
            {
                serverIpInput.text = "";
                serverPortInput.text = "";

                Debug.Log("서버 접속 실패");
                return; 
            }
            else
            {
                serverIpInput.interactable = false;
                serverPortInput.interactable = false;
            }
        }

        // Player 이름 전송
        string playerName = playerNameInput.text;
        byte[] playerNameBytes = Encoding.ASCII.GetBytes(playerName);
        RPC.proxy.CONNECTION(playerNameBytes, (Byte)playerName.Length);

        playerNameInput.interactable = false;
        connBtn.interactable = false;
    }

    private void OnCreateBtnClicked(PointerEventData data)
    {
        CreateBtnHandler.Invoke();
    }

    private void OnJoinBtnClicked(PointerEventData data)
    {
        JoinBtnHandler.Invoke();
    }

    private void OnSettingBtnClicked(PointerEventData data)
    {
        SettingBtnHandler.Invoke();
    }

    private void OnQuitBtnClicked(PointerEventData data)
    {
        QuitBtnHandler.Invoke();
    }
}
