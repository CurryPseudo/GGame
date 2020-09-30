using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LanternStates;
using Sirenix.OdinInspector;

public class Lantern : Autonomy, IPlayerAttackable
{
    public float jumpTime;
    public float jumpXDis;
    public float jumpYDis;
    [ShowInInspector]
    public float jumpYGrav
    {
        get
        {
            return (2 * jumpYDis) / (jumpTime * jumpTime);
        }
    }
    public float jumpYVel
    {
        get => jumpYGrav * jumpTime;
    }
    public float jumpXSpeed
    {
        get
        {
            return jumpXDis / (2 * jumpTime);
        }
    }
    public float yGrav;
    public float yVelMax;
    public float waitJumpTime;
    public float prepareJumpTime;
    public bool faceLeft;
    public new GameObject animation;
    public BoxPhysics detectBoxAirLeft;
    public BoxPhysics detectBoxAirRight;
    public BoxPhysics detectBoxGroundLeft;
    public BoxPhysics detectBoxGroundRight;
    public GameObjectInstantiator dieLight;
    public Animator animator
    {
        get => animation.GetComponentInChildren<Animator>();
    }
    public SpriteRenderer spriteRenderer
    {
        get => animation.GetComponentInChildren<SpriteRenderer>();
    }
    private FSM<Lantern, LanternState> fsm;
    protected override void Awake()
    {
        base.Awake();
        fsm = new FSM<Lantern, LanternState>(this);
    }
    void Start()
    {
        fsm.ChangeState(new Drop());
    }
    public bool OnAttack()
    {
        dieLight.Instantiate();
        Destroy(gameObject);
        return true;
    }

    public bool validBox(BoxPhysics box)
    {
        return box == moveBox;
    }
}
namespace LanternStates
{
    public abstract class LanternState : State<Lantern, LanternState> { }
    public class Drop : LanternState
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
                var down = mono.VelocityY < 0;
                if (mono.BlockMoveY() && down)
                {
                    fsm.ChangeState(new Idle());
                    yield break;
                }
            }
        }
    }
    public class Idle : LanternState
    {
        public override IEnumerator Main()
        {
            while (true)
            {
                yield return new WaitForSeconds(mono.waitJumpTime);
                var detectBoxAir = mono.faceLeft ? mono.detectBoxAirLeft : mono.detectBoxAirRight;
                var detectBoxGround = mono.faceLeft ? mono.detectBoxGroundLeft : mono.detectBoxGroundRight;
                if (!detectBoxAir.InBoxCollision(mono.blockLayer, null) && Autonomy.DetectOnGround(detectBoxGround, mono.blockLayer))
                {
                    break;
                }
                else
                {
                    mono.faceLeft = !mono.faceLeft;
                    mono.spriteRenderer.flipX = !mono.faceLeft;
                }
            }
            fsm.ChangeState(new Attack());
        }
    }
    public class Attack : LanternState
    {
        public override IEnumerator Main()
        {
            mono.animator.SetTrigger("Jump");
            yield return new WaitForSeconds(mono.prepareJumpTime);
            var xSign = mono.faceLeft ? -1 : 1;
            var timeLeft = mono.jumpTime * 2;
            mono.VelocityY = mono.jumpYVel;
            mono.VelocityX = mono.jumpXSpeed * xSign;
            while (timeLeft > 0)
            {
                yield return new WaitForFixedUpdate();
                timeLeft -= Time.fixedDeltaTime;
                mono.VelocityY -= mono.jumpYGrav * Time.fixedDeltaTime;
                mono.BlockMoveY();
                mono.BlockMoveX();
            }
            mono.VelocityX = 0;
            fsm.ChangeState(new Drop());
        }
    }
}