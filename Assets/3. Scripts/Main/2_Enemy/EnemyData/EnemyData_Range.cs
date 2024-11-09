using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

[CreateAssetMenu(fileName = "EnemyInitData_Range", menuName = "SO/EnemyData/01_Range", order = int.MaxValue)]
public class EnemyData_Range : EnemyDataSO
{
    public EnemyData_Range()
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
    }
    
    
    [SerializeField] GameObject prefab_enemyProjectile;

    public override void Attack(Enemy enemy, Vector3 targetPos)
    {
        Debug.Log("빵야");
        
        var ep = Instantiate(prefab_enemyProjectile).GetComponent<EnemyProjectile>();
        ep.Init(dmg, enemy.t.position, targetPos);
    }   
}