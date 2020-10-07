using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterState
{
    AttackResult OnDamage(Vector2Int attackDirection, bool willDead);
}

public class Monster<T, S> : Autonomy, IPlayerAttackable where T : Monster<T, S> where S : State<T, S>, IMonsterState
{
    public float yGrav;
    public float yVelMax;
    [SerializeField]
    private bool faceLeft;
    public int life;
    public new GameObject animation;
    public BoxPhysics attackableBox;
    public bool FaceLeft
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

    public AttackResult OnAttack(Vector2Int attackDirection)
    {
        if (life > 1)
        {
            var result = fsm.Current.OnDamage(attackDirection, false);
            if (result == AttackResult.Damage)
            {
                life -= 1;
            }
            return result;
        }
        else
        {
            return fsm.Current.OnDamage(attackDirection, true);
        }
    }
    public bool ValidBox(BoxPhysics box)
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
}
