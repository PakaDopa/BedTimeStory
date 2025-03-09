using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketProjectile : Projectile
{
    [SerializeField] float explosionRadius = 6f;

    // bool exploded;

    // private void Start()
    // {
    //     StartCoroutine(DestroyCoroutine());
    // }

    // IEnumerator DestroyCoroutine()
    // {
    //     yield return new WaitForSeconds(2);

    //     Explode(transform.position);
    // }

    private void OnTriggerEnter(Collider co)
    {
        // Damage Enemy
        if(co.CompareTag("Enemy"))
        {
            // CreateSphere(explosionRadius, transform.position);
            DamageEnemy(co);
            
            Explode(transform.position);
            StartCoroutine(DestroyParticle(0f));
        }
        

    }


    protected void DamageEnemy(Collider co)
    {
        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        float damage = PlayerStats.Instance.AttackPower * 2;
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy))
            {
                Vector3 hitPoint = collider.ClosestPoint(transform.position);
                Vector3 damageEffectPos = new Vector3(hitPoint.x, enemy.damageEffectPos , hitPoint.z);
                
                enemy.GetDamaged(damage);
                EffectPoolManager.Instance.GetNormalDamageText(damageEffectPos, damage);

            }
        }
    }



    /// <summary>
    /// 범위 확인용 .
    /// </summary>
    /// <param name="r"></param>
    /// <param name="initPos"></param>
    void CreateSphere(float r, Vector3 initPos)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // 기본 구체 생성
        sphere.transform.position = initPos; // 위치 설정
        sphere.transform.localScale = Vector3.one * (r * 2); // 유니티 기본 구체는 지름 1이므로 2배
        sphere.GetComponent<Renderer>().material.color = Color.red.WithAlpha(0.5f); // 색상 변경
    }
    
}
