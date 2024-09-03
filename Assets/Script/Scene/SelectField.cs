using UnityEngine;

public class SelectField : BaseScene
{
    MOW_PRE_BATTLE_FIELD stub_MOW_PRE_BATTLE_FIELD;

    [SerializeField]
    GameObject InitPoint;

    protected override void Init()
    {
        base.Init();

        stub_MOW_PRE_BATTLE_FIELD = gameObject.GetComponent<MOW_PRE_BATTLE_FIELD>();
        if(stub_MOW_PRE_BATTLE_FIELD == null)
        {
            stub_MOW_PRE_BATTLE_FIELD = gameObject.AddComponent<MOW_PRE_BATTLE_FIELD>();
        }
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }

    void Start()
    {
        GamaManager.UnitSelection.Init();
        RPC.proxy.ENTER_TO_SELECT_FIELD();
    }

    public void OnReplyEnterSelectField()
    {
        for(byte i=0; i < GamaManager.Instance.SelectorCnt; i++)
        {
            GameObject selector = Manager.Resource.Instantiate($"SelectField/Selector{GamaManager.Instance.Team}");
            Vector3 initPosition = GetRandomPositionOnCylinder(InitPoint);
            selector.transform.position = initPosition;
            selector.SetActive(true);   
        }
    }

    Vector3 GetRandomPositionOnCylinder(GameObject spawnPoint)
    {
        Vector2 randomPos = Random.insideUnitCircle * InitPoint.transform.localScale.x / 2f;
        Vector3 spawnPosition = new Vector3(randomPos.x, 0, randomPos.y) + InitPoint.transform.position;
        return spawnPosition;
    }
}
