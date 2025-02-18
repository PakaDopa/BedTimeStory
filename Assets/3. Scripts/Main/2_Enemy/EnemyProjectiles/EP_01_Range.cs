using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtil;

using DG.Tweening;

public class EP_01_Range  : EnemyProjectile
{
    public float explosionRadius = 1.5f;
    
    [SerializeField] ParticleSystem attackEffect;
    
    Rigidbody rb;
    Collider _collider;


    public float lifeTime = 5f;
    public float speed = 7f;
    public float maxHeightOffest = 0.8f;

    [SerializeField] ParticleSystem explosionEffect;
    
    Sequence seq_projectileMotion;

    float initTime;
    bool onCollision;
    
    //==================================================

    protected override void Init_Custom()
    {
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        
        Vector3 velocity = myTransform.forward * speed;

        float maxTTT = enemy.enemyData.attackRange / speed;

        onCollision = false;
        initTime = Time.time;
        lifeTime = CalTTT(targetPos, velocity);
        rb.velocity = velocity;


        float ratio = lifeTime/ maxTTT;
        float currHeightOffest = maxHeightOffest * ratio;
        // Debug.Log($" 수명 비율 {ratio} -> {currHeightOffest}");
        
        explosionEffect.gameObject.SetActive(false);
        var main = explosionEffect.main;
        main.startSize =  new ParticleSystem.MinMaxCurve( explosionRadius* 2f);

        seq_projectileMotion = PlaySeq_ProjectileMotion(currHeightOffest);
    }


    protected override IEnumerator DestroyCondition()
    {
        yield return new WaitUntil( ()=>Time.time >=initTime + lifeTime || onCollision);
        Explode();


        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play();
        yield return new WaitUntil( ()=> explosionEffect.IsAlive()==false); 
    }
    //============================================

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Tower"))
        {
            onCollision  = true;
        }
    }


    /// <summary>
    ///  현재 위치에서 해당 위치로의 도달시간.  (Time to target)
    /// </summary>
    float CalTTT(Vector3 targetPos, Vector3 velocity )      
    {
        // XZ 평면에서 현재 위치와 목표 위치의 거리 계산
        Vector3 currentXZ = myTransform.position.WithFloorHeight();
        Vector3 targetXZ = targetPos.WithFloorHeight();
        float distance = Vector3.Distance(currentXZ, targetXZ);

        // XZ 평면에서의 속도 (Y축을 제외한 속도)
        Vector3 velocityXZ = velocity.WithFloorHeight();
        float speed = velocityXZ.magnitude; 

        if (speed == 0) return Mathf.Infinity; // 속도가 0이면 도달 불가능
        return distance / speed; // 시간 = 거리 / 속도
    }



    void Explode()
    {
        rb.velocity = Vector3.zero;
        
        Collider[] hits = Physics.OverlapSphere(myTransform.position, explosionRadius, GameConstants.playerLayer | GameConstants.towerLayer);

        // 충돌된 오브젝트들에 대해 반복 실행
        for(int i=0;i<hits.Length;i++)
        {
            Collider hit = hits[i];
            
            // 적에게 피해를 입히는 로직
            if( hit.TryGetComponent(out Player player))
            {
                PlayerStats.Instance.TakeDamage(dmg);
                continue;  
            }
            if(hit.TryGetComponent(out Tower tower))
            {
                tower.GetDamaged(dmg);
            }
        }


        //
        if( seq_projectileMotion!=null && seq_projectileMotion.IsActive())
        {
            seq_projectileMotion.Kill();
        }
    }

    Sequence PlaySeq_ProjectileMotion(float currHeightOffest)
    {
        float finalYOffset = -0.75f;
        float diff = currHeightOffest - finalYOffset;


        float ascendingTime = lifeTime * currHeightOffest / diff;
        float remainTime = lifeTime - ascendingTime ;
        
        float highestY = myTransform.position.y + currHeightOffest;
        float finalY = myTransform.position.y + finalYOffset ;

        // Debug.Log($"상승시간  {ascendingTime} / {lifeTime}" );
        
        return DOTween.Sequence()
        .Append( myTransform.DOMoveY(highestY, ascendingTime ).SetEase(Ease.OutCirc))
        .Append( myTransform.DOMoveY(finalY, remainTime).SetEase(Ease.InCirc))
        .Play();
    }

}
