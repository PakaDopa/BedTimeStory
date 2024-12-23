using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : DestroyableSingleton<GamePlayManager>
{
    [SerializeField] SoundEventSO bgm;
    [SerializeField] SoundEventSO gameOver;
    [SerializeField] SoundEventSO gameWin;

    // [SerializeField] Button testWaveStartBtn;
    [SerializeField] CutSceneManager cutSceneManager;


    //======= UI ==========
    [SerializeField] VictoryPanel victoryPanel;
    [SerializeField] GameOverPanel gameOverPanel;


    //=====================

    public static bool isGamePlaying;

    //
    void Start()
    {
        // testWaveStartBtn.onClick.AddListener(  StartWave );
        bgm.Raise();
        StartGame();
    }


    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Alpha0))
        {
            StartWave();
        }

    }


    public void StartGame()
    {
        isGamePlaying = false;
        
        Tower.Instance.Init();
        Stage.Instance.Init();
        
        GameEventManager.Instance.onGameStart.Invoke();

        StartCoroutine(cutSceneManager.PlayCutScene());

        Debug.Log("게임 시작");
    }


    public void OnCutSceneFinish()
    {
        isGamePlaying = true;
        Time.timeScale = 1;

        StartWave();
    }

    
    public void StartWave()
    {
        Debug.Log("웨이브 시작");
        WaveManager.Instance.FinishWave();
        WaveManager.Instance.StartWave();

        
    }


    public void GameOver()
    {
        gameOver.Raise();

        isGamePlaying = false;
        GameManager.Instance.LockCursor(false);

        gameOverPanel.Open();
    }

    public void Victory()
    {
        Debug.Log("승리!");
        
        isGamePlaying = false;
        
        GameManager.Instance.LockCursor(false);

        victoryPanel.Open();
        
    }

}
