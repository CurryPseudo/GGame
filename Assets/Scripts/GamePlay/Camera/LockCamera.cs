using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LockCamera : MonoBehaviour, IBoxDetectGuest<Player>
{
    public Transform lockPoint;
    public bool lockX;
    public bool lockY;
    private BoxCollider2D DetectBox
    {
        get => GetComponent<BoxCollider2D>();
    }
    private Vector2 Origin
    {
        get => transform.TransformPoint(DetectBox.offset);
    }
    private Vector2 Size
    {
        get => transform.TransformVector(DetectBox.size);
    }
    private Transform lastFollow = null;
    void UpdateLockPoint(Player player)
    {
        var position = lockPoint.position;
        if (!lockX)
        {
            position.x = player.transform.position.x;
        }
        else if (!lockY)
        {
            position.y = player.transform.position.y;
        }
        lockPoint.position = position;

    }
    void Update()
    {

        UpdateLockPoint(SceneSingleton.Get<Player>());
    }

    public void Enter(Player t)
    {
        var camera = SceneSingleton.Get<CinemachineVirtualCamera>();
        lastFollow = camera.Follow;
        camera.Follow = lockPoint;
    }

    public void Stay(Player t)
    {
    }

    public void Exit(Player t)
    {
        var camera = SceneSingleton.Get<CinemachineVirtualCamera>();
        camera.Follow = lastFollow;
    }
}
