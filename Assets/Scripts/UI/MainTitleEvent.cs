using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainTitleEvent : MonoBehaviour, IMainTitleUI
{
    public UnityEvent enterEvent;
    public UnityEvent exitEvent;
    public UnityEvent initEvent;
    public void Enter()
    {
        enterEvent.Invoke();
    }

    public void Exit()
    {
        exitEvent.Invoke();
    }

    public void Init()
    {
        initEvent.Invoke();
    }

}
