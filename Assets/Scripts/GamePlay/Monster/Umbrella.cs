using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UmbrellaStates;

public class Umbrella : Monster<Umbrella, UmbrellaState>
{
    public float waitAttackTime;
    public float detectAccumulatedTime;
    public float attackVel;
    public float parryTime;
    public float dieTime;
    public float afterAttackToIdleTime;
    public BoxPhysics attackDetectBox;
    public BoxPhysics damageBox;
    void Start()
    {
        fsm.ChangeState(new Drop(false));
    }
    public bool IdleParry(Vector2Int attackDirection)
    {
        return attackDirection.y < 0;
    }
    public bool AttackParry(Vector2Int attackDirection)
    {
        return (attackDirection.x == 0 && attackDirection.y < 0)
            //|| (attackDirection.y == 0 && ((attackDirection.x < 0) ^ FaceLeft))
            //|| (attackDirection.y < 0 && ((attackDirection.x < 0) ^ FaceLeft));
            || ((attackDirection.x < 0) ^ FaceLeft);
    }
}

namespace UmbrellaStates
{
    public abstract class UmbrellaState : State<Umbrella, UmbrellaState>, IMonsterState
    {
        public virtual AttackResult OnDamage(Vector2Int attackDirection, bool willDead)
        {
            return AttackResult.None;
        }

    }
    public class Drop : UmbrellaState
    {
        private bool attacking = false;
        public Drop(bool attacking)
        {
            this.attacking = attacking;
        }
        public override AttackResult OnDamage(Vector2Int attackDirection, bool willDead)
        {
            if ((attacking && mono.AttackParry(attackDirection)) || (!attacking && mono.IdleParry(attackDirection)))
            {
                fsm.ChangeState(new Parry());
                return AttackResult.Parry;
            }
            fsm.ChangeState(new Die());
            return AttackResult.Dead;
        }
        public override IEnumerator Main()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (mono.Drop())
                {
                    if (attacking)
                    {
                        mono.Animator.SetBool("Attack", false);
                        yield return new WaitForSeconds(mono.afterAttackToIdleTime);
                    }
                    fsm.ChangeState(new Idle());
                    yield break;
                }
            }
        }
    }
    public class Idle : UmbrellaState
    {
        public override IEnumerator Main()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (mono.attackDetectBox.InBoxCollision(mono.PlayerLayer) != null)
                {
                    fsm.ChangeState(new Detected());
                }
            }
        }
        public override AttackResult OnDamage(Vector2Int attackDirection, bool willDead)
        {
            if (mono.IdleParry(attackDirection))
            {
                fsm.ChangeState(new Parry());
                return AttackResult.Parry;
            }
            fsm.ChangeState(new Die());
            return AttackResult.Dead;
        }
    }
    public class Detected : UmbrellaState
    {
        public override IEnumerator Main()
        {
            var timeLeft = mono.detectAccumulatedTime;
            while (timeLeft > 0)
            {
                yield return new WaitForFixedUpdate();
                timeLeft -= Time.fixedDeltaTime;
                var player = SceneSingleton.Get<Player>();
                mono.FaceLeft = (player.transform.position - mono.transform.position).x < 0;
                if (mono.attackDetectBox.InBoxCollision(mono.PlayerLayer) == null)
                {
                    fsm.ChangeState(new Idle());
                }
            }
            fsm.ChangeState(new Attack());
        }
        public override AttackResult OnDamage(Vector2Int attackDirection, bool willDead)
        {
            if (mono.IdleParry(attackDirection))
            {
                fsm.ChangeState(new Parry());
                return AttackResult.Parry;
            }
            fsm.ChangeState(new Die());
            return AttackResult.Dead;
        }
    }
    public class Attack : UmbrellaState
    {
        public override IEnumerator Main()
        {
            mono.Animator.SetBool("Attack", true);
            yield return new WaitForSeconds(mono.waitAttackTime);
            int sgn = mono.FaceLeft ? -1 : 1;
            mono.VelocityX = sgn * mono.attackVel;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (mono.BlockMoveX())
                {
                    mono.Animator.SetBool("Attack", false);
                    yield return new WaitForSeconds(mono.afterAttackToIdleTime);
                    fsm.ChangeState(new Idle());
                    yield break;
                }
                if (!mono.IsOnGround)
                {
                    mono.VelocityX = 0;
                    fsm.ChangeState(new Drop(true));
                }
            }
        }
        public override AttackResult OnDamage(Vector2Int attackDirection, bool willDead)
        {
            if (mono.AttackParry(attackDirection))
            {
                fsm.ChangeState(new Parry());
                return AttackResult.Parry;
            }
            fsm.ChangeState(new Die());
            return AttackResult.Dead;
        }
    }
    public class Parry : UmbrellaState
    {
        public override IEnumerator Main()
        {
            mono.Animator.SetTrigger("Parry");
            mono.VelocityX = 0;
            yield return new WaitForSeconds(mono.parryTime);
            fsm.ChangeState(new Attack());
        }
    }
    public class Die : UmbrellaState
    {
        public override IEnumerator Main()
        {
            mono.damageBox.gameObject.SetActive(false);
            mono.attackableBox.gameObject.SetActive(false);
            mono.Animator.SetTrigger("Die");
            mono.dieLight.Instantiate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(mono.dieTime);
            GameObject.Destroy(mono.gameObject);
            yield break;
        }
    }
}