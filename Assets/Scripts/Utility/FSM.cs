using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    public T mono;
    public abstract IEnumerator Main();
    public virtual void Exit()
    {
    }
    public Coroutine coroutine;
}

public class FSM<T> : MonoBehaviour where T : FSM<T>
{
    public State<T> current = null;
    public void ChangeState(State<T> next)
    {
        if (current != null)
        {
            StopCoroutine(current.coroutine);
            current.Exit();
        }
        if (next != null)
        {
            next.mono = this as T;
            next.coroutine = StartCoroutine(next.Main());
        }
        current = next;
    }
}