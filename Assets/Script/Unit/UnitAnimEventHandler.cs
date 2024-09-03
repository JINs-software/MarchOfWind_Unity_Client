using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimEventHandler : MonoBehaviour
{
    UnitController unitController;
    public GameObject muzzleObject;

    private void Start()
    {
        unitController = gameObject.GetComponent<UnitController>(); 
    }

    public void OnAnimAttackStartEvent()
    {
        if(unitController != null)
        {
            unitController.ATTACK();
        }
        muzzleObject.SetActive(true);
    }

    public void OnAnimAttackEndEvent()
    {
        muzzleObject.SetActive(false);
    }
}
