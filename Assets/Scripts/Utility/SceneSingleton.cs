using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSingleton : MonoBehaviour
{
    private static SceneSingleton sceneSingleton;
    void Awake()
    {
        sceneSingleton = this;
    }
    public static T Get<T>() where T : class
    {
        if (sceneSingleton == null)
        {
            return null;
        }
        return sceneSingleton.GetComponentInChildren<T>();
    }
}
