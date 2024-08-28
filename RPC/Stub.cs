
using System;
using System.Collections.Generic;
using UnityEngine;


public class Stub : MonoBehaviour
{
    public Dictionary<UInt16, Action<byte[]>> methods = new Dictionary<UInt16, Action<byte[]>>();
    
    protected Dictionary<string, UInt16> MessageIDs = new Dictionary<string, UInt16>()
    {
        {"CONNECTION_REPLY", 1001},
        {"CREATE_MATCH_ROOM_REPLY", 1003},
        {"MATCH_ROOM_LIST", 1006},
        {"JOIN_TO_MATCH_ROOM_REPLY", 1008},
        {"MATCH_PLAYER_LIST", 1010},
        {"MATCH_START_REPLY", 1013},
        {"CHANGE_MATCH_HOST", 1014},
        {"ENTER_TO_SELECT_FIELD_REPLY", 2003},
        {"SELECTOR_OPTION_REPLY", 2005},
        {"S_PLAYER_CREATE", 3003},
    };
}

public abstract class Stub_MOW_SERVER : Stub
{
    public void Init() 
    {
        RPC.Instance.AttachStub(this);
    }


}

public abstract class Stub_MOW_HUB : Stub
{
    public void Init() 
    {
        methods.Add(MessageIDs["CONNECTION_REPLY"], CONNECTION_REPLY);
        methods.Add(MessageIDs["CREATE_MATCH_ROOM_REPLY"], CREATE_MATCH_ROOM_REPLY);
        methods.Add(MessageIDs["MATCH_ROOM_LIST"], MATCH_ROOM_LIST);
        methods.Add(MessageIDs["JOIN_TO_MATCH_ROOM_REPLY"], JOIN_TO_MATCH_ROOM_REPLY);
        methods.Add(MessageIDs["MATCH_PLAYER_LIST"], MATCH_PLAYER_LIST);
        methods.Add(MessageIDs["MATCH_START_REPLY"], MATCH_START_REPLY);
        methods.Add(MessageIDs["CHANGE_MATCH_HOST"], CHANGE_MATCH_HOST);
        RPC.Instance.AttachStub(this);
    }


    public void CONNECTION_REPLY(byte[] payload)
    {
        int offset = 0;
        byte REPLY_CODE = payload[offset++];
        UInt16 PLAYER_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        CONNECTION_REPLY(REPLY_CODE, PLAYER_ID);
    }

    public void CREATE_MATCH_ROOM_REPLY(byte[] payload)
    {
        int offset = 0;
        byte REPLY_CODE = payload[offset++];
        UInt16 MATCH_ROOM_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        CREATE_MATCH_ROOM_REPLY(REPLY_CODE, MATCH_ROOM_ID);
    }

    public void MATCH_ROOM_LIST(byte[] payload)
    {
        int offset = 0;
        UInt16 MATCH_ROOM_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        char[] MATCH_ROOM_NAME = new char[50];
        Buffer.BlockCopy(payload, offset, MATCH_ROOM_NAME, 0, sizeof(char) * 50);
        offset += sizeof(char) * 50;
        byte LENGTH = payload[offset++];
        UInt16 MATCH_ROOM_INDEX = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        UInt16 TOTAL_MATCH_ROOM = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        MATCH_ROOM_LIST(MATCH_ROOM_ID, MATCH_ROOM_NAME, LENGTH, MATCH_ROOM_INDEX, TOTAL_MATCH_ROOM);
    }

    public void JOIN_TO_MATCH_ROOM_REPLY(byte[] payload)
    {
        int offset = 0;
        byte REPLY_CODE = payload[offset++];
        JOIN_TO_MATCH_ROOM_REPLY(REPLY_CODE);
    }

    public void MATCH_PLAYER_LIST(byte[] payload)
    {
        int offset = 0;
        UInt16 PLAYER_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        char[] MATCH_PLAYER_NAME = new char[30];
        Buffer.BlockCopy(payload, offset, MATCH_PLAYER_NAME, 0, sizeof(char) * 30);
        offset += sizeof(char) * 30;
        byte LENGTH = payload[offset++];
        byte MATCH_PLAYER_INDEX = payload[offset++];
        byte TOTAL_MATCH_PLAYER = payload[offset++];
        MATCH_PLAYER_LIST(PLAYER_ID, MATCH_PLAYER_NAME, LENGTH, MATCH_PLAYER_INDEX, TOTAL_MATCH_PLAYER);
    }

