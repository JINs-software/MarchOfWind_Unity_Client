
using System;

public class MOW_PRE_BATTLE_FIELD : Stub_MOW_PRE_BATTLE_FIELD
{
    protected void Start() 
    {
        base.Init();
    }

    protected void OnDestroy()
    {
        base.Clear();  
    }


    protected override void READY_TO_BATTLE_REPLY(UInt16 BATTLE_FIELD_ID, byte TEAM) 
    {
        GamaManager.Instance.BattleFieldID = BATTLE_FIELD_ID;
        GamaManager.Instance.Team = TEAM;
    }

    protected override void ALL_PLAYER_READY() 
    {
        LoadingScene loading = gameObject.GetComponent<LoadingScene>();
        loading.OnReceiveAllPlayersReadyMessage();
    }

    protected override void ENTER_TO_SELECT_FIELD_REPLY(byte SELECTOR_COUNT) 
    {
        GamaManager.Instance.SelectorCnt = SELECTOR_COUNT;
        SelectField selectField = gameObject.GetComponent<SelectField>();
        selectField.OnReplyEnterSelectField();
    }

    protected override void SELECTOR_OPTION_REPLY(byte REPLY_CODE, byte REPLY_VALUE) 
    {
        throw new NotImplementedException("SELECTOR_OPTION_REPLY");
    }

}
