
using System;

public class MOW_PRE_BATTLE_FIELD_TEST : MOW_PRE_BATTLE_FIELD
{
	protected void Start()
	{
		base.Start();
	}

	protected void OnDestroy()
	{
		base.OnDestroy();
	}


	protected override void READY_TO_BATTLE_REPLY(UInt16 BATTLE_FIELD_ID, byte TEAM)
	{
		TestManager.Instance.READY_TO_BATTLE_REPLY(BATTLE_FIELD_ID, TEAM);
	}

	protected override void ALL_PLAYER_READY()
	{
		TestManager.Instance.PROC_ALL_PLAYER_READY();
	}

	protected override void ENTER_TO_SELECT_FIELD_REPLY(byte SELECTOR_COUNT)
	{
		TestManager.Instance.PROC_ENTER_TO_SELECT_FIELD_REPLY(SELECTOR_COUNT);
	}

	protected override void SELECTOR_OPTION_REPLY(byte REPLY_CODE, byte REPLY_VALUE)
	{
	}

}
