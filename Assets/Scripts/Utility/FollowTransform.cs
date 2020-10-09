using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FollowTransform : MonoBehaviour
{
    public Transform followed;
    public bool lockX;
    public bool lockY;
    void Start()
    {
        Update();
    }
    void Update()
    {
        if (followed != null)
        {
            var position = transform.position;
            if (!lockX)
            {
                position.x = followed.transform.position.x;
            }
            if (!lockY)
            {
                position.y = followed.transform.position.y;
            }
            transform.position = position;
        }
    }
}
