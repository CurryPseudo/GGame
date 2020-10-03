using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UmbrellaStates;

public class Umbrella : Monster<Umbrella, UmbrellaState>
{
    public float waitAttackTime;
    public float detectAccumulatedTime;
    public float attackVel;
    public BoxPhysics attackDetectBox;
    void Start()
    {
        fsm.ChangeState(new Drop());
    }
}

namespace UmbrellaStates
{
    public abstract class UmbrellaState : State<Umbrella, UmbrellaState>, IMonsterState
    {
        public void OnDamage()
        {
            Debug.Log("haha");
        }

        public void OnDie()
        {
            Debug.Log("haha ehhh");
        }
    }
    public class Drop : UmbrellaState
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
    public class Idle : UmbrellaState
    {
        public override IEnumerator Main()
        {
            mono.Animator.SetBool("Attack", false);
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (mono.attackDetectBox.InBoxCollision(mono.PlayerLayer) != null)
                {
                    fsm.ChangeState(new Detected());
                }
            }
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
                    fsm.ChangeState(new Idle());
                    yield break;
                }
                if (!mono.IsOnGround)
                {
                    mono.VelocityX = 0;
                    fsm.ChangeState(new Drop());
                }
            }
        }
    }
}