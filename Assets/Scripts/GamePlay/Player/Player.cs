using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
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

public interface IDashPowerUI
{
    void SetDashPower(float value);
}

public class Player : Autonomy
{
    // Start is called before the first frame update
    public float fartVelMaxRatio;
    public XMoveParam idleMoveX;
    public XMoveParam dropMoveX;
    public InputAction moveAction;
    public InputAction dashAction;
    public float yGrav;
    public float yVelMax;
    public float dashTime;
    public float dashDistance;
    public float dashRemainSpeed;
    public float attackFrameDelay;
    public float zeroInputThreshold = 0;
    public float lightDashPowerAccBase = 0;
    public float lightDashPowerAccMultiply = 0;
    public float maxDashPower = 6;
    public LayerMask attackLayer;
    public bool isInCollision;
    public BoxPhysics attackBox;
    public event Action OnDashEvent;
    private Vector2Int moveInput = Vector2Int.zero;
    private int lastMoveInputX = 0;
    private float dashPower;
    private HashSet<IGLight> inLights = new HashSet<IGLight>();
    public void UpdateInLights(IGLight light, bool isIn)
    {
        if (inLights.Contains(light))
        {
            if (!isIn)
            {
                inLights.Remove(light);
            }
        }
        else
        {
            if (isIn)
            {
                inLights.Add(light);
            }

        }
    }
    public float DashPower
    {
        get => dashPower;
        set
        {
            dashPower = value;
            if (dashPower > maxDashPower)
            {
                dashPower = maxDashPower;
            }
            SceneSingleton.Get<IDashPowerUI>().SetDashPower(dashPower);
        }
    }

    [ShowInInspector]
    public Vector2Int MoveInput { get => moveInput; }

    [NonSerialized]
    public new PlayerAnimation animation;
    private FSM<Player> mainFsm;
    protected override void Awake()
    {
        base.Awake();
        animation = GetComponentInChildren<PlayerAnimation>();
        Assert.IsNotNull(animation);
        Assert.IsNotNull(moveBox);
        Assert.IsNotNull(attackBox);
        mainFsm = new FSM<Player>(this);
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
        mainFsm.ChangeState(new Drop());
        DashPower = maxDashPower;
    }
    void FixedUpdate()
    {
        isInCollision = moveBox.InBoxCollision(blockLayer, null) != null;
        if (inLights.Count > 0)
        {
            DashPower += lightDashPowerAccBase * Time.fixedDeltaTime;
            DashPower += lightDashPowerAccMultiply * inLights.Count * Time.fixedDeltaTime;
        }
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
        if (moveFloat.magnitude < zeroInputThreshold)
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
    public void MoveX(XMoveParam param, bool onGround)
    {
        bool notFartSpeed = Mathf.Abs(VelocityX) < param.xVelMax * fartVelMaxRatio;
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
        if (notFartSpeed && onGround && Mathf.Abs(VelocityX) > param.xVelMax * fartVelMaxRatio)
        {
            animation.RunFart((int)(Mathf.Sign(MoveInput.x)));
        }

        animation.SignDirectionX = SignDirectionX;
        animation.RunningSpeed = Mathf.Abs(VelocityX) / param.xVelMax;
        BlockMoveX();
        lastMoveInputX = MoveInput.x;
    }
}

namespace PlayerState
{
    public class Idle : State<Player>
    {
        void OnDash()
        {
            if (mono.DashPower >= 1)
            {
                mono.DashPower -= 1;
                fsm.ChangeState(new Dash(true));
            }
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
                mono.MoveX(mono.idleMoveX, true);
                {
                    if (!mono.IsOnGround)
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
            if (mono.DashPower >= 1)
            {
                mono.DashPower -= 1;
                fsm.ChangeState(new Dash(false));
            }
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
                mono.MoveX(mono.dropMoveX, false);
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
        private bool onGroundWhileDash;
        public Dash(bool onGroundWhileDash)
        {
            this.onGroundWhileDash = onGroundWhileDash;
        }
        private HashSet<IPlayerAttackable> attacked;
        public override IEnumerator Main()
        {
            attacked = new HashSet<IPlayerAttackable>();
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
                mono.StartCoroutine(BlockMove(dirInt));
                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
            mono.Velocity = mono.dashRemainSpeed * dir;
            mono.StartCoroutine(BlockMove(dirInt));
            if (!mono.IsOnGround)
            {
                fsm.ChangeState(new Drop());
            }
            else
            {
                fsm.ChangeState(new Idle());
                if (!onGroundWhileDash)
                {
                    mono.animation.OnGround();
                }
            }
            yield break;
        }
        public IEnumerator BlockMove(Vector2Int dirInt)
        {
            mono.BlockMoveY();
            mono.BlockMoveX();
            var attackableGo = mono.attackBox.InBoxCollision(mono.attackLayer, (go) =>
            {
                var attackable = go.GetComponentInParent<IPlayerAttackable>();
                return attackable != null && attackable.validBox(go.GetComponent<BoxPhysics>());
            });
            if (attackableGo != null)
            {
                var attackable = attackableGo.GetComponentInParent<IPlayerAttackable>();
                if (!attacked.Contains(attackable))
                {
                    attacked.Add(attackable);
                    Time.timeScale = 0;
                    attackable.OnAttack();
                    mono.animation.Attack(dirInt);
                    yield return new WaitForSecondsRealtime(mono.attackFrameDelay);
                    Time.timeScale = 1;
                }
            }
            yield break;
        }

    }
}
