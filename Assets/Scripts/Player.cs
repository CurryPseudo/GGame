using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
public class Player : FSM<Player>
{
    // Start is called before the first frame update
    public float xAcc;
    public float xRevAcc;
    public float xFri;
    public float xVelMax;
    [ReadOnly]
    public Vector2 move = Vector2.zero;
    [ReadOnly]
    public Vector2 velocity = Vector2.zero;
    void Start()
    {
        ChangeState(new Idle());
    }
    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }
}

namespace PlayerState
{
    public class Idle : State<Player>
    {
        public override IEnumerator Main()
        {
            Debug.Log("Hello");
            while (true)
            {
                Debug.Log(mono.move);
                yield return new WaitForFixedUpdate();
            }
        }
    }

}
