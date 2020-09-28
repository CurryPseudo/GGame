using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : PlayerAttackable
{
    public override void OnAttack()
    {
        Debug.Log("on attack");
    }
}