using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

[CreateAssetMenu(fileName = "EnemyInitData_Tank", menuName = "SO/EnemyData/02_Tank", order = int.MaxValue)]
public class EnemyData_02_Tank : EnemyDataSO
{
    public override EnemyType type => EnemyType.Tank;

    [Header("Melee Setting")]
    public float attackAreaRadius = 2.5f;

    public EnemyData_02_Tank()
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
    
    



    //===============================================================================

    public override void Attack(Enemy enemy, Vector3 targetPos)
    {

        Collider[] hits = Physics.OverlapSphere(targetPos.WithFloorHeight(), attackAreaRadius, GameConstants.playerLayer | GameConstants.towerLayer);
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
        return AreaIndicatorGenerator.Instance.GetCircle(enemy, targetPos, castDelay, attackAreaRadius);
    }


    // 얘는 지정시간동안, targetPos로 점프해서 착지할거임. 
    public override IEnumerator CastRoutine(Enemy enemy, Vector3 targetPos)
    {
        WaitForFixedUpdate wffu = new();
        
        Vector3 startPos = enemy.t.position;
        Vector3 endPos = targetPos.WithFloorHeight();

        float elapsed = 0;
        while (elapsed < castDelay)
        {
            // code here
            float t = elapsed / castDelay;


            // Y축을 고정하고 싶다면, 예: startPos.y를 유지하도록 처리
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            
            // 실제 이동 적용
            enemy.t.position = newPos;

            elapsed += Time.fixedDeltaTime;
            yield return wffu;  
        }

        enemy.t.position = endPos;
    }
}
