
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
        {"MATCH_START_REPLY", 1012},
        {"MATCH_READY_REPLY", 1014},
        {"LAUNCH_MATCH", 1015},
        {"READY_TO_BATTLE_REPLY", 2001},
        {"ALL_PLAYER_READY", 2002},
        {"ENTER_TO_SELECT_FIELD_REPLY", 2004},
        {"SELECTOR_OPTION_REPLY", 2006},
        {"S_PLAYER_CREATE", 3003},
        {"S_PLAYER_MOVE", 3005},
        {"S_PLAYER_TRACE_PATH_FINDING_REPLY", 3008},
        {"S_PLAYER_TRACE_PATH", 3009},
        {"S_PLAYER_LAUNCH_ATTACK", 3011},
        {"S_PLAYER_STOP_ATTACK", 3013},
        {"S_PLAYER_ATTACK", 3015},
        {"S_PLAYER_DAMAGE", 3016},
        {"S_PLAYER_DIE", 3017},
    };
}

public abstract class Stub_MOW_SERVER : Stub
{
    public void Init() 
    {
        RPC.Instance.AttachStub(this);
    }

    public void Clear()
    {
        RPC.Instance.DetachStub(this);
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
        methods.Add(MessageIDs["MATCH_READY_REPLY"], MATCH_READY_REPLY);
        methods.Add(MessageIDs["LAUNCH_MATCH"], LAUNCH_MATCH);
        RPC.Instance.AttachStub(this);
    }

    public void Clear()
    {
        RPC.Instance.DetachStub(this);
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
        byte[] MATCH_ROOM_NAME = new byte[50];
        Buffer.BlockCopy(payload, offset, MATCH_ROOM_NAME, 0, sizeof(byte) * 50);
        offset += sizeof(byte) * 50;
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
        byte[] MATCH_PLAYER_NAME = new byte[30];
        Buffer.BlockCopy(payload, offset, MATCH_PLAYER_NAME, 0, sizeof(byte) * 30);
        offset += sizeof(byte) * 30;
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

    public void MATCH_READY_REPLY(byte[] payload)
    {
        int offset = 0;
        UInt16 PLAYER_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        MATCH_READY_REPLY(PLAYER_ID);
    }

    public void LAUNCH_MATCH(byte[] payload)
    {
        int offset = 0;
        LAUNCH_MATCH();
    }

    protected abstract void CONNECTION_REPLY(byte REPLY_CODE, UInt16 PLAYER_ID);

    protected abstract void CREATE_MATCH_ROOM_REPLY(byte REPLY_CODE, UInt16 MATCH_ROOM_ID);

    protected abstract void MATCH_ROOM_LIST(UInt16 MATCH_ROOM_ID, byte[] MATCH_ROOM_NAME, byte LENGTH, UInt16 MATCH_ROOM_INDEX, UInt16 TOTAL_MATCH_ROOM);

    protected abstract void JOIN_TO_MATCH_ROOM_REPLY(byte REPLY_CODE);

    protected abstract void MATCH_PLAYER_LIST(UInt16 PLAYER_ID, byte[] MATCH_PLAYER_NAME, byte LENGTH, byte MATCH_PLAYER_INDEX, byte TOTAL_MATCH_PLAYER);

    protected abstract void MATCH_START_REPLY(byte REPLY_CODE);

    protected abstract void MATCH_READY_REPLY(UInt16 PLAYER_ID);

    protected abstract void LAUNCH_MATCH();

}

public abstract class Stub_MOW_PRE_BATTLE_FIELD : Stub
{
    public void Init() 
    {
        methods.Add(MessageIDs["READY_TO_BATTLE_REPLY"], READY_TO_BATTLE_REPLY);
        methods.Add(MessageIDs["ALL_PLAYER_READY"], ALL_PLAYER_READY);
        methods.Add(MessageIDs["ENTER_TO_SELECT_FIELD_REPLY"], ENTER_TO_SELECT_FIELD_REPLY);
        methods.Add(MessageIDs["SELECTOR_OPTION_REPLY"], SELECTOR_OPTION_REPLY);
        RPC.Instance.AttachStub(this);
    }

    public void Clear()
    {
        RPC.Instance.DetachStub(this);
    }

    public void READY_TO_BATTLE_REPLY(byte[] payload)
    {
        int offset = 0;
        UInt16 BATTLE_FIELD_ID = BitConverter.ToUInt16(payload, offset); offset += sizeof(UInt16);
        byte TEAM = payload[offset++];
        READY_TO_BATTLE_REPLY(BATTLE_FIELD_ID, TEAM);
    }

    public void ALL_PLAYER_READY(byte[] payload)
    {
        int offset = 0;
        ALL_PLAYER_READY();
    }

    public void ENTER_TO_SELECT_FIELD_REPLY(byte[] payload)
    {
        int offset = 0;
        byte SELECTOR_COUNT = payload[offset++];
        ENTER_TO_SELECT_FIELD_REPLY(SELECTOR_COUNT);
    }

    public void SELECTOR_OPTION_REPLY(byte[] payload)
    {
        int offset = 0;
        byte REPLY_CODE = payload[offset++];
        byte REPLY_VALUE = payload[offset++];
        SELECTOR_OPTION_REPLY(REPLY_CODE, REPLY_VALUE);
    }

    protected abstract void READY_TO_BATTLE_REPLY(UInt16 BATTLE_FIELD_ID, byte TEAM);

    protected abstract void ALL_PLAYER_READY();

    protected abstract void ENTER_TO_SELECT_FIELD_REPLY(byte SELECTOR_COUNT);

