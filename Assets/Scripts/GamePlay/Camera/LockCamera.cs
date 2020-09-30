using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LockCamera : MonoBehaviour
{
    public Transform lockPoint;
    private BoxCollider2D detectBox;
    public bool lockX;
    public bool lockY;
    private Vector2 Origin
    {
        get => transform.TransformPoint(detectBox.offset);
    }
    private Vector2 Size
    {
        get => transform.TransformVector(detectBox.size);
    }
    private bool lastInRect = false;
    void Awake()
    {
        detectBox = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        var player = SceneSingleton.Get<Player>().transform;
        var position = lockPoint.position;
        var camera = SceneSingleton.Get<CinemachineVirtualCamera>();
        if (!lockX)
        {
            position.x = player.transform.position.x;
        }
        else if (!lockY)
        {
            position.y = player.transform.position.y;
        }
        lockPoint.position = position;
        Rect rect = new Rect(Origin - Size / 2, Size);
        if (rect.Contains(player.position))
        {
            camera.Follow = lockPoint;
            lastInRect = true;
        }
        else if (lastInRect)
        {
            camera.Follow = player;
            lastInRect = false;
        }
    }
}
