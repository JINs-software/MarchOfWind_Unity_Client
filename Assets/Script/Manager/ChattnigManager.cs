
using System.Text;
using System;
using UnityEngine;

public class ChattingManager : MonoBehaviour
{
    static ChattingManager s_Instance;
    public static ChattingManager Instance { get { init(); return s_Instance; } }

    SimpleConnection m_ChatServerConn = new SimpleConnection();
    public static SimpleConnection ChatServerConn { get { return Instance.m_ChatServerConn; } }

    ChattingUI m_ChattingUI;
    Action LoginSuccessCallback;

    private static void init()
    {
        if(s_Instance == null)
        {
            GameObject go = GameObject.Find("@ChatManager");
            if(go == null)
            {
                go = new GameObject { name = "@ChatManager" };
                go.AddComponent<ChattingManager>();
            }
            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<ChattingManager>();

            if(s_Instance.m_ChatServerConn == null)
            {
                s_Instance.m_ChatServerConn = new SimpleConnection();
                //s_Instance.m_ChatServerConn.Connect(PROTOCOL_CHAT_CONSTANT.CHAT_SERVER_IP, PROTOCOL_CHAT_CONSTANT.CHAT_SERVER_PORT);
            }
        }
    }

    private void OnApplicationQuit()
    {
        ChatServerConn.Clear();
    }

    private void Update()
    {
        if(s_Instance.m_ChatServerConn != null)
        {
            s_Instance.m_ChatServerConn.Update();
        }
    }

    public bool Connect()
    {
        bool ret = false;
        if(s_Instance.m_ChatServerConn.Connect(PROTOCOL_CHAT_CONSTANT.CHAT_SERVER_IP, PROTOCOL_CHAT_CONSTANT.CHAT_SERVER_PORT))
        {
            ret = true;
            s_Instance.m_ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.REPLY_CODE, OnReplyCode);
            s_Instance.m_ChatServerConn.RegistPacketHandler((ushort)enPacketType_Chat.RECV_CHAT_MSG, OnRecvChatMsg);
        }

        return ret;
    }

    public void Login(ushort accountNo, byte[] token, int tokenLen, string accountID,  Action callback)
    {
        LoginSuccessCallback += callback;

        MSG_REQ_LOGIN_CHAT login = new MSG_REQ_LOGIN_CHAT();
        login.type = (ushort)enPacketType_Chat.REQ_LOGIN;
        login.accountNo = accountNo;
        login.token = token;
        login.tokenLength = tokenLen;

        // test
        byte[] accountIDBytes = Encoding.Unicode.GetBytes(accountID);
        login.accountID = accountIDBytes;
        login.accountLength = accountIDBytes.Length;
        
        ChatServerConn.Send<MSG_REQ_LOGIN_CHAT>(login, true);
    }

    public void EnterMatch(ushort matchID)
    {
        MSG_REQ_ENTER_MATCH enterMsg = new MSG_REQ_ENTER_MATCH();
        enterMsg.type = (ushort)enPacketType_Chat.REQ_ENTER_MATCH;
        enterMsg.roomID = matchID;

        ChatServerConn.Send<MSG_REQ_ENTER_MATCH>(enterMsg, true);
    }

    public static void RegistUI(ChattingUI chattingUI)
    {
        s_Instance.m_ChattingUI = chattingUI;
        s_Instance.m_ChattingUI.ChatHandler -= OnInputChat;
        s_Instance.m_ChattingUI.ChatHandler += OnInputChat;
    }
    public static void DeRegistUI(ChattingUI chattingUI)
    {
        if (s_Instance.m_ChattingUI == chattingUI)
        {
            s_Instance.m_ChattingUI = null;
        }
    }

    private static void OnInputChat(string chat)
    {
        MSG_SEND_CHAT_MSG chatMsg = new MSG_SEND_CHAT_MSG();
        chatMsg.type = (ushort)enPacketType_Chat.SEND_CHAT_MSG;
        chatMsg.chat = Encoding.Unicode.GetBytes(chat);
        chatMsg.chatLength = chatMsg.chat.Length;

        ChatServerConn.Send<MSG_SEND_CHAT_MSG>(chatMsg, true);
    }

    private static void OnReplyCode(byte[] payload)
    {
        MSG_REPLY_CODE_CHAT reply = ChatServerConn.BytesToMessage<MSG_REPLY_CODE_CHAT>(payload);

        if (reply.replyCode == (ushort)enReplyCode_Chat.LOGIN_SUCCESS)
        {
            s_Instance.LoginSuccessCallback?.Invoke();
        }

    }

    private static void OnRecvChatMsg(byte[] payload)
    {
        MSG_RECV_CHAT_MSG recvChat = ChatServerConn.BytesToMessage<MSG_RECV_CHAT_MSG>(payload);
        byte[] trimChat = new byte[recvChat.chatLength];
        Array.Copy(recvChat.chat, trimChat, recvChat.chatLength);

        byte[] trimAccountID = new byte[recvChat.accountLength];
        Array.Copy(recvChat.accountID, trimAccountID, recvChat.accountLength);  

        string accountID = Encoding.Unicode.GetString(trimAccountID);
        string chat = Encoding.Unicode.GetString(trimChat);
        if (s_Instance.m_ChattingUI != null)
        {
            s_Instance.m_ChattingUI.DisplayChat(accountID, chat);
        }
    }
}