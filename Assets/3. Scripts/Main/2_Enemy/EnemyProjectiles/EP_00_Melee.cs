using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

public class EP_00_Melee : EnemyProjectile
{
    public float attackAngle = 120f;


    //================================================

    protected override void Init_Custom()
    { 
        Attack();
    }


    void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(myTransform.position.WithFloorHeight(), enemy.enemyData.attackRange, GameConstants.playerLayer | GameConstants.towerLayer);
        // Debug.Log(hits.Length);
        // 충돌된 오브젝트들에 대해 반복 실행
        for(int i=0;i<hits.Length;i++)
        {
            Collider hit = hits[i];
            

            Vector3 toTarget = (hit.transform.position- myTransform.position).WithFloorHeight().normalized; // 타겟까지의 방향 벡터
          
            float dot = Vector3.Dot(enemy.t.forward, toTarget); // 기준 방향과 타겟 방향의 내적
            float targetAngle = Mathf.Acos(dot) * Mathf.Rad2Deg; // 내적을 각도로 변환

            if (targetAngle <= attackAngle  *0.5f)  // 부채꼴 범위 내에 있는지 확인
            {
                // 적에게 피해를 입히는 로직

                Player player = hit.GetComponent<Player>();
                if (player != null)
                {
                    PlayerStats.Instance.TakeDamage(dmg);
                    continue;   
                }
                
                Tower tower = hit.GetComponent<Tower>();
                if (tower !=null)
                {
                    // Debug.Log( dmg);
                    tower.GetDamaged(dmg);
                }
            }
        }
    } 



    protected override IEnumerator DestroyCondition()
    {
        yield return new WaitUntil( ()=> initEffect== null ||  initEffect.IsAlive() == false );
    }
  
}
