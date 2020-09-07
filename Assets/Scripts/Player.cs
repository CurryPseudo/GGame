using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
public class Player : FSM<Player>
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(new Idle());
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
                yield return null;
            }
        }
    }

}
