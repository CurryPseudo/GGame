using System.Collections;
using System.Collections.Generic;
using GhostStates;
using UnityEngine;

public class Ghost : Monster<Ghost, GhostState>, IPlayerAttackable
{
    public ContainPlayer attackDetect;
    public DamagePlayer damagePlayer;
    public float detectTime;
    public float dieTime;
    public float attackVel;
    public float damageTime;
    public override bool CouldAffectedByDamageMonster { get => false; }
    void Start()
    {
        fsm.ChangeState(new Idle());
    }
}

namespace GhostStates
{
    public abstract class GhostState : State<Ghost, GhostState>, IMonsterState
    {
        public bool IsParried(Vector2Int attackDirection)
        {
            return false;
        }
        public void Parry(Vector2Int attackDirection)
        {
        }

        void IMonsterState.OnDamage(Vector2Int attackDirection, bool willDead)
        {
            if (willDead)
            {
                fsm.ChangeState(new Die());
            }
            else
            {
                fsm.ChangeState(new Damage());
            }
        }
    }
    public class Idle : GhostState
    {
        public override IEnumerator Main()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (mono.attackDetect.Player != null)
                {
                    fsm.ChangeState(new Detect());
                }
            }
        }
    }
    public class Detect : GhostState
    {
        public override IEnumerator Main()
        {
            mono.Animator.SetTrigger("Detect");
            var player = mono.attackDetect.Player;
            var timeLeft = mono.detectTime;
            while (timeLeft > 0)
            {
                yield return new WaitForFixedUpdate();
                timeLeft -= Time.fixedDeltaTime;
                mono.FaceLeft = (player.Position - mono.Position).x < 0;
            }
            fsm.ChangeState(new Attack(player.Position));

        }
    }
    public class Attack : GhostState
    {
        private Vector2 target;
        public Attack(Vector2 target)
        {
            this.target = target;
        }
        public override IEnumerator Main()
        {
            mono.Animator.SetBool("Attack", true);
            var dir = target - mono.Position;
            var timeLeft = dir.magnitude / mono.attackVel;
            mono.Velocity = mono.attackVel * dir.normalized;
            while (timeLeft > 0)
            {
                yield return new WaitForFixedUpdate();
                if (timeLeft < Time.fixedDeltaTime)
                {
                    mono.Velocity = mono.attackVel * dir.normalized * timeLeft / Time.fixedDeltaTime;
                }
                timeLeft -= Time.fixedDeltaTime;
                mono.BlockMoveY();
                mono.BlockMoveX();
            }
            mono.Velocity = Vector2.zero;
            fsm.ChangeState(new Idle());
        }
        public override void Exit()
        {
            mono.Animator.SetBool("Attack", false);
        }

    }
    public class Damage : GhostState
    {
        public override IEnumerator Main()
        {
            mono.damagePlayer.gameObject.SetActive(false);
            yield return new WaitForSeconds(mono.damageTime);
            mono.damagePlayer.gameObject.SetActive(true);
            fsm.ChangeState(new Idle());
        }
    }
    public class Die : GhostState
    {
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
