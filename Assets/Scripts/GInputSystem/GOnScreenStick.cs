﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

////TODO: custom icon for OnScreenStick component


/// <summary>
/// A stick control displayed on screen and moved around by touch or other pointer
/// input.
/// </summary>
[AddComponentMenu("Input/G-On-Screen Stick")]
public class GOnScreenStick : Toucher, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool isPressed;
    public override void OnPress()
    {
        isPressed = true;
    }

    public override void OnRelease()
    {
        isPressed = false;
    }
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(),
            eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(),
            eventData.position, eventData.pressEventCamera, out var position);
        var delta = position - m_PointerDownPos;

        //delta = Vector2.ClampMagnitude(delta, movementRange);
        ((RectTransform) transform).anchoredPosition = m_StartPos + (Vector3) delta;

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        SendValueToControl(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ((RectTransform) transform).anchoredPosition = m_StartPos;
        SendValueToControl(Vector2.zero);
    }

    private void Start()
    {
        m_StartPos = ((RectTransform) transform).anchoredPosition;
    }

    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    [FormerlySerializedAs("movementRange")] [SerializeField]
    private float m_MovementRange = 100;

    [InputControl(layout = "Vector2")] [SerializeField]
    private string m_ControlPath;

    private Vector3 m_StartPos;
    private Vector2 m_PointerDownPos;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}