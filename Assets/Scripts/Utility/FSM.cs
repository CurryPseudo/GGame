using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T, S> where T : MonoBehaviour where S : State<T, S>
{
    public T mono;
    public FSM<T, S> fsm;
    public abstract IEnumerator Main();
    public virtual void Exit()
    {
    }
    public Coroutine coroutine;
}

public class FSM<T, S> where T : MonoBehaviour where S : State<T, S>
{
    private T mono;
    private S current = null;

    public FSM(T mono)
    {
        this.mono = mono;
    }

    public S Current { get => current; }

    public void ChangeState(S next)
    {
        if (current != null)
        {
            if (Current.coroutine != null)
            {
                mono.StopCoroutine(Current.coroutine);
            }
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