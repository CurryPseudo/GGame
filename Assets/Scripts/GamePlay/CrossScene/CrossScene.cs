using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossScene : MonoBehaviour
{
    // Start is called before the first frame update
    public ClipInfo bgm;
    public AudioUtility Audio
    {
        get => GetComponent<AudioUtility>();
    }
    private static bool created = false;
    void Awake()
    {
        if (!created)
        {
            created = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Audio.PlaySound(bgm, true);
    }

}
