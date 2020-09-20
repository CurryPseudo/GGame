﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> where T : MonoBehaviour
{
    public T mono;
    public FSM<T> fsm;
    public abstract IEnumerator Main();
    public virtual void Exit()
    {
    }
    public Coroutine coroutine;
}

public class FSM<T> where T : MonoBehaviour
{
    private T mono;
    public State<T> current = null;
    public FSM(T mono)
    {
        this.mono = mono;
    }
    public void ChangeState(State<T> next)
    {
        if (current != null)
        {
            mono.StopCoroutine(current.coroutine);
            current.Exit();
        }
        if (next != null)
        {
            next.mono = mono;
            next.fsm = this;
            next.coroutine = mono.StartCoroutine(next.Main());
        }
        current = next;
    }
}