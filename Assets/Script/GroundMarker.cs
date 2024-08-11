using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMarker : MonoBehaviour
{
    [SerializeField]
    Material GreenMark;
    [SerializeField]
    Material RedMark;

    public void SetGreenMark()
    {
        gameObject.GetComponent<Renderer>().material = GreenMark;
    }
    public void SetRedMark()
    {
        gameObject.GetComponent<Renderer>().material = RedMark;    
    }

    public void StayActive()
    {
        StartCoroutine(StayActiveFunc());  
    }

    private IEnumerator StayActiveFunc()
    {
        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
    }
}
