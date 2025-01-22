using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : Projectile
{
    private void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(2);

        Explode();
    }

    private void OnTriggerEnter(Collider co)
    {
        // Damage Enemy
        if(co.CompareTag("Enemy"))
        {
            DamageEnemy(co);

            Explode();
            StartCoroutine(DestroyParticle(0f));
        }
        
    }

    protected void DamageEnemy(Collider co)
    {
        var colliders = Physics.OverlapSphere(transform.position, 2f);

        float dmg = PlayerStats.Instance.AttackPower * 2;
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy))
            {
                enemy.GetDamaged(dmg);
            }
        }
    }
}
