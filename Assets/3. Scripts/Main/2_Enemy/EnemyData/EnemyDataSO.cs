using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

public abstract class EnemyDataSO : ScriptableObject
{
    public float maxHp = 100;
    
    public float movementSpeed = 4;
    public float attackSpeed = 2;    
    public float attackRange = 1.5f;
    public float playerDectectionRange = 10;

    public float dmg = 10;


    public float inc_maxHp = 10;
    public float inc_movementSpeed = 0.3f;
    public float inc_dmg = 3;


    public abstract void Attack(Enemy enemy, Vector3 targetPos);
}