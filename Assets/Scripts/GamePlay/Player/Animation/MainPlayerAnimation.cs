using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class MainPlayerAnimation : PlayerAnimation
{
    public GameObjectInstantiator runFartLeft;
    public GameObjectInstantiator runFartRight;
    public List<GameObjectInstantiator> dashFartLefts;
    public List<GameObjectInstantiator> dashFartRights;
    public GameObjectInstantiator onGroundFart;
    public List<GameObjectInstantiator> swordLightLefts;
    public List<GameObjectInstantiator> swordLightRights;
    public List<GameObjectInstantiator> parriedEffectLefts;
    public List<GameObjectInstantiator> parriedEffectRights;
    public ClipInfo dashSound;
    public ClipInfo preAttackSound;
    public ClipInfo landSound;
    public ClipInfo damageSound;
    public ClipInfo bornSound;
    public AudioUtility Audio
    {
        get => GetComponent<AudioUtility>();
    }
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool flipXLock = false;
    private bool lockedFlipX = false;
    private bool lockingAdditionFlipX = false;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Assert.IsNotNull(runFartLeft.source);
        Assert.IsNotNull(runFartRight.source);
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

    public override void RunFart(int signDirectionX)
    {
        var fart = signDirectionX >= 0 ? runFartRight : runFartLeft;
        fart.Instantiate();
    }
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

    public override void Drop()
    {
        animator.SetBool("OnGround", false);
    }

    public override void OnGround()
    {
        Audio.PlaySound(landSound);
        onGroundFart.Instantiate();
        animator.SetBool("OnGround", true);
    }

    private int DirectionNumber(Vector2Int direction)
    {
        if (direction.x == 0)
        {
            if (direction.y > 0)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                return 3;
            }
            else if (direction.y < 0)
            {
                return 4;

            }
            else
            {
                return 0;
            }
        }

    }
    public override void Dash(Vector2Int direction)
    {
        Audio.PlaySound(dashSound);
        animator.SetTrigger("Dash");
        int n = DirectionNumber(direction);
        var dashFarts = (direction.x >= 0 ? dashFartRights : dashFartLefts);
        if (n < dashFarts.Count)
        {
            dashFarts[n].Instantiate();
        }
        animator.SetInteger("DashNo", n);
    }
    public override void PreAttack()
    {
        Audio.PlaySound(preAttackSound);
    }
    public override void Attack(Vector2Int direction, bool parried)
    {
        int n = DirectionNumber(direction);
        var sowrdLights = (direction.x >= 0 ? swordLightRights : swordLightLefts);
        if (n < sowrdLights.Count)
        {
            sowrdLights[n].Instantiate(transform);
        }
    }

    public override void OnParried(Vector2Int direction)
    {
        int n = DirectionNumber(direction);
        var parriedEffects = (direction.x >= 0 ? parriedEffectRights : parriedEffectLefts);
        if (n < parriedEffects.Count)
        {
            parriedEffects[n].Instantiate();
        }

    }

    public override void Die()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("Die");
    }

    public override void Born()
    {
        Audio.PlaySound(bornSound);
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("Born");
    }

    public override void AfterBorn()
    {
        animator.updateMode = AnimatorUpdateMode.Normal;
    }

    public override void BounceUp()
    {
        animator.SetBool("BouncingUp", true);
    }

    public override void AfterBounceUp()
    {
        animator.SetBool("BouncingUp", false);
    }

    public override void Invincible()
    {
        animator.SetBool("Damage", true);
    }

    public override void AfterInvincible()
    {
        animator.SetBool("Damage", false);
    }

    public override void Damage()
    {
        Audio.PlaySound(damageSound);
        animator.SetBool("OnGround", false);
    }

    public override void Parried()
    {
        animator.SetBool("OnGround", false);
    }
}
