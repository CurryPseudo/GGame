using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
[RequireComponent(typeof(PlayerAnimation))]
public class Player : FSM<Player>
{
    // Start is called before the first frame update
    public float xAcc;
    public float xRevAcc;
    public float xFri;
    public float xVelMax;
    public Vector2 move = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    public int signDirectionX = 1;
    [HideInInspector]
    public new PlayerAnimation animation;
    void Awake()
    {
        animation = GetComponent<PlayerAnimation>();
    }
    void Start()
    {
        ChangeState(new Idle());
    }
    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
        if (move.x != 0)
        {
            signDirectionX = (int)Mathf.Sign(move.x);
        }
    }
}

namespace PlayerState
{
    public class Idle : State<Player>
    {
        public override IEnumerator Main()
        {
            while (true)
            {
                mono.animation.SetSignDirectionX(mono.signDirectionX);
                yield return new WaitForFixedUpdate();
            }
        }
    }

}
