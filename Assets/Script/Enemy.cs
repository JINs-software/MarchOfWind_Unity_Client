using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public delegate void EnemyDestroyed(GameObject enemy);
    public static event EnemyDestroyed OnEnemyDestroyed = null;
    public int ID;

    private void Start()
    {
        //gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    private void OnDestroy() 
    {
        if(OnEnemyDestroyed != null)     
        {
            OnEnemyDestroyed(gameObject);
        }
    }
}
