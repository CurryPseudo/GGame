using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightIntensityFadeIn : MonoBehaviour
{
    public float prepareTime;
    public float time;
    void Start()
    {
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        var light = GetComponent<Light2D>();
        var initIntensity = light.intensity;
        var timeLeft = time;
        light.intensity = 0;
        yield return new WaitForSeconds(prepareTime);
        while (timeLeft > 0)
        {
            yield return null;
            timeLeft -= Time.deltaTime;
            if (timeLeft > 0)
            {
                light.intensity = (1 - timeLeft / time) * initIntensity;
            }
            else
            {
                light.intensity = initIntensity;
            }
        }
        yield break;
    }
}
