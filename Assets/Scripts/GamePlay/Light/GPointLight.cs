using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GPointLight : MonoBehaviour, IGLight
{
    public LayerMask blockLayer;
    void FixedUpdate()
    {
        var light = GetComponent<Light2D>();
        if (light.lightType == Light2D.LightType.Point)
        {
            Vector2 origin = transform.position;
            var player = SceneSingleton.Get<Player>();
            Vector2 target = player.transform.position;
            Vector2 dir = target - origin;
            if (!Physics2D.Raycast(origin, dir.normalized, dir.magnitude, blockLayer) && dir.magnitude < light.pointLightOuterRadius)
            {
                Vector2 localDir = transform.InverseTransformDirection(dir);
                var angle = Vector2.Angle(Vector2.up, localDir);
                if (angle * 2 < light.pointLightOuterAngle)
                {
                    player.UpdateInLights(this, true);
                    return;
                }
            }
            player.UpdateInLights(this, false);
        }
    }
}
