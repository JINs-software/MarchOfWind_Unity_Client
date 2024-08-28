
using System;

public class MOW_BATTLE_FIELD : Stub_MOW_BATTLE_FIELD
{
    private void Start() 
    {
        base.Init();
    }


    protected override void S_PLAYER_CREATE(Int32 CRT_CODE, byte UNIT_TYPE, byte TEAM_CODE, float POS_X, float POS_Z, float NORM_X, float NORM_Z, UInt16 UNIT_ID, float SPEED, Int32 HP, float RADIUS, float ATTACK_DISTANCE, float ATTACK_RATE, float ATTACK_DELAY) 
    {
        throw new NotImplementedException("S_PLAYER_CREATE");
    }

}
