using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using Fixed = PlayerState.Fixed;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System;

[RequireComponent(typeof(PlayerAnimation), typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public float xAcc;
    public float xRevAcc;
    public float xFri;
    public float xVelMax;
    private Vector2Int moveInput = Vector2Int.zero;
    private bool moveInputDirty = false;
    private Vector2Int lastMoveInput = Vector2Int.zero;
    private int signDirectionX = 1;
    public Vector2 Velocity { get { return rigid.velocity; } set { rigid.velocity = value; } }
    public float VelocityX { get { return rigid.velocity.x; } set { rigid.velocity = new Vector2(value, rigid.velocity.y); } }
    public float VelocityY { get { return rigid.velocity.y; } set { rigid.velocity = new Vector2(rigid.velocity.x, value); } }

    [ShowInInspector]
    public Vector2Int MoveInput { get => moveInput; }
    public int SignDirectionX { get => signDirectionX; }
    [ShowInInspector]
    public Vector2Int LastMoveInput { get => lastMoveInput; }
    public bool MoveInputDirty { get => moveInputDirty; }
    public void MoveInputClean() { moveInputDirty = false; }

    [NonSerialized]
    public new PlayerAnimation animation;
    private Rigidbody2D rigid;
    private FSM<Player> fsm;
    private FSM<Player> fsmFixed;
    void Awake()
    {
        animation = GetComponent<PlayerAnimation>();
        rigid = GetComponent<Rigidbody2D>();
        fsm = new FSM<Player>(this);
        fsmFixed = new FSM<Player>(this);
    }
    void Start()
    {
        fsm.ChangeState(new Idle());
        fsmFixed.ChangeState(new Fixed.Idle());
    }
    void OnMove(InputValue value)
    {
        var moveFloat = value.Get<Vector2>();
        lastMoveInput = moveInput;
        moveInput.x = Mathf.Approximately(moveFloat.x, 0) ? 0 : Mathf.FloorToInt(Mathf.Sign(moveFloat.x));
        moveInput.y = Mathf.Approximately(moveFloat.y, 0) ? 0 : Mathf.FloorToInt(Mathf.Sign(moveFloat.y));
        if (MoveInput.x != 0)
        {
            signDirectionX = MoveInput.x;
        }
        moveInputDirty = true;
    }
    void Update()
    {
    }
}

namespace PlayerState
{
    public class Idle : State<Player>
    {
        public override IEnumerator Main()
        {
            while (true)
            {
                mono.animation.SetSignDirectionX(mono.SignDirectionX);
                yield return null;
            }
        }
    }
    namespace Fixed
    {
        public class Idle : State<Player>
        {
            public override IEnumerator Main()
            {
                while (true)
                {
                    if (mono.MoveInput.x == 0)
                    {
                        if (mono.LastMoveInput.x != 0 && mono.MoveInputDirty)
                        {
                            mono.MoveInputClean();
                            mono.animation.StopRun();
                        }
                        mono.VelocityX = Mathf.Max(Mathf.Abs(mono.VelocityX) - mono.xFri * Time.fixedDeltaTime, 0) * Mathf.Sign(mono.VelocityX);
                    }
                    else
                    {
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
                    yield return new WaitForFixedUpdate();
                }
            }
        }

    }

}
