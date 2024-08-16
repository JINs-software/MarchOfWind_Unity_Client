using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPath : MonoBehaviour
{
    private float lifeTime = 0;

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime > 5.0f)
        {
            Destroy(gameObject);    
        }
    }
}
