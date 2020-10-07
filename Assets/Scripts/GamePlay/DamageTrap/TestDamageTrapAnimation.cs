using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageTrapAnimation : DamageTrapAnimation
{
    private float originScale;
    private float originY;
    void Awake()
    {
        originY = transform.position.y;
        originScale = transform.localScale.y;
    }
    public override void SetRatio(float ratio)
    {
        var position = transform.position;
        position.y = Mathf.Lerp(originY - originScale * 0.5f, originY, ratio);
        transform.position = position;
        var scale = transform.localScale;
        scale.y = Mathf.Lerp(0, originScale, ratio);
        transform.localScale = scale;
    }
}
