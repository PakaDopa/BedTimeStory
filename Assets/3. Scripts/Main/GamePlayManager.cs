using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum GamePlayState
{
    WaitForInitialized,
    IsPlaying,
    IsFinished
}

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

    public static bool isGamePlaying;   //
    public static bool gameActiavated;  // 게임 활성화 
    public bool initialized;


    [Header("Events")]
    public Action onGamePlayStart;


    //
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>SoundManager.Instance.initialized);
        
        skipCutScene = GetComponent<SkipCutScene>();
        // testWaveStartBtn.onClick.AddListener(  StartWave );
        gameActiavated = false;
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
#if UNITY_EDITOR
        if( Input.GetKeyDown( KeyCode.Alpha0))
        {
            Stage.Instance.FinishWave();
        }
#endif
    }


    // public void StartGame()
    // {
        

    //     Debug.Log("게임 시작");
    // }

    IEnumerator GameStartRoutine()
    {
        
        GameManager.Instance.PauseGamePlay(false);

        isGamePlaying = false;
        
        //
        SetStage();                         // 스테이지 초기화 
        Tower.Instance.Init();              // 타워 초기화
        Player.Instance.Init();             // 플레이어 초기화
        mainUICanvas.enabled = false; 

        //
        onGamePlayStart?.Invoke();          // 게임 초기화 알림. : UI 초기작업
        
        // 컷씬대기, 디버그할 땐 플래그 false 해놓기
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

        // 게임 플레이 시작
        gameActiavated = true;
        isGamePlaying = true;
        Time.timeScale = 1;
        initialized =  true;

        mainUICanvas.enabled = true;
        yield return null;
        StartWave();        
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
        SoundManager.Instance.PlayBgm(bgm);
    }

    
    public void StartWave()
    {
        Debug.Log("웨이브 시작");
        Stage.Instance.StartWave();
    }


    public void GameOver()
    {
        if( gameActiavated ==false)
        {
            return;
        }
        
        
        // gameOver.Raise();
        SoundManager.Instance.Play(gameOver,transform.position);


        
        GameManager.Instance.LockCursor(false);
        gameOverPanel.Open();

        isGamePlaying = false;
        gameActiavated = false;
    }

    public void Victory()
    {
        if( gameActiavated == false)
        {
            return;
        }
        
        
        // 통계 기록. 
        SoundManager.Instance.Play(gameWin,transform.position);
        GameManager.Instance.currGamePlayInfo.OnVictory();
        //
        Debug.Log("승리!");
        
        
        GameManager.Instance.LockCursor(false);
        victoryPanel.Open();

        isGamePlaying = false;
        gameActiavated = false;
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
