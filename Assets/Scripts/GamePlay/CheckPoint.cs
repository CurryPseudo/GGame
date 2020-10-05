using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public BoxPhysics box;
    public Transform point;
    private bool active = false;
    private Animator Animator
    {
        get => GetComponentInChildren<Animator>();
    }
    public void FixedUpdate()
    {
        if (!active)
        {
            if (box.InBoxCollision(Player.DetectPlayerLayer))
            {
                active = true;
                Animator.SetBool("Active", true);
                PlayerPrefs.SetInt("BornPoint", this.gameObject.name.GetHashCode());
            }
        }
    }
    private static Dictionary<int, CheckPoint> map;
    void Awake()
    {
        if (map == null)
        {
            map = new Dictionary<int, CheckPoint>();
        }
        if (!map.ContainsKey(this.gameObject.name.GetHashCode()))
        {
            map.Add(this.gameObject.name.GetHashCode(), this);
        }
        else
        {
            map[this.gameObject.name.GetHashCode()] = this;
        }
        if (Current == this)
        {
            active = true;
            Animator.SetBool("Active", true);
        }
    }
    public static CheckPoint Current
    {
        get
        {
            if (map == null)
            {
                return null;
            }
            if (!PlayerPrefs.HasKey("BornPoint"))
            {
                return null;
            }
            var hash = PlayerPrefs.GetInt("BornPoint");
            if (!map.ContainsKey(hash))
            {
                return null;
            }
            return map[hash];
        }
    }
    public Vector2 Point
    {
        get => point.position;
    }
    [Button]
    public void ResetCheckPoint()
    {
        PlayerPrefs.DeleteKey("BornPoint");
    }
    [Button]
    public void SetCurrentCheckPoint()
    {
        PlayerPrefs.SetInt("BornPoint", this.gameObject.name.GetHashCode());

    }
}
