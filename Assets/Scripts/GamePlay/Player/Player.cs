using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;
using Cinemachine;

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
    float DashPower
    {
        set;
    }
}

public class Player : Autonomy
{
    // Start is called before the first frame update
    public float fartVelMaxRatio;
    public XMoveParam idleMoveX;
    public XMoveParam dropMoveX;
    public InputAction moveAction;
    public InputAction dashDirAction;
    public float yGrav;
    public float yVelMax;
    public float dashTime;
    public float dashDistance;
    public float dashRemainSpeed;
    public float attackFrameDelay;
    public float zeroInputThreshold = 0;
    public AnimationCurve lightDashPowerAccBase;
    public AnimationCurve lightDashPowerAccMultiply;
    public bool resetPoweringWhileDash;
    public float maxDashPower = 6;
    public float restoreDashPowerAfterKill;
    public float dashAfterDamageDelayTime;
    public float damageInvincibleTime;
    public float parriedInvincibleTime;
    public float attackInvincibleTime;
    public float dieTime;
    public float bornTime;
    public float attackAmplitude;
    public float attackFrequency;
    public float damageAmplitude;
    public float damageFrequency;
    public float bounceUpDis;
    public float lastDashPowerRestoreTime;
    public float onDamageFrameDelay;
    [ShowInInspector]
    public float BounceUpVel
    {
        get
        {
            return Mathf.Sqrt(2 * yGrav * bounceUpDis);
        }
    }
    public Vector2 damageVelDrop;
    public Vector2 damageVelIdle;
    public Vector2 damageVelParried;
    public LayerMask attackLayer;
    public LayerMask damageLayer;
    public LayerMask detectPlayerLayer;
    public bool isInCollision;
    public BoxPhysics attackBox;
    public BoxPhysics damageBox;
    public static LayerMask DetectPlayerLayer
    {
        get
        {
            var player = SceneSingleton.Get<Player>();
            return player.detectPlayerLayer;
        }
    }
    public PlayerInputQueue InputQueue
    {
        get => GetComponent<PlayerInputQueue>();
    }
    private Vector2Int dashDirInput = Vector2Int.zero;
    [ShowInInspector]
    private int moveInput;
    private int lastMoveInputX = 0;
    private float dashPower;
    private HashSet<IGLight> inLights = new HashSet<IGLight>();
    private float poweringTime = 0;
    private float lastPowerRestoreTimeLeft = 0;
    private bool pausePowering = false;
    private bool invincible = false;
    public bool dashable = true;
    public void SetInvincibleTime(float time, bool playAnimation)
    {
        invincible = true;
        if (playAnimation)
        {
            animation.Invincible();
        }
        StartCoroutine(InvincibleCoroutine(time, playAnimation));
    }
    private IEnumerator InvincibleCoroutine(float time, bool playAnimation)
    {
        yield return new WaitForSeconds(time);
        invincible = false;
        if (playAnimation)
        {
            animation.AfterInvincible();
        }
    }
    public void ResetPoweringTime()
    {
        poweringTime = 0;
    }
    public void PausePowering()
    {
        pausePowering = true;
    }
    public void ContinuePowering()
    {
        pausePowering = false;
    }
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
            var last = dashPower;
            dashPower = value;
            if (dashPower < 1)
            {
                lastPowerRestoreTimeLeft = lastDashPowerRestoreTime;
            }
            if (dashPower > maxDashPower)
            {
                dashPower = maxDashPower;
            }
            SceneSingleton.Get<IDashPowerUI>().DashPower = dashPower;
        }
    }

    [ShowInInspector]
    public Vector2Int DashDirInput { get => dashDirInput; }

    [NonSerialized]
    public new PlayerAnimation animation;
    [ShowInInspector]
    public String CurrentState
    {
        get
        {
            if (mainFsm != null && mainFsm.Current != null)
            {
                return mainFsm.Current.ToString();
            }
            else
            {
                return "null";
            }
        }
    }

    private FSM<Player, PlayerState> mainFsm;
    protected override void Awake()
    {
        base.Awake();
        animation = GetComponentInChildren<PlayerAnimation>();
        Assert.IsNotNull(animation);
        Assert.IsNotNull(moveBox);
        Assert.IsNotNull(attackBox);
        mainFsm = new FSM<Player, PlayerState>(this);
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        dashDirAction.performed += OnDashDir;
        dashDirAction.canceled += OnDashDir;
    }
    void OnEnable()
    {
        moveAction.Enable();
        dashDirAction.Enable();
    }
    void OnDisable()
    {
        moveAction.Disable();
        dashDirAction.Disable();
    }
    void Start()
    {
        var currentCheckPoint = CheckPoint.Current;
        if (currentCheckPoint != null)
        {
            transform.position = currentCheckPoint.Point;
        }
        mainFsm.ChangeState(new Born());
        DashPower = maxDashPower;
    }
    void FixedUpdate()
    {
        isInCollision = moveBox.InBoxCollision(blockLayer, null) != null;
        if (inLights.Count > 0)
        {
            DashPower += lightDashPowerAccBase.Evaluate(poweringTime) * Time.fixedDeltaTime;
            DashPower += lightDashPowerAccMultiply.Evaluate(poweringTime) * inLights.Count * Time.fixedDeltaTime;
            if (!pausePowering)
            {
                poweringTime += Time.fixedDeltaTime;
            }
        }
        else
        {
            poweringTime = 0;
        }
        if (mainFsm.Current != null && mainFsm.Current.CouldDash && dashable)
        {
            var action = InputQueue.GetAction();
            if (action != null)
            {
                mainFsm.Current.OnDash(action.Value.dashDir);
            }
        }
        if (lastPowerRestoreTimeLeft > 0)
        {
            lastPowerRestoreTimeLeft -= Time.fixedDeltaTime;
        }
        if (DashPower < 1 && lastPowerRestoreTimeLeft <= 0)
        {
            DashPower = 1;
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
        var moveFloat = context.ReadValue<float>();
        if (Mathf.Abs(moveFloat) < zeroInputThreshold)
        {
            moveInput = 0;
            return;
        }
        moveInput = moveFloat >= 0 ? 1 : -1;
    }
    void OnDashDir(CallbackContext context)
    {
        var dashDirFloat = context.ReadValue<Vector2>();
        if (dashDirFloat.magnitude < zeroInputThreshold)
        {
            dashDirInput = new Vector2Int(0, 0);
            return;
        }
        dashDirFloat.Normalize();
        var angle = Vector2.SignedAngle(Vector2.right, dashDirFloat);
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
        dashDirInput = choose[index];
        if (dashDirInput != new Vector2Int(0, 0))
        {
            InputQueue.AddAction(new PlayerAction(dashDirInput));
        }
    }
    public void MoveX(XMoveParam param, bool onGround)
    {
        bool notFartSpeed = Mathf.Abs(VelocityX) < param.xVelMax * fartVelMaxRatio;
        if (moveInput == 0)
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
            if (moveInput * Mathf.Sign(VelocityX) >= 0)
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
            animation.RunFart(moveInput);
        }

        animation.SignDirectionX = SignDirectionX;
        animation.RunningSpeed = Mathf.Abs(VelocityX) / param.xVelMax;
        BlockMoveX();
        lastMoveInputX = moveInput;
    }
    public void DropY()
    {
        VelocityY -= yGrav * Time.fixedDeltaTime;
        if (Mathf.Abs(VelocityY) > yVelMax)
        {
            VelocityY = Mathf.Sign(VelocityY) * yVelMax;
        }
        var down = VelocityY < 0;
        if (BlockMoveY() && down)
        {
            mainFsm.ChangeState(new Idle());
            animation.OnGround();
        }
    }
    public void OnDamage(Vector2 dir)
    {
        if (invincible)
        {
            return;
        }
        mainFsm.Current.OnDamage(dir);
    }
    public void DamageVel(Vector2 direction, Vector2 velocity)
    {
        VelocityX = (direction.x >= 0 ? 1 : -1) * velocity.x;
        VelocityY = velocity.y;
    }
    public void ProcessDamage()
    {
        DashPower -= 1;
        if (DashPower < 0)
        {
            mainFsm.ChangeState(new Die());
            return;
        }
        ResetPoweringTime();
        PausePowering();
        SetInvincibleTime(damageInvincibleTime, true);
        if (!mainFsm.Current.IsDamage())
        {
            mainFsm.ChangeState(new Damage(false));
        }
    }
    public void OnBounceUp()
    {
        mainFsm.Current?.OnBounceUp();
    }
    public void AtCheckPoint()
    {
        DashPower = maxDashPower;
    }
    public static void SetNoise(float amplitude, float frequency)
    {
        var cameraBrain = SceneSingleton.Get<CinemachineBrain>();
        cameraBrain.m_IgnoreTimeScale = true;
        var virtualCamera = SceneSingleton.Get<CinemachineVirtualCamera>();
        var perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;
    }
}

