
using System;
using System.Text;
using UnityEngine;

/*
public class ChattingTest : BaseScene
{
    ChattingUI chatUI;

    protected override void Init()
    {
        base.Init();

        //GamaManager.ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.REPLY_CODE, OnReplyCcde);
        //GamaManager.ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.RECV_CHAT_MSG, OnRecvChatMsg);
        //GamaManager.ChatServerConn.Connect("127.0.0.1", 12130);
        ChattingManager.ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.REPLY_CODE, OnReplyCcde);
        ChattingManager.ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.RECV_CHAT_MSG, OnRecvChatMsg);

        GameObject chatUIObj = Manager.Resource.Instantiate("UI/ChattingUI");
        if( chatUIObj != null )
        {
            chatUI = chatUIObj.GetComponent<ChattingUI>();
            chatUI.ChatHandler -= OnChattingInput;
            chatUI.ChatHandler += OnChattingInput;
        }
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }

    private void Start()
    {
        // 테스트 코드
        // 1) 채팅 서버 로그인
        MSG_REQ_LOGIN_CHAT   login = new MSG_REQ_LOGIN_CHAT();
        login.type = (ushort)enPacketType_Chat.REQ_LOGIN;
        login.accountNo = 100;
        string tokenStr = "12345";
        login.token = Encoding.Unicode.GetBytes(tokenStr);
        ChattingManager.ChatServerConn.Send<MSG_REQ_LOGIN_CHAT>(login, true);
    }

    private void OnChattingInput(string chat)
    {
        MSG_SEND_CHAT_MSG chatMsg = new MSG_SEND_CHAT_MSG();
        chatMsg.type = (ushort)enPacketType_Chat.SEND_CHAT_MSG;
        chatMsg.chat = Encoding.Unicode.GetBytes(chat);
        chatMsg.chatLength = chatMsg.chat.Length;

        ChattingManager.ChatServerConn.Send<MSG_SEND_CHAT_MSG>(chatMsg, true);
    }

    private void OnReplyCcde(byte[] payload)
    {
        MSG_REPLY_CODE_CHAT reply = ChattingManager.ChatServerConn.BytesToMessage<MSG_REPLY_CODE_CHAT>(payload);

        if(reply.replyCode == (ushort)enReplyCode_Chat.LOGIN_SUCCESS)
        {
            MSG_REQ_ENTER_MATCH enterMsg = new MSG_REQ_ENTER_MATCH();   
            enterMsg.type = (ushort)enPacketType_Chat.REQ_ENTER_MATCH;
            enterMsg.roomID = (ushort)1000;

            ChattingManager.ChatServerConn.Send<MSG_REQ_ENTER_MATCH>(enterMsg, true);   
        }

        //chatUI.BeInteractable();
    }

    private void OnRecvChatMsg(byte[] payload)
    {
        MSG_RECV_CHAT_MSG recvChat = ChattingManager.ChatServerConn.BytesToMessage<MSG_RECV_CHAT_MSG>(payload);
        byte[] trimChat = new byte[recvChat.chatLength];
        Array.Copy(recvChat.chat, trimChat, recvChat.chatLength);   
        string chat = Encoding.Unicode.GetString(trimChat);
        chatUI.DisplayChat(recvChat.accountNo.ToString(), chat);
    }
}
*/

public class ChattingTest : BaseScene
{
    protected override void Init()
    {
        base.Init();

        ChattingManager.Instance.Connect();
        string tokenStr = "12345";
        byte[] token = Encoding.Unicode.GetBytes(tokenStr);
        //ChattingManager.Instance.Login(12345, token, token.Length, OnLogin);

    }
    public override void Clear()
    {
        throw new NotImplementedException();
    }

    void OnLogin()
    {
        Debug.Log("로그인 성공!");
    }
}