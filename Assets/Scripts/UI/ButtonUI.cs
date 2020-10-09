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
        get => image;
    }
    private Image image;
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
        image = GetComponent<Image>();
        action.performed += Press;
        action.canceled += Release;
    }
    void SetSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Image.enabled = false;
        }
        else
        {
            Image.enabled = true;
            Image.sprite = sprite;
        }

    }
    void Press(CallbackContext context)
    {
        SetSprite(Pressed);
    }
    void Release(CallbackContext context)
    {
        SetSprite(Released);
    }
}
