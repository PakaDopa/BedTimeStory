using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GamePlayManager : DestroyableSingleton<GamePlayManager>
{
    [Header("Stage Setting")]
    // [SerializeField] List<GameObject> prefabs_stage = new();
    // [SerializeField] List<TotalWaveInfoSO> totalWaves = new();   

    [SerializeField] SerializableDictionary<Difficulty, GameObject> prefabs_stage= new();
    [SerializeField] SerializableDictionary<Difficulty, TotalWaveInfoSO> totalWaves =new(); //난이도에 따른 웨이브 정보.
    [SerializeField] SerializableDictionary<Difficulty, SoundEventSO> bgms= new();

    
    [Header("Sound Events")]
    // [SerializeField] SoundEventSO bgm;
    [SerializeField] SoundEventSO gameOver;
    [SerializeField] SoundEventSO gameWin;

    // [SerializeField] Button testWaveStartBtn;
    [SerializeField] CutSceneManager cutSceneManager;
    [SerializeField] bool inGameCutSceneEnable = true;
    [SerializeField] PlayableDirector inGameCutScene;
    SkipCutScene skipCutScene;


    //======= UI ==========
    [Header("UI")]
    [SerializeField] Canvas mainUICanvas;
    [SerializeField] VictoryPanel victoryPanel;
    [SerializeField] GameOverPanel gameOverPanel;


    //=====================

    public static bool isGamePlaying;
    public static bool gameFinished;
    public bool initialized;

    //
    void Start()
    {
        skipCutScene = GetComponent<SkipCutScene>();
        // testWaveStartBtn.onClick.AddListener(  StartWave );
        gameFinished = false;
        // bgm.Raise();
        // StartGame();
        if(GameManager.Instance.currGamePlayInfo == null)
        {
            GameManager.Instance.currGamePlayInfo = new(Difficulty.Easy);
        }

        StartCoroutine( GameStartRoutine() );
    }


    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Alpha0))
        {
            Stage.Instance.FinishWave();
        }

    }


    // public void StartGame()
    // {
        

    //     Debug.Log("게임 시작");
    // }

    IEnumerator GameStartRoutine()
    {
        isGamePlaying = false;
        
        //
        SetStage();
        Tower.Instance.Init();              // 
        Player.Instance.Init();             // 
        mainUICanvas.enabled = false; 

        //
        GameEventManager.Instance.onGameStart.Invoke();
        
        // 컷씬대기, 지금은 컷신 비 활성하임.
        if( CutSceneManager.isCutSceneEnabled )
        {
            StartCoroutine(cutSceneManager.PlayCutScene());
            yield return new WaitUntil( ()=>cutSceneManager.gameObject.activeSelf == false);
        }
        
        if(inGameCutSceneEnable == true)
        {
            inGameCutScene.Play();
            yield return new WaitUntil( () => inGameCutScene.state != PlayState.Playing);
        }

        initialized =  true;
        // 게임 플레이 시작
        isGamePlaying = true;
        Time.timeScale = 1;
        StartWave();
        mainUICanvas.enabled = true;
    }

    void SetStage()
    {
        Difficulty currDifficulty = GameManager.Instance.currGamePlayInfo.currDifficulty;
        Debug.Log($"현재 난이도 : {currDifficulty}");

        GameObject prefab_stage = prefabs_stage[currDifficulty];
        TotalWaveInfoSO waves = totalWaves[currDifficulty];


        Stage stage = Instantiate(prefab_stage, Vector3.zero, Quaternion.identity ).GetComponent<Stage>();
        stage.Init(waves);

        
        // 브금도 재생
        var bgm = bgms[currDifficulty];
        bgm.Raise();
    }

    
    public void StartWave()
    {
        Debug.Log("웨이브 시작");
        Stage.Instance.StartWave();
    }


    public void GameOver()
    {
        if( gameFinished )
        {
            return;
        }
        
        
        gameOver.Raise();

        isGamePlaying = false;
        GameManager.Instance.LockCursor(false);
        gameOverPanel.Open();

        gameFinished = true;
    }

    public void Victory()
    {
        if( gameFinished )
        {
            return;
        }
        
        
        // 통계 기록. 
        GameManager.Instance.currGamePlayInfo.OnVictory();
        //
        Debug.Log("승리!");
        
        isGamePlaying = false;
        GameManager.Instance.LockCursor(false);
        victoryPanel.Open();


        gameFinished = true;
    }

    public void EnablePanel(bool enable)
    {
        isGamePlaying = enable;
        GameManager.Instance.LockCursor(enable);
    }





    public void Debug_GetGold()
    {
        int value = 10000;
        PlayerStats.Instance.GetGold( value );
    }

    public void Debug_Recover()
    {
        int value = 100;
        PlayerStats.Instance.Recover(value);
    }   

}
