
using System;

public class MOW_HUB_TEST : MOW_HUB
{
    private void Start()
    {
        base.Start();
    }
    private void OnDestroy()
    {
        base.Clear();
    }

    protected override void CONNECTION_REPLY(byte REPLY_CODE, UInt16 PLAYER_ID)
    {
        TestManager.Instance.PROC_CONNECTION_REPLY(REPLY_CODE, PLAYER_ID);  
    }

    protected override void CREATE_MATCH_ROOM_REPLY(byte REPLY_CODE, UInt16 MATCH_ROOM_ID)
    {
        TestManager.Instance.PROC_CREATE_MATCH_ROOM_REPLY(REPLY_CODE, MATCH_ROOM_ID);
    }

    protected override void MATCH_ROOM_LIST(UInt16 MATCH_ROOM_ID, byte[] MATCH_ROOM_NAME, byte LENGTH, UInt16 MATCH_ROOM_INDEX, UInt16 TOTAL_MATCH_ROOM)
    {
    }

    protected override void JOIN_TO_MATCH_ROOM_REPLY(byte REPLY_CODE)
    {
    }

    protected override void MATCH_PLAYER_LIST(UInt16 PLAYER_ID, byte[] MATCH_PLAYER_NAME, byte LENGTH, byte MATCH_PLAYER_INDEX, byte TOTAL_MATCH_PLAYER)
    {
    }

    protected override void MATCH_START_REPLY(byte REPLY_CODE)
    {
    }

    protected override void MATCH_READY_REPLY(UInt16 PLAYER_ID)
    {
    }

    protected override void LAUNCH_MATCH()
    {
        TestManager.Instance.PROC_LAUNCH_MATCH();
    }
}