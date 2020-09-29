using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Autonomy : MonoBehaviour
{
    public LayerMask blockLayer;
    public BoxPhysics moveBox;
    private Vector2 velocity;
    private int signDirectionX = 1;
    [ShowInInspector]
    public int SignDirectionX { get => signDirectionX; }
    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    public float PositionX
    {
        get { return transform.position.x; }
        set
        {
            transform.position = new Vector3(value, transform.position.y, transform.position.z);
        }
    }
    public float PositionY
    {
        get { return transform.position.y; }
        set
        {
            transform.position = new Vector3(transform.position.x, value, transform.position.z);
        }
    }
    public Vector2 Velocity
    {
        get { return velocity; }
        set
        {
            if (value != velocity)
            {
                VelocityOnChange(value);
            }
            velocity = value;
        }
    }
    public float VelocityX
    {
        get { return Velocity.x; }
        set
        {
            if (value != Velocity.x)
            {
                VelocityOnChange(new Vector2(value, Velocity.y));
            }
            Velocity = new Vector2(value, Velocity.y);
        }
    }
    public float VelocityY
    {
        get { return Velocity.y; }
        set
        {
            if (value != Velocity.y)
            {
                VelocityOnChange(new Vector2(Velocity.x, value));
            }
            Velocity = new Vector2(Velocity.x, value);
        }
    }
    private void VelocityOnChange(Vector2 value)
    {
        if (value.x != 0)
        {
            var next = (int)Mathf.Sign(value.x);
            signDirectionX = next;
        }
    }
    public bool BlockMove(Vector2Component component, LayerMask layer)
    {
        var velDis = component.Get(Velocity) * Time.fixedDeltaTime;
        if (moveBox.InBoxCollision(layer))
        {
            var position = Position;
            component.Set(ref position, component.Get(position) + velDis);
            Position = position;
            return false;
        }
        var result = moveBox.BlockMove(layer, component, velDis);
        if (result.HasValue)
        {
            (var go, var dis) = result.Value;
            var position = Position;
            component.Set(ref position, component.Get(position) + dis);
            Position = position;
            var velocity = Velocity;
            component.Set(ref velocity, 0);
            Velocity = velocity;
            return true;
        }
        else
        {
            var position = Position;
            component.Set(ref position, component.Get(position) + velDis);
            Position = position;
            return false;
        }

    }
    public bool BlockMoveX()
    {
        return BlockMoveX(blockLayer);
    }
    public bool BlockMoveX(LayerMask layer)
    {
        return BlockMove(Vector2Component.X, layer);
    }
    public bool BlockMoveY()
    {
        return BlockMoveY(blockLayer);
    }
    public bool BlockMoveY(LayerMask layer)
    {
        return BlockMove(Vector2Component.Y, layer);
    }
    public void StepVelocityX()
    {
        PositionX += VelocityX * Time.fixedDeltaTime;
    }
    public void StepVelocityY()
    {
        PositionY += VelocityY * Time.fixedDeltaTime;
    }
    public bool IsOnGround
    {
        get
        {
            var result = moveBox.BlockMove(blockLayer, Vector2Component.Y, -0.01f);
            return result.HasValue;
        }
    }
}
