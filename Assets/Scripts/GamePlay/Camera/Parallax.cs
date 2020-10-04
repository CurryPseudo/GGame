using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Vector2 cameraOrigin;
    private Vector2 origin;
    public float followRatio;
    void Start()
    {
        cameraOrigin = Camera.main.transform.position;
        origin = transform.position;
    }
    void Update()
    {
        transform.position = origin + (Camera.main.transform.position.y - cameraOrigin.y) * followRatio * Vector2.up;
    }
}
