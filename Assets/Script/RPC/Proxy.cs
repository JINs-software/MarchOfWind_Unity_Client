
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Proxy
{
    Dictionary<string, UInt16> MessageIDs = new Dictionary<string, UInt16>()
    {

        {"CONNECTION", 1000},
        {"CREATE_MATCH_ROOM", 1002},
        {"ENTER_TO_ROBBY", 1004},
        {"QUIT_FROM_ROBBY", 1005},
        {"JOIN_TO_MATCH_ROOM", 1007},
        {"QUIT_FROM_MATCH_ROOM", 1009},
        {"MATCH_START", 1011},
        {"MATCH_READY", 1012},
        {"READY_TO_BATTLE", 2000},
        {"ENTER_TO_SELECT_FIELD", 2002},
        {"SELECTOR_OPTION", 2004},
        {"ENTER_TO_BATTLE_FIELD", 3000},
        {"UNIT_CONN_TO_BATTLE_FIELD", 3001},
        {"UNIT_S_CREATE", 3002},
    };


    public void CONNECTION(byte[] PLAYER_NAME, byte LENGTH)
    {
        UInt16 type = MessageIDs["CONNECTION"];
        byte[] payload = new byte[sizeof(UInt16) + sizeof(byte) * 30 + sizeof(byte)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        Buffer.BlockCopy(PLAYER_NAME, 0, payload, offset, sizeof(byte) * 30); offset += sizeof(byte) * 30;
        payload[offset++] = LENGTH;
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void CREATE_MATCH_ROOM(byte[] MATCH_ROOM_NAME, byte LENGTH, byte NUM_OF_PARTICIPANTS)
    {
        UInt16 type = MessageIDs["CREATE_MATCH_ROOM"];
        byte[] payload = new byte[sizeof(UInt16) + sizeof(byte) * 50 + sizeof(byte) + sizeof(byte)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        Buffer.BlockCopy(MATCH_ROOM_NAME, 0, payload, offset, sizeof(byte) * 50); offset += sizeof(byte) * 50;
        payload[offset++] = LENGTH;
        payload[offset++] = NUM_OF_PARTICIPANTS;
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void ENTER_TO_ROBBY()
    {
        UInt16 type = MessageIDs["ENTER_TO_ROBBY"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void QUIT_FROM_ROBBY()
    {
        UInt16 type = MessageIDs["QUIT_FROM_ROBBY"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void JOIN_TO_MATCH_ROOM(UInt16 MATCH_ROOM_ID)
    {
        UInt16 type = MessageIDs["JOIN_TO_MATCH_ROOM"];
        byte[] payload = new byte[sizeof(UInt16) + sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        Buffer.BlockCopy(BitConverter.GetBytes(MATCH_ROOM_ID), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void QUIT_FROM_MATCH_ROOM()
    {
        UInt16 type = MessageIDs["QUIT_FROM_MATCH_ROOM"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void MATCH_START()
    {
        UInt16 type = MessageIDs["MATCH_START"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void MATCH_READY()
    {
        UInt16 type = MessageIDs["MATCH_READY"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void READY_TO_BATTLE()
    {
        UInt16 type = MessageIDs["READY_TO_BATTLE"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void ENTER_TO_SELECT_FIELD()
    {
        UInt16 type = MessageIDs["ENTER_TO_SELECT_FIELD"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void SELECTOR_OPTION(byte OPTION_CODE, byte OPTION_VALUE)
    {
        UInt16 type = MessageIDs["SELECTOR_OPTION"];
        byte[] payload = new byte[sizeof(UInt16) + sizeof(byte) + sizeof(byte)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        payload[offset++] = OPTION_CODE;
        payload[offset++] = OPTION_VALUE;
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void ENTER_TO_BATTLE_FIELD()
    {
        UInt16 type = MessageIDs["ENTER_TO_BATTLE_FIELD"];
        byte[] payload = new byte[sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void UNIT_CONN_TO_BATTLE_FIELD(UInt16 BATTLE_FIELD_ID)
    {
        UInt16 type = MessageIDs["UNIT_CONN_TO_BATTLE_FIELD"];
        byte[] payload = new byte[sizeof(UInt16) + sizeof(UInt16)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        Buffer.BlockCopy(BitConverter.GetBytes(BATTLE_FIELD_ID), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

    public void UNIT_S_CREATE(Int32 CRT_CODE, byte UNIT_TYPE, byte TEAM_CODE, float POS_X, float POS_Z, float NORM_X, float NORM_Z)
    {
        UInt16 type = MessageIDs["UNIT_S_CREATE"];
        byte[] payload = new byte[sizeof(UInt16) + sizeof(Int32) + sizeof(byte) + sizeof(byte) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float)];
        int offset = 0;
        Buffer.BlockCopy(BitConverter.GetBytes(type), 0, payload, offset, sizeof(UInt16)); offset += sizeof(UInt16);
        Buffer.BlockCopy(BitConverter.GetBytes(CRT_CODE), 0, payload, offset, sizeof(Int32)); offset += sizeof(Int32);
        payload[offset++] = UNIT_TYPE;
        payload[offset++] = TEAM_CODE;
        Buffer.BlockCopy(BitConverter.GetBytes(POS_X), 0, payload, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(POS_Z), 0, payload, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(NORM_X), 0, payload, offset, sizeof(float)); offset += sizeof(float);
        Buffer.BlockCopy(BitConverter.GetBytes(NORM_Z), 0, payload, offset, sizeof(float)); offset += sizeof(float);
        RPC.Network.SendPacketBytes(payload, RPC.EnDecodeFlag);
    }

}
