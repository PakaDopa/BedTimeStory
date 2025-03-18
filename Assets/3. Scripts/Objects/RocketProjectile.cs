using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RocketProjectile : Projectile
{
    [SerializeField] float explosionRadius = 6f;
    [SerializeField] float rocketProjectileSpeed = 25f;
    [SerializeField] float heightOffset = 1f;
    // bool exploded;

    Sequence seq_projectileMotion;


    //================================================================
    public void Init(Vector3 initPos, Vector3 dir)
    {
        t.position = initPos;
        t.rotation =  Quaternion.Euler(dir);

        Vector3 velocity = dir * rocketProjectileSpeed;
        rb.velocity = velocity;

        // Vector3 destination = CalDestination(initPos, velocity, lifeTime);
        seq_projectileMotion = PlaySeq_ProjectileMotion(heightOffset);
    }



    private void OnTriggerEnter(Collider co)
    {
        // Damage Enemy
        if(co.CompareTag("Enemy"))
        {
            // CreateSphere(explosionRadius, transform.position);
            DamageEnemy(co);
            
            Explode(transform.position);
            StartCoroutine(DestroyParticle(0f));
            
            
            //
            if( seq_projectileMotion!=null && seq_projectileMotion.IsActive())
            {
                seq_projectileMotion.Kill();
            }
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


    Vector3 CalDestination(Vector3 initPos, Vector3 velocity, float lifeTime)
    {
        return initPos + velocity * lifeTime;
    }


    Sequence PlaySeq_ProjectileMotion(float currHeightOffset)
    {
        float ascendingTime = lifeTime * 0.5f;
        float remainTime = lifeTime - ascendingTime ;
        
        float highestY = t.position.y + currHeightOffset;
        float finalY = t.position.y;

        // Debug.Log($"상승시간  {ascendingTime} / {lifeTime}" );
        
        return DOTween.Sequence()
        .Append( t.DOMoveY(highestY, ascendingTime ).SetEase(Ease.OutCirc))
        .Append( t.DOMoveY(finalY, remainTime).SetEase(Ease.InCirc))
        .Play();
    }




    /// <summary>
    /// 범위 확인용 .
    /// </summary>
    /// <param name="r"></param>
    /// <param name="initPos"></param>
    // void CreateSphere(float r, Vector3 initPos)
    // {
    //     GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // 기본 구체 생성
    //     sphere.transform.position = initPos; // 위치 설정
    //     sphere.transform.localScale = Vector3.one * (r * 2); // 유니티 기본 구체는 지름 1이므로 2배
    //     sphere.GetComponent<Renderer>().material.color = Color.red.WithAlpha(0.5f); // 색상 변경
    // }




    
    
}
