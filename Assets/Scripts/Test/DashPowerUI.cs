using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashPowerUI : MonoBehaviour, IDashPowerUI
{
    public float DashPower
    {
        set
        {
            GetComponent<Text>().text = value.ToString();
        }
    }
}
