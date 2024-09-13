using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimEventHandler : MonoBehaviour
{
    UnitController unitController;
    AttackController attackController;  
    public GameObject muzzleObject;

    private void Start()
    {
        unitController = gameObject.GetComponent<UnitController>(); 
        attackController = gameObject.GetComponent<AttackController>();
    }

    public void OnAnimAttackStartEvent()
    {
        Attack();
        muzzleObject.SetActive(true);
    }

    public void OnAnimAttackEndEvent()
    {
        muzzleObject.SetActive(false);
    }

    public void OnAnimMuzzleStart()
    {
        muzzleObject.SetActive(true);
    }
    public void OnAnimAttackStart()
    {
        Attack();
    }

    private void Attack()
    {
        if (unitController != null && attackController != null && gameObject.tag == GamaManager.TEAM_TAG)
        {
            if(attackController.HasTarget() && attackController.m_TargetObject.tag == GamaManager.ENEMY_TAG)
            {
                unitController.ATTACK();
            }
            else
            {
                unitController.ATTACK_ARC();
            }
        }
    }
}
