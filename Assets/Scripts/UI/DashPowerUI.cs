using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashPowerUI : MonoBehaviour, IDashPowerUI
{
    public List<ClipInfo> dashPowerRestoreSounds = new List<ClipInfo>();
    public List<Animator> DashPowerAnimators = new List<Animator>();
    private int soundPowerValue = 5;
    private int dashPower = 5;
    public float soundStep;
    public AudioUtility Audio
    {
        get => GetComponent<AudioUtility>();
    }
    public void SetDashPower(float value, bool playSound)
    {
        var lastDashPower = dashPower;
        dashPower = Mathf.FloorToInt(value);
        if (!playSound)
        {
            soundPowerValue = dashPower;
        }
        if (dashPower != lastDashPower)
        {
            for (int i = 0; i < dashPower + 1; i++)
            {
                if (i < DashPowerAnimators.Count)
                {
                    DashPowerAnimators[i].SetBool("Active", true);
                }
            }
            for (int i = dashPower + 1; i < 6; i++)
            {
                if (i < DashPowerAnimators.Count)
                {
                    DashPowerAnimators[i].SetBool("Active", false);
                }
            }
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
