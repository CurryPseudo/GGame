using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LanternStates;
using Sirenix.OdinInspector;

public class Lantern : Monster<Lantern, LanternState>, IPlayerAttackable
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
    public float waitJumpTime;
    public float prepareJumpTime;
    public float dieTime;
    public float damageTime;
    public BoxPhysics detectBoxAirLeft;
    public BoxPhysics detectBoxAirRight;
    public BoxPhysics detectBoxGroundLeft;
    public BoxPhysics detectBoxGroundRight;
    public BoxPhysics attackDetectBox;
    public BoxPhysics damageBox;
    void Start()
    {
        fsm.ChangeState(new Drop());
    }
}
namespace LanternStates
{
    public abstract class LanternState : State<Lantern, LanternState>, IMonsterState
    {
        public virtual void OnDie()
        {
            fsm.ChangeState(new Die());
        }
        public virtual void OnDamage()
        {
            fsm.ChangeState(new Damage());
        }
    }
    public class Drop : LanternState
    {
        public override IEnumerator Main()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (mono.Drop())
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
            yield return new WaitForSeconds(mono.dieTime);
            GameObject.Destroy(mono.gameObject);
            yield break;
        }
    }
    public class Damage : LanternState
    {
        public override IEnumerator Main()
        {
            mono.Velocity = Vector2.zero;
            mono.damageBox.gameObject.SetActive(false);
            mono.Animator.SetTrigger("Damage");
            yield return new WaitForSeconds(mono.damageTime);
            mono.damageBox.gameObject.SetActive(true);
            fsm.ChangeState(new Drop());
            yield break;
        }
    }
}