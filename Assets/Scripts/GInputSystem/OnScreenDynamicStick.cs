using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class OnScreenDynamicStick : OnScreenControl
{
    private void FixedUpdate()
    {
        if (isPressed)
        {
            var dir = Position - pressedPosition;
            var rectTransform = GetComponent<RectTransform>();
            var scaledRange = rectTransform.TransformVector(Vector2.right * range).magnitude;
            if (dir.magnitude > scaledRange)
            {
                dir = dir.normalized * scaledRange;
                //pressedPosition = Position - dir;
            }
            SendValueToControl(dir / scaledRange);
            rectTransform.position = pressedPosition;
            handle.position = pressedPosition + dir;
        }
        else
        {
            SendValueToControl(Vector2.zero);
        }
    }


    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
    public InputAction positionAction;
    public InputAction pressAction;
    public RectTransform handle;
    public Image handleImage;
    public float range;
    private bool isPressed = false;
    private Vector2 pressedPosition = Vector2.zero;
    void Start()
    {
        pressAction.performed += (context) => OnPress();
        pressAction.canceled += (context) => OnRelease();
        positionAction.Enable();
        pressAction.Enable();
    }
    public Vector2 Position
    {
        get => positionAction.ReadValue<Vector2>();
    }
    public void OnPress()
    {
        isPressed = true;
        pressedPosition = Position;
        GetComponent<Image>().enabled = true;
        handleImage.enabled = true;
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.position = Position;
        handle.position = Position;
    }
    public void OnRelease()
    {
        isPressed = false;
        SendValueToControl(Vector2.zero);
        handleImage.enabled = false;
        GetComponentInChildren<Image>().enabled = false;
    }
}