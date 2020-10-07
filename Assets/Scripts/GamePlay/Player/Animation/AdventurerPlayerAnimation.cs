using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class AdventurerPlayerAnimation : PlayerAnimation
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(Main());
    }

    IEnumerator Main()
    {
        while (true)
        {
            yield return null;
        }
    }
    public override int SignDirectionX { set => spriteRenderer.flipX = value < 0; }
    public override float RunningSpeed { set => animator.SetFloat("RunningSpeed", value); }

    public override void BeginRun()
    {
        animator.SetBool("Running", true);
    }

    public override void StopRun()
    {
        animator.SetBool("Running", false);
    }

    public override void TurnAround(int lastSignDirectionX) { }

    public override void Dash(Vector2Int direction)
    {
    }

    public override void Drop()
    {
    }

    public override void OnGround()
    {
    }

    public override void RunFart(int signDirectionX)
    {
    }

    public override void Attack(Vector2Int direction)
    {
    }

    public override void OnParried(Vector2Int direction)
    {
    }

    public override void Die()
    {
    }

    public override void Born()
    {
    }
    public override void AfterBorn()
    {
    }

    public override void BounceUp()
    {
    }

    public override void AfterBounceUp()
    {
    }

    public override void Damage()
    {
    }

    public override void AfterDamage()
    {
    }
}
