using System;
using System.Runtime.InteropServices;


static class PROTOCOL_CONSTANT
{
	public const int MAX_OF_PLAYER_NAME_LEN = 32;
	public const int MAX_OF_ROOM_NAME_LEN = 50;
	public const int END_OF_LIST = 255;
	public const int MAX_OF_PLAYER_IN_ROOM = 4;
	public const int MAX_OF_MATCH_ROOM = 100;
	public const int MAX_OF_COLLIDER_ELEMENTS = 100;
};

public 
enum enPacketType
{
	COM_REQUSET,
	COM_REPLY,
	REQ_SET_PLAYER_NAME,
	REQ_MAKE_MATCH_ROOM,
	FWD_REGIST_MATCH_ROOM,
	SERVE_PLAYER_LIST,
	SERVE_READY_TO_START,
	FWD_PLAYER_INFO_TO_BATTLE_THREAD,
	SERVE_BATTLE_START,
	SERVE_MATCH_ROOM_LIST,
	REQ_JOIN_MATCH_ROOM,
	REPLY_NUM_OF_SELECTORS,
	REPLY_ENTER_TO_SELECT_FIELD,
	UNIT_S_CONN_BATTLE_FIELD,
	UNIT_S_CREATE_UNIT,
	S_MGR_CREATE_UNIT,
	UNIT_S_MOVE,
	S_MGR_MOVE,
	UNIT_S_SYNC_POSITION,
	UNIT_S_SYNC_DIRECTION,
	UNIT_S_REQ_TRACE_PATH_FINDING,
	S_MGR_REPLY_TRACE_PATH_FINDING,
	S_MGR_TRACE_SPATH,
	UNIT_S_ATTACK,
	S_MGR_ATTACK,
	S_MGR_ATTACK_INVALID,
	UNIT_S_ATTACK_STOP,
	S_MGR_ATTACK_STOP,
	S_MGR_UINT_DAMAGED,
	S_MGR_UNIT_DIED,
	MGR_UNIT_DIE_REQUEST,
	S_MONT_COLLIDER_MAP_RENEW,
	S_MONT_COLLIDER_MAP,
	S_MONT_JPS_OBSTACLE,
};

public 
enum enProtocolComRequest
{
	REQ_ENTER_MATCH_LOBBY,
	REQ_ENTER_MATCH_ROOM,
	REQ_QUIT_MATCH_ROOM,
	REQ_GAME_START,
	REQ_ENTER_TO_SELECT_FIELD,
	REQ_ENTER_TO_BATTLE_FIELD,
	REQ_EXIT_FROM_BATTLE_FIELD,
	REQ_NUM_OF_SELECTORS,
	REQ_MOVE_SELECT_FIELD_TO_BATTLE_FIELD,
	REQ_MOVE_BATTLE_FIELD_TO_SELECT_FIELD,
};

public 
enum enProtocolComReply
{
	SET_PLAYER_NAME_SUCCESS,
	SET_PLAYER_NAME_FAIL,
	MAKE_MATCH_ROOM_SUCCESS,
	MAKE_MATCH_ROOM_FAIL,
	ENTER_MATCH_LOBBY_SUCCESS,
	ENTER_MATCH_LOBBY_FAIL,
	JOIN_MATCH_ROOM_SUCCESS,
	JOIN_MATCH_ROOM_FAIL,
	ENTER_MATCH_ROOM_SUCCESS,
	ENTER_MATCH_ROOM_FAIL,
	QUIT_MATCH_ROOM_SUCCESS,
	QUIT_MATCH_ROOM_FAIL,
	REPLY_MOVE_BATTLE_FIELD_TO_SELECT_FIELD,
};

public 
enum enPlayerTypeInMatchRoom
{
	Manager,
	Guest,
};

public 
enum enReadyToStartCode
{
	Ready,
	ReadToStart,
};

public 
enum enPlayerTeamInBattleField
{
	Team_A,
	Team_B,
	Team_C,
	Team_D,
	Team_Dummy,
	Team_Test,
};

public 
enum enSceneID
{
	SelectField,
	BattleField,
};

public 
enum enUnitType
{
	Terran_Marine,
	Terran_Firebat,
	Terran_Tank,
	Terran_Robocop,
	Zerg_Zergling,
	Zerg_Hydra,
	Zerg_Golem,
	Zerg_Tarantula,
	None,
};

public 
enum enUnitMoveType
{
	Move_Start,
	Move_Stop,
	Move_Change_Dir,
};

public 
enum enUnitAttackType
{
	ATTACK_NORMAL,
};

public 
enum enSPathStateType
{
	PATH,
	END_OF_PATH,
};

