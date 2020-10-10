using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossScene<T> : MonoBehaviour where T : CrossScene<T>
{
    private bool isCustomDestroy;
    private static bool created = false;
    void Awake()
    {
        if (!created)
        {
            created = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            isCustomDestroy = true;
            Destroy(gameObject);
            return;
        }
    }
    void OnDestroy()
    {
        if (!isCustomDestroy)
        {
            created = false;
        }
    }

}