    protected abstract void SELECTOR_OPTION_REPLY(byte REPLY_CODE, byte REPLY_VALUE);

}

public abstract class Stub_MOW_BATTLE_FIELD : Stub
{
    public void Init() 
    {
        methods.Add(MessageIDs["S_PLAYER_CREATE"], S_PLAYER_CREATE);
        methods.Add(MessageIDs["S_PLAYER_MOVE"], S_PLAYER_MOVE);
        methods.Add(MessageIDs["S_PLAYER_TRACE_PATH_FINDING_REPLY"], S_PLAYER_TRACE_PATH_FINDING_REPLY);
        methods.Add(MessageIDs["S_PLAYER_TRACE_PATH"], S_PLAYER_TRACE_PATH);
        methods.Add(MessageIDs["S_PLAYER_LAUNCH_ATTACK"], S_PLAYER_LAUNCH_ATTACK);
        methods.Add(MessageIDs["S_PLAYER_STOP_ATTACK"], S_PLAYER_STOP_ATTACK);
        methods.Add(MessageIDs["S_PLAYER_ATTACK"], S_PLAYER_ATTACK);
        methods.Add(MessageIDs["S_PLAYER_DAMAGE"], S_PLAYER_DAMAGE);
        methods.Add(MessageIDs["S_PLAYER_DIE"], S_PLAYER_DIE);
        RPC.Instance.AttachStub(this);
    }

    public void Clear()
    {
        RPC.Instance.DetachStub(this);
    }

    public void S_PLAYER_CREATE(byte[] payload)
    {
        int offset = 0;
        Int32 CRT_CODE = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        byte UNIT_TYPE = payload[offset++];
        byte TEAM = payload[offset++];
        float POS_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float POS_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float SPEED = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        Int32 MAX_HP = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        Int32 HP = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        float RADIUS = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float ATTACK_DISTANCE = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float ATTACK_RATE = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float ATTACK_DELAY = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        S_PLAYER_CREATE(CRT_CODE, UNIT_ID, UNIT_TYPE, TEAM, POS_X, POS_Z, NORM_X, NORM_Z, SPEED, MAX_HP, HP, RADIUS, ATTACK_DISTANCE, ATTACK_RATE, ATTACK_DELAY);
    }

    public void S_PLAYER_MOVE(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        byte TEAM = payload[offset++];
        byte MOVE_TYPE = payload[offset++];
        float POS_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float POS_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float SPEED = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float DEST_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float DEST_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        S_PLAYER_MOVE(UNIT_ID, TEAM, MOVE_TYPE, POS_X, POS_Z, NORM_X, NORM_Z, SPEED, DEST_X, DEST_Z);
    }

    public void S_PLAYER_TRACE_PATH_FINDING_REPLY(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        Int32 SPATH_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        S_PLAYER_TRACE_PATH_FINDING_REPLY(UNIT_ID, SPATH_ID);
    }

    public void S_PLAYER_TRACE_PATH(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        Int32 SPATH_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        float POS_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float POS_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        byte SPATH_OPT = payload[offset++];
        S_PLAYER_TRACE_PATH(UNIT_ID, SPATH_ID, POS_X, POS_Z, SPATH_OPT);
    }

    public void S_PLAYER_LAUNCH_ATTACK(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        byte TEAM = payload[offset++];
        float POS_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float POS_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        S_PLAYER_LAUNCH_ATTACK(UNIT_ID, TEAM, POS_X, POS_Z, NORM_X, NORM_Z);
    }

    public void S_PLAYER_STOP_ATTACK(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        byte TEAM = payload[offset++];
        S_PLAYER_STOP_ATTACK(UNIT_ID, TEAM);
    }

    public void S_PLAYER_ATTACK(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        byte TEAM = payload[offset++];
        float POS_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float POS_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_X = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        float NORM_Z = BitConverter.ToSingle(payload, offset); offset += sizeof(float);
        Int32 TARGET_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        byte ATTACK_TYPE = payload[offset++];
        S_PLAYER_ATTACK(UNIT_ID, TEAM, POS_X, POS_Z, NORM_X, NORM_Z, TARGET_ID, ATTACK_TYPE);
    }

    public void S_PLAYER_DAMAGE(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        Int32 HP = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        S_PLAYER_DAMAGE(UNIT_ID, HP);
    }

    public void S_PLAYER_DIE(byte[] payload)
    {
        int offset = 0;
        Int32 UNIT_ID = BitConverter.ToInt32(payload, offset); offset += sizeof(Int32);
        S_PLAYER_DIE(UNIT_ID);
    }

    protected abstract void S_PLAYER_CREATE(Int32 CRT_CODE, Int32 UNIT_ID, byte UNIT_TYPE, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float SPEED, Int32 MAX_HP, Int32 HP, float RADIUS, float ATTACK_DISTANCE, float ATTACK_RATE, float ATTACK_DELAY);

    protected abstract void S_PLAYER_MOVE(Int32 UNIT_ID, byte TEAM, byte MOVE_TYPE, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float SPEED, float DEST_X, float DEST_Z);

    protected abstract void S_PLAYER_TRACE_PATH_FINDING_REPLY(Int32 UNIT_ID, Int32 SPATH_ID);

    protected abstract void S_PLAYER_TRACE_PATH(Int32 UNIT_ID, Int32 SPATH_ID, float POS_X, float POS_Z, byte SPATH_OPT);

    protected abstract void S_PLAYER_LAUNCH_ATTACK(Int32 UNIT_ID, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z);

    protected abstract void S_PLAYER_STOP_ATTACK(Int32 UNIT_ID, byte TEAM);

    protected abstract void S_PLAYER_ATTACK(Int32 UNIT_ID, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, Int32 TARGET_ID, byte ATTACK_TYPE);

    protected abstract void S_PLAYER_DAMAGE(Int32 UNIT_ID, Int32 HP);

    protected abstract void S_PLAYER_DIE(Int32 UNIT_ID);

}
