using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;


[AddComponentMenu("Input/G-Dynamic Stick")]
public class DynamicStickHandle : OnScreenControl
{
    public DynamicJoystick dynamicJoystick;
    private void Update()
    {
        if (dynamicJoystick.m_isPointerDown)
        {
            SendValueToControl(dynamicJoystick.input);
        }
        else
        {
            SendValueToControl(Vector2.zero);
        }
    }
    
    
    [InputControl(layout = "Vector2")] [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

}
