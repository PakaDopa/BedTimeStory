using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

[CreateAssetMenu(fileName = "EnemyInitData_Range", menuName = "SO/EnemyData/01_Range", order = int.MaxValue)]
public class EnemyData_01_Range : EnemyDataSO
{
    public override EnemyType type => EnemyType.Range;
    
    public EnemyData_01_Range()
    {
        maxHp = 60;
    
        movementSpeed = 4;
        attackSpeed = 4;    
        attackRange = 10f;
        playerDectectionRange = 20;

        dmg = 15;


        inc_maxHp = 10;
        inc_movementSpeed = 0.3f;
        inc_dmg = 3;


        castDelay = 1.5f;
        offsetWeight = 0.5f;
    }
}