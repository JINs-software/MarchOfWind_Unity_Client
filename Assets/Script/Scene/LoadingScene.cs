using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : BaseScene
{
    MOW_PRE_BATTLE_FIELD stup_MOW_PRE_BATTLE_FIELD;


    protected override void Init()
    {
        base.Init();

        stup_MOW_PRE_BATTLE_FIELD = gameObject.GetComponent<MOW_PRE_BATTLE_FIELD>();
        if (stup_MOW_PRE_BATTLE_FIELD == null)
        {
            stup_MOW_PRE_BATTLE_FIELD = gameObject.AddComponent<MOW_PRE_BATTLE_FIELD>();
        }
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }

    void Start()
    {
        RPC.proxy.READY_TO_BATTLE();
    }

    public void OnReceiveAllPlayersReadyMessage()
    {
        Manager.Scene.Clear();
        Manager.Scene.LoadScene(Define.Scene.SelectField);
    }
}
