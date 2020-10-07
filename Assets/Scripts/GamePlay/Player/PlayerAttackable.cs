using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAttackable
{
    // Start is called before the first frame update
    AttackResult OnAttack(Vector2Int attackDirection);
    bool ValidBox(BoxPhysics box);
}

public enum AttackResult
{
    Damage, Parry, Dead, None
}
