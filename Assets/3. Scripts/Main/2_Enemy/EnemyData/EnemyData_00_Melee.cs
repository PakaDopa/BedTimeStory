using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;
using Unity.VisualScripting;


[CreateAssetMenu(fileName = "EnemyInitData_Melee", menuName = "SO/EnemyData/00_Melee", order = int.MaxValue)]
public class EnemyData_00_Melee : EnemyDataSO
{
    public override EnemyType type => EnemyType.Melee;
    
    [Header("Melee Setting")]
    public float attackAngle = 20f;



    public EnemyData_00_Melee()
    {
           maxHp = 100;
    
        movementSpeed = 4;
        attackSpeed = 2;    
        attackRange = 2f;
        playerDectectionRange = 10;

        dmg = 10;


        inc_maxHp = 10;
        inc_movementSpeed = 0.3f;
        inc_dmg = 3;


        castDelay = 1f;
        offsetWeight = 0.5f;
    }
    
    
    public override void Attack(Enemy enemy, Vector3 targetPos)
    {
        // Vector3 dir = (targetPos - enemy.t.position).WithFloorHeight().normalized;
        Collider[] hits = Physics.OverlapSphere(enemy.t.position.WithFloorHeight(), attackRange, GameConstants.playerLayer | GameConstants.towerLayer);
        Debug.Log(hits.Length);
        // 충돌된 오브젝트들에 대해 반복 실행
        for(int i=0;i<hits.Length;i++)
        {
            
            Collider hit = hits[i];
            
            // Stage.Instance.testPos = hit.transform.position; 

            Vector3 toTarget = (hit.transform.position- enemy.t.position).WithFloorHeight().normalized; // 타겟까지의 방향 벡터
          
            float dot = Vector3.Dot(enemy.t.forward, toTarget); // 기준 방향과 타겟 방향의 내적
            float targetAngle = Mathf.Acos(dot) * Mathf.Rad2Deg; // 내적을 각도로 변환

            // Debug.Log(targetAngle);
            if (targetAngle <= attackAngle *0.5f)  // 부채꼴 범위 내에 있는지 확인
            {
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
                    Debug.Log( dmg);
                    tower.GetDamaged(dmg);
                }
            }
        }
    }

    public override AreaIndicator GetAttackAreaIndicator(Enemy enemy, Vector3 targetPos)
    {
        
        return AreaIndicatorGenerator.Instance.GetCone(enemy, enemy.t.position, targetPos - enemy.t.position, castDelay, attackRange, attackAngle );
    }

    public override IEnumerator CastRoutine(Enemy enemy, Vector3 targetPos)
    {
        Debug.Log($"욥 {Time.time}");
        yield return new WaitForSeconds(castDelay);
        Debug.Log($"얍 {Time.time}");
    }
}
