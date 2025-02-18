using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_03_Speed  : EnemyProjectile
{
    protected override void Init_Custom()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator DestroyCondition()
    {
        yield return new WaitUntil( ()=> initEffect== null ||  initEffect.IsAlive() == false );
    }
}
