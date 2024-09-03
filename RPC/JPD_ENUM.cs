

public enum enPLAYER_QUIT_TYPE_FROM_MATCH_ROOM
{
    CANCEL,
    DISCONNECTION,
};

public enum enCONNECTION_REPLY_CODE
{
    SUCCESS,
    PLAYER_CAPACITY_EXCEEDED,
    INVALID_MSG_FIELD_VALUE,
    PLAYER_NAME_ALREADY_EXIXTS,
};

public enum enCREATE_MATCH_ROOM_REPLY_CODE
{
    SUCCESS,
    MATCH_ROOM_CAPACITY_EXCEEDED,
    INVALID_MSG_FIELD_VALUE,
    MATCH_ROOM_NAME_ALREADY_EXIXTS,
};

public enum enJOIN_TO_MATCH_ROOM_REPLY_CODE
{
    SUCCESS,
    INVALID_MATCH_ROOM_ID,
    PLAYER_CAPACITY_IN_ROOM_EXCEEDED,
};

public enum enMATCH_START_REPLY_CODE
{
    SUCCESS,
    NOT_FOUND_IN_MATCH_ROOM,
    NO_HOST_PRIVILEGES,
    UNREADY_PLAYER_PRESENT,
};

public enum enMATCH_ROOM_CLOSE_CODE
{
    EMPTY_PLAYER,
};

public enum enUNIT_TYPE
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

public enum enMOVE_TYPE
{
    MOVE_START,
    MOVE_STOP,
};

public enum enATTACK_TYPE
{
    BASE,
};
