using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour, IPlayerAttackable
{
    public void OnAttack()
    {
        Debug.Log("on attack");
    }
}