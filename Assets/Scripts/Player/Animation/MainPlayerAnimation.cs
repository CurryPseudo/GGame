using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class GameObjectInstantiator
{
    public GameObject source;
    public int maxInstanceCount = 1;
    private List<GameObject> instances;
    public void Instantiate()
    {
        if (instances == null)
        {
            instances = new List<GameObject>();
            for (int i = 0; i < maxInstanceCount; i++)
            {
                instances.Add(null);
            }
        }
        int index = instances.FindIndex((instance) => instance == null);
        if (index != -1)
        {
            instances[index] = GameObject.Instantiate(source, source.transform.position, source.transform.rotation);
            instances[index].SetActive(true);
        }
    }
}

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class MainPlayerAnimation : PlayerAnimation
{
    public GameObjectInstantiator runFartLeft;
    public GameObjectInstantiator runFartRight;
    public List<GameObjectInstantiator> dashFartLefts;
    public List<GameObjectInstantiator> dashFartRights;
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

    public override void Dash(Vector2Int direction)
    {
        animator.SetTrigger("Dash");
        int dashNo = 0;
        if (direction.x == 0)
        {
            if (direction.y > 0)
            {
                dashNo = 1;
            }
            else
            {
                dashNo = 2;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                dashNo = 3;
            }
            else if (direction.y < 0)
            {
                dashNo = 4;

            }
            else
            {
                dashNo = 0;
            }
        }
        var dashFarts = (direction.x >= 0 ? dashFartRights : dashFartLefts);
        if (dashNo < dashFarts.Count)
        {
            dashFarts[dashNo].Instantiate();
        }
        animator.SetInteger("DashNo", dashNo);
    }

    public override void Drop()
    {
        animator.SetBool("OnGround", false);
    }

    public override void OnGround()
    {
        animator.SetBool("OnGround", true);
    }
}
