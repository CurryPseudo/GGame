using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour, IBoxDetectGuest<Player>
{
    private Func<Vector2> damageDirClosure;

    public Func<Vector2> DamageDirClosure { set => damageDirClosure = value; }
    public Vector2 GetDamageDir(Player player)
    {
        if (damageDirClosure == null)
        {
            return new Vector2(-player.SignDirectionX, 0);
        }
        return damageDirClosure();
    }

    public void Enter(Player t)
    {
        t.OnDamage(GetDamageDir(t));
    }

    public void Exit(Player t)
    {
    }

    public void Stay(Player t)
    {
        t.OnDamage(GetDamageDir(t));
    }

}