public 
enum enJpsObstacleSetting
{
	SET,
	UNSET,
	CLEAR,
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_COM_REQUEST
{
	public ushort type;
	public ushort requestCode;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_COM_REPLY
{
	public ushort type;
	public ushort replyCode;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_REQ_SET_PLAYER_NAME
{
	public ushort type;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = PROTOCOL_CONSTANT.MAX_OF_PLAYER_NAME_LEN)]
	public char[] playerName;
	public int playerNameLen;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_REQ_MAKE_ROOM
{
	public ushort type;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = PROTOCOL_CONSTANT.MAX_OF_ROOM_NAME_LEN)]
	public char[] roomName;
	public int roomNameLen;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_REGIST_ROOM
{
	public ushort type;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = PROTOCOL_CONSTANT.MAX_OF_PLAYER_NAME_LEN)]
	public char[] playerName;
	public int playerNameLen;
	public byte playerType;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_SERVE_PLAYER_LIST
{
	public ushort type;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = PROTOCOL_CONSTANT.MAX_OF_PLAYER_NAME_LEN)]
	public char[] playerName;
	public int playerNameLen;
	public ushort playerID;
	public byte playerType;
	public byte order;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_SERVE_READY_TO_START
{
	public ushort type;
	public ushort code;
	public ushort playerID;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_FWD_PLAYER_INFO
{
	public ushort type;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = PROTOCOL_CONSTANT.MAX_OF_PLAYER_NAME_LEN)]
	public char[] playerName;
	public int playerNameLen;
	public int team;
	public int numOfTotalPlayers;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_SERVE_BATTLE_START
{
	public ushort type;
	public byte Team;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_SERVE_ROOM_LIST
{
	public ushort type;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = PROTOCOL_CONSTANT.MAX_OF_ROOM_NAME_LEN)]
	public char[] roomName;
	public int roomNameLen;
	public ushort roomID;
	public byte order;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_REQ_JOIN_ROOM
{
	public ushort type;
	public ushort roomID;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_REPLY_ENTER_TO_SELECT_FIELD
{
	public ushort type;
	public int fieldID;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_REPLY_NUM_OF_SELECTORS
{
	public ushort type;
	public int numOfSelector;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_CONN_BATTLE_FIELD
{
	public ushort type;
	public int fieldID;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_CREATE_UNIT
{
	public ushort type;
	public int crtCode;
	public int unitType;
	public int team;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_CREATE_UNIT
{
	public ushort type;
	public int crtCode;
	public int unitID;
	public int unitType;
	public int team;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
	public float speed;
	public int nowHP;
	public int maxHP;
	public float radius;
	public float attackDistance;
	public float attackRate;
	public float attackDelay;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_MOVE
{
	public ushort type;
	public byte moveType;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
	public float destX;
	public float destZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_MOVE
{
	public ushort type;
	public int unitID;
	public int team;
	public byte moveType;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
	public float speed;
	public float destX;
	public float destZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_SYNC_POSITION
{
	public ushort type;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_SYNC_DIRECTION
{
	public ushort type;
	public float normX;
	public float normZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_REQ_TRACE_PATH_FINDING
{
	public ushort type;
	public int spathID;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
	public float destX;
	public float destZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_REPLY_TRACE_PATH_FINDING
{
	public ushort type;
	public int unitID;
	public int spathID;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_TRACE_SPATH
{
	public ushort type;
	public int unitID;
	public int spathID;
	public float posX;
	public float posZ;
	public byte spathState;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_ATTACK
{
	public ushort type;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
	public int targetID;
	public int attackType;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_ATTACK
{
	public ushort type;
	public int unitID;
	public int team;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
	public int targetID;
	public int attackType;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_ATTACK_INVALID
{
	public ushort type;
	public int unitID;
	public int team;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_UNIT_S_ATTACK_STOP
{
	public ushort type;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_ATTACK_STOP
{
	public ushort type;
	public int unitID;
	public float posX;
	public float posZ;
	public float normX;
	public float normZ;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_UINT_DAMAGED
{
	public ushort type;
	public int unitID;
	public int renewHP;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MGR_UNIT_DIED
{
	public ushort type;
	public int unitID;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_MGR_UNIT_DIE_REQUEST
{
	public ushort type;
	public int unitID;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MONT_COLLIDER_MAP_RENEW
{
	public ushort type;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MONT_COLLIDER_MAP
{
	public ushort type;
	public int numOfElements;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = PROTOCOL_CONSTANT.MAX_OF_COLLIDER_ELEMENTS)]
	public Position[] colliders;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MSG_S_MONT_JPS_OBSTACLE
{
	public ushort type;
	public byte setting;
	public int x;
	public int y;
};

