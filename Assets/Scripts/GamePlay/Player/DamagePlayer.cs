using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour, IBoxDetectGuest<Player>
{
    private Func<Vector2> damageDir;

    public Func<Vector2> DamageDir { set => damageDir = value; }

    public void Enter(Player t)
    {
        t.OnDamage(damageDir());
    }

    public void Exit(Player t)
    {
    }

    public void Stay(Player t)
    {
        t.OnDamage(damageDir());
    }

}
