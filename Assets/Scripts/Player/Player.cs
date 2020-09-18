using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using Fixed = PlayerState.Fixed;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public float xAcc;
    public float xRevAcc;
    public float xFri;
    public float xVelMax;
    public Joystick joystick;
    private Vector2Int moveInput = Vector2Int.zero;
    private bool moveInputDirty = false;
    private Vector2Int lastMoveInput = Vector2Int.zero;
    private int signDirectionX = 1;
    private bool signDirectionXDirty = true;
    private int turnAroundSignDirectionX = 0;
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
            if (next != signDirectionX)
            {
                signDirectionXDirty = true;
            }
            signDirectionX = next;
        }
    }

    [ShowInInspector]
    public Vector2Int MoveInput { get => moveInput; }
    public int SignDirectionX { get => signDirectionX; }
    [ShowInInspector]
    public Vector2Int LastMoveInput { get => lastMoveInput; }
    public bool MoveInputDirty { get => moveInputDirty; }
    public bool SignDirectionXDirty { get => signDirectionXDirty; }
    public int TurnAroundSignDirectionX { get => turnAroundSignDirectionX; set => turnAroundSignDirectionX = value; }

    public void MoveInputClean() { moveInputDirty = false; }
    public void SignDirectionXClean() { signDirectionXDirty = false; }

    [NonSerialized]
    public new PlayerAnimation animation;
    private Rigidbody2D rigid;
    private FSM<Player> fsm;
    private FSM<Player> fsmFixed;
    void Awake()
    {
        animation = GetComponentInChildren<PlayerAnimation>();
        Assert.IsNotNull(animation);
        rigid = GetComponent<Rigidbody2D>();
        fsm = new FSM<Player>(this);
        fsmFixed = new FSM<Player>(this);
    }
    void Start()
    {
        fsmFixed.ChangeState(new Fixed.Idle());
    }
    void OnMove(InputValue value)
    {
        var moveFloat = value.Get<Vector2>();
        lastMoveInput = moveInput;
        moveInput.x = Mathf.Approximately(moveFloat.x, 0) ? 0 : Mathf.FloorToInt(Mathf.Sign(moveFloat.x));
        moveInput.y = Mathf.Approximately(moveFloat.y, 0) ? 0 : Mathf.FloorToInt(Mathf.Sign(moveFloat.y));
        moveInputDirty = true;
    }
    void Update()
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
                while (true)
                {
                    var monoMoveInput = mono.MoveInput;
                    monoMoveInput.x = (int) mono.joystick.Horizontal;
                    if (mono.MoveInput.x == 0)
                    {
                        if (mono.LastMoveInput.x != 0 && mono.MoveInputDirty)
                        {
                            mono.MoveInputClean();
                            mono.animation.StopRun();
                        }
                        var lastVelocityX = mono.VelocityX;
                        mono.VelocityX = Mathf.Max(Mathf.Abs(mono.VelocityX) - mono.xFri * Time.fixedDeltaTime, 0) * Mathf.Sign(mono.VelocityX);
                        if (mono.TurnAroundSignDirectionX != 0 && Mathf.Approximately(mono.VelocityX, 0) && Mathf.Sign(lastVelocityX) != mono.TurnAroundSignDirectionX)
                        {
                            mono.VelocityX = Mathf.Sign(lastVelocityX) * -1 * Mathf.Epsilon;
                            mono.TurnAroundSignDirectionX = 0;
                        }
                    }
                    else
                    {
                        if (mono.SignDirectionX * mono.MoveInput.x < 0 && mono.SignDirectionXDirty)
                        {
                            mono.SignDirectionXClean();
                            mono.animation.TurnAround(mono.SignDirectionX);
                            mono.TurnAroundSignDirectionX = (int)Mathf.Sign(mono.MoveInput.x);
                        }
                        if (mono.SignDirectionX * mono.MoveInput.x > 0)
                        {
                            mono.TurnAroundSignDirectionX = 0;
                        }
                        if (mono.LastMoveInput.x == 0 && mono.MoveInputDirty)
                        {
                            mono.MoveInputClean();
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
                    yield return new WaitForFixedUpdate();
                }
            }
        }

    }

}
