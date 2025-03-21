using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;
using System;


public class Enemy_00_Melee : Enemy
{

    public override void Attack(Vector3 initPos, Vector3 targetPos)
    {     
        EnemyProjectile ep = Instantiate(enemyData.prefab_enemyProjectile.gameObject).GetComponent<EnemyProjectile>(); 
        ep.Init(this, initPos, targetPos);
    }

    public override AreaIndicator GetAttackAreaIndicator( Vector3 initPos, Vector3 targetPos)
    {
        float attackAngle =  ((EP_00_Melee)enemyData.prefab_enemyProjectile).attackAngle;
        return AreaIndicatorGenerator.Instance.GetCone(this, t.position, targetPos - t.position, enemyData.castDelay, enemyData.attackRange, attackAngle );
    }

    public override IEnumerator CastRoutine(Vector3 initPos, Vector3 targetPos)
    {
        yield return new WaitForSeconds(enemyData.castDelay);
    }
}
