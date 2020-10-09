using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioUtility))]
public class BounceUpPlayer : MonoBehaviour, IBoxDetectGuest<Player>
{
    public ClipInfo bounceUpClip;
    public AudioUtility Audio
    {
        get => GetComponent<AudioUtility>();
    }
    public void Enter(Player t)
    {
        Audio.PlaySound(bounceUpClip);
        t.OnBounceUp();
    }

    public void Exit(Player t)
    {
    }

    public void Stay(Player t)
    {
        Audio.PlaySound(bounceUpClip);
        t.OnBounceUp();
    }

}
