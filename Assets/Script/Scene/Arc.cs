using System;
using UnityEngine;

public class Arc : MonoBehaviour
{
    public static float RADIUS = 20f;

    public byte TEAM;
    public Int32 MAX_HP;
    public Int32 HP;

    public delegate void ArcDestroyed(GameObject arc);
    public static event ArcDestroyed OnArcDestroyed = null;

    HealthController m_HearthController;

    private void Awake()
    {
        m_HearthController = GetComponent<HealthController>();  
    }

    public void Init(byte team, Int32 maxHp, Int32 hp)
    {
        TEAM = team;
        MAX_HP = maxHp; 
        HP = hp;    
        m_HearthController.InitHealth(HP, maxHp);   
    }

    public void UpdateHP(Int32 hp)
    {
        HP = hp;
        m_HearthController.UpdateHealth(hp);    
    }

    public void Destroy()
    {
        if(OnArcDestroyed != null)
        {
            OnArcDestroyed(gameObject);
        }

        GameObject.Destroy(gameObject); 
    }
}
