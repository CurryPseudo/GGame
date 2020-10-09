using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class ButtonUI : MonoBehaviour
{
    public Sprite Pressed;
    public Sprite Released;
    public Image Image
    {
        get => GetComponent<Image>();
    }
    public InputAction action;
    public void OnEnable()
    {
        action.Enable();
    }
    public void OnDisable()
    {
        action.Disable();
    }
    void Awake()
    {
        action.performed += Press;
        action.canceled += Release;
    }
    void Press(CallbackContext context)
    {
        Image.sprite = Pressed;
    }
    void Release(CallbackContext context)
    {
        Image.sprite = Released;
    }
}
