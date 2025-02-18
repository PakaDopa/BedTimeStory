using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

using GameUtil;
using UnityEditor.Rendering;


public abstract class EnemyProjectile : MonoBehaviour,IPoolObject
{
    public bool activated;
    
    protected Enemy enemy;
    protected Transform myTransform;

    public float dmg;


    public Vector3 initPos;
    public Vector3 targetPos;
    public Vector3 dir;


    [SerializeField] protected  ParticleSystem initEffect;
    //=======================================================================


    public void OnCreatedInPool()
    {
        
    }

    public void OnGettingFromPool()
    {
       activated = false;
    }



    public void Init(Enemy enemy, Vector3 initPos,Vector3 targetPos)
    {
        
        activated = true;
        
        this.enemy = enemy;
        this.initPos = initPos;
        this.targetPos = targetPos;
        
        this.dir = (targetPos - initPos).WithFloorHeight().normalized;;
        
        myTransform = transform;
        myTransform.position = initPos;
        if( dir != Vector3.zero)
        {
            myTransform.rotation = Quaternion.LookRotation(dir);
        }
        

        this.dmg = enemy.dmg;        


        Init_Custom();
    
        StartCoroutine(DestroyRoutine());
    }

    protected abstract void Init_Custom();

    IEnumerator DestroyRoutine()
    {
        yield return DestroyCondition();
        activated = false;
        Destroy(gameObject);
    }

    protected abstract IEnumerator DestroyCondition();
    
}