    public void MATCH_START_REPLY(byte[] payload)
    {
        int offset = 0;
        byte REPLY_CODE = payload[offset++];
        MATCH_START_REPLY(REPLY_CODE);
    }

    public void CHANGE_MATCH_HOST(byte[] payload)
    {
        int offset = 0;
        UInt16 HOST_PLAYER_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        CHANGE_MATCH_HOST(HOST_PLAYER_ID);
    }

    protected abstract void CONNECTION_REPLY(byte REPLY_CODE, UInt16 PLAYER_ID);

    protected abstract void CREATE_MATCH_ROOM_REPLY(byte REPLY_CODE, UInt16 MATCH_ROOM_ID);

    protected abstract void MATCH_ROOM_LIST(UInt16 MATCH_ROOM_ID, char[] MATCH_ROOM_NAME, byte LENGTH, UInt16 MATCH_ROOM_INDEX, UInt16 TOTAL_MATCH_ROOM);

    protected abstract void JOIN_TO_MATCH_ROOM_REPLY(byte REPLY_CODE);

    protected abstract void MATCH_PLAYER_LIST(UInt16 PLAYER_ID, char[] MATCH_PLAYER_NAME, byte LENGTH, byte MATCH_PLAYER_INDEX, byte TOTAL_MATCH_PLAYER);

    protected abstract void MATCH_START_REPLY(byte REPLY_CODE);

    protected abstract void CHANGE_MATCH_HOST(UInt16 HOST_PLAYER_ID);

}

public abstract class Stub_MOW_PRE_BATTLE_FIELD : Stub
{
    public void Init() 
    {
        methods.Add(MessageIDs["ENTER_TO_SELECT_FIELD_REPLY"], ENTER_TO_SELECT_FIELD_REPLY);
        methods.Add(MessageIDs["SELECTOR_OPTION_REPLY"], SELECTOR_OPTION_REPLY);
        RPC.Instance.AttachStub(this);
    }


    public void ENTER_TO_SELECT_FIELD_REPLY(byte[] payload)
    {
        int offset = 0;
        byte REPLY_CODE = payload[offset++];
        byte SELECTOR_COUNT = payload[offset++];
        ENTER_TO_SELECT_FIELD_REPLY(REPLY_CODE, SELECTOR_COUNT);
    }

    public void SELECTOR_OPTION_REPLY(byte[] payload)
    {
        int offset = 0;
        byte REPLY_CODE = payload[offset++];
        byte REPLY_VALUE = payload[offset++];
        SELECTOR_OPTION_REPLY(REPLY_CODE, REPLY_VALUE);
    }

    protected abstract void ENTER_TO_SELECT_FIELD_REPLY(byte REPLY_CODE, byte SELECTOR_COUNT);

    protected abstract void SELECTOR_OPTION_REPLY(byte REPLY_CODE, byte REPLY_VALUE);

}

public abstract class Stub_MOW_BATTLE_FIELD : Stub
{
    public void Init() 
    {
        methods.Add(MessageIDs["S_PLAYER_CREATE"], S_PLAYER_CREATE);
        RPC.Instance.AttachStub(this);
    }


    public void S_PLAYER_CREATE(byte[] payload)
    {
        int offset = 0;
        Int32 CRT_CODE = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        byte UNIT_TYPE = payload[offset++];
        byte TEAM_CODE = payload[offset++];
        float POS_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float POS_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        UInt16 UNIT_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        float SPEED = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        Int32 HP = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        float RADIUS = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float ATTACK_DISTANCE = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float ATTACK_RATE = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float ATTACK_DELAY = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        S_PLAYER_CREATE(CRT_CODE, UNIT_TYPE, TEAM_CODE, POS_X, POS_Z, NORM_X, NORM_Z, UNIT_ID, SPEED, HP, RADIUS, ATTACK_DISTANCE, ATTACK_RATE, ATTACK_DELAY);
    }

    protected abstract void S_PLAYER_CREATE(Int32 CRT_CODE, byte UNIT_TYPE, byte TEAM_CODE, float POS_X, float POS_Z, float NORM_X, float NORM_Z, UInt16 UNIT_ID, float SPEED, Int32 HP, float RADIUS, float ATTACK_DISTANCE, float ATTACK_RATE, float ATTACK_DELAY);

}
