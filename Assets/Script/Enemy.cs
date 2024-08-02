using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Unit m_Unit;

    //public bool isTest = false;

    public delegate void EnemyDestroyed(GameObject enemy);
    public static event EnemyDestroyed OnEnemyDestroyed = null;

    private void Start()
    {
        //if (isTest)
        //{
        //    EnemyTest enemy = gameObject.GetComponent<EnemyTest>(); 
        //    m_Unit = new Unit(gameObject, enemy.m_id, enemy.m_type, enemy.m_team, gameObject.transform.position, gameObject.transform.forward, 0, enemy.m_MaxHP, 0, 0);
        //}
    }

    private void OnDestroy() 
    {
        if(OnEnemyDestroyed != null)     
        {
            OnEnemyDestroyed(gameObject);
        }
    }
}
