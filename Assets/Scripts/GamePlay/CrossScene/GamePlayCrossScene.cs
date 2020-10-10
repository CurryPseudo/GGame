using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCrossScene : CrossScene<GamePlayCrossScene>
{
    public ClipInfo bgm;
    public AudioUtility Audio
    {
        get => GetComponent<AudioUtility>();
    }
    void Start()
    {
        Audio.PlaySound(bgm, true);
    }
    public void Pause()
    {
        Audio.FadeOut(0.5f);
    }
    public void CancelPause()
    {
        Audio.Restore();
    }
}
