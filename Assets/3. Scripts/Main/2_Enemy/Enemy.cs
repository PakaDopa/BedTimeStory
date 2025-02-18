using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

using GameUtil;
using DG.Tweening;
using UnityEditor.Playables;


[RequireComponent(typeof(EnemyAI), typeof(Collider), typeof(Rigidbody) )]
public abstract class Enemy : MonoBehaviour, IPoolObject
{
    [SerializeField] Animator animator;

    public Transform t;
    public EnemyAI enemyAI;
    public EnemyDataSO enemyData;

    //
    [SerializeField] Transform t_damageEffectPos;
    protected CapsuleCollider _collider;

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

    public bool canRotate;
    public bool isCasting;

    public float angleWithTarget;

    WaitForFixedUpdate wffu = new();
    Vector3 currTargetPosOffset;
    // Slider_EnemyHp enemyState;
    //===================================
    public abstract IEnumerator CastRoutine(Vector3 targetPos);
    
    public abstract void Attack(Vector3 targetPos);


    public abstract AreaIndicator GetAttackAreaIndicator( Vector3 targetPos);


    //========================================================================================
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
        bool aiUpdated = enemyAI.OnUpdate();

        RotateToTarget();
    }    

    public void OnCreatedInPool()
    {
        t = transform;
        enemyAI = GetComponent<EnemyAI>();
        _collider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        animator= GetComponentInChildren<Animator>();


        attackAnimationEvent = GetComponentInChildren<EnemyAnimationEvent_Attack>();
    }

    public void OnGettingFromPool()
    {
        // animator.applyRootMotion = false; 
        // animator.transform.localPosition = Vector3.zero;
        // animator.transform.rotation = Quaternion.identity;



        isCasting = false;
        canRotate = true;
    }



    public void Init(EnemyDataSO enemyData,int clearedwaveCount, Vector3 initPos)
    {
        transform.position = initPos;
        this.enemyData = enemyData;

        InitStatus(clearedwaveCount);
    
        enemyAI.Init( this, clearedwaveCount);

        _collider.enabled = true;


        canRotate = true;
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
        // animator.applyRootMotion = true;
        animator.SetTrigger(hash_die);

        GameManager.Instance.currGamePlayInfo.killCount ++;
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

    static int hash_isCasting = Animator.StringToHash("isCasting");
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
        isCasting = true;

        currTargetPosOffset = new Vector3( UnityEngine.Random.Range(-1,1), 0, UnityEngine.Random.Range(-1,1) ).normalized * enemyData.offsetWeight;
        Vector3 fixedTargetPos = targetPos+ currTargetPosOffset;
        // Debug.Log($" a0  {currTargetPosOffset} / {fixedTargetPos}");
        yield return WaitUntilLookAtTarget();
        canRotate = false;


        GetAttackAreaIndicator(fixedTargetPos);
        // Debug.Log("공격시작");
        
        animator.SetTrigger(hash_attack);
        animator.SetBool(hash_isCasting, true);
        attackAnimationEvent.OnStart();

        // Debug.Log(" a1");
        yield return CastRoutine(fixedTargetPos); 
        if(isAlive == false)
        {
            yield break;
        }

        // Debug.Log(" a2");
        Attack( fixedTargetPos);
        lastAttackTime = Time.time;
        

        yield return new WaitUntil(()=> attackAnimationEvent.animationFinished == true || isCasting == false);
        // Debug.Log("공격끝");
        animator.SetBool(hash_isCasting, false);
        isCasting = false;
        canRotate = true;
    }


    void RotateToTarget()
    {
        if ( canRotate == false)
        {
            return;
        }


        // 1) 현재 바라보는 방향
        float rotationSpeed = 180f;
        Vector3 currentForward = t.forward;
        Vector3 targetDirection = (enemyAI.targetPos + currTargetPosOffset-t.position).normalized;
        // 2) SignedAngle로 현재 방향과 목표 방향의 '사이 각도' (단위: 도) 구하기
        float angleToTarget = Vector3.SignedAngle(currentForward, targetDirection, Vector3.up);

        // 양수면 왼쪽, 음수면 오른쪽으로 회전해야 함
        // (Unity 좌표계에서, 위쪽(Y)축 기준)

        // 3) 이번 프레임에 회전할 수 있는 최대 각도
        float maxRotateThisFrame = rotationSpeed * Time.deltaTime;

        // 4) '남은 각도'의 절댓값이 이번에 회전할 각도보다 작거나 같으면,
        //    그 각도만큼만 한 번에 회전해 최종 방향을 맞춤
        if (Mathf.Abs(angleToTarget) <= maxRotateThisFrame)
        {
            // 목표 방향으로 정확히 맞춤
            t.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        }
        else
        {
            // 그렇지 않다면, maxRotateThisFrame(도)을 부호에 맞춰 회전
            // 부호(+/-)는 angleToTarget에 따름
            float rotateStep = maxRotateThisFrame * Mathf.Sign(angleToTarget);

            // Y축 기준 회전만 적용하는 예시 (Pitch, Roll은 고정)
            t.Rotate(Vector3.up, rotateStep, Space.World);
        }
    }

    IEnumerator WaitUntilLookAtTarget()
    {
        // float angle = ;
        // angleWithTarget  = angle;
        yield return new  WaitUntil(()=> Vector3.Angle(t.forward, (enemyAI.targetPos + currTargetPosOffset -t.position).WithFloorHeight()) <= 5f  );
    }









    public void OnMove(float movementSpeed)
    {
        animator.SetFloat(hash_movementSpeed, movementSpeed );
    }



    #endregion





    // private void OnDrawGizmos()
    // {
      
    //     // 1) OverlapSphere와 동일한 지점 + 반경 시각화
    //     //    코드에서 OverlapSphere(center= targetPos.WithFloorHeight(), radius=attackRange)
    //     Vector3 centerPos = t.position.WithFloorHeight();
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(centerPos, enemyData.attackRange);

    //     // 2) 부채꼴(각도) 표시
    //     //    - 중심점: enemy.t.position
    //     //    - 기준 방향: enemy.t.forward
    //     //    - 각도: attackAngle
    //     //    - 실제로는 공격 각도의 절반(`attackAngle * 0.5f`)을 양옆으로 긋게 됨

    //     //   (a) 원점
    //     Vector3 enemyPos = t.position;

    //     //   (b) 가장 정면선
    //     Vector3 forwardDir = t.forward.normalized * enemyData.attackRange;
        
    //     //   (c) 부채꼴 양옆 방향
    //     float halfAngle = ((EnemyData_00_Melee)enemyData).attackAngle * 0.5f;
    //     Quaternion leftRot = Quaternion.Euler(0f, -halfAngle, 0f);
    //     Quaternion rightRot = Quaternion.Euler(0f, halfAngle, 0f);

    //     Vector3 leftDir = leftRot * forwardDir;
    //     Vector3 rightDir = rightRot * forwardDir;
        
    //     Gizmos.color = Color.yellow;
    //     // 정면
    //     Gizmos.DrawLine(enemyPos, enemyPos + forwardDir);
    //     // 왼쪽
    //     Gizmos.DrawLine(enemyPos, enemyPos + leftDir);
    //     // 오른쪽
    //     Gizmos.DrawLine(enemyPos, enemyPos + rightDir);
    // }
}

