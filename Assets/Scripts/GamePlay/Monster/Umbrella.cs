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
    public ContainPlayer attackDetect;
    public DamagePlayer damagePlayer;
    void Start()
    {
        fsm.ChangeState(new Drop(false));
        damagePlayer.DamageDirClosure = () => FaceDir;
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
        public virtual bool IsParried(Vector2Int attackDirection)
        {
            if (Attacking)
            {
                return mono.AttackParry(attackDirection);
            }
            else
            {
                return mono.IdleParry(attackDirection);
            }
        }
        public abstract bool Attacking
        {
            get;
        }

        void IMonsterState.OnDamage(Vector2Int attackDirection, bool willDead)
        {
            fsm.ChangeState(new Die());
        }

        public void Parry(Vector2Int attackDirection)
        {
            fsm.ChangeState(new Parry(Attacking));
        }
    }
    public class Drop : UmbrellaState
    {
        private bool attacking = false;
        public Drop(bool attacking)
        {
            this.attacking = attacking;
        }

        public override bool Attacking
        {
            get => attacking;
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
        public override bool Attacking => false;

        public override IEnumerator Main()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (mono.attackDetect.Player != null)
                {
                    fsm.ChangeState(new Detected());
                }
            }
        }
    }
    public class Detected : UmbrellaState
    {
        public override bool Attacking => false;

        public override IEnumerator Main()
        {
            var timeLeft = mono.detectAccumulatedTime;
            while (timeLeft > 0)
            {
                yield return new WaitForFixedUpdate();
                timeLeft -= Time.fixedDeltaTime;
                var player = SceneSingleton.Get<Player>();
                mono.FaceLeft = (player.transform.position - mono.transform.position).x < 0;
                if (mono.attackDetect.Player == null)
                {
                    fsm.ChangeState(new Idle());
                }
            }
            fsm.ChangeState(new Attack());
        }
    }
    public class Attack : UmbrellaState
    {
        public override bool Attacking => true;

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
    }
    public class Parry : UmbrellaState
    {
        private bool lastAttacking;
        public Parry(bool lastAttacking)
        {
            this.lastAttacking = lastAttacking;
        }
        public override bool Attacking => lastAttacking;

        public override IEnumerator Main()
        {
            mono.Animator.SetTrigger("Parry");
            mono.VelocityX = 0;
            yield return new WaitForSeconds(mono.parryTime);
            if (lastAttacking)
            {
                fsm.ChangeState(new Attack());
            }
            else
            {
                fsm.ChangeState(new Idle());
            }
        }
    }
    public class Die : UmbrellaState
    {
        public override bool Attacking => false;

        public override IEnumerator Main()
        {
            mono.damagePlayer.gameObject.SetActive(false);
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