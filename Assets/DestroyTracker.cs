using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTracker : MonoBehaviour
{
    private void OnDestroy() {
        Debug.Log(gameObject.ToString() + "Destroy..");
    }
}
