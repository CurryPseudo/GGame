using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTitleUIHelper : MonoBehaviour
{
    public void Pause()
    {
        MainTitleDirector.Current.Pause();
    }
    public void MainTitle()
    {
        MainTitleDirector.Current.MainMenu();
    }
}
