using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

using GameUtil;
using UnityEditor.Rendering;


public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float yOffset = 1f;
    
    [SerializeField] float lifeTime = 10f;

    [SerializeField] float speed = 8f;

    [SerializeField] float dmg;



    public void Init(float dmg, Vector3 currPos, Vector3 targetPos)
    {
        transform.position = currPos + new Vector3(0, yOffset, 0);
        
        
        Rigidbody rb = GetComponent<Rigidbody>();
        
        
        Vector3 dir = (targetPos.WithPlayerWaistHeight() - transform.position ).normalized;
        transform.LookAt(targetPos, Vector3.up);    
        
        rb.velocity = dir * speed;

        
        this.dmg = dmg;

        Destroy(gameObject ,lifeTime );
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(dmg);
            Destroy(gameObject);
        }

        else if (other.CompareTag("Tower"))
        {
            Tower.Instance.GetDamaged(dmg);
            Destroy(gameObject);
        }
    }




    


}
