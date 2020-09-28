using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public abstract class Toucher: OnScreenControl
{
    public InputAction positionAction;

    public InputAction pressAction;
    void Awake()
    {
        pressAction.performed += (context) => OnPress();
        pressAction.canceled += (context) => OnRelease();
    }
    void OnEnable()
    {
        positionAction.Enable();
        pressAction.Enable();
    }
    void OnDisable()
    {
        positionAction.Disable();
        pressAction.Disable();
    }
    public Vector2 Position
    {
        get => positionAction.ReadValue<Vector2>();
    }
    public abstract void OnPress();
    public abstract void OnRelease();
}
