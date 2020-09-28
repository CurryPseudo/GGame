using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestToucher : Toucher
{
    public Text text;
    private bool isPressed;
    public override void OnPress()
    {
        isPressed = true;
    }

    public override void OnRelease()
    {
        isPressed = false;
        text.text = "Release" + Position.ToString();
    }
    void Update()
    {
        if (isPressed)
        {
            text.text = "Press" + Position.ToString();
        }
    }

    protected override string controlPathInternal { get; set; }
}
