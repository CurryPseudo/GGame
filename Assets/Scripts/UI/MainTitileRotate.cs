using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MainTitileRotate : MonoBehaviour, IMainTitleUI
{
    public float angle;
    public float offset;
    public float time;
    private RectTransform Rect
    {
        get => GetComponent<RectTransform>();
    }
    private Quaternion origin;
    void Awake()
    {
        var rect = GetComponent<RectTransform>();
        origin = rect.rotation;
    }
    public void Init()
    {
        Rect.rotation = origin * Quaternion.Euler(0, 0, angle);
    }
    [Button]
    public void Enter()
    {
        StartCoroutine(EnterCoroutine());
    }
    IEnumerator EnterCoroutine()
    {
        var timeLeft = time;
        var rect = GetComponent<RectTransform>();
        var from = origin * Quaternion.Euler(0, 0, angle);
        var to = origin;
        Rect.rotation = from;
        yield return new WaitForSecondsRealtime(offset);
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
            timeLeft = Mathf.Max(0, timeLeft);
            Rect.rotation = Quaternion.Lerp(from, to, Mathf.SmoothStep(0, 1, 1 - timeLeft / time));
        }
    }
    [Button]
    public void Exit()
    {
        StartCoroutine(ExitCoroutine());
    }
    IEnumerator ExitCoroutine()
    {
        var timeLeft = time;
        var rect = GetComponent<RectTransform>();
        var from = origin;
        var to = origin * Quaternion.Euler(0, 0, angle);
        Rect.rotation = from;
        yield return new WaitForSecondsRealtime(offset);
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
            timeLeft = Mathf.Max(0, timeLeft);
            Rect.rotation = Quaternion.Lerp(from, to, Mathf.SmoothStep(0, 1, 1 - timeLeft / time));
        }

    }
}
