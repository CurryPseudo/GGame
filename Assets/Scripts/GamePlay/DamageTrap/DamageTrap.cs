using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageTrapAnimation : MonoBehaviour
{
    public abstract void SetRatio(float ratio);
}

public class DamageTrap : MonoBehaviour
{
    public float upTime;
    public float downTime;
    public float waitTime;
    public float holdTime;
    public float disableBoxRatio;
    public new DamageTrapAnimation animation;
    public BoxCollider2D Box
    {
        get => box;
    }
    private Vector2 originOffset;
    private Vector2 originSize;
    private BoxCollider2D box;
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        originOffset = Box.offset;
        originSize = Box.size;
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        while (true)
        {
            SetRatio(0);
            yield return new WaitForSeconds(waitTime);
            {
                float timeLeft = upTime;
                while (timeLeft > 0)
                {
                    yield return new WaitForFixedUpdate();
                    timeLeft -= Time.fixedDeltaTime;
                    SetRatio(1 - timeLeft / upTime);
                }
                SetRatio(1);
            }
            yield return new WaitForSeconds(holdTime);
            {
                float timeLeft = downTime;
                while (timeLeft > 0)
                {
                    yield return new WaitForFixedUpdate();
                    timeLeft -= Time.fixedDeltaTime;
                    SetRatio(timeLeft / upTime);
                }
            }
        }
    }
    public void SetRatio(float ratio)
    {
        if (animation != null)
        {
            animation.SetRatio(ratio);
        }
        SetBoxRatio(ratio);
    }
    public void SetBoxRatio(float ratio)
    {
        if (ratio < disableBoxRatio)
        {
            box.enabled = false;
            return;
        }
        box.enabled = true;
        var offset = originOffset;
        offset.y = Mathf.Lerp(originOffset.y - originSize.y * 0.5f, originOffset.y, ratio);
        Box.offset = offset;
        var size = originSize;
        size.y = Mathf.Lerp(0, originSize.y, ratio);
        Box.size = size;
    }
}