using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterState
{
    void OnDamage();
    void OnDie();
}

public class Monster<T, S> : Autonomy, IPlayerAttackable where T : Monster<T, S> where S : State<T, S>, IMonsterState
{
    public float yGrav;
    public float yVelMax;
    [SerializeField]
    private bool faceLeft;
    public int life;
    public new GameObject animation;
    public bool FaceLeft
    {
        get => faceLeft;
        set
        {
            faceLeft = value;
            SpriteRenderer.flipX = !faceLeft;
        }
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

    public bool OnAttack()
    {
        if (life > 1)
        {
            life -= 1;
            fsm.Current.OnDamage();
            return false;
        }
        else
        {
            fsm.Current.OnDie();
            return true;
        }
    }
    public bool ValidBox(BoxPhysics box)
    {
        return box == moveBox;
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
