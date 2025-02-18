using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;


public class Enemy_02_Tank: Enemy
{
    static WaitForFixedUpdate wffu = new();
     
    public override void Attack(Vector3 targetPos)
    {
        Vector3 initPos =  targetPos;
        
        var ep = Instantiate(enemyData.prefab_enemyProjectile).GetComponent<EnemyProjectile>();
        ep.Init(  this, initPos, targetPos );
    }   

    public override AreaIndicator GetAttackAreaIndicator(Vector3 targetPos)
    {
        float attackAreaRadius =  ((EP_02_Tank)enemyData.prefab_enemyProjectile).radius;
        
        return AreaIndicatorGenerator.Instance.GetCircle(this, targetPos, enemyData.castDelay, attackAreaRadius);
    }

    public override IEnumerator CastRoutine( Vector3 targetPos)
    { 
        Vector3 startPos = t.position;
        Vector3 endPos = targetPos.WithFloorHeight();

        float elapsed = 0;
        while (elapsed < enemyData.castDelay)
        {
            if (isAlive== false)
            {
                yield break;
            }
            
            
            float progress = elapsed / enemyData.castDelay;


            // Y축을 고정하고 싶다면, 예: startPos.y를 유지하도록 처리
            Vector3 newPos = Vector3.Lerp(startPos, endPos, progress);
            
            // 실제 이동 적용
            t.position = newPos;

            elapsed += Time.fixedDeltaTime;
            yield return wffu;  
        }

        t.position = endPos;
    }






    

}

