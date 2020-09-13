using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class MainPlayerAnimation : PlayerAnimation
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool flipXLock = false;
    private bool lockedFlipX = false;
    private bool lockingAdditionFlipX = false;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    public override int SignDirectionX
    {
        set
        {
            if (!flipXLock)
            {
                spriteRenderer.flipX = value >= 0;
            }
        }
    }
    public override float RunningSpeed { set => animator.SetFloat("RunningSpeed", value); }

    public override void BeginRun()
    {
        animator.SetBool("Running", true);
    }

    public override void StopRun()
    {
        animator.SetBool("Running", false);
    }
    public override void TurnAround(int lastSignDirectionX)
    {
        LockFlipX();
        animator.SetTrigger("TurnAround");
    }
    public void LockFlipX()
    {
        flipXLock = true;
        lockedFlipX = spriteRenderer.flipX;
        lockingAdditionFlipX = false;
    }
    public void UnlockFlipX()
    {
        flipXLock = false;
    }
    public void FlipLockedX()
    {
        lockingAdditionFlipX = !lockingAdditionFlipX;
        spriteRenderer.flipX = lockingAdditionFlipX ^ lockedFlipX;
    }
}
