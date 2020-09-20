using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BoxPhysics : MonoBehaviour
{
    private BoxCollider2D box;
    public bool debugIsX;
    public float debugDistance;
    public LayerMask debugLayer;

    public BoxCollider2D Box { get => box; }

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    IEnumerable<(Vector2, Vector2)> BlockMoveLine(Vector2Component component, float sgn)
    {
        var other = component.Other();
        for (int i = 0; i < 2; i++)
        {
            Vector2 dir = transform.TransformDirection(component.Create(sgn, 0));
            Vector2 origin = transform.TransformPoint(component.Create(
                component.Get(box.offset) + component.Get(box.size) / 2.0f * sgn,
                other.Get(box.offset) + other.Get(box.size) * (i - 0.5f)));
            yield return (origin, dir);
        }
        yield break;
    }

    public (GameObject, float)? BlockMove(LayerMask layer, Vector2Component component, float distance, float disEpsilon = 0.001f)
    {
        if (Mathf.Approximately(distance, 0f))
        {
            return null;
        }
        var other = component.Other();
        foreach ((Vector2 origin, Vector2 dir) in BlockMoveLine(component, Mathf.Sign(distance)))
        {
            var hit = Physics2D.Raycast(origin, dir, Mathf.Abs(distance), layer);
            if (hit)
            {
                return (hit.collider.gameObject, Mathf.Max(hit.distance - disEpsilon, 0) * Mathf.Sign(distance));
            }

        }
        return null;
    }
    void OnDrawGizmosSelected()
    {
        if (box == null)
        {
            box = GetComponent<BoxCollider2D>();
        }
        Gizmos.color = Color.yellow;
        Vector2Component component = debugIsX ? Vector2Component.X : Vector2Component.Y;
        foreach ((Vector2 origin, Vector2 dir) in BlockMoveLine(component, Mathf.Sign(debugDistance)))
        {
            var result = BlockMove(debugLayer, component, debugDistance);
            if (result.HasValue)
            {
                (var go, var dis) = result.Value;
                Gizmos.DrawLine(origin, origin + dir * Mathf.Abs(dis));
            }
            else
            {
                Gizmos.DrawLine(origin, origin + dir * Mathf.Abs(debugDistance));
            }
        }
    }

}