public abstract class PlayerState : State<Player, PlayerState>
{
    public virtual void OnDash(Vector2Int dir)
    {
        if (mono.DashPower >= 1)
        {
            mono.DashPower -= 1;
            fsm.ChangeState(new Dash(dir, OnGround));
        }
    }
    public virtual bool CouldDash
    {
        get => true;
    }
    public virtual bool OnGround
    {
        get => false;
    }
    public abstract void OnDamage(Vector2 direction);
    public virtual bool IsDamage()
    {
        return false;
    }
    public virtual void OnBounceUp()
    {
        if (CouldBounceUp)
        {
            mono.VelocityY = mono.BounceUpVel;
            fsm.ChangeState(new BounceUp());
        }
    }
    public virtual bool CouldBounceUp
    {
        get => true;
    }
}
namespace PlayerStates
{
    public class Idle : PlayerState
    {
        public override IEnumerator Main()
        {
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

        public override void OnDamage(Vector2 direction)
        {
            mono.DamageVel(direction, mono.damageVelIdle);
            mono.ProcessDamage();
        }
        public override bool OnGround
        {
            get => true;
        }
    }
    public class Drop : PlayerState
    {

        public override IEnumerator Main()
        {
            mono.animation.Drop();
            while (true)
            {
                yield return new WaitForFixedUpdate();
                mono.MoveX(mono.dropMoveX, false);
                mono.DropY();
            }
        }

