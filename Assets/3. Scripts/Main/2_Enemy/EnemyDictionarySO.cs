using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



[CreateAssetMenu(fileName = "EnemyDictionary", menuName = "SO/Dictionary/Enemy", order = int.MaxValue) ]

public class EnemyDictionarySO : ScriptableObject
{
    Dictionary<Difficulty, SerializableDictionary<EnemyType, GameObject>> totalShapes;
    Dictionary<Difficulty, SerializableDictionary<EnemyType, EnemyDataSO>> totalData;
    
    [Header("Enemy Shape")]
    [SerializeField] SerializableDictionary<EnemyType, GameObject> shapes_easy;
    [SerializeField] SerializableDictionary<EnemyType, GameObject> shapes_normal;
    [SerializeField] SerializableDictionary<EnemyType, GameObject> shapes_hard;


    [Space(30)]
    [Header("Enemy Data")]
    [SerializeField] SerializableDictionary<EnemyType, EnemyDataSO> data_easy;
    [SerializeField] SerializableDictionary<EnemyType, EnemyDataSO> data_normal;
    [SerializeField] SerializableDictionary<EnemyType, EnemyDataSO> data_hard;

    //=======================================================================================

    public Dictionary<EnemyType, (GameObject, EnemyDataSO) > GetEssentialEnemyData(Difficulty difficulty)
    {
        // Debug.LogError("TRY 2-1");
        // Debug.Log(   totalShapes  );
        // Debug.Log(   totalShapes.Count  );
         

        SerializableDictionary<EnemyType, GameObject> shapeList = totalShapes[difficulty];
        // Debug.LogError("TRY 2-2");
        SerializableDictionary<EnemyType, EnemyDataSO> dataList = totalData[difficulty];
        // Debug.LogError("TRY 2-3");
        
        
        Dictionary<EnemyType, (GameObject, EnemyDataSO) > ret = new();
        
        foreach(var kv in shapeList)
        {
            EnemyType enemyType  = kv.Key;

            GameObject shape = shapeList[enemyType];
            if( shape !=null &&  dataList.TryGetValue(enemyType, out EnemyDataSO data) )
            {
                (GameObject, EnemyDataSO) items = new( shape, data);
                ret[enemyType] = items;
            }

        }

        // Debug.LogError("TRY 2-4");
        return ret;
    } 



    void Awake()
    {
        Sync();
    }

    
    // 유니티 에디터에서 값이 변경될 때마다 호출되는 메서드
    private void OnValidate()
    {
        Sync();
    }


    private void Sync()
    {
        
        //----------------------------------------------
        foreach (EnemyType enemyType in Enum.GetValues(typeof(EnemyType)))
        {
            if( shapes_easy.ContainsKey(enemyType)== false)
            {
                shapes_easy[enemyType] = null;
            }   
            if( shapes_normal.ContainsKey(enemyType)== false)
            {
                shapes_normal[enemyType] = null;
            }   
            if( shapes_hard.ContainsKey(enemyType)== false)
            {
                shapes_hard[enemyType] = null;
            }   

            if( data_easy.ContainsKey(enemyType)== false)
            {
                data_easy[enemyType] = null;
            }   
            if( data_normal.ContainsKey(enemyType)== false)
            {
                data_normal[enemyType] = null;
            }   
            if( data_hard.ContainsKey(enemyType)== false)
            {
                data_hard[enemyType] = null;
            }   
        }



        // Debug.Log($"[난이도별 데이터 초기화] {shapes_hard.Count}");

        //------------------------------------------------------
        totalShapes=new();
        totalShapes[Difficulty.Easy] = shapes_easy;
        totalShapes[Difficulty.Normal] = shapes_normal;
        totalShapes[Difficulty.Hard] = shapes_hard;
        

        totalData=new();
        totalData[Difficulty.Easy] = data_easy;
        totalData[Difficulty.Normal] = data_normal;
        totalData[Difficulty.Hard] = data_hard;


        // Debug.Log($"[적 사전 초기화] 형태 : {totalShapes.Count}, 데이터 :  {totalData.Count}");
    }
}
