using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

using GameUtil;
using UnityEngine.UIElements;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    Transform t;
    // Transform t_tower;
    // Transform t_player;
    
    // [SerializeField] Transform target;

    [SerializeField] Collider targetCollider;    
    public Vector3 targetPos=> targetCollider==null ? t.position + t.forward: targetCollider.transform.position;
    
    NavMeshAgent navAgent;
    
    Enemy enemy;

    public float playerDetectionRange = 10;
    public float attackRange = 1.5f;
    float targetDistSqr;

    float reactionTime = 0.2f;  // 업데이트 갱신시간
    float lastReactionTime;

    bool canReact => Time.time >= lastReactionTime + reactionTime;


    //=================================
    void Update()
    {
        if (GamePlayManager.isGamePlaying == false)
        {
            Stop();
        }
    }


    /// <summary>
    /// 인공지능 업데이트 : 성공시 true
    /// </summary>
    public bool OnUpdate()
    {        
        // 
        if( canReact ==false || enemy.isCasting )
        {
            return false;
        }
        lastReactionTime = Time.time;
        
        // 타겟 업데이트 
        UpdateTarget();     

        // 공격 or 접근
        if( IsTargetInAttackRange( out Vector3 targetPos))
        {
            Attack();
        }
        else
        {
            Approach();
        }
        return true;
    }



    public void Init(Enemy enemy, int waveNum)
    {
        this.enemy = enemy;
        
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.autoBraking = false;
        

        t=transform;
        // t_tower = Tower.Instance.transform;
        // t_player = Player.Instance.transform;


        //
        attackRange = enemy.enemyData.attackRange;
        playerDetectionRange = enemy.enemyData.playerDectectionRange;

        navAgent.speed = enemy.movementSpeed;
        navAgent.stoppingDistance = attackRange *0.85f;
        // navAgent.stoppingDistance = attackRange;
    }

    //=====================================================================
    // 플레이어가 감지 사거리 안인지, - 타겟 업데이트할 때 사용,
    bool IsPlayerInDectectionRange(out Vector3 targetPoint)
    {
        Collider _targetCollider = Player.Instance.playerCollider;
        return IsTargetInRange(_targetCollider, playerDetectionRange, out targetPoint );
    }
    
    
    // 타겟이 공격 사거리 안인지, - 공격 시전할 때 사용, 
    bool IsTargetInAttackRange(out Vector3 targetPoint)
    {
        //
        return IsTargetInRange(targetCollider, attackRange , out targetPoint);
    }

    /// <summary>
    /// 해당 콜라이더가 해당 범위 안 인지 판단 - > 넥서스가 적과 포개지는 상황을 방지하기 위함. 
    /// </summary>
    bool IsTargetInRange(Collider _targetCollider, float dist, out Vector3 targetPoint)
    {
        //
        Vector3 currPos = t.position.WithFloorHeight();
        if(_targetCollider == null)
        {
            targetPoint = currPos;
            return false;
        }
        
        targetPoint = _targetCollider.ClosestPoint(currPos);
        float sqrDist = currPos.GetSqrDistWith(targetPoint);

        return sqrDist <= (dist-0.15f) * (dist-0.15f);
    }

    /// <summary>
    /// 타겟 변경 - 플레이어가 공격 범위 내라면 플레이어를 공격/ 아니면 넥서스 공격
    /// </summary>
    void UpdateTarget()
    {
        Vector3 targetPosition;
        if (IsPlayerInDectectionRange( out targetPosition))
        {
            targetCollider = Player.Instance.playerCollider;
        }
        else
        {
            targetCollider = Tower.Instance.towerCollider;
            targetPosition = targetCollider.ClosestPoint(t.position);
        }

        navAgent.SetDestination( targetPosition );    // 타워 쫓음
    }


    // void OnPlayerInRange()
    // {
    //     target = t_player;
    //     navAgent.SetDestination( target.position );   // 플레이어 쫓음
    // }

    // void OnPlayerOutOfRange()
    // {
    //     target = t_tower;
    //     navAgent.SetDestination( target.position );    // 타워 쫓음
    // }

    void Attack( )
    {
        
        if( enemy.attackAvailable )
        {
            enemy.StartAttack( targetPos.WithPlayerWaistHeight() );
        }
        
        Stop();
         
    }


    void Approach()
    {
        navAgent.isStopped = false;
        navAgent.velocity = navAgent.desiredVelocity;
        enemy.OnMove(99);
    }


    //================================
    
    public void Stop()
    {
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;

        enemy.OnMove(0);
    }

    public void OnDie()
    {
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;


    }

    //==================================
}
