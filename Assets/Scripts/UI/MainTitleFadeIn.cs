using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTitleFadeIn : MonoBehaviour, IMainTitleUI
{
    public float time;
    public Image Image
    {
        get => GetComponent<Image>();
    }
    public float Alpha
    {
        set
        {
            var color = Image.color;
            color.a = value;
            Image.color = color;
        }
    }
    public void Enter()
    {
        StartCoroutine(EnterCoroutine());
    }
    IEnumerator EnterCoroutine()
    {
        var timeLeft = time;
        Alpha = 0;
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
            timeLeft = Mathf.Max(0, timeLeft);
            Alpha = 1 - timeLeft / time;
        }
    }

    public void Exit()
    {
        StartCoroutine(ExitCoroutine());
    }

    IEnumerator ExitCoroutine()
    {
        var timeLeft = time;
        Alpha = 1;
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
            timeLeft = Mathf.Max(0, timeLeft);
            Alpha = timeLeft / time;
        }
    }
    public void Init()
    {
        Alpha = 0;
    }
}
