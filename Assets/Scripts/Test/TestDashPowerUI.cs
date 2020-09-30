using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDashPowerUI : MonoBehaviour, IDashPowerUI
{
    public void SetDashPower(float value)
    {
        GetComponent<Text>().text = value.ToString();
    }
}
