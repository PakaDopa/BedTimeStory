using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
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
    [SerializeField] bool inGameCutSceneEnable = true;
    [SerializeField] PlayableDirector inGameCutScene;
    SkipCutScene skipCutScene;


    //======= UI ==========
    [Header("UI")]
    [SerializeField] Canvas mainUICanvas;
    [SerializeField] GameObject victoryNotice;
    [SerializeField] GameObject gameOverNotice;
    // [SerializeField] VictoryPanel victoryPanel;
    // [SerializeField] GameOverPanel gameOverPanel;

    [SerializeField] ComboToastUI comboToastUI;
    [SerializeField] KillMark killMark;


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

        if(Input.GetKey(KeyCode.Alpha6) )
        {
            if( Input.GetKeyDown(KeyCode.Alpha8))
            {
                Victory();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                GameOver();
            }


            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug_SpawnItem();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.PauseGamePlay(!GameManager.Instance.isPaused);
        }

#endif
    }


    // public void StartGame()
    // {
        

    //     Debug.Log("게임 시작");
    // }



    IEnumerator GameStartRoutine()
    {
        comboToastUI.Init();
        killMark.Init();


        GameManager.Instance.PauseGamePlay(false);

        isGamePlaying = false;
        
        //
        SetStage();                         // 스테이지 초기화 
        Tower.Instance.Init();              // 타워 초기화
        Player.Instance.Init();             // 플레이어 초기화
        mainUICanvas.enabled = false; 

        //
        onGamePlayStart?.Invoke();          // 게임 초기화 알림. : UI 초기작업
        
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

        // Debug_WaveText(waves);

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
        
        Debug.Log("패배");
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isGamePlaying = false;
        gameActiavated = false;
        Player.Instance.OnDefeated();


        //
        SoundManager.Instance.Play(gameOver,transform.position);
        gameOverNotice.SetActive(true);
        GameManager.Instance.LockCursor(false);
        
        //
        yield return new WaitForSecondsRealtime(2);
        
        //
        SceneLoadManager.Instance.Load_Result();
    }



    public void Victory()
    {
        if( gameActiavated == false)
        {
            return;
        }
        
    
        //    
        Debug.Log("승리");
        StartCoroutine(VictoryRoutine());
    }

    IEnumerator VictoryRoutine()
    {
        isGamePlaying = false;
        gameActiavated = false;
        
        Player.Instance.OnVictory();
        
        //
        SoundManager.Instance.Play(gameWin,transform.position);
        GameManager.Instance.LockCursor(false);
        victoryNotice.SetActive(true);

        // 통계 기록. 
        GameManager.Instance.currGamePlayInfo.OnVictory();
        EnemyPoolManager.Instance.OnPlayerVictory();

        // yield return null;
        yield return new WaitForSecondsRealtime(2);
        

        //
        SceneLoadManager.Instance.Load_Result();
    }

    public void OnKillEnemy()
    {
        int killCount = ++GameManager.Instance.currGamePlayInfo.killCount;
        Debug.Log($"킬 {killCount}");

        if( killCount%30 ==0)
        {
            comboToastUI.Activate(killCount);
        }

        killMark.Activate();
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

    [SerializeField] TextMeshProUGUI text_debug;
    public void Debug_WaveText(TotalWaveInfoSO waves)
    {
        string str=$"{waves.difficulty}\n";
        foreach( var wave in waves.waves)
        {
            string str2= $"{wave.waveNum}  {wave.waveDuration} : ";
            string str3= "";
            foreach( var si in wave.spawnInfos)
            {
                str3 += $"{si.enemyType} {si.spawnCount}, ";
            }
            str2 += str3;
            str += $"{str2} {str3}\n";
        }

        text_debug.SetText(str);
    }



    public void Debug_SpawnItem()
    {
            // 골드 주머니: 0~9 (10%)
        int rand = UnityEngine.Random.Range(0, 100);

        Vector3 randomSpawnPoint = Player.Instance.T.position + new Vector3(2,0,2);
        if (rand < 50)
        {
            DropItemManager.Instance.GetItem_Pouch(randomSpawnPoint);
        }
        // 소형 포션: 10~19 (10%)
        else
        {
            DropItemManager.Instance.GetItem_Potion(randomSpawnPoint);
        }
    }
}
