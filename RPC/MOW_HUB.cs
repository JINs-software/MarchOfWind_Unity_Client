
using System;
using UnityEditor;

public class MOW_HUB : Stub_MOW_HUB
{
    private void Start() 
    {
        base.Init();
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

    protected override void MATCH_ROOM_LIST(UInt16 MATCH_ROOM_ID, char[] MATCH_ROOM_NAME, byte LENGTH, UInt16 MATCH_ROOM_INDEX, UInt16 TOTAL_MATCH_ROOM) 
    {
        HubScene hubScene = gameObject.GetComponent<HubScene>();

        if(hubScene.lobbyUI != null)
        {
            hubScene.lobbyUI.RegistMatchRoom(MATCH_ROOM_ID, new string(MATCH_ROOM_NAME, 0, LENGTH), MATCH_ROOM_INDEX, TOTAL_MATCH_ROOM);
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

    protected override void MATCH_PLAYER_LIST(UInt16 PLAYER_ID, char[] MATCH_PLAYER_NAME, byte LENGTH, byte MATCH_PLAYER_INDEX, byte TOTAL_MATCH_PLAYER) 
    {
        throw new NotImplementedException("MATCH_PLAYER_LIST");
    }

    protected override void MATCH_START_REPLY(byte REPLY_CODE) 
    {
        throw new NotImplementedException("MATCH_START_REPLY");
    }

    protected override void CHANGE_MATCH_HOST(UInt16 HOST_PLAYER_ID) 
    {
        throw new NotImplementedException("CHANGE_MATCH_HOST");
    }

}
