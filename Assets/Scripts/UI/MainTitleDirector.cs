using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainTitleDirector : MonoBehaviour
{
    private static MainTitleDirector instance = null;
    public static MainTitleDirector Current
    {
        get => instance;
    }
    public float waitToEnter;
    public float waitToExit;
    public string sceneName;
    public Button continueGameButton;
    public Image continueGameImage;
    public Sprite validButtonSprite;
    public Sprite notValidButtonSprite;
    public ClipInfo mainTitleBGM;
    public AudioUtility Audio
    {
        get => GetComponent<AudioUtility>();
    }
    public MainTitleUICombiner Combiner
    {
        get => GetComponent<MainTitleUICombiner>();
    }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Combiner.Init();
        StartCoroutine(Main());
    }
    IEnumerator Main()
    {
        PlayMainTitleBGM();
        yield return new WaitForSecondsRealtime(waitToEnter);
        Combiner.Enter();
        SetContinueGameStatus(CheckPoint.HasValidSave);
    }
    public void CancelPause()
    {
        GamePlayCrossScene.Current.CancelPause();
        Time.timeScale = 1;
        Combiner.Exit();
    }
    public void Pause()
    {
        PlayMainTitleBGM();
        GamePlayCrossScene.Current.Pause();
        Time.timeScale = 0;
        foreach (var mainTitleOnly in GetComponentsInChildren<MainTitleOnly>(true))
        {
            mainTitleOnly.gameObject.SetActive(false);
        }
        foreach (var pauseOnly in GetComponentsInChildren<PauseOnly>(true))
        {
            pauseOnly.gameObject.SetActive(true);
        }
        Combiner.Enter();
    }
    public void ToMainMenu()
    {
        foreach (var mainTitleOnly in GetComponentsInChildren<MainTitleOnly>(true))
        {
            mainTitleOnly.gameObject.SetActive(true);
            foreach (var ui in mainTitleOnly.GetComponentsInChildren<IMainTitleUI>())
            {
                ui.Enter();
            }
        }
        foreach (var pauseOnly in GetComponentsInChildren<PauseOnly>(true))
        {
            foreach (var ui in pauseOnly.GetComponentsInChildren<IMainTitleUI>())
            {
                ui.Exit();
            }
        }
        SetContinueGameStatus(CheckPoint.HasValidSave);
    }
    public void PlayMainTitleBGM()
    {
        Audio.PlaySound(mainTitleBGM);
    }
    public void StartGame()
    {
        Time.timeScale = 1;
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
        foreach (var pauseOnly in GetComponentsInChildren<PauseOnly>())
        {
            pauseOnly.gameObject.SetActive(false);
        }
        Combiner.Exit();
    }
    private void SetContinueGameStatus(bool valid)
    {
        if (!valid)
        {
            continueGameButton.enabled = false;
            continueGameImage.sprite = notValidButtonSprite;
        }
        else
        {
            continueGameButton.enabled = true;
            continueGameImage.sprite = validButtonSprite;
        }
    }
}