        public override void OnDamage(Vector2 direction)
        {
            mono.DamageVel(direction, mono.damageVelDrop);
            mono.ProcessDamage();
        }
    }
    public class Dash : PlayerState
    {
        private bool onGroundWhileDash;
        private Vector2Int dirInt;
        public Dash(Vector2Int dirInt, bool onGroundWhileDash)
        {
            this.dirInt = dirInt;
            this.onGroundWhileDash = onGroundWhileDash;
        }
        private HashSet<IPlayerAttackable> attacked;
        public override IEnumerator Main()
        {
            if (mono.resetPoweringWhileDash)
            {
                mono.ResetPoweringTime();
            }
            attacked = new HashSet<IPlayerAttackable>();
            var dashSpeed = mono.dashDistance / mono.dashTime;
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
            bool preAttacked = false;
            foreach (var attackable in mono.attackBox.InBoxCollisionMapAll(mono.attackLayer, go =>
            {
                var attackable = go.GetComponentInParent<IPlayerAttackable>();
                if (attackable == null)
                {
                    return null;
                }
                if (!attackable.ValidBox(go.GetComponent<BoxPhysics>()))
                {
                    return null;
                }
                return attackable;
            }))
            {
                if (preAttacked)
                {
                    preAttacked = true;
                    mono.animation.PreAttack();
                }
                if (!attacked.Contains(attackable))
                {
                    attacked.Add(attackable);
                    Player.SetNoise(mono.attackAmplitude, mono.attackFrequency);
                    var attackResult = attackable.GetAttackResult(dirInt);
                    mono.animation.Attack(dirInt, attackResult == AttackResult.Parry);
                    if (mono.attackFrameDelay > 0)
                    {
                        Time.timeScale = 0;
                        yield return new WaitForSecondsRealtime(mono.attackFrameDelay);
                    }
                    Player.SetNoise(0, 0);
                    attackable.OnAttack(dirInt);
                    if (attackResult == AttackResult.Dead)
                    {
                        mono.DashPower = mono.DashPower + mono.restoreDashPowerAfterKill;
                        mono.SetInvincibleTime(mono.attackInvincibleTime, false);
                    }
                    else if (attackResult == AttackResult.Parry)
                    {
                        mono.DamageVel(-dirInt, mono.damageVelParried);
                        mono.SetInvincibleTime(mono.parriedInvincibleTime, false);
                        mono.animation.OnParried(dirInt);
                        fsm.ChangeState(new Damage(true));
                    }
                    else
                    {
                        mono.SetInvincibleTime(mono.attackInvincibleTime, false);
                    }
                    Time.timeScale = 1;
                }

            }
        }

