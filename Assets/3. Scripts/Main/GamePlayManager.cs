using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : DestroyableSingleton<GamePlayManager>
{
    [SerializeField] SoundEventSO bgm;
    [SerializeField] SoundEventSO gameOver;
    [SerializeField] SoundEventSO gameWin;

    [SerializeField] Button testWaveStartBtn;

    public static bool isGamePlaying;

    //
    void Start()
    {
        testWaveStartBtn.onClick.AddListener(  StartWave );
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
        isGamePlaying = true;
        
        Tower.Instance.Init();
        Stage.Instance.Init();
        
        GameEventManager.Instance.onGameStart.Invoke();


        Debug.Log("게임 시작");
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
    }

}
