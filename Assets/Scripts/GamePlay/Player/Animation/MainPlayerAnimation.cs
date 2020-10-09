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
    public GameObjectInstantiator dashFart;
    public GameObjectInstantiator onGroundFart;
    public GameObjectInstantiator parriedEffect;
    public GameObjectInstantiator swordLight;
    public GameObjectInstantiator swordLightLine;
    public ClipInfo dashSound;
    public ClipInfo preAttackSound;
    public ClipInfo landSound;
    public ClipInfo damageSound;
    public ClipInfo bornSound;
    public ClipInfo parriedSound;
    public ClipInfo healingSound;
    public float quickHealingSpeed;
    public float slowHealingSpeed;
    public float quickHealingParticleSpeed;
    public float slowHealingParticleSpeed;
    public float healingSoundFadeOutTime;
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
    void Update()
    {
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
        animator.SetInteger("DashNo", n);
        Vector2 directionFloat = direction;
        float angle = Vector2.SignedAngle(Vector2.left, directionFloat);
        dashFart.Instantiate(angle);
    }
    public override void PreAttack()
    {
        Audio.PlaySound(preAttackSound);
    }
    public override void Attack(Vector2Int direction, bool parried)
    {
        if (parried)
        {
            Audio.PlaySound(parriedSound);
        }
        Vector2 directionFloat = direction;
        float angle = Vector2.SignedAngle(Vector2.left, directionFloat);
        swordLight.Instantiate(angle, transform);
        swordLightLine.Instantiate(angle);
    }

    public override void OnParried(Vector2Int direction)
    {
        Vector2 directionFloat = direction;
        float angle = Vector2.SignedAngle(Vector2.left, directionFloat);
        parriedEffect.Instantiate(angle);
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

    public override void BeginHealing()
    {
        animator.SetBool("Healing", true);
        GetComponentInChildren<ParticleSystem>().Play();
        Audio.PlaySound(healingSound);
    }

    public override void StopHealing()
    {
        animator.SetBool("Healing", false);
        GetComponentInChildren<ParticleSystem>().Stop();
        Audio.FadeOut(healingSoundFadeOutTime);
    }

    public override void QuickHealing()
    {
        animator.SetFloat("HealingSpeed", quickHealingSpeed);
        var main = GetComponentInChildren<ParticleSystem>().main;
        main.startSpeed = quickHealingParticleSpeed;
    }

    public override void SlowHealing()
    {
        animator.SetFloat("HealingSpeed", slowHealingSpeed);
        var main = GetComponentInChildren<ParticleSystem>().main;
        main.startSpeed = slowHealingParticleSpeed;
    }

    public override void HealingOneShot()
    {
        animator.SetTrigger("HealingOneShot");
    }
}
