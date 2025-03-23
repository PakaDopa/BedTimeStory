using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

public class EP_02_Tank  : EnemyProjectile
{
    
    public float radius = 2.5f;


    protected override void Init_Custom()
    {
        Attack();
    }


    void Attack()
    {
        Collider[] hits = Physics.OverlapSphere( myTransform.position, radius, GameConstants.playerLayer | GameConstants.towerLayer);
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
                // Debug.Log( dmg);
                tower.GetDamaged(dmg);
            }
        }
    }


    protected override IEnumerator DestroyCondition()
    {
        yield return new WaitUntil( ()=> initEffect== null || initEffect.IsAlive() == false );
    }
}
