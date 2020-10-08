using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public abstract int SignDirectionX { set; }
    public abstract float RunningSpeed { set; }

    public abstract void BeginRun();
    public abstract void RunFart(int signDirectionX);
    public abstract void StopRun();
    public abstract void TurnAround(int lastSignDirectionX);
    public abstract void Dash(Vector2Int direction);
    public abstract void Drop();
    public abstract void OnGround();
    public abstract void Attack(Vector2Int direction, bool parried);
    public abstract void OnParried(Vector2Int direction);
    public abstract void Die();
    public abstract void Born();
    public abstract void AfterBorn();
    public abstract void BounceUp();
    public abstract void AfterBounceUp();
    public abstract void Damage();
    public abstract void AfterDamage();
}
