using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMonster : MonoBehaviour, IBoxDetectGuest<IMonster>
{
    public void Enter(IMonster t)
    {
        if (t.CouldAffectedByDamageMonster)
        {
            t.OnDamage((t.FaceLeft ? -1 : 1) * Vector2Int.right);
        }
    }

    public void Exit(IMonster t)
    {
    }

    public void Stay(IMonster t)
    {
        if (t.CouldAffectedByDamageMonster)
        {
            t.OnDamage((t.FaceLeft ? -1 : 1) * Vector2Int.right);
        }
    }
}
