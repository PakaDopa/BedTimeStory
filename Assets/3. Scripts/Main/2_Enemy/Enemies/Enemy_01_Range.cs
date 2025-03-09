using System.Collections;
using System.Collections.Generic;
using GameUtil;
using UnityEngine;

public class Enemy_01_Range : Enemy
{
    [SerializeField] Transform t_muzzle;
    
    
    public override void Attack(Vector3 targetPos)
    {
        Vector3 initPos =  t_muzzle.position;
        
        EP_01_Range ep = Instantiate(enemyData.prefab_enemyProjectile).GetComponent<EP_01_Range>();
        ep.Init( this, initPos, targetPos);

        AreaIndicatorGenerator.Instance.GetCircle(this, targetPos, ep.lifeTime, ep.explosionRadius);
    }   

    public override AreaIndicator GetAttackAreaIndicator(Vector3 targetPos)
    {
        // float explosionRadius = ((EP_01_Range)enemyData.prefab_enemyProjectile).explosionRadius;
        // return AreaIndicatorGenerator.Instance.GetCircle(this, targetPos, enemyData.castDelay, explosionRadius);
        return null;
    }

    public override IEnumerator CastRoutine( Vector3 targetPos)
    {
        yield return new WaitForSeconds(enemyData.castDelay);
    }
}
