using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerAction
{
    public Vector2Int dashDir;

    public PlayerAction(Vector2Int dashDir)
    {
        this.dashDir = dashDir;
    }
}
public class PlayerInputQueue : TimeQueue<PlayerAction>
{
}
