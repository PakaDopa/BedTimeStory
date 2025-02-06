using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;


[CreateAssetMenu(fileName = "EnemyInitData_Melee", menuName = "SO/EnemyData/00_Melee", order = int.MaxValue)]
public class EnemyData_00_Melee : EnemyDataSO
{
    public override EnemyType type => EnemyType.Melee;
    public EnemyData_00_Melee()
    {
           maxHp = 100;
    
        movementSpeed = 4;
        attackSpeed = 2;    
        attackRange = 1.5f;
        playerDectectionRange = 10;

        dmg = 10;


        inc_maxHp = 10;
        inc_movementSpeed = 0.3f;
        inc_dmg = 3;
    }
    
    
    public override void Attack(Enemy enemy, Vector3 targetPos)
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
}
