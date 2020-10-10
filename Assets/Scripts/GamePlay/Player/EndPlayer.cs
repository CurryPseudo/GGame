using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPlayer : MonoBehaviour, IBoxDetectGuest<Player>
{
    public void Enter(Player t)
    {
        t.OnEnd();
    }

    public void Exit(Player t)
    {
    }

    public void Stay(Player t)
    {
    }
}
