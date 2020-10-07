using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ContainPlayer : MonoBehaviour, IBoxDetectGuest<Player>
{
    private Player player;

    [ShowInInspector]
    public Player Player { get => player; }

    public void Enter(Player t)
    {
        player = t;
    }

    public void Exit(Player t)
    {
        player = null;
    }

    public void Stay(Player t)
    {
    }
}
