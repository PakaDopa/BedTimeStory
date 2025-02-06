using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    public EnemyType enemyType; 
    
    public int spawnCount;
    
}

// [CreateAssetMenu(fileName = "WaveData", menuName = "SO/WaveData", order = int.MaxValue)]
[System.Serializable]
public class StageWaveInfoSO
{
    public int waveNum;         // 해당 웨이브 번호
    public float waveDuration;    // 해당 지속시간
    
    public List<SpawnInfo> spawnInfos = new();
}
