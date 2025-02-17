using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : DestroyableSingleton<Stage>
{
     
    
    
    [SerializeField] Transform t_playerSpawnPoint;
    
    public Vector3 playerInitPos => t_playerSpawnPoint.position;
    
    [SerializeField] Transform t_enemySpawnAreaParent;
    [SerializeField] BoxCollider[] enemySpawnArea;

    [Header("Wave")]
    [SerializeField] TotalWaveInfoSO currWaveInfos;     // 해당 스테이지에 사용될 웨이브 정보.
    public int clearedWaveCount;
    public bool isVictory => clearedWaveCount >= currWaveInfos.waves.Count;
    public bool isWavePlaying; 
    public float waveStartTime;
    public float wavePlayTime => Time.time - waveStartTime;

    List<Coroutine> spawnRoutines = new(); 
    Coroutine waveRoutine;





    //==========================================================================================================================
    //
    public void Init(TotalWaveInfoSO waveInfos)
    {
        this.currWaveInfos = waveInfos;
        
        enemySpawnArea = t_enemySpawnAreaParent.GetComponentsInChildren<BoxCollider>();
        spawnRoutines = new();
    }



    public void StartWave()
    {
        // 진행중인 웨이브 종료 
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
        
        waveStartTime = Time.time;
        isWavePlaying = true;
        //
        StageWaveInfoSO currWaveInfo = currWaveInfos.waves[clearedWaveCount];
        waveRoutine = StartCoroutine( WaveRoutine( currWaveInfo ) );
        //

        GameEventManager.Instance.onWaveStart.Invoke();
    }


    public void FinishWave()
    {
        isWavePlaying = false;
        
        //
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
        
        
        GameEventManager.Instance.onWaveFinish.Invoke();
    }

    //=========================================================================


    IEnumerator WaveRoutine(StageWaveInfoSO currWaveInfo )
    {
        yield return new WaitUntil(()=>EnemyPoolManager.Instance.initialized );
        
        
        int waveNum = currWaveInfo.waveNum;
        float waveDuration = currWaveInfo.waveDuration;
        
        
        spawnRoutines.Clear();
        foreach( SpawnInfo spawnInfo in currWaveInfo.spawnInfos)
        {
            spawnRoutines.Add( StartCoroutine( SpawnRoutine( waveDuration, spawnInfo)) ); 
        }

        
        yield return new WaitForSeconds(waveDuration);

        FinishWave();
    }

    IEnumerator SpawnRoutine(float waveDuration, SpawnInfo spawnInfo)
    {
        int totalSpawnCount = spawnInfo.spawnCount;

        if(totalSpawnCount <= 0)
        {
            yield break;


        }
        //
        float spawnInterval = waveDuration / totalSpawnCount;
        WaitForSeconds wfs = new WaitForSeconds(spawnInterval);

        EnemyType enemyType = spawnInfo.enemyType;

        while( wavePlayTime < waveDuration )
        {
            Vector3 spawnPos =  GetRandomSpawnPoint();

            EnemyPoolManager.Instance.GetEnemy(enemyType, spawnPos, clearedWaveCount );        //풀링해서 소환.

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
            int randIdx = Random.Range(0,enemySpawnArea.Length);
            BoxCollider area = enemySpawnArea[randIdx];

            Bounds bounds = area.bounds;

            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

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

