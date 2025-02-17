using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

[CreateAssetMenu(fileName = "EnemyInitData_Speed", menuName = "SO/EnemyData/03_Speed", order = int.MaxValue)]
public class EnemyData_03_Speed : EnemyDataSO
{




    public override EnemyType type => EnemyType.Speed;

    public float attackAreaRadius = 2.5f;

    public EnemyData_03_Speed()
    {
        maxHp = 80;
    
        movementSpeed = 3;
        attackSpeed = 3;    
        attackRange = 8f;
        playerDectectionRange = 12;

        dmg = 30;


        inc_maxHp = 10;
        inc_movementSpeed = 0.3f;
        inc_dmg = 3;


        castDelay = 1.5f;
        offsetWeight = 1.5f;
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
    public override AreaIndicator GetAttackAreaIndicator(Enemy enemy, Vector3 targetPos)
    {
        
        return AreaIndicatorGenerator.Instance.GetCone(enemy, enemy.t.position, targetPos - enemy.t.position, castDelay, attackRange, 1 );
    }

    public override IEnumerator CastRoutine(Enemy enemy, Vector3 targetPos)
    {
        yield return new WaitForSeconds(castDelay);
    }
}
