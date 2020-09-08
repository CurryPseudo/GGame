using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        StartCoroutine(Main());
    }

    IEnumerator Main()
    {
        while (true)
        {
            yield return null;
        }
    }

    public void SetSignDirectionX(int sign)
    {
        spriteRenderer.flipX = sign < 0;
    }

}
