using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class TestEvent : MonoBehaviour
{
    public UnityEvent e;
    [Button]
    public void Test()
    {
        e.Invoke();
    }

}
