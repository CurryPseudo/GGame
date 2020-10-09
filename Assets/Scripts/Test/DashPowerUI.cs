using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashPowerUI : MonoBehaviour, IDashPowerUI
{
    public List<ClipInfo> dashPowerRestoreSounds = new List<ClipInfo>();
    private int soundPowerValue = 5;
    private int dashPower = 5;
    public float soundStep;
    public AudioUtility Audio
    {
        get => GetComponent<AudioUtility>();
    }
    public void SetDashPower(float value, bool playSound)
    {
        GetComponent<Text>().text = value.ToString();
        dashPower = Mathf.FloorToInt(value);
        if (!playSound)
        {
            soundPowerValue = dashPower;
        }
    }
    void Start()
    {
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        while (true)
        {
            while (soundPowerValue < dashPower)
            {
                if (soundPowerValue < dashPowerRestoreSounds.Count)
                {
                    Audio.PlaySound(dashPowerRestoreSounds[soundPowerValue]);
                }
                soundPowerValue += 1;
                yield return new WaitForSecondsRealtime(soundStep);
            }
            soundPowerValue = dashPower;
            yield return null;
        }
    }
}
