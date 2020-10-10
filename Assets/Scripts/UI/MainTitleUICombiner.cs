using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MainTitleUICombiner : MonoBehaviour
{
    [Button]
    public void Enter()
    {
        foreach (var move in GetComponentsInChildren<IMainTitleUI>())
        {
            move.Enter();
        }
    }
    [Button]
    public void Exit()
    {
        foreach (var move in GetComponentsInChildren<IMainTitleUI>())
        {
            move.Exit();
        }
    }

    public void Init()
    {
        foreach (var move in GetComponentsInChildren<IMainTitleUI>())
        {
            move.Init();
        }
    }
}
