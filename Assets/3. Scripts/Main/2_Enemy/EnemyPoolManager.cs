using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EnemyPoolManager : DestroyableSingleton<EnemyPoolManager>
{
  public Transform t;
    [SerializeField] EnemyDictionarySO enemyDictionary;                           // 프리팹 정보들로 미리 풀 데이터 세팅
    [SerializeField] SerializableDictionary<EnemyType, Pool<Enemy>> _pools = new();     // 풀
 
    [SerializeField] SerializableDictionary<EnemyType, EnemyDataSO> enemyData= new();
    // [SerializeField] SerializableDictionary<EnemyType, PoolData> totalPoolData = new(); // 이번스테이지에 사용할 풀 데이터 


    private void Start()
    {
        Init();
    }


    void Init()
    {
        t=transform;
        //
        InitPoolsWithDifficulty();





        foreach(var kv in _pools)
        {
            Debug.Log($"{kv.Key} {kv.Value}");
        }
    }

    void InitPoolsWithDifficulty()
    {
        Difficulty difficulty = GameManager.Instance.currDifficulty;

        _pools = new();
        enemyData = new();
        SerializableDictionary<EnemyType, PoolData> totalPoolData = new();
        
        var essentialData = enemyDictionary.GetEssentialEnemyData(difficulty);
        foreach(var kv in essentialData)
        { 
            EnemyType type = kv.Key;
            GameObject shape = kv.Value.Item1;
            EnemyDataSO data = kv.Value.Item2;

            
            Enemy enemy = shape.GetComponent<Enemy>();

            // 풀데이터 
            if(totalPoolData.ContainsKey(type) == false)
            {
                PoolData poolData = new();
                poolData._name = $"{type}_{difficulty}";
                poolData._component = enemy;
                
                totalPoolData[type] = poolData;          // 풀데이터 초기화.


                var pool = Pool<Enemy>.Create( (Enemy)poolData.Component, poolData.Count, t);
                _pools[type] = pool;

                if (poolData.NonLazy)
                {
                    pool.NonLazy();
                }

                //
                enemyData[type] = data;             //데이터도 초기화
            }
        }
    }



    //================================================================================
    public Enemy GetEnemy(EnemyType enemyType, Vector3 initPos,int waveNum)
    {

        // 적 데이터를 가져옵니다.
        EnemyDataSO data  = enemyData[enemyType];

        // Enemy 풀에서 객체를 가져옵니다.
        var pool = GetPool(enemyType);
        var enemy = pool.Get();        // 초기화 필요.
        enemy.transform.position = initPos;
        enemy.Init( data, waveNum,initPos );
        
        return enemy;
    }


    private Pool<Enemy> GetPool(EnemyType type)
    {
        if (_pools.TryGetValue(type, out var pool))
        {
            return pool;
        }

        throw new InvalidOperationException($"No pool exists for type {type}");
    }


    public void ReturnEnemy(Enemy enemy)
    {
        var pool = GetPool(enemy.enemyData.type);
        pool.Take(enemy);
    }

    //======================================================
}
