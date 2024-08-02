using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleEffect : MonoBehaviour
{
    public GameObject MuzzleObject;
    public float MuzzleRate;

    private Coroutine MuzzleEffectCoroutine = null;

    public void StartMuzzleEffectCoroutine()
    {
        if(MuzzleEffectCoroutine == null)
        {
            MuzzleEffectCoroutine = StartCoroutine(MuzzleEffectCoroutineFunc());
        }
    }
    public void StoptMuzzleEffectCoroutine()
    {
        if (MuzzleEffectCoroutine != null)
        {
            StopCoroutine(MuzzleEffectCoroutine);
            MuzzleEffectCoroutine = null;
        }
    }
    private IEnumerator MuzzleEffectCoroutineFunc()
    {
        while (true)
        {
            MuzzleObject.SetActive(true);
            yield return new WaitForSeconds((1f / MuzzleRate) / 3);

            MuzzleObject.SetActive(false);
            yield return new WaitForSeconds((1f / MuzzleRate) * 2 / 3);
        }
    }
}
