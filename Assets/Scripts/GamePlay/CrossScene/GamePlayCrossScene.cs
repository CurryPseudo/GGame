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
}
