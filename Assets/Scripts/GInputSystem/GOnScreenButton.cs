using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class GOnScreenButton : OnScreenControl, IPointerEnterHandler, IPointerExitHandler
{
    [InputControl(layout = "Button")]
    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SendValueToControl(1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SendValueToControl(0.0f);
    }

}
