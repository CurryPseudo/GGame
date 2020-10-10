using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossScene<T> : MonoBehaviour where T : CrossScene<T>
{
    private bool isCustomDestroy;
    private static T created;
    public static T Current
    {
        get => created;
    }
    void Awake()
    {
        if (created == null)
        {
            created = this as T;
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
            created = null;
        }
    }

}
