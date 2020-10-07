using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableDashPlayer : MonoBehaviour, IBoxDetectGuest<Player>
{
    public void Enter(Player t)
    {
        t.dashable = false;
    }

    public void Exit(Player t)
    {
        t.dashable = true;
    }

    public void Stay(Player t)
    {
    }
}
