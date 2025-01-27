using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TotalaveData", menuName = "SO/TotalWaveData", order = int.MaxValue)]
public class TotalWaveInfoSO : ScriptableObject
{
    public int difficulty;
    public List<StageWaveInfoSO> waves;


    void OnValidate()
    {
        for(int i=0;i<waves.Count;i++)
        {
            waves[i].waveNum = i+1;

        }
    }










}





