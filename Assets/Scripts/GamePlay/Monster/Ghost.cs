using System.Collections;
using System.Collections.Generic;
using GhostStates;
using UnityEngine;

public class Ghost : Monster<Ghost, GhostState>, IPlayerAttackable
{
    public DamagePlayer damagePlayer;
    public float dieTime;
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
            fsm.ChangeState(new Die());
        }
    }
    public class Idle : GhostState
    {
        public override IEnumerator Main()
        {
            yield break;
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
