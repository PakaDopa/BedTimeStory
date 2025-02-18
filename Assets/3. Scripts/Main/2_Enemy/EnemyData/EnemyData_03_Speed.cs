using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

[CreateAssetMenu(fileName = "EnemyInitData_Speed", menuName = "SO/EnemyData/03_Speed", order = int.MaxValue)]
public class EnemyData_03_Speed : EnemyDataSO
{




    public override EnemyType type => EnemyType.Speed;



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
    
    









}
