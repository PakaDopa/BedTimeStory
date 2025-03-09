using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;
using Unity.VisualScripting;

public class Enemy_03_Speed : Enemy
{
    static WaitForFixedUpdate wffu = new();

    public override void Attack( Vector3 targetPos)
    {
        Vector3 initPos =  t.position;

        var ep = Instantiate(enemyData.prefab_enemyProjectile).GetComponent<EP_03_Speed>();
        ep.Init(  this, initPos, targetPos );



        StartCoroutine(DiveRoll(targetPos));
    }

    public override AreaIndicator GetAttackAreaIndicator(Vector3 targetPos)
    {
        float width = ((EP_03_Speed)enemyData.prefab_enemyProjectile).width;
        return AreaIndicatorGenerator.Instance.GetSquare(this, t.position, targetPos - t.position, enemyData.castDelay, width, enemyData.attackRange );
    }

    public override IEnumerator CastRoutine( Vector3 targetPos)
    {
        yield return new WaitForSeconds(enemyData.castDelay);
    }


    IEnumerator DiveRoll(Vector3 targetPos)
    {
        // float duration = enemyData.attackAnimationDuration - enemyData.castDelay * 0.8f;
        float duration  = 1.5f;     // 직접 이렇게 찍는 게 자연스러움. 
        // Debug.Log(duration);
        Vector3 startPos= t.position;
        Vector3 dir = (targetPos - startPos).WithFloorHeight().normalized;
        Vector3 endPos = startPos + dir * enemyData.attackRange;  

        
        float elapsed= 0;
        while(elapsed< duration)
        {
            if (isAlive== false)
            {
                yield break;
            }
            
            float progress = elapsed / duration;

            Vector3 newPos = Vector3.Lerp(startPos, endPos, progress);
            
            // 실제 이동 적용
            t.position = newPos;

            elapsed+= Time.fixedDeltaTime;
            yield return wffu;
        }
        t.position = endPos;
    }
}

