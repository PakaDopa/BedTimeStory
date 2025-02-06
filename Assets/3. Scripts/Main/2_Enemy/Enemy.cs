using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

using GameUtil;
using DG.Tweening;


[RequireComponent(typeof(EnemyAI), typeof(Collider), typeof(Rigidbody) )]
public class Enemy : MonoBehaviour, IPoolObject
{
    [SerializeField] Animator animator;

    public Transform t;
    public EnemyAI enemyAI;
    public EnemyDataSO enemyData;

    //
    [SerializeField] Transform t_damageEffectPos;
    CapsuleCollider _collider;

    Rigidbody _rb;

    public float damageEffectPos =>t_damageEffectPos?t_damageEffectPos.position.y:t.position.y;

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


    // Slider_EnemyHp enemyState;
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

    public void OnCreatedInPool()
    {
        t = transform;
        enemyAI = GetComponent<EnemyAI>();
        _collider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        animator= GetComponentInChildren<Animator>();
    }

    public void OnGettingFromPool()
    {

    }



    public void Init(EnemyDataSO enemyData,int clearedwaveCount, Vector3 initPos)
    {
        transform.position = initPos;
        this.enemyData = enemyData;

        InitStatus(clearedwaveCount);
    
        enemyAI.Init( this, clearedwaveCount);

        _collider.enabled = true;
    }

    void InitStatus(int clearedWaveCount)
    {
        maxHp = enemyData.maxHp +  enemyData.inc_maxHp * clearedWaveCount;
        movementSpeed = enemyData.movementSpeed + enemyData.inc_movementSpeed * clearedWaveCount;
        dmg = enemyData.dmg + enemyData.inc_dmg * clearedWaveCount;
        
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
        // enemyState?.OnUpdateEnemyHp();
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
        // enemyState?.OnEnemyDie();
        enemyAI.OnDie();
        animator.SetTrigger(hash_die);


        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(5f);

        EnemyPoolManager.Instance.ReturnEnemy(this);
    }


    void DropItem()
    {        
        // string str = "돈1원 ";

        PlayerStats.Instance.GetGold(30);
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


    #region Ability
    [Header("Ability")]
    [SerializeField] EnemyAnimationEvent_Attack attackAnimationEvent;

    static int hash_attack = Animator.StringToHash("attack");
    static int hash_die = Animator.StringToHash("die");
    static int hash_movementSpeed = Animator.StringToHash("movementSpeed");
    

    public void StartAttack(Vector3 targetPos)
    {
        
        StartCoroutine(AbilityRoutine(targetPos));
    }

    IEnumerator AbilityRoutine(Vector3 targetPos)
    {
        // 
        if( attackAnimationEvent == null)
        {
            yield break;
        }
        
        //
        // Debug.Log("공격시작");
        isCasting = true;
        animator.SetBool(hash_attack, true);
        attackAnimationEvent.OnStart();
        yield return new WaitUntil(()=> attackAnimationEvent.AbilityActivationTime == true || isCasting == false);
        // Debug.Log("퍽");
        enemyData.Attack(this, targetPos);
        lastAttackTime = Time.time;
        

        yield return new WaitUntil(()=> attackAnimationEvent.animationFinished == true || isCasting == false);
        // Debug.Log("공격끝");
        animator.SetBool(hash_attack, false);
        isCasting = false;
    }

    public void OnMove(float movementSpeed)
    {
        animator.SetFloat(hash_movementSpeed, movementSpeed );
    }



    #endregion
}

