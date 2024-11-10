using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class WaveManager : DestroyableSingleton<WaveManager>
{
    [SerializeField] WaveInfoUI waveInfoUI;
    
    public List<GameObject> enemyPrefabs = new();
    
    public GameObject prefab_enemy;

    public StageWaveInfoSO stageWaveInfo;

    public int clearedWaveNum;

    public int currWaveNum => clearedWaveNum +1;

    public bool isVictory => clearedWaveNum >= stageWaveInfo.waveInfos.Count;

    public bool isWavePlaying; 
    public float waveStartTime;
    public float wavePlayTime;
    
    Coroutine durationRoutine;
    Coroutine spawnRoutine;

    //=======================================================================================================s

    public void StartWave()
    {
        // Time.timeScale = 16;

        waveStartTime = Time.time;
        wavePlayTime = 0;

        WaveInfo waveInfo  = stageWaveInfo.waveInfos[clearedWaveNum];
        
        float waveDuration = waveInfo.duration;
        float cycleGap = waveInfo.cycleGap;
        int spawnPerCycle = waveInfo.spawnPerCycle;
        

        IncreaseDifficulty();
        durationRoutine = StartCoroutine( DurationRoutine (waveDuration));
        spawnRoutine = StartCoroutine( SpawnRoutine (spawnPerCycle, cycleGap, waveDuration));


        GameEventManager.Instance.onWaveStart.Invoke();
    }  


    /// <summary>
    ///  웨이브 종료 : 진행중이던 코루틴을 종료한다. 
    /// </summary>
    public void FinishWave()
    {
        if( isWavePlaying == false)
        {
            return;
        }
        isWavePlaying = false;
        
        
        if (durationRoutine != null)
        {
            StopCoroutine(durationRoutine);
        }
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        clearedWaveNum++;
        
        if (isVictory)
        {
            GamePlayManager.Instance.Victory();
        }
        else
        {
            StartWave();
        }
        
        
        GameEventManager.Instance.onWaveFinish.Invoke();
    }
    

    /// <summary>
    /// 현재 웨이브에 맞춰 난이도를 높인다. 
    /// </summary>
    public void IncreaseDifficulty()
    {
        //적 체력 향상
        
    }

    /// <summary>
    /// 웨이브 지속시간 끝나는거 감지
    /// </summary>
    /// <param name="waveDuration"></param>
    /// <returns></returns>
    IEnumerator DurationRoutine( float waveDuration)
    {
        waveStartTime = Time.time;
        isWavePlaying = true;
        while(wavePlayTime < waveDuration )
        {
            wavePlayTime = Time.time - waveStartTime;

            yield return new WaitForSeconds(1f);
        }
        FinishWave();
        
    }

    /// <summary>
    /// 적 생성 루틴. 
    /// </summary>
    /// <param name="spawnPerCycle"></param>
    /// <returns></returns>
    IEnumerator SpawnRoutine( int spawnPerCycle,float cycleGap, float waveDuration)
    {
        int totalSpawnCount = spawnPerCycle *   (int)(waveDuration/ cycleGap);
        int currSpawnCount = 0;
        
        while( isWavePlaying )
        {
            for(int i=0;i< spawnPerCycle;i++)
            {
                Vector3 spawnPos = Stage.Instance.GetRandomSpawnPoint();
                
                int idx = Random.Range(0, enemyPrefabs.Count); 
                GameObject enemyPrefab =  enemyPrefabs[idx];
                
                Enemy enemy = Instantiate( enemyPrefab,spawnPos,Quaternion.identity ).GetComponent<Enemy>();  
                enemy.Init( clearedWaveNum);
            }

            currSpawnCount += spawnPerCycle;        //2초마다이면 1초마다 어느정도는 소환하도록 수정할 예정. 
            // Debug.Log($"  적생성 : {cycleGap}초 마다 {spawnPerCycle}마리 , 총 :  {currSpawnCount} / {totalSpawnCount}");

            yield return new WaitForSeconds(cycleGap);
        }

        
    }

}
