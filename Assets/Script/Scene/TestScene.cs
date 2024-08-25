using TMPro;
using UnityEngine;


public class TestScene : MonoBehaviour
{
    bool InConnectedToServer = false;
    string PlayerName;
    string ServerIP = "127.0.0.1";
    string RoomName;
    string JoinRoomNum = string.Empty;

    bool InMatchRoom = false;
    bool InLobbyRoom = false;

    bool SendEnterMatchRoomMsg = false;

    bool ReadToStart = false;

    public bool ColliderMarkDebug = true;
    public bool JpsObstacleMarkDebug = true;

    private void Start()
    {
        if (ColliderMarkDebug || JpsObstacleMarkDebug)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/ColliderElement");
            //gameObj = Instantiate(prefab, position, Quaternion.LookRotation(dir));
            Manager.GamePlayer.SetColliderMarkObject(prefab);
        }

    }

    public void OnInputPlayerName()
    {
        PlayerName = GameObject.Find("Input_PlayerName").GetComponent<TMP_InputField>().text;
    }

    public void OnInputServerIP()
    {
        ServerIP = GameObject.Find("Input_GameServerIP").GetComponent<TMP_InputField>().text;
    }

    public void OnClickConnectBtn()
    {
        if(!Manager.Network.Connected)
        {
            if(Manager.Network.Connect(ServerIP))
            {
                InConnectedToServer = true;
                Manager.GamePlayer.GameServerIP = ServerIP;
            }
            else
            {
                //Debug.Log("Manager.Network.Connect(ServerIP) Fail.......");
                return;
            }
        }

        MSG_REQ_SET_PLAYER_NAME msg = new MSG_REQ_SET_PLAYER_NAME();
        msg.type = (ushort)enPacketType.REQ_SET_PLAYER_NAME;
        msg.playerName = PlayerName.ToCharArray();
        msg.playerNameLen = PlayerName.Length;

        if (!Manager.Network.SendPacket(msg))
        {
            //Debug.Log("Send MSG_REQ_SET_PLAYER_NAME fail..");
            return;
        }
        else
        {
            MSG_COM_REPLY reply = new MSG_COM_REPLY();
            if (!Manager.Network.ReceivePacket<MSG_COM_REPLY>(out reply))
            {
                //Debug.Log("Recv Rely SET_NAME fail..");
            }
            else
            {
                if (Manager.Network.CheckMsgType(reply.type, enPacketType.COM_REPLY, "[MSG_REQ_SET_PLAYER_NAME] reply.replyCode != (ushort)enPacketType.COM_REPLY"))
                {
                    if(Manager.Network.CheckReplyCode(reply.replyCode, enProtocolComReply.SET_PLAYER_NAME_SUCCESS, "reply.replyCode != (ushort)enProtocolComReply.SET_PLAYER_NAME_SUCCESS"))
                    {
                        //Debug.Log("Connectted to Server!!");
                    }
                }
            }
        }


        if(PlayerName == "admin")
        {
            Manager.DebugManager.DebugMode = true;   
        }
    }

    public void OnInputRoomName()
    {
        RoomName = GameObject.Find("Input_MatchRoom").GetComponent<TMP_InputField>().text;
    }

    public void OnClickCreateRoomBtn()
    {
        MSG_REQ_MAKE_ROOM msg = new MSG_REQ_MAKE_ROOM();
        msg.type = (ushort)enPacketType.REQ_MAKE_MATCH_ROOM;
        msg.roomName = RoomName.ToCharArray();  
        msg.roomNameLen = RoomName.Length;
        if (!Manager.Network.SendPacket(msg))
        {
            //Debug.Log("Send MSG_REQ_MAKE_ROOM fail..");
            return;
        }
        else
        {
            MSG_COM_REPLY reply = new MSG_COM_REPLY();
            if (!Manager.Network.ReceivePacket<MSG_COM_REPLY>(out reply))
            {
                //Debug.Log("Recv Reply MAKE_ROOM faill");
            }
            else
            {
                if (Manager.Network.CheckMsgType(reply.type, enPacketType.COM_REPLY, "��ġ�� ���� ��û ��ȯ �޽��� Ÿ�� ����ġ"))
                {
                    if(Manager.Network.CheckReplyCode(reply.replyCode, enProtocolComReply.MAKE_MATCH_ROOM_SUCCESS, "��ġ�� ���� ��û ��ȯ �޽��� ���� �ڵ� ����ġ"))
                    {
                        //Debug.Log("��ġ�� ���� �Ϸ�");
                        InMatchRoom = true;
                    }
                }
            }
        }
    }

    public void OnClickStart()
    {
        if(InMatchRoom && ReadToStart)
        {
            MSG_COM_REQUEST req = new MSG_COM_REQUEST();
            Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_GAME_START);
            if (!Manager.Network.SendPacket(req))
            {
                //Debug.Log("REQ_GAME_START �޽��� ���� ����");
            }
        }
    }
    public void OnClickReady()
    {

    }
    public void OnClickExit()
    {
        InMatchRoom = false;
        SendEnterMatchRoomMsg = false ;
        ReadToStart = false;
    }


    public void OnLobbyBtn()
    {
        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_ENTER_MATCH_LOBBY);
        if (!Manager.Network.SendPacket(req))
        {
            //Debug.Log("REQ_ENTER_MATCH_LOBBY �޽��� �۽� ����");
        }
        else
        {
            MSG_COM_REPLY reply = new MSG_COM_REPLY();
            if (!Manager.Network.ReceivePacket<MSG_COM_REPLY>(out reply))
            {
                //Debug.Log("REQ_ENTER_MATCH_LOBBY �޽��� ���� ����");
            }
            else
            {
                if (Manager.Network.CheckMsgType(reply.type, enPacketType.COM_REPLY, "REQ_ENTER_MATCH_LOBBY ���� �޽��� Ÿ�� ����ġ"))
                {
                    if (Manager.Network.CheckReplyCode(reply.replyCode, enProtocolComReply.ENTER_MATCH_LOBBY_SUCCESS, "REQ_ENTER_MATCH_LOBBY ���� �ڵ� ����"))
                    {
                        //Debug.Log("�κ� ���� ����");
                        InLobbyRoom = true;
                    }
                }
            }
        }
    }

    public void OnInputJoinRoom()
    {
        JoinRoomNum = GameObject.Find("Input_JoinRoom").GetComponent<TMP_InputField>().text;
    }

    public void OnClickJoinBtn()
    {
        if (InLobbyRoom && JoinRoomNum != string.Empty)
        {
            MSG_REQ_JOIN_ROOM req = new MSG_REQ_JOIN_ROOM();
            req.type = (ushort)enPacketType.REQ_JOIN_MATCH_ROOM;
            //req.roomID = JoinRoomNum.
            ushort.TryParse(JoinRoomNum, out req.roomID);

            if (!Manager.Network.SendPacket<MSG_REQ_JOIN_ROOM>(req))
            {
                //Debug.Log("Join Room �޽��� �۽� ����");
                return;
            }
        }
    }

    public void OnClickQuitBtn()
    {
        InLobbyRoom = false;
    }

    private void Update()
    {
        // ��ġ�� �� ó��
        if (InMatchRoom)
        {
            if(!SendEnterMatchRoomMsg)
            {
                MSG_COM_REQUEST req = new MSG_COM_REQUEST();    
                Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_ENTER_MATCH_ROOM);
                if (!Manager.Network.SendPacket(req))
                {
                    //Debug.Log("��ġ�� ���� Ȯ�� ��û �޽��� �۽� ����");
                }
                else
                {
                    MSG_COM_REPLY reply = new MSG_COM_REPLY();
                    if (!Manager.Network.ReceivePacket<MSG_COM_REPLY>(out reply, true, 100000000))
                    {
                        //Debug.Log("��ġ�� ���� Ȯ�� ��û �޽��� ���� ����");
                    }
                    else
                    {
                        if(Manager.Network.CheckMsgType(reply.type, enPacketType.COM_REPLY, "��ġ�� ���� Ȯ�� ��û �޽��� Ÿ�� ����ġ"))
                        {
                            if (Manager.Network.CheckReplyCode(reply.replyCode, enProtocolComReply.ENTER_MATCH_ROOM_SUCCESS, "��ġ�� ���� Ȯ�� ��û �޽��� Ÿ�� ����ġ"))
                            {
                                //Debug.Log("��ġ�� ���� Ȯ�� ����!");
                                SendEnterMatchRoomMsg = true;
                            }
                        }
                    }
                }
            }

            if(Manager.Network.ReceiveDataAvailable())
            {
                byte[] payload = Manager.Network.ReceivePacket();
                if (payload == null)
                {
                    //Debug.Log("��ġ�� �޽��� ���� ����");
                    return;
                }
                else
                {
                    enPacketType packetType = Manager.Network.GetMsgTypeInBytes(payload);
                    // �÷��̾� ����
                    if (packetType == enPacketType.SERVE_PLAYER_LIST)
                    {
                        MSG_SERVE_PLAYER_LIST msg = Manager.Network.BytesToMessage<MSG_SERVE_PLAYER_LIST>(payload);

                        string playerName = new string(msg.playerName);
                        string enterStr = "name: " + playerName + ", type: " + msg.playerType + ", order: " + msg.order;
                        //Debug.Log(enterStr);
                    }
                    // Ready or ReadToStart �޽��� ����
                    else if (packetType == enPacketType.SERVE_READY_TO_START)
                    {
                        MSG_SERVE_READY_TO_START msg = Manager.Network.BytesToMessage<MSG_SERVE_READY_TO_START>(payload);

                        if (msg.code == (ushort)enReadyToStartCode.Ready)
                        {
                            ushort playerID = msg.playerID;
                            string readyInfo = "" + playerID + ", Ready!";
                            //Debug.Log(readyInfo);
                        }
                        else if (msg.code == (ushort)enReadyToStartCode.ReadToStart)
                        {
                            //Debug.Log("Ready To Start!");
                            ReadToStart = true;
                        }
                    }
                    // ���� ���� �޽��� ����
                    else if (packetType == enPacketType.SERVE_BATTLE_START)
                    {
                        MSG_SERVE_BATTLE_START msg = Manager.Network.BytesToMessage<MSG_SERVE_BATTLE_START>(payload);

                        Manager.GamePlayer.m_Team = msg.Team;
                        Manager.GamePlayer.m_SelectorCnt = 1;
                        Manager.SceneTransfer.TransferToLoadingScene();
                    }
                    else
                    {
                        //Debug.Log("type:" + packetType);
                    }
                }
            }

        }
        else if (InLobbyRoom)
        {
            // ��ġ�� ����Ʈ �߰� �޽��� ����
            if (Manager.Network.ReceiveDataAvailable())
            {
                byte[] payload = Manager.Network.ReceivePacket();
                if (payload == null)
                {
                    //Debug.Log("��ġ�� �޽��� ���� ����");
                    return;
                }
                else
                {
                    enPacketType packetType = Manager.Network.GetMsgTypeInBytes(payload);
                    // �� ��� ����
                    if (packetType == enPacketType.SERVE_MATCH_ROOM_LIST)
                    {
                        MSG_SERVE_ROOM_LIST msg = Manager.Network.BytesToMessage<MSG_SERVE_ROOM_LIST>(payload);

                        string roomName = new string(msg.roomName);
                        string roomList = "roomName: " + roomName + ", roomID: " + msg.roomID + ", order: " + msg.order;
                        //Debug.Log(roomList);
                    }
                    else if(packetType == enPacketType.COM_REPLY)
                    {
                        MSG_COM_REPLY reply = Manager.Network.BytesToMessage<MSG_COM_REPLY>(payload);
                        if(reply.replyCode == (ushort)enProtocolComReply.JOIN_MATCH_ROOM_SUCCESS)
                        {
                            //Debug.Log("join ��ġ�� ����");

                            MSG_COM_REQUEST req = new MSG_COM_REQUEST();
                            Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_ENTER_MATCH_ROOM);
                            if (!Manager.Network.SendPacket(req))
                            {
                                //Debug.Log("��ġ�� ���� Ȯ�� ��û �޽��� �۽� ����");
                            }
                        }
                        else if(reply.replyCode == (ushort)enProtocolComReply.ENTER_MATCH_ROOM_SUCCESS)
                        {
                            //Debug.Log("��ġ�� ���� Ȯ�� ��û ���� Ȯ��");
                        }
                    }
                    // �÷��̾� ����
                    else if (packetType == enPacketType.SERVE_PLAYER_LIST)
                    {
                        MSG_SERVE_PLAYER_LIST msg = Manager.Network.BytesToMessage<MSG_SERVE_PLAYER_LIST>(payload);

                        string playerName = new string(msg.playerName);
                        string enterStr = "name: " + playerName + ", type: " + msg.playerType + ", order: " + msg.order;
                        //Debug.Log(enterStr);
                    }
                    // ���� ���� �޽���
                    else if (packetType == enPacketType.SERVE_BATTLE_START)
                    {
                        MSG_SERVE_BATTLE_START msg = Manager.Network.BytesToMessage<MSG_SERVE_BATTLE_START>(payload);

                        Manager.GamePlayer.m_Team = msg.Team;
                        Manager.GamePlayer.m_SelectorCnt = 1;
                        Manager.SceneTransfer.TransferToLoadingScene();
                    }
                    else
                    {
                        //Debug.Log("type:" + packetType);
                    }
                }
            }
        }
    }
}
