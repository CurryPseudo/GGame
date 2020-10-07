using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceUpPlayer : MonoBehaviour, IBoxDetectGuest<Player>
{
    public void Enter(Player t)
    {
        t.OnBounceUp();
    }

    public void Exit(Player t)
    {
    }

    public void Stay(Player t)
    {
        t.OnBounceUp();
    }

}
