using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : DestroyableSingleton<Stage>
{
     public bool initialized {get;private set;}
    
    
    [SerializeField] Transform t_playerSpawnPoint;
    
    public Vector3 playerInitPos => t_playerSpawnPoint.position;
    
    [SerializeField] Transform t_enemySpawnAreaParent;
    [SerializeField] BoxCollider[] enemySpawnArea;

    [Header("Wave")]
    [SerializeField] TotalWaveInfoSO currWaveInfos;     // 해당 스테이지에 사용될 웨이브 정보.
    public int clearedWaveCount;
    public int totalWaveCount => currWaveInfos.waves.Count;
    public bool isVictory => clearedWaveCount >= totalWaveCount;
    public bool isWavePlaying; 
    public float waveStartTime;
    public float wavePlayTime => Time.time - waveStartTime;

    List<Coroutine> spawnRoutines = new(); 
    Coroutine waveRoutine;


    [Header("Events")]
    public Action onWaveStart;





    //==========================================================================================================================
    //
    public void Init(TotalWaveInfoSO waveInfos)
    {
        this.currWaveInfos = waveInfos;
        
        enemySpawnArea = t_enemySpawnAreaParent.GetComponentsInChildren<BoxCollider>();
        spawnRoutines = new();


        initialized = true;
    }


    /// <summary>
    /// 데이터의 새로운 생성 루틴을 실행한다.
    /// </summary>
    public void StartWave()
    {
        //
        waveStartTime = Time.time;
        isWavePlaying = true;
        
        // 해당 웨이브 정보에 해당하는 웨이브 싱행
        StageWaveInfoSO currWaveInfo = currWaveInfos.waves[clearedWaveCount];
        waveRoutine = StartCoroutine( WaveRoutine( currWaveInfo ) );
        
        // 이벤트 알림 : 관련 UI 초기화
        onWaveStart?.Invoke();
    }


    public void FinishWave()
    {
        isWavePlaying = false;

        // 진행중인 웨이브 종료하고 생성정보 초기화.
        if (waveRoutine !=null)
        {
            StopCoroutine(waveRoutine);
        }
        foreach(Coroutine spawnRoutine in spawnRoutines)
        {
            if( spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
            }
        }

        // 승리 판정
        clearedWaveCount++;
        if (isVictory)
        {
            GameManager.Instance.PauseGamePlay(true);
            GamePlayManager.Instance.Victory();
        }
        else
        {
            StartWave();
        }
    }

    //=========================================================================
    /// <summary>
    /// 웨이브 루틴 : 스폰 루틴들의 모음
    /// </summary>
    IEnumerator WaveRoutine(StageWaveInfoSO currWaveInfo )
    {
        int waveNum = currWaveInfo.waveNum;
        float waveDuration = currWaveInfo.waveDuration;
        
        spawnRoutines.Clear();
        foreach( SpawnInfo spawnInfo in currWaveInfo.spawnInfos)
        {
            spawnRoutines.Add( StartCoroutine( SpawnRoutine( waveDuration, spawnInfo)) ); // 추후 종료하기 위해 코루틴 타입으로 생성
        }

        yield return new WaitForSeconds(waveDuration);
        FinishWave();
    }

    /// <summary>
    /// 적 생성 루틴 : waveDuration동안 enemyType적을 totalSpawnCount명을 spawnInterval마다 생성한다. 
    /// </summary>
    IEnumerator SpawnRoutine(float waveDuration, SpawnInfo spawnInfo)
    {
        // 필수조건 :  풀링 매니저가 초기화되어야함.
        yield return new WaitUntil(()=>EnemyPoolManager.Instance.initialized );
        
        // 진행조건 판정 
        int totalSpawnCount = spawnInfo.spawnCount;
        if(totalSpawnCount <= 0)
        {
            yield break;
        }

        //
        EnemyType enemyType = spawnInfo.enemyType;  
        float spawnInterval = waveDuration / totalSpawnCount;
        WaitForSeconds wfs = new WaitForSeconds(spawnInterval);
        
        // 웨이브 진행시간동안 생성 반복
        while( wavePlayTime < waveDuration )
        {
            Vector3 spawnPos =  GetRandomSpawnPoint();
            // Vector3 spawnPos = Vector3.zero;
            EnemyPoolManager.Instance.GetEnemy(enemyType, spawnPos, clearedWaveCount ); 
            // Debug.Log($"루틴 돌아감 {enemyType}");
            yield return wfs;
        }
    }



    //===================================================================
    /// <summary>
    /// 해당 영역에서 임의의 좌표를 얻는다. 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRandomSpawnPoint()
    {
        Vector3 ret = Vector3.zero;

        if (enemySpawnArea.Length>0)
        {
            int randIdx = UnityEngine.Random.Range(0,enemySpawnArea.Length);
            BoxCollider area = enemySpawnArea[randIdx];

            Bounds bounds = area.bounds;

            float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

            ret = new Vector3(randomX, 0, randomZ);
        }

        return ret;
    }




    // //
    // public Vector3 testPos;
    // public float radius = 0.2f; // 구의 반지름
    // public Color color = Color.red;
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = color;
    //     Gizmos.DrawSphere(testPos, radius);
    // }

}

