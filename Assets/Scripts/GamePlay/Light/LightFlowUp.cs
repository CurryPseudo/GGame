using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlowUp : MonoBehaviour
{
    public float flowDis;
    public float flowTime;
    void Start()
    {
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        var timeLeft = flowTime;
        Vector2 position = transform.position;
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.deltaTime;
            if (timeLeft > 0)
            {
                transform.position = position + Vector2.up * flowDis * Mathf.SmoothStep(0, 1, 1 - timeLeft / flowTime);
            }
            else
            {
                transform.position = position + Vector2.up * flowDis;
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.up * flowDis);
    }
}
