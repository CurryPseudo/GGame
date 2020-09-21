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

[Serializable]
public struct XMoveParam
{
    public float xAcc;
    public float xRevAcc;
    public float xFri;
    public float xVelMax;
}

[RequireComponent(typeof(BoxPhysics))]
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public XMoveParam idleMoveX;
    public XMoveParam dropMoveX;
    public InputAction moveAction;
    public InputAction dashAction;
    public float yGrav;
    public float yVelMax;
    public float dashTime;
    public float dashDistance;
    public float dashRemainSpeed;
    public LayerMask onGroundLayer;
    public bool isInCollision;
    public event Action OnDashEvent;
    private Vector2Int moveInput = Vector2Int.zero;
    private int lastMoveInputX = 0;
    private int signDirectionX = 1;
    private Vector2 velocity;
    public float PositionX
    {
        get { return transform.position.x; }
        set
        {
            transform.position = new Vector3(value, transform.position.y, transform.position.z);
        }
    }
    public float PositionY
    {
        get { return transform.position.y; }
        set
        {
            transform.position = new Vector3(transform.position.x, value, transform.position.z);
        }
    }
    public Vector2 Velocity
    {
        get { return velocity; }
        set
        {
            if (value != velocity)
            {
                VelocityOnChange(value);
            }
            velocity = value;
        }
    }
    public float VelocityX
    {
        get { return Velocity.x; }
        set
        {
            if (value != Velocity.x)
            {
                VelocityOnChange(new Vector2(value, Velocity.y));
            }
            Velocity = new Vector2(value, Velocity.y);
        }
    }
    public float VelocityY
    {
        get { return Velocity.y; }
        set
        {
            if (value != Velocity.y)
            {
                VelocityOnChange(new Vector2(Velocity.x, value));
            }
            Velocity = new Vector2(Velocity.x, value);
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
    private FSM<Player> fsm;
    private FSM<Player> fsmFixed;
    void Awake()
    {
        animation = GetComponentInChildren<PlayerAnimation>();
        Assert.IsNotNull(animation);
        boxPhysics = GetComponentInChildren<BoxPhysics>();
        fsm = new FSM<Player>(this);
        fsmFixed = new FSM<Player>(this);
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        dashAction.performed += OnDash;
    }
    void OnEnable()
    {
        moveAction.Enable();
        dashAction.Enable();
    }
    void OnDisable()
    {
        moveAction.Disable();
        dashAction.Disable();
    }
    void Start()
    {
        fsmFixed.ChangeState(new Fixed.Drop());
    }
    void FixedUpdate()
    {
        isInCollision = boxPhysics.InBoxCollision(onGroundLayer, null) != null;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var target = transform.position + Vector3.right * dashDistance;
        Gizmos.DrawLine(transform.position, target);
    }
    void OnMove(CallbackContext context)
    {
        var moveFloat = context.ReadValue<Vector2>();
        if (Mathf.Approximately(moveFloat.magnitude, 0))
        {
            moveInput = new Vector2Int(0, 0);
            return;
        }
        moveFloat.Normalize();
        var angle = Vector2.SignedAngle(Vector2.right, moveFloat);
        Vector2Int[] choose = {
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
        };
        var index = Mathf.FloorToInt((angle + 22.5f) / 45f) + 3;
        if (index == -1)
        {
            index = 7;
        }
        moveInput = choose[index];
    }
    void OnDash(CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            OnDashEvent?.Invoke();
        }
    }
    public bool BlockMoveX()
    {
        var velDis = VelocityX * Time.fixedDeltaTime;
        if (boxPhysics.InBoxCollision(onGroundLayer, null))
        {
            PositionX += velDis;
            return false;
        }
        var result = boxPhysics.BlockMove(onGroundLayer, Vector2Component.X, velDis);
        if (result.HasValue)
        {
            (var go, var dis) = result.Value;
            PositionX += dis;
            VelocityX = 0;
            return true;
        }
        PositionX += velDis;
        return false;
    }
    public bool BlockMoveY()
    {
        var velDis = VelocityY * Time.fixedDeltaTime;
        if (boxPhysics.InBoxCollision(onGroundLayer, null))
        {
            PositionY += velDis;
            return false;
        }
        var result = boxPhysics.BlockMove(onGroundLayer, Vector2Component.Y, velDis);
        if (result.HasValue)
        {
            (var go, var dis) = result.Value;
            PositionY += dis;
            VelocityY = 0;
            return true;
        }
        PositionY += velDis;
        return false;
    }
    public void MoveX(XMoveParam param)
    {
        if (MoveInput.x == 0)
        {
            if (lastMoveInputX != 0)
            {
                animation.StopRun();
            }
            VelocityX = Mathf.Max(Mathf.Abs(VelocityX) - param.xFri * Time.fixedDeltaTime, 0) * Mathf.Sign(VelocityX);
        }
        else
        {
            if (lastMoveInputX == 0)
            {
                animation.BeginRun();
            }
            if (Mathf.Sign(MoveInput.x) * Mathf.Sign(VelocityX) >= 0)
            {
                VelocityX += Mathf.Sign(VelocityX) * param.xAcc * Time.fixedDeltaTime;
            }
            else
            {
                VelocityX -= Mathf.Sign(VelocityX) * param.xRevAcc * Time.fixedDeltaTime;
            }

        }
        if (Mathf.Abs(VelocityX) > param.xVelMax)
        {
            VelocityX = Mathf.Sign(VelocityX) * param.xVelMax;
        }

        animation.SignDirectionX = SignDirectionX;
        animation.RunningSpeed = Mathf.Abs(VelocityX) / param.xVelMax;
        BlockMoveX();
        lastMoveInputX = MoveInput.x;
    }
    public void StepVelocityX()
    {
        PositionX += VelocityX * Time.fixedDeltaTime;
    }
    public void StepVelocityY()
    {
        PositionY += VelocityY * Time.fixedDeltaTime;
    }
}

