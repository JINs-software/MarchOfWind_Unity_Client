using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField]    
    HealthTracker healthTracker;

    int MaxHP { get; set; }
    int NowHP { get; set; }

    public void InitHealth(int initHP, int maxHP)
    {
        MaxHP = maxHP;
        NowHP = initHP; 

        if (healthTracker != null)
        {
            healthTracker.UpdateSliderValue(NowHP, MaxHP);
        }
    }

    public void UpdateHealth(int health) 
    {
        NowHP = health;
        healthTracker.UpdateSliderValue(NowHP, MaxHP);
    }
}
