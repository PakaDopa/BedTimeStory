using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

public class Enemy_03_Speed : Enemy
{


    public override void Attack( Vector3 targetPos)
    {
        // Vector3 dir = (targetPos - enemy.t.position).WithFloorHeight().normalized;
        float radius = 1;

        Collider[] hits = Physics.OverlapSphere(targetPos.WithFloorHeight(), radius, GameConstants.playerLayer | GameConstants.towerLayer);
        // Debug.Log(hits.Length);
        // 충돌된 오브젝트들에 대해 반복 실행
        for(int i=0;i<hits.Length;i++)
        {
            Collider hit = hits[i];

            // 적에게 피해를 입히는 로직
            Player player = hit.GetComponent<Player>();
            if (player != null)
            {
                // Debug.Log("으악");
                PlayerStats.Instance.TakeDamage(dmg);
                continue;   
            }
            
            Tower tower = hit.GetComponent<Tower>();
            if (tower !=null)
            {
                // Debug.Log("타워워어");
                Debug.Log( dmg);
                tower.GetDamaged(dmg);
            }
        }
    }
    public override AreaIndicator GetAttackAreaIndicator(Vector3 targetPos)
    {
        
        return AreaIndicatorGenerator.Instance.GetCone(this, t.position, targetPos - t.position, enemyData.castDelay, enemyData.attackRange, 1 );
    }

    public override IEnumerator CastRoutine( Vector3 targetPos)
    {
        yield return new WaitForSeconds(enemyData.castDelay);
    }
}

