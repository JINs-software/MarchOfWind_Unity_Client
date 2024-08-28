
using System;

public class MOW_PRE_BATTLE_FIELD : Stub_MOW_PRE_BATTLE_FIELD
{
    private void Start() 
    {
        base.Init();
    }


    protected override void ENTER_TO_SELECT_FIELD_REPLY(byte REPLY_CODE, byte SELECTOR_COUNT) 
    {
        throw new NotImplementedException("ENTER_TO_SELECT_FIELD_REPLY");
    }

    protected override void SELECTOR_OPTION_REPLY(byte REPLY_CODE, byte REPLY_VALUE) 
    {
        throw new NotImplementedException("SELECTOR_OPTION_REPLY");
    }

}
