using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour, IPoolObject
{
    public abstract EffectEvent effectEvent {get;}  // 발생하는 상황
    public float lifeTime;
    public bool permanent {get;private set;}
    float spawnTime;
    
    Coroutine playingDestroyRoutine;


    protected void Init(Vector3 initPos,float lifeTime)
    {
        transform.position = initPos;
        this.lifeTime = lifeTime;
        
        permanent = lifeTime <=0;
        spawnTime = Time.time;

        // 영구적인 이펙트가 아니라면 일정 시간 후 파괴. 
        if( permanent == false )
        {
            if( playingDestroyRoutine != null )
            {
                StopCoroutine(playingDestroyRoutine);
            }

            playingDestroyRoutine = StartCoroutine( DestroyRoutine());
        }
    }


    //==================================================
    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        
        if (gameObject.activeSelf)
            EffectPoolManager.Instance.ReturnEffect(this);
    }
    
    public void OnCreatedInPool()
    {
        
    }

    public void OnGettingFromPool()
    {
        
    }

}
