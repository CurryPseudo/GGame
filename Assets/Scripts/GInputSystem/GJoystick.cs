using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Input/G-On-Screen Parent Stick")]
public class GJoystick : Toucher
{
    public RectTransform rectTrans;
    public GOnScreenStick childStick;
    private bool isPressed;
    private bool isPosChanged;
    private Vector2 m_startPos;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(rectTrans.rect.height);
        Vector2 vec2 = new Vector2(rectTrans.rect.width / 2, rectTrans.rect.height / 2);
        m_startPos = transform.position;
        m_startPos += vec2;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed && !isPosChanged)
        {
            transform.position = Position - m_startPos;
            isPosChanged = true;
            //GOnScreenStick child = GetComponentInChildren<GOnScreenStick>();
            //PointerEventData eventData = new PointerEventData(EventSystem.current);
            //child.OnDrag(eventData);
        }
        
        // var delta = Vector3.Distance(childStick.transform.position, transform.position);
        // Debug.Log("child pos is " + childStick.transform.position + " parent pos is " + transform.position + " delta is " + delta);
        // if (delta > 500.0f)
        // {
        //     Debug.Log("in this");
        //     Vector3 diff = transform.position - childStick.transform.position;
        //     diff.z = 0.0f; // ignore Y (as requested in question)
        //     transform.position = childStick.transform.position + diff.normalized * 200.0f;
        // }
        
        //if (childTransform.rect.position > )
    }

    protected override string controlPathInternal { get; set; }
    public override void OnPress()
    {
        isPressed = true;
    }

    public override void OnRelease()
    {
        isPressed = false;
        isPosChanged = false;
    }
}
