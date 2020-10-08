using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterState
{
    bool IsParried(Vector2Int attackDirection);
    void OnDamage(Vector2Int attackDirection, bool willDead);
    void Parry(Vector2Int attackDirection);
}

public abstract class IMonster : Autonomy, IPlayerAttackable
{
    public abstract bool FaceLeft { get; set; }

    public abstract AttackResult GetAttackResult(Vector2Int attackDirection);

    public abstract void OnAttack(Vector2Int attackDirection);

    public abstract void OnDamage(Vector2Int damageDirection);

    public abstract bool ValidBox(BoxPhysics box);
}

public class Monster<T, S> : IMonster where T : Monster<T, S> where S : State<T, S>, IMonsterState
{
    public float yGrav;
    public float yVelMax;
    [SerializeField]
    private bool faceLeft;
    public int life;
    public new GameObject animation;
    public BoxPhysics attackableBox;
    public override bool FaceLeft
    {
        get => faceLeft;
        set
        {
            faceLeft = value;
            SpriteRenderer.flipX = !faceLeft;
        }
    }
    public Vector2 FaceDir
    {
        get => (faceLeft ? -1 : 1) * Vector2.right;
    }
    public Animator Animator
    {
        get => animation.GetComponentInChildren<Animator>();
    }
    public SpriteRenderer SpriteRenderer
    {
        get => animation.GetComponentInChildren<SpriteRenderer>();
    }
    public GameObjectInstantiator dieLight;
    public LayerMask PlayerLayer
    {
        get
        {
            var player = SceneSingleton.Get<Player>();
            return 1 << player.gameObject.layer;

        }
    }


    protected FSM<T, S> fsm;
    protected override void Awake()
    {
        base.Awake();
        fsm = new FSM<T, S>(this as T);
    }

    public override AttackResult GetAttackResult(Vector2Int attackDirection)
    {
        if (fsm.Current.IsParried(attackDirection))
        {
            return AttackResult.Parry;
        }
        if (life > 1)
        {
            return AttackResult.Damage;
        }
        else
        {
            return AttackResult.Dead;
        }
    }
    public override bool ValidBox(BoxPhysics box)
    {
        return box == attackableBox;
    }
    public bool Drop()
    {

        VelocityY -= yGrav * Time.fixedDeltaTime;
        if (Mathf.Abs(VelocityY) > yVelMax)
        {
            VelocityY = Mathf.Sign(VelocityY) * yVelMax;
        }
        var down = VelocityY < 0;
        return BlockMoveY() && down;
    }

    public override void OnAttack(Vector2Int attackDirection)
    {
        if (fsm.Current.IsParried(attackDirection))
        {
            fsm.Current.Parry(attackDirection);
            return;
        }
        OnDamage(attackDirection);
    }
    public override void OnDamage(Vector2Int attackDirection)
    {
        if (life > 1)
        {
            life -= 1;
            fsm.Current.OnDamage(attackDirection, false);
        }
        else
        {
            fsm.Current.OnDamage(attackDirection, true);
        }
    }
}
