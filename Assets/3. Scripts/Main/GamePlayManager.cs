using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : DestroyableSingleton<GamePlayManager>
{
    [Header("Stage Setting")]
    // [SerializeField] List<GameObject> prefabs_stage = new();
    // [SerializeField] List<TotalWaveInfoSO> totalWaves = new();   

    [SerializeField] SerializableDictionary<Difficulty, GameObject> prefabs_stage= new();
    [SerializeField] SerializableDictionary<Difficulty, TotalWaveInfoSO> totalWaves =new(); //난이도에 따른 웨이브 정보.

    
    [Header("Sound Events")]
    [SerializeField] SoundEventSO bgm;
    [SerializeField] SoundEventSO gameOver;
    [SerializeField] SoundEventSO gameWin;

    // [SerializeField] Button testWaveStartBtn;
    [SerializeField] CutSceneManager cutSceneManager;


    //======= UI ==========
    [Header("UI")]
    [SerializeField] VictoryPanel victoryPanel;
    [SerializeField] GameOverPanel gameOverPanel;


    //=====================

    public static bool isGamePlaying;

    //
    void Start()
    {
        // testWaveStartBtn.onClick.AddListener(  StartWave );
        bgm.Raise();
        // StartGame();

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


        //
        GameEventManager.Instance.onGameStart.Invoke();
        
        // 컷씬대기
        if( CutSceneManager.isCutSceneEnabled )
        {
            StartCoroutine(cutSceneManager.PlayCutScene());
            yield return new WaitUntil( ()=>cutSceneManager.gameObject.activeSelf == false);
        }


        // 게임 플레이 시작
        isGamePlaying = true;
        Time.timeScale = 1;
        StartWave();
    }

    void SetStage()
    {
        Difficulty currDifficulty = GameManager.Instance.currDifficulty;
        Debug.Log($"현재 난이도 : {currDifficulty}");

        GameObject prefab_stage = prefabs_stage[currDifficulty];
        TotalWaveInfoSO waves = totalWaves[currDifficulty];

        Stage stage = Instantiate(prefab_stage, Vector3.zero, Quaternion.identity ).GetComponent<Stage>();
        stage.Init(waves);
    }

    
    public void StartWave()
    {
        Debug.Log("웨이브 시작");
        Stage.Instance.StartWave();
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
