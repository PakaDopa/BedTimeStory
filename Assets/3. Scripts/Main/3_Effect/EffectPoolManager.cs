using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct PoolData
{
    [SerializeField]
    public string _name;

    public string Name => _name;

    [SerializeField]
    public Component _component;

    public Component Component => _component;

    [SerializeField]
    [Min(0)]
    private int _count;

    public int Count => _count;

    [SerializeField]
    private Transform _container;

    public Transform Container => _container;

    [SerializeField]
    private bool _nonLazy;

    public bool NonLazy => _nonLazy;
}



public class EffectPoolManager : DestroyableSingleton<EffectPoolManager>
{
    public Transform t;
    [SerializeField] EffectDictionarySO effectDictionary;                           // 프리팹 정보들로 미리 풀 데이터 세팅
    [SerializeField] SerializableDictionary<Type, Pool<Effect>> _pools = new();     // 풀


    private void Start()
    {
        Init();
    }


    void Init()
    {
        t=transform;
        //
        _pools = new();
        foreach( var kv in effectDictionary.totalPoolData)
        {
            PoolData poolData = kv.Value;


            var type = poolData.Component.GetType();
            var pool = Pool<Effect>.Create( (Effect)poolData.Component, poolData.Count, t);
            _pools[type] = pool;

            if (poolData.NonLazy)
            {
                pool.NonLazy();
            }
        }


        foreach(var kv in _pools)
        {
            Debug.Log($"{kv.Key} {kv.Value}");
        }
    }


    //================================================================================
    Effect GetEffect(EffectEvent effectEvent)
    {
        // EnemyDataSO data = enemyData.GetData(id);
        // Enemy enemy = GetFromPool<Enemy>(id); 
        // enemy.Init( data,initPos );
        // 적 데이터를 가져옵니다.
        PoolData poolData = effectDictionary.totalPoolData[effectEvent];

        // Enemy 풀에서 객체를 가져옵니다.
        var pool = GetOrCreatePool(poolData);

        var effect = pool.Get();        // 초기화 필요.
        return effect;
    }


    private Pool<Effect> GetPool(Type type)
    {
        if (_pools.TryGetValue(type, out var pool))
        {
            return pool;
        }

        throw new InvalidOperationException($"No pool exists for type {type}");
    }

    private Pool<Effect> GetOrCreatePool(PoolData poolData)
    {
        var type = poolData.Component.GetType();

        if (!_pools.TryGetValue(type, out var pool))
        {
            pool = Pool<Effect>.Create( (Effect)poolData.Component, poolData.Count, poolData.Container);
            _pools[type] = pool;
        }

        return pool;
    }

    public void ReturnEffect(Effect effect)
    {
        var pool = GetPool(effect.GetType());
        pool.Take(effect);
    }

    //======================================================
    #region TextEffect
    public void GetNormalDamageText(Vector3 initPos, float value)
    {
        float lifeTime = 0.6f;

        TextEffect effect = (TextEffect)GetEffect(EffectEvent.Text);
        effect.Init(initPos, lifeTime, $"{value:0}");
    }
    
    public void GetHealAmountText(Vector3 initPos, float value)
    {
        float lifeTime = 0.6f;
        TextEffect effect = (TextEffect)GetEffect(EffectEvent.Text);
        effect.Init(initPos, lifeTime, $"{value:0}", TextEffectType.PlayerRecover);
    }

    #endregion


}
