using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Pair<T>
{
    public T action;
    public float timeLeft;

    public Pair(T action, float timeLeft)
    {
        this.action = action;
        this.timeLeft = timeLeft;
    }
}
public class TimeQueue<T> : MonoBehaviour where T : struct
{
    public float cacheTime;
    private List<Pair<T>> actions;
    void FixedUpdate()
    {
        if (actions == null)
        {
            return;
        }
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            if (actions[i].timeLeft < Time.fixedDeltaTime)
            {
                actions.RemoveAt(i);
            }
            else
            {
                actions[i].timeLeft -= Time.fixedDeltaTime;
            }
        }
    }
    public T? GetAction(Func<T, bool> valid = null)
    {
        if (actions == null)
        {
            return null;
        }
        foreach (var pair in actions)
        {
            var action = pair.action;
            if (valid == null || valid(action))
            {
                actions.Remove(pair);
                return action;
            }
        }
        return null;
    }
    public void AddAction(T action)
    {
        if (actions == null)
        {
            actions = new List<Pair<T>>();
        }
        actions.Add(new Pair<T>(action, cacheTime));
    }
}
