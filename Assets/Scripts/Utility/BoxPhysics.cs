using System;
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

    public Vector2 Origin
    {
        get => transform.TransformPoint(box.offset);
    }
    public Vector2 Size
    {
        get => transform.TransformVector(box.size);
    }
    public BoxCollider2D Box { get => box; }

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    Vector2 BoxEdgeOrigin(Vector2Component component, float sgn)
    {
        var other = component.Other();
        Vector2 origin = transform.TransformPoint(component.Create(
            component.Get(box.offset) + component.Get(box.size) / 2.0f * sgn,
            other.Get(box.offset)));
        return origin;
    }
    Vector2 Dir(Vector2Component component, float sgn)
    {
        return transform.TransformDirection(component.Create(sgn, 0));
    }

    public (GameObject, float)? BlockMove(LayerMask layer, Vector2Component component, float distance, Func<GameObject, bool> isValid = null)
    {
        const float disEpsilon = 0.01f;
        if (Mathf.Approximately(distance, 0f))
        {
            return null;
        }
        var other = component.Other();
        float sgn = Mathf.Sign(distance);
        Vector2 dir = Dir(component, sgn);
        var hits = Physics2D.BoxCastAll(Origin, Size, 0, dir, Mathf.Abs(distance), layer);
        foreach (var hit in hits)
        {
            if (hit)
            {
                if (isValid == null || isValid(hit.collider.gameObject))
                {
                    return (hit.collider.gameObject, (hit.distance - Mathf.Abs(disEpsilon)) * sgn);
                }
            }
        }
        return null;
    }
    public GameObject InBoxCollision(LayerMask layer, Func<GameObject, bool> isValid = null)
    {
        foreach (var go in InBoxCollisionAll(layer, isValid))
        {
            return go;
        }
        return null;
    }
    public IEnumerable<T> InBoxCollisionMapAllFlatten<T>(LayerMask layer, Func<GameObject, IEnumerable<T>> map)
    {
        foreach (var ts in InBoxCollisionMapAll(layer, map))
        {
            foreach (var t in ts)
            {
                yield return t;
            }
        }
    }
    public IEnumerable<T> InBoxCollisionMapAll<T>(LayerMask layer, Func<GameObject, T> map)
    {
        var size = Size * 0.9f;
        var colliders = Physics2D.OverlapBoxAll(Origin, size, 0, layer);
        foreach (var collider in colliders)
        {
            var go = collider.gameObject;
            var t = map(go);
            if (t != null)
            {
                yield return t;
            }
        }
        yield break;
    }
    public IEnumerable<GameObject> InBoxCollisionAll(LayerMask layer, Func<GameObject, bool> isValid = null)
    {
        var size = Size * 0.9f;
        var colliders = Physics2D.OverlapBoxAll(Origin, size, 0, layer);
        foreach (var collider in colliders)
        {
            var go = collider.gameObject;
            if (isValid == null || isValid(go))
            {
                yield return go;
            }
        }
        yield break;
    }
    void OnDrawGizmosSelected()
    {
        if (box == null)
        {
            box = GetComponent<BoxCollider2D>();
        }
        Gizmos.color = Color.yellow;
        Vector2Component component = debugIsX ? Vector2Component.X : Vector2Component.Y;
        float sgn = Mathf.Sign(debugDistance);
        Vector2 dir = Dir(component, sgn);
        Vector2 lineOrigin = BoxEdgeOrigin(component, sgn);
        var result = BlockMove(debugLayer, component, debugDistance);
        if (result.HasValue)
        {
            (var go, var dis) = result.Value;
            Gizmos.DrawLine(lineOrigin, lineOrigin + dir * Mathf.Abs(dis));
        }
        else
        {
            Gizmos.DrawLine(lineOrigin, lineOrigin + dir * Mathf.Abs(debugDistance));
        }
    }

}
