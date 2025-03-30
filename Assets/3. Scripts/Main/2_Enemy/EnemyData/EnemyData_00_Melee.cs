using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;
using Unity.VisualScripting;


[CreateAssetMenu(fileName = "EnemyInitData_Melee", menuName = "SO/EnemyData/00_Melee", order = int.MaxValue)]
public class EnemyData_00_Melee : EnemyDataSO
{
    public override EnemyType type => EnemyType.Melee;
    

    public EnemyData_00_Melee()
    {
        maxHp = 90;
    
        movementSpeed = 500;
        attackSpeed = 2;    
        attackRange = 2f;
        playerDectectionRange = 30;

        dmg = 5;


        inc_maxHp = 10;
        inc_movementSpeed = 0.3f;
        inc_dmg = 3;


        castDelay = 0.4f;
        offsetWeight = 0.5f;
    }
}
