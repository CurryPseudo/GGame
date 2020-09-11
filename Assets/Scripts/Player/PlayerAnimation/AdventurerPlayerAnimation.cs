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
    public override void SetSignDirectionX(int sign)
    {
        spriteRenderer.flipX = sign < 0;
    }

    public override void BeginRun()
    {
        Debug.Log("Begin run");
    }

    public override void StopRun()
    {
        Debug.Log("Stop run");
    }
}
