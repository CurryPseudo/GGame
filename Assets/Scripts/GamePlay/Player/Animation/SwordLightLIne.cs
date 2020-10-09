using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLightLIne : MonoBehaviour
{
    public float fromDistance;
    public float toDistance;
    public float time;
    void Start()
    {
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        var from = transform.TransformPoint(Vector2.right * fromDistance);
        var to = transform.TransformPoint(Vector2.left * toDistance);
        transform.position = from;
        var timeLeft = time;
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
            timeLeft = Mathf.Max(0, timeLeft);
            transform.position = Vector3.Lerp(from, to, 1 - timeLeft / time);
        }
        GameObject.Destroy(gameObject);
    }
}
