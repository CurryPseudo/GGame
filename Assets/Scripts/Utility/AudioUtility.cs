using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClipInfo
{
    public AudioClip clip;
    public float volumeScale = 1;
}
[RequireComponent(typeof(AudioSource))]
public class AudioUtility : MonoBehaviour
{
    public AudioSource Audio
    {
        get => GetComponent<AudioSource>();
    }
    public void PlaySound(ClipInfo clip, bool loop = false)
    {
        if (loop)
        {
            Audio.loop = true;
            Audio.volume = clip.volumeScale;
            Audio.clip = clip.clip;
            Audio.Play();
        }
        else
        {
            Audio.PlayOneShot(clip.clip, clip.volumeScale);
        }
    }
    public void StopSoundLoop()
    {
        Audio.loop = false;
    }
}
