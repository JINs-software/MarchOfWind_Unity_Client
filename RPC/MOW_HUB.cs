
using System;
using System.Text;

public class MOW_HUB : Stub_MOW_HUB
{
    private void Start() 
    {
        base.Init();
    }

    private void OnDestroy()
    {
        base.Clear();  
    }


    protected override void CONNECTION_REPLY(byte REPLY_CODE, UInt16 PLAYER_ID) 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();
        hubScene.PlayerID = PLAYER_ID;
        hubScene.initUI.OnReceiveConnectReply(REPLY_CODE);
    }

    protected override void CREATE_MATCH_ROOM_REPLY(byte REPLY_CODE, UInt16 MATCH_ROOM_ID) 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();
        hubScene.RoomID = MATCH_ROOM_ID;    
        
        switch((enCREATE_MATCH_ROOM_REPLY_CODE)REPLY_CODE)
        {
            case enCREATE_MATCH_ROOM_REPLY_CODE.SUCCESS:
                {
                    hubScene.OnReceiveCreateRoomSuccess();
                }
                break;
            case enCREATE_MATCH_ROOM_REPLY_CODE.MATCH_ROOM_CAPACITY_EXCEEDED:
                {
                    if(hubScene.createMatchUI != null)
                    {
                        hubScene.createMatchUI.SetOnlyCancelBtn();
                        hubScene.createMatchUI.SetStatusText("SERVER: MATCH_ROOM_CAPACITY_EXCEEDED");
                    }
                }
                break;
            case enCREATE_MATCH_ROOM_REPLY_CODE.INVALID_MSG_FIELD_VALUE:
                {
                    if(hubScene.createMatchUI != null)
                    {
                        hubScene.createMatchUI.ResetUI();
                        hubScene.createMatchUI.SetStatusText("SERVER: INVALID_MSG_FIELD_VALUE");
                    }
                }
                break;
            case enCREATE_MATCH_ROOM_REPLY_CODE.MATCH_ROOM_NAME_ALREADY_EXIXTS:
                {
                    if(hubScene.createMatchUI != null)
                    {
                        hubScene.createMatchUI.ResetUI();
                        hubScene.createMatchUI.SetStatusText("SERVER: MATCH_ROOM_NAME_ALREADY_EXIXTS");
                    }
                }
                break;
        }
    }

    protected override void MATCH_ROOM_LIST(UInt16 MATCH_ROOM_ID, byte[] MATCH_ROOM_NAME, byte LENGTH, UInt16 MATCH_ROOM_INDEX, UInt16 TOTAL_MATCH_ROOM) 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();

        if(hubScene.lobbyUI != null)
        {
            string matchRoomName = Encoding.ASCII.GetString(MATCH_ROOM_NAME, 0, LENGTH);
            hubScene.lobbyUI.SetMatchRoomInLobby(MATCH_ROOM_ID, matchRoomName, MATCH_ROOM_INDEX, TOTAL_MATCH_ROOM);
        }
    }

    protected override void JOIN_TO_MATCH_ROOM_REPLY(byte REPLY_CODE) 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();

        switch ((enJOIN_TO_MATCH_ROOM_REPLY_CODE)REPLY_CODE)
        {
            case enJOIN_TO_MATCH_ROOM_REPLY_CODE.SUCCESS:
                {
                    hubScene.OnReceiveJoinRoomSuccess();
                }
                break;
            case enJOIN_TO_MATCH_ROOM_REPLY_CODE.INVALID_MATCH_ROOM_ID:
                {
                    if(hubScene.lobbyUI != null)
                    {
                        hubScene.lobbyUI.SetStatusText("SERVER: INVALID_MATCH_ROOM_ID");
                        hubScene.lobbyUI.ResetUI();
                    }
                }
                break;
            case enJOIN_TO_MATCH_ROOM_REPLY_CODE.PLAYER_CAPACITY_IN_ROOM_EXCEEDED:
                {
                    if (hubScene.lobbyUI != null)
                    {
                        hubScene.lobbyUI.SetStatusText("SERVER: PLAYER_CAPACITY_IN_ROOM_EXCEEDED");
                        hubScene.lobbyUI.ResetUI();
                    }
                }
                break;
        }
    }

    protected override void MATCH_PLAYER_LIST(UInt16 PLAYER_ID, byte[] MATCH_PLAYER_NAME, byte LENGTH, byte MATCH_PLAYER_INDEX, byte TOTAL_MATCH_PLAYER) 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();
        if (hubScene != null)
        {

            if (hubScene.matchRoomUI != null)
            {
                string playerName = Encoding.ASCII.GetString(MATCH_PLAYER_NAME, 0, LENGTH);
                hubScene.matchRoomUI.SetPlayerInMatchRoom(PLAYER_ID, playerName, MATCH_PLAYER_INDEX, TOTAL_MATCH_PLAYER);

                if (MATCH_PLAYER_INDEX == 0)
                {
                    if (hubScene.PlayerID == PLAYER_ID)
                    {
                        hubScene.IsHost = true;
                    }
                    else
                    {
                        hubScene.IsHost = false;
                    }
                }
            }
        }
    }

    protected override void MATCH_START_REPLY(byte REPLY_CODE) 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();

        switch ((enMATCH_START_REPLY_CODE)REPLY_CODE)
        {
            case enMATCH_START_REPLY_CODE.SUCCESS:
                {
                    // °ÔÀÓ ½ÃÀÛ~!
                }
                break;
            case enMATCH_START_REPLY_CODE.NOT_FOUND_IN_MATCH_ROOM:
                {
                    if (hubScene.matchRoomUI != null)
                    {
                        hubScene.matchRoomUI.SetStatusText("SERVER: NOT_FOUND_IN_MATCH_ROOM");
                    }
                }
                break;
            case enMATCH_START_REPLY_CODE.NO_HOST_PRIVILEGES:
                {
                    if (hubScene.matchRoomUI != null)
                    {
                        hubScene.matchRoomUI.SetStatusText("SERVER: NO_HOST_PRIVILEGES");
                    }
                }
                break;
            case enMATCH_START_REPLY_CODE.UNREADY_PLAYER_PRESENT:
                {
                    if (hubScene.matchRoomUI != null)
                    {
                        hubScene.matchRoomUI.SetStatusText("SERVER: NO_HOST_PRIVILEGES");
                    }
                }
                break;
        }
    }

    protected override void MATCH_READY_REPLY(UInt16 PLAYER_ID) 
    {
        throw new NotImplementedException("MATCH_READY_REPLY");
    }

    protected override void LAUNCH_MATCH() 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();
        if (hubScene.matchRoomUI != null)
        {
            hubScene.OnReceiveLaunchMatch();
        }
    }

}