namespace PlayerState
{
    namespace Fixed
    {
        public class Idle : State<Player>
        {
            void OnDash()
            {
                fsm.ChangeState(new Dash());
            }
            public override void Exit()
            {
                mono.OnDashEvent -= OnDash;
            }
            public override IEnumerator Main()
            {
                mono.OnDashEvent += OnDash;
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    mono.MoveX(mono.idleMoveX);
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

            void OnDash()
            {
                fsm.ChangeState(new Dash());
            }
            public override void Exit()
            {
                mono.OnDashEvent -= OnDash;
            }

            public override IEnumerator Main()
            {
                mono.OnDashEvent += OnDash;
                mono.animation.Drop();
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    mono.MoveX(mono.dropMoveX);
                    mono.VelocityY -= mono.yGrav * Time.fixedDeltaTime;
                    if (Mathf.Abs(mono.VelocityY) > mono.yVelMax)
                    {
                        mono.VelocityY = Mathf.Sign(mono.VelocityY) * mono.yVelMax;
                    }
                    var down = mono.VelocityY < 0;
                    if (mono.BlockMoveY() && down)
                    {
                        fsm.ChangeState(new Idle());
                        mono.animation.OnGround();
                        yield break;
                    }
                }
            }

        }
        public class Dash : State<Player>
        {
            public override IEnumerator Main()
            {
                var dashSpeed = mono.dashDistance / mono.dashTime;
                Vector2Int dirInt = mono.MoveInput;
                if (dirInt == Vector2Int.zero)
                {
                    dirInt = Vector2Int.right * mono.SignDirectionX;
                }
                var dir = ((Vector2)dirInt).normalized;
                var vel = dir * dashSpeed;
                float time = 0;
                mono.animation.Dash(dirInt);
                while (time < mono.dashTime)
                {
                    mono.Velocity = vel;
                    mono.animation.SignDirectionX = mono.SignDirectionX;
                    BlockMove();
                    yield return new WaitForFixedUpdate();
                    time += Time.fixedDeltaTime;
                }
                mono.Velocity = mono.dashRemainSpeed * dir;
                BlockMove();
                fsm.ChangeState(new Drop());
                yield break;
            }
            public void BlockMove()
            {
                if (mono.VelocityY > 0)
                {
                    mono.BlockMoveY();
                }
                else
                {
                    mono.StepVelocityY();
                }
                mono.BlockMoveX();

            }

        }

    }

}
