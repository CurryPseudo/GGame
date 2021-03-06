﻿using System;
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
    private bool fadingOut = false;
    private float fadeOutTimeLeft = 0;
    private float currentFadeOutTime = 0;
    private Coroutine fadeOutCoroutine;
    private ClipInfo current;
    public AudioSource Audio
    {
        get => GetComponent<AudioSource>();
    }
    public void PlaySound(ClipInfo clip, bool loop = false)
    {
        fadingOut = false;
        if (loop)
        {
            current = clip;
            Audio.Stop();
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
    public void FadeOut(float time)
    {
        fadeOutTimeLeft = time;
        currentFadeOutTime = time;
        fadingOut = true;
    }
    public void Restore()
    {
        Audio.volume = current.volumeScale;
    }
    void Update()
    {
        if (fadeOutTimeLeft > 0)
        {
            fadeOutTimeLeft -= Time.unscaledDeltaTime;
        }
        if (fadeOutTimeLeft < 0 && fadingOut)
        {
            fadeOutTimeLeft = 0;
            fadingOut = false;
        }
        else if (fadingOut)
        {
            Audio.volume = fadeOutTimeLeft / currentFadeOutTime * current.volumeScale;
        }
    }
}
