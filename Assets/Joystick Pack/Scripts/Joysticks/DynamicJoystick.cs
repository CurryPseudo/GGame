using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

public class DynamicJoystick : Joystick
{
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }
    public bool m_isPointerDown;

    [SerializeField] 
    private float moveThreshold = 1;
    
    [InputControl(layout = "Vector2")] [SerializeField]
    private string m_ControlPath;
    
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    protected override void Start()
    {
        MoveThreshold = moveThreshold;
        base.Start();
        //background.gameObject.SetActive(false);
        background.gameObject.SetActive(true);
        m_StartPos = background.anchoredPosition;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        //background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
        m_isPointerDown = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
        background.anchoredPosition = m_StartPos;
        m_isPointerDown = false;
        
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }

    private Vector3 m_StartPos;
    
}