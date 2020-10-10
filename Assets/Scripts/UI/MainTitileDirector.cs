using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainTitileDirector : MonoBehaviour
{
    public float waitToEnter;
    public float waitToExit;
    public string sceneName;
    public Button continueGameButton;
    public Image continueGameImage;
    public Sprite notValidButtonSprite;
    public MainTitleUICombiner Combiner
    {
        get => GetComponent<MainTitleUICombiner>();
    }
    void Start()
    {
        Combiner.Init();
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        yield return new WaitForSecondsRealtime(waitToEnter);
        Combiner.Enter();
        SetContinueGameStatus(CheckPoint.HasValidSave);
    }
    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
    public void NewGame()
    {
        CheckPoint.ResetCheckPoint();
        StartGame();
    }
    IEnumerator StartGameCoroutine()
    {
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSecondsRealtime(waitToExit);
        Combiner.Exit();
    }
    private void SetContinueGameStatus(bool valid)
    {
        if (!valid)
        {
            continueGameButton.enabled = false;
            continueGameImage.sprite = notValidButtonSprite;
        }
    }
}
