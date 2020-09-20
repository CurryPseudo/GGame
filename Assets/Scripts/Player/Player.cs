using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using Fixed = PlayerState.Fixed;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxPhysics))]
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public float xAcc;
    public float xRevAcc;
    public float xFri;
    public float xVelMax;
    public InputAction moveAction;
    public float yGrav;
    public float yVelMax;
    public LayerMask onGroundLayer;
    private Vector2Int moveInput = Vector2Int.zero;
    private int signDirectionX = 1;
    public Vector2 Velocity
    {
        get { return rigid.velocity; }
        set
        {
            if (value != rigid.velocity)
            {
                VelocityOnChange(value);
            }
            rigid.velocity = value;
        }
    }
    public float VelocityX
    {
        get { return rigid.velocity.x; }
        set
        {
            if (value != rigid.velocity.x)
            {
                VelocityOnChange(new Vector2(value, rigid.velocity.y));
            }
            rigid.velocity = new Vector2(value, rigid.velocity.y);
        }
    }
    public float VelocityY
    {
        get { return rigid.velocity.y; }
        set
        {
            if (value != rigid.velocity.y)
            {
                VelocityOnChange(new Vector2(rigid.velocity.x, value));
            }
            rigid.velocity = new Vector2(rigid.velocity.x, value);
        }
    }
    private void VelocityOnChange(Vector2 value)
    {
        if (value.x != 0)
        {
            var next = (int)Mathf.Sign(value.x);
            signDirectionX = next;
        }
    }

    [ShowInInspector]
    public Vector2Int MoveInput { get => moveInput; }
    [ShowInInspector]
    public int SignDirectionX { get => signDirectionX; }
    [ShowInInspector]


    [NonSerialized]
    public new PlayerAnimation animation;
    [NonSerialized]
    public BoxPhysics boxPhysics;
    private Rigidbody2D rigid;
    private FSM<Player> fsm;
    private FSM<Player> fsmFixed;
    void Awake()
    {
        animation = GetComponentInChildren<PlayerAnimation>();
        Assert.IsNotNull(animation);
        boxPhysics = GetComponentInChildren<BoxPhysics>();
        rigid = GetComponent<Rigidbody2D>();
        fsm = new FSM<Player>(this);
        fsmFixed = new FSM<Player>(this);
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
    }
    void OnEnable()
    {
        moveAction.Enable();
    }
    void OnDisable()
    {
        moveAction.Disable();
    }
    void Start()
    {
        fsmFixed.ChangeState(new Fixed.Drop());
    }
    void OnMove(CallbackContext context)
    {
        var moveFloat = context.ReadValue<Vector2>();
        moveInput.x = Mathf.Approximately(moveFloat.x, 0) ? 0 : Mathf.FloorToInt(Mathf.Sign(moveFloat.x));
        moveInput.y = Mathf.Approximately(moveFloat.y, 0) ? 0 : Mathf.FloorToInt(Mathf.Sign(moveFloat.y));
    }
    public void StepVelocity()
    {
        rigid.position += Velocity * Time.fixedDeltaTime;
    }
    void FixedUpdate()
    {
    }
}

namespace PlayerState
{
    namespace Fixed
    {
        public class Idle : State<Player>
        {
            public override IEnumerator Main()
            {
                var lastMoveInputX = mono.MoveInput.x;
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    if (mono.MoveInput.x == 0)
                    {
                        if (lastMoveInputX != 0)
                        {
                            mono.animation.StopRun();
                        }
                        mono.VelocityX = Mathf.Max(Mathf.Abs(mono.VelocityX) - mono.xFri * Time.fixedDeltaTime, 0) * Mathf.Sign(mono.VelocityX);
                    }
                    else
                    {
                        if (lastMoveInputX == 0)
                        {
                            mono.animation.BeginRun();
                        }
                        if (Mathf.Sign(mono.MoveInput.x) * Mathf.Sign(mono.VelocityX) >= 0)
                        {
                            mono.VelocityX += Mathf.Sign(mono.VelocityX) * mono.xAcc * Time.fixedDeltaTime;
                        }
                        else
                        {
                            mono.VelocityX -= Mathf.Sign(mono.VelocityX) * mono.xRevAcc * Time.fixedDeltaTime;
                        }

                    }
                    if (Mathf.Abs(mono.VelocityX) > mono.xVelMax)
                    {
                        mono.VelocityX = Mathf.Sign(mono.VelocityX) * mono.xVelMax;
                    }
                    mono.animation.SignDirectionX = mono.SignDirectionX;
                    mono.animation.RunningSpeed = Mathf.Abs(mono.VelocityX) / mono.xVelMax;
                    lastMoveInputX = mono.MoveInput.x;
                    {
                        var result = mono.boxPhysics.BlockMove(mono.onGroundLayer, Vector2Component.Y, -0.01f);
                        if (!result.HasValue)
                        {
                            fsm.ChangeState(new Drop());
                        }
                    }
                }
            }
        }
        public class Drop : State<Player>
        {
            public override IEnumerator Main()
            {
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    mono.VelocityY -= mono.yGrav * Time.fixedDeltaTime;
                    if (Mathf.Abs(mono.VelocityY) > mono.yVelMax)
                    {
                        mono.VelocityY = Mathf.Sign(mono.VelocityY) * mono.yVelMax;
                    }
                    {
                        var velDis = mono.VelocityY * Time.fixedDeltaTime;
                        var result = mono.boxPhysics.BlockMove(mono.onGroundLayer, Vector2Component.Y, velDis);
                        if (result.HasValue)
                        {
                            (var go, var dis) = result.Value;
                            mono.VelocityY = dis / Time.fixedDeltaTime;
                            mono.StepVelocity();
                            mono.VelocityY = 0;
                            fsm.ChangeState(new Idle());
                        }
                    }
                }
            }
        }

    }

}
