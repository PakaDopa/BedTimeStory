using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

using GameUtil;
using DG.Tweening;

[RequireComponent(typeof(EnemyAI), typeof(Collider), typeof(Rigidbody) )]
public class Enemy : MonoBehaviour
{
    [SerializeField] Animator aniamtor;

    public Transform t;
    public EnemyAI enemyAI;
    public EnemyDataSO enemyData;

    //
    Collider _collider;
    Rigidbody _rb;

    public float maxHp;
    public float currHp;

    public float dmg;
    public float movementSpeed;


    //
    public bool isAlive => currHp > 0;
    Vector3 lastHitPoint;

    [SerializeField] float stopDurationRemain;
    public bool stopped => stopDurationRemain > 0;
    //
    public float stunDurationRemain;
    public bool stunned => stunDurationRemain > 0;

    public float lastAttackTime;
    public bool attackAvailable => Time.time >= lastAttackTime + enemyData.attackSpeed;

    public bool isCasting;


    Slider_EnemyHp enemyState;
    //===================================

    void Update()
    {
        if (isAlive == false || GamePlayManager.isGamePlaying == false )
        {
            return;
        }

        // 정지 지속시간 감소
        if (stopDurationRemain > 0)
        {
            stopDurationRemain -= Time.deltaTime;
        }

        // 스턴 지속시간 감소
        if (stunDurationRemain > 0)
        {
            stunDurationRemain -= Time.deltaTime;
        
        }

        if (stunned)
        {
            return;
        }

        // 스턴걸리면 아래까지 안내려가게.
        enemyAI.OnUpdate();
    }    



    public void Init(int waveNum)
    {
        t = transform;
        
        // this.enemyData = enemyData;
        enemyAI = GetComponent<EnemyAI>();
        

        _collider = GetComponent<Collider>();
        _collider.enabled = true;

        _rb = GetComponent<Rigidbody>();

    
        InitStatus(waveNum);
        

        enemyAI.Init( this, waveNum);


        enemyState = EnemyCanvas.Instance.Generate_EnemyHpBar();
        enemyState?.Init(this);
    }

    void InitStatus(int waveNum)
    {
        maxHp = enemyData.maxHp +  enemyData.inc_maxHp * waveNum;
        movementSpeed = enemyData.movementSpeed + enemyData.inc_movementSpeed * waveNum;
        dmg = enemyData.dmg + enemyData.inc_dmg * waveNum;
        
        currHp = maxHp;
    }

    // void OnTriggerEnter(Collider other)
    // {
        // lastHitPoint = other.ClosestPoint(transform.position);
    // }


    //======================================================

    public void GetDamaged(float damage)
    {
        // float nockbackPower = 5;
        // GetKnockback(nockbackPower, lastHitPoint);
        
        //
        currHp -= damage;
        if (currHp <= 0)
        {
            Die();
        }
        // Debug.Log($"앗 {currHp}/ {maxHp}");
        // ui
        enemyState?.OnUpdateEnemyHp();
    }


    // void GetKnockback(float power, Vector3 hitPoint)
    // {
    //     enemyAI.SetStunned(0.5f);

    //     Vector3 dir = (t.position - hitPoint).WithFloorHeight().normalized;
    //     _rb.velocity = dir * power;

    //     DOTween.Sequence()
    //     .AppendInterval(0.2f)
    //     .AppendCallback(() => _rb.velocity = Vector3.zero)
    //     .Play();
    // }

    /// <summary>
    ///  정지 상태 적용 - 움직이지 못하게. - 스킬 사용, 피격 or 사망  등
    /// </summary>
    /// <param name="duration"></param>
    // public void SetStopped(float duration)
    // {
    //     stopDurationRemain = Math.Max(stopDurationRemain, duration);
    //     enemyAI.OnStopped();

    // }

    /// <summary>
    /// 기절 상태 적용 - 넉백시. or 기타 군중제어 
    /// </summary>
    /// <param name="duration"></param>
    // public void SetStunned(float duration)
    // {
    //     stunDurationRemain = Math.Max(stunDurationRemain, duration);
    //     SetStopped(duration);   // 
    // }

    void Die()
    {
        _collider.enabled = false;
        DropItem();
        enemyState?.OnEnemyDie();
        enemyAI.OnDie();
        aniamtor.SetBool("isDead", true);
        Destroy(gameObject, 6.0f);
    }

    void DropItem()
    {        
        // string str = "돈1원 ";

        PlayerStats.Instance.GetGold(10);
        int rand = UnityEngine.Random.Range(0, 100);
        if ( 95<= rand)
        // if ( 66<= rand)
        {
            // str+="골드주머니 ";
            DropItemManager.Instance.GetItem_Pouch(t.position);
        }
        else if ( 90 <=rand )
        {
            // str+="소형 포션 ";
            DropItemManager.Instance.GetItem_Potion(t.position);

        }
        // Debug.Log($"아이템 드랍  r {rand} : {str}");
    }


    public void Attack(Vector3 targetPos)
    {
        isCasting = true;
        enemyData.Attack(this,targetPos);
        lastAttackTime = Time.time;

        StopAttack();
    }

    public void StopAttack()
    {
        isCasting = false;
    }



}

