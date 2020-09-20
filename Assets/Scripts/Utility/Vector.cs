using UnityEngine;

public class Vector2Component
{
    Vector2Component(bool isX)
    {
        this.isX = isX;
    }
    bool isX;
    public static Vector2Component X => new Vector2Component(true);
    public static Vector2Component Y => new Vector2Component(false);
    public Vector2Component Other()
    {
        return new Vector2Component(!isX);
    }
    public float Get(Vector2 v)
    {
        return isX ? v.x : v.y;
    }
    public void Set(ref Vector2 v, float value)
    {
        if (isX)
        {
            v.x = value;
        }
        else
        {
            v.y = value;
        }
    }
    public Vector2 Create(float thisValue, float otherValue)
    {
        if (isX)
        {
            return new Vector2(thisValue, otherValue);
        }
        else
        {
            return new Vector2(otherValue, thisValue);

        }
    }
}