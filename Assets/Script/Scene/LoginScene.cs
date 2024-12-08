using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class LoginScene : BaseScene
{
    SimpleConnection loginServConn;
    LoginUI loginUI;

    protected override void Init()
    {
        base.Init();

        loginServConn = new SimpleConnection();

        //GamaManager.ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.REPLY_CODE, OnChatServerLoginReply);
        //ChattingManager.ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.REPLY_CODE, OnChatServerLoginReply);
        loginServConn.RegistPacketHandler((ushort)enPacketType_Login.REPLY_CREATE_ACCOUNT, OnCreateAccountReply);
        loginServConn.RegistPacketHandler((ushort)enPacketType_Login.REPLY_LOGIN, OnLoginReply);

        if(!loginServConn.Connect(PROTOCOL_LOGIN_CONSTANT.LOGIN_SERVER_IP, PROTOCOL_LOGIN_CONSTANT.LOGIN_SERVER_PORT))
        {
            Debug.Log("Login Server Connection Fail..");
            return;
        }

        GameObject loginUIObj = Manager.Resource.Instantiate("UI/LoginUI");
        if (loginUIObj != null)
        {
            loginUI = loginUIObj.GetComponent<LoginUI>();
            loginUI.LoginServerSetBtnHandler -= OnLoginServerSetBtnClicked;
            loginUI.LoginServerSetBtnHandler += OnLoginServerSetBtnClicked;
            loginUI.CreateBtnHandler -= OnCreateAccountBtnClicked;
            loginUI.CreateBtnHandler += OnCreateAccountBtnClicked;
            loginUI.LoginBtnHandler -= OnLoginBtnClicked;
            loginUI.LoginBtnHandler += OnLoginBtnClicked; 
        }
    }

    public override void Clear()
    {
        loginServConn.Clear();
    }

    private void Update()
    {
        if(loginServConn != null)
        {
            loginServConn.Update();
        }
    }

    public void OnLoginServerSetBtnClicked(string serverIP, UInt16 serverPort)
    {
        if(loginServConn.Connect(serverIP, serverPort))
        {
            loginUI.ResetStatusText("OK");
        }
        else
        {
            loginUI.ResetStatusText("FAIL");
        }
    }

    public void OnCreateAccountBtnClicked(string accountID, string password) 
    {
        MSG_AUTH_REQUEST_CREATE_ACCOUNT msg = new MSG_AUTH_REQUEST_CREATE_ACCOUNT();
        msg.type = (ushort)enPacketType_Login.REQ_CREATE_ACCOUNT;
        msg.accountId = Encoding.Unicode.GetBytes(accountID);
        msg.accountIdLen = msg.accountId.Length;
        msg.accountPassword = Encoding.Unicode.GetBytes(password);   
        msg.accountPasswordLen = msg.accountPassword.Length;

        loginServConn.Send<MSG_AUTH_REQUEST_CREATE_ACCOUNT>(msg, true);
    }

    public void OnLoginBtnClicked(string accountID, string password) 
    {
        MSG_AUTH_REQUEST_LOGIN msg = new MSG_AUTH_REQUEST_LOGIN();
        msg.type = (ushort)enPacketType_Login.REQ_LOGIN;
        msg.accountId = Encoding.Unicode.GetBytes(accountID);
        msg.accountIdLen = msg.accountId.Length;
        msg.accountPassword = Encoding.Unicode.GetBytes(password);
        msg.accountPasswordLen = msg.accountPassword.Length;

        GamaManager.Instance.AccountID = accountID;

        loginServConn.Send<MSG_AUTH_REQUEST_LOGIN>(msg, true);
    }

    public void OnCreateAccountReply(byte[] payload)
    {
        MSG_AUTH_REPLY_CREATE_ACCOUNT reply = loginServConn.BytesToMessage<MSG_AUTH_REPLY_CREATE_ACCOUNT>(payload);
        if (reply.replyCode == (ushort)enReplyCode_Login.CRETAE_ACCOUNT_SUCCESS)
        {
            loginUI.ResetStatusText("SUCESS: Create a New Account!");
        }
        else if (reply.replyCode == (ushort)enReplyCode_Login.CRETAE_ACCOUNT_FAILURE)
        {
            loginUI.ResetStatusText("FAIL: FAILED TO CREATE a ACCOUNT");
        }
        else
        {
            loginUI.ResetStatusText("OnCreateAccountReply, invalid reply code");
        }
    }

    public void OnLoginReply(byte[] payload)
    {
        MSG_AUTH_REPLY_LOGIN reply = loginServConn.BytesToMessage<MSG_AUTH_REPLY_LOGIN>(payload);
        if (reply.replyCode == (ushort)enReplyCode_Login.LOGIN_SUCCESS)
        {
            loginUI.ResetStatusText("LOGIN SUCESS!");

            GamaManager.Instance.AccountToken = reply.token;
            GamaManager.Instance.AccountNo = reply.accountNo;

            // 채팅 서버 연결
            if(!ChattingManager.Instance.Connect())
            {
                Debug.Log("채팅 서버 접속 실패");
                return;
            }
            ChattingManager.Instance.Login(reply.accountNo, reply.token, reply.tokenLength, GamaManager.Instance.AccountID, OnChatServerLoginReply);
        }
        else if (reply.replyCode == (ushort)enReplyCode_Login.LOGIN_FAILURE)
        {
            loginUI.ResetStatusText("LOGIN FAILED!");
        }
        else
        {
            loginUI.ResetStatusText("OnLoginReply, invalid reply code");
        }
    }

    public void OnChatServerLoginReply()
    {
        Manager.Scene.Clear();
        Manager.Scene.LoadScene(Define.Scene.HubScene);
    }
}