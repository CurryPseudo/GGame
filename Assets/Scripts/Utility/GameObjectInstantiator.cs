using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObjectInstantiator
{
    public GameObject source;
    public int maxInstanceCount = 1;
    private List<GameObject> instances;
    public void Instantiate(Transform transform = null)
    {
        if (instances == null)
        {
            instances = new List<GameObject>();
            for (int i = 0; i < maxInstanceCount; i++)
            {
                instances.Add(null);
            }
        }
        int index = instances.FindIndex((instance) => instance == null);
        if (index != -1)
        {
            instances[index] = GameObject.Instantiate(source, source.transform.position, source.transform.rotation);
            if (transform != null)
            {
                instances[index].transform.SetParent(transform, true);
            }
            instances[index].SetActive(true);
        }
    }
}