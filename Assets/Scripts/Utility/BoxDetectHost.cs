using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IBoxDetectGuest<T>
{
    void Enter(T t);
    void Stay(T t);
    void Exit(T t);
}

public class EventDetectGuest<T> : MonoBehaviour, IBoxDetectGuest<T>
{
    public UnityEvent<T> OnDetect;
    public UnityEvent<T> OnStay;
    public UnityEvent<T> OnExit;

    public void Enter(T t)
    {
        OnDetect.Invoke(t);
    }

    public void Exit(T t)
    {
        OnExit.Invoke(t);
    }

    public void Stay(T t)
    {
        OnStay.Invoke(t);
    }
}

public class BoxDetectHost<T> : MonoBehaviour
{
    public LayerMask detectLayer;
    public T host;
    private List<IBoxDetectGuest<T>> lasts;
    private List<IBoxDetectGuest<T>> currents;
    void FixedUpdate()
    {
        var box = GetComponent<BoxPhysics>();
        if (box == null)
        {
            return;
        }
        if (host == null)
        {
            return;
        }
        if (currents == null)
        {
            currents = new List<IBoxDetectGuest<T>>();
        }
        else
        {
            currents.Clear();
        }
        foreach (var guest in box.InBoxCollisionMapAllFlatten(detectLayer, go => go.GetComponents<IBoxDetectGuest<T>>()))
        {
            if (lasts == null || !lasts.Contains(guest))
            {
                guest.Enter(host);
            }
            else
            {
                guest.Stay(host);
            }
            currents.Add(guest);
        }
        if (lasts != null)
        {
            foreach (var last in lasts)
            {
                if (!currents.Contains(last))
                {
                    last.Exit(host);
                }
            }
        }
        var t = lasts;
        lasts = currents;
        currents = t;
    }
}
