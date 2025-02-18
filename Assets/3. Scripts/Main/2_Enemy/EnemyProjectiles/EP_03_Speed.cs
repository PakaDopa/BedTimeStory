using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP_03_Speed  : EnemyProjectile
{
    public float width =2;
    [SerializeField] SphereCollider _collider;


    //=============================
    protected override void Init_Custom()
    {
        _collider = GetComponentInChildren<SphereCollider>();
        _collider.radius = width*0.5f;

        myTransform.position += new Vector3(0,width*0.5f,0);
        myTransform.SetParent(enemy.t);
    }

    protected override IEnumerator DestroyCondition()
    {
        yield return new WaitForSeconds( 1.5f);
    }
    //=================================


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player) ) 
        {
            PlayerStats.Instance.TakeDamage(dmg);
        }
        else if ( other.CompareTag("Tower") && other.TryGetComponent(out Tower tower))
        {
            tower.GetDamaged(dmg);
        }
    }



}
