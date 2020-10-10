using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IMainTitleUI
{
    void Init();
    void Enter();
    void Exit();
}
public class MainTitileMove : MonoBehaviour, IMainTitleUI
{
    public Vector2 direction;
    public float offset;
    public float time;
    private RectTransform Rect
    {
        get => GetComponent<RectTransform>();
    }
    private Vector2 origin;
    void Awake()
    {
        var rect = GetComponent<RectTransform>();
        origin = rect.anchoredPosition;
    }
    public void Init()
    {
        var from = origin + direction;
        Rect.anchoredPosition = from;
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
        var from = origin + direction;
        var to = origin;
        Rect.anchoredPosition = from;
        yield return new WaitForSecondsRealtime(offset);
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
            timeLeft = Mathf.Max(0, timeLeft);
            Rect.anchoredPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0, 1, 1 - timeLeft / time));
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
        var to = origin + direction;
        Rect.anchoredPosition = from;
        yield return new WaitForSecondsRealtime(offset);
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
            timeLeft = Mathf.Max(0, timeLeft);
            Rect.anchoredPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0, 1, 1 - timeLeft / time));
        }

    }

}
