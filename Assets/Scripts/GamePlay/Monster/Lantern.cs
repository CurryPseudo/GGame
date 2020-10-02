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
    [SerializeField]
    private bool faceLeft;
    public bool FaceLeft
    {
        get => faceLeft;
        set
        {
            faceLeft = value;
            SpriteRenderer.flipX = !faceLeft;
        }
    }
    public new GameObject animation;
    public BoxPhysics detectBoxAirLeft;
    public BoxPhysics detectBoxAirRight;
    public BoxPhysics detectBoxGroundLeft;
    public BoxPhysics detectBoxGroundRight;
    public BoxPhysics attackDetectBox;
    public BoxPhysics damageBox;
    public GameObjectInstantiator dieLight;
    public Animator Animator
    {
        get => animation.GetComponentInChildren<Animator>();
    }
    public SpriteRenderer SpriteRenderer
    {
        get => animation.GetComponentInChildren<SpriteRenderer>();
    }
    public LayerMask PlayerLayer
    {
        get
        {
            var player = SceneSingleton.Get<Player>();
            return 1 << player.gameObject.layer;

        }
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
        fsm.Current.OnDie();
        return true;
    }
    public bool ValidBox(BoxPhysics box)
    {
        return box == moveBox;
    }
}
namespace LanternStates
{
    public abstract class LanternState : State<Lantern, LanternState>
    {
        public virtual void OnDie()
        {
            fsm.ChangeState(new Die());
        }
    }
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
                if (mono.attackDetectBox.InBoxCollision(mono.PlayerLayer) != null)
                {
                    var player = SceneSingleton.Get<Player>();
                    mono.FaceLeft = (player.transform.position - mono.transform.position).x < 0;
                    if (ValidAttackGround(mono.FaceLeft))
                    {
                        break;
                    }
                }
                else
                {
                    if (ValidAttackAir(mono.FaceLeft) && ValidAttackGround(mono.FaceLeft))
                    {
                        break;
                    }
                    else if (ValidAttackAir(!mono.FaceLeft) && ValidAttackGround(!mono.FaceLeft))
                    {
                        mono.FaceLeft = !mono.FaceLeft;
                        break;
                    }
                }

            }
            fsm.ChangeState(new Attack());
        }
        private bool ValidAttackAir(bool left)
        {
            var detectBoxAir = left ? mono.detectBoxAirLeft : mono.detectBoxAirRight;
            return !detectBoxAir.InBoxCollision(mono.blockLayer, null);
        }
        private bool ValidAttackGround(bool left)
        {

            var detectBoxGround = left ? mono.detectBoxGroundLeft : mono.detectBoxGroundRight;
            return Autonomy.DetectOnGround(detectBoxGround, mono.blockLayer);
        }
    }
    public class Attack : LanternState
    {
        public override IEnumerator Main()
        {
            mono.Animator.SetTrigger("Jump");
            yield return new WaitForSeconds(mono.prepareJumpTime);
            var xSign = mono.FaceLeft ? -1 : 1;
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
    public class Die : LanternState
    {
        public override IEnumerator Main()
        {
            mono.damageBox.gameObject.SetActive(false);
            mono.Animator.SetTrigger("Die");
            mono.dieLight.Instantiate();
            yield return new WaitForFixedUpdate();
            var dieTime = mono.Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(dieTime);
            GameObject.Destroy(mono.gameObject);
            yield break;
        }
    }
}