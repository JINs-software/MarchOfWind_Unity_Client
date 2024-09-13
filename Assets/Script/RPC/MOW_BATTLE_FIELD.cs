
using System;
using UnityEngine;

public class MOW_BATTLE_FIELD : Stub_MOW_BATTLE_FIELD
{
    private void Start() 
    {
        base.Init();
    }

    private void OnDestroy()
    {
        base.Clear();  
    }

    BattleField battleField;

    protected override void S_PLAYER_ARC_INFO(byte TEAM, Int32 MAX_HP, Int32 HP)
    {
        if(battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();   
        }
        battleField.S_PLAYER_ARC_INFO(TEAM, MAX_HP, HP);    
    }

    protected override void S_PLAYER_CREATE(Int32 CRT_CODE, Int32 UNIT_ID, byte UNIT_TYPE, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float SPEED, Int32 MAX_HP, Int32 HP, float RADIUS, float ATTACK_DISTANCE, float ATTACK_RATE, float ATTACK_DELAY) 
    {
        if(battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>(); 
        }
        battleField.S_PLAYER_CREATE(CRT_CODE, UNIT_ID, UNIT_TYPE, TEAM, POS_X, POS_Z, NORM_X, NORM_Z, SPEED, MAX_HP, HP, RADIUS, ATTACK_DISTANCE, ATTACK_RATE, ATTACK_DELAY);
    }

    protected override void S_PLAYER_MOVE(Int32 UNIT_ID, byte TEAM, byte MOVE_TYPE, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float SPEED, float DEST_X, float DEST_Z) 
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_MOVE(UNIT_ID, TEAM, MOVE_TYPE, POS_X, POS_Z, NORM_X, NORM_Z, SPEED, DEST_X, DEST_Z);
    }

    protected override void S_PLAYER_TRACE_PATH_FINDING_REPLY(Int32 UNIT_ID, Int32 SPATH_ID) 
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_TRACE_PATH_FINDING_REPLY(UNIT_ID, SPATH_ID);
    }

    protected override void S_PLAYER_TRACE_PATH(Int32 UNIT_ID, Int32 SPATH_ID, float POS_X, float POS_Z, byte SPATH_OPT) 
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_TRACE_PATH(UNIT_ID, SPATH_ID, POS_X, POS_Z, SPATH_OPT);
    }

    protected override void S_PLAYER_LAUNCH_ATTACK(Int32 UNIT_ID, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z) 
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_LAUNCH_ATTACK(UNIT_ID, TEAM, POS_X, POS_Z, NORM_X, NORM_Z);
    }

    protected override void S_PLAYER_STOP_ATTACK(Int32 UNIT_ID, byte TEAM) 
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_STOP_ATTACK(UNIT_ID, TEAM);
    }

    protected override void S_PLAYER_ATTACK(Int32 UNIT_ID, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, Int32 TARGET_ID, byte ATTACK_TYPE) 
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_ATTACK(UNIT_ID, TEAM, POS_X, POS_Z, NORM_X, NORM_Z, TARGET_ID, ATTACK_TYPE);
    }

    protected override void S_PLAYER_ATTACK_ARC(int UNIT_ID, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, byte ARC_TEAM, byte ATTACK_TYPE)
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_ATTACK_ARC(UNIT_ID, TEAM, POS_X, POS_Z, NORM_X, NORM_Z, ARC_TEAM, ATTACK_TYPE);
    }

    protected override void S_PLAYER_DAMAGE(Int32 UNIT_ID, Int32 HP) 
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_DAMAGE(UNIT_ID, HP);
    }

    protected override void S_PLAYER_DAMAGE_ARC(byte ARC_TEAM, int HP)
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_DAMAGE_ARC(ARC_TEAM, HP);
    }

    protected override void S_PLAYER_DIE(Int32 UNIT_ID)
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_DIE(UNIT_ID);
    }

    protected override void S_PLAYER_ARC_DESTROY(byte ARC_TEAM)
    {
        if (battleField == null)
        {
            battleField = gameObject.GetComponent<BattleField>();
        }
        battleField.S_PLAYER_ARC_DESTROY(ARC_TEAM);
    }
}