        public override void OnDamage(Vector2 direction)
        {
        }
        public override bool CouldDash
        {
            get => false;
        }
    }
    public class Damage : PlayerState
    {
        private bool couldDash = false;
        private bool isParried;
        public Damage(bool isParried)
        {
            this.isParried = isParried;
        }
        public override IEnumerator Main()
        {
            if (isParried)
            {
                mono.animation.Parried();
            }
            else
            {
                mono.animation.Damage();
            }
            mono.animation.SignDirectionX = mono.VelocityX >= 0 ? -1 : 1;
            if (!isParried)
            {
                Player.SetNoise(mono.damageAmplitude, mono.damageFrequency);
                Time.timeScale = 0;
                yield return new WaitForSecondsRealtime(mono.onDamageFrameDelay);
                Time.timeScale = 1;
                Player.SetNoise(0, 0);
            }
            var timeLeft = mono.dashAfterDamageDelayTime;
            while (timeLeft > 0)
            {
                yield return new WaitForFixedUpdate();
                timeLeft -= Time.fixedDeltaTime;
                mono.BlockMoveX();
                mono.DropY();
            }
            couldDash = true;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                mono.BlockMoveX();
                mono.DropY();
            }
        }
        public override void Exit()
        {
            mono.ContinuePowering();
        }

        public override void OnDamage(Vector2 direction)
        {
            mono.DamageVel(direction, mono.damageVelDrop);
            mono.ProcessDamage();
        }
        public override bool IsDamage()
        {
            return true;
        }

        public override void OnDash(Vector2Int dir)
        {
            if (couldDash)
            {
                fsm.ChangeState(new Dash(dir, false));
            }
        }
        public override bool CouldDash
        {
            get => couldDash;
        }
        public override bool CouldBounceUp
        {
            get => false;
        }
    }
    public class Die : PlayerState
    {
        public override IEnumerator Main()
        {
            mono.Velocity = Vector2.zero;
            mono.animation.Die();
            yield return new WaitForSeconds(mono.dieTime);
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            yield break;
        }

        public override void OnDamage(Vector2 direction)
        {
        }
        public override bool CouldDash
        {
            get => false;
        }
        public override bool CouldBounceUp
        {
            get => false;
        }

    }
    public class Born : PlayerState
    {
        public override IEnumerator Main()
        {
            mono.animation.Born();
            yield return new WaitForSeconds(mono.bornTime);
            mono.animation.AfterBorn();
            fsm.ChangeState(new Drop());
        }

        public override void OnDamage(Vector2 direction)
        {
        }
        public override bool CouldDash
        {
            get => false;
        }

    }
    public class BounceUp : PlayerState
    {
        public override IEnumerator Main()
        {
            mono.animation.BounceUp();
            while (true)
            {
                yield return new WaitForFixedUpdate();
                mono.MoveX(mono.dropMoveX, false);
                mono.DropY();
                if (mono.VelocityY < 0)
                {
                    fsm.ChangeState(new Drop());
                }
            }
        }

        public override void OnDamage(Vector2 direction)
        {
            mono.DamageVel(direction, mono.damageVelDrop);
            mono.ProcessDamage();
        }
        public override void Exit()
        {
            mono.animation.AfterBounceUp();
        }
        public override bool CouldBounceUp
        {
            get => false;
        }

    }
}
