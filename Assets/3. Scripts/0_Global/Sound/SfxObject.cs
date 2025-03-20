using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SfxObject : MonoBehaviour, IPoolObject
{
    Transform myTransform;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float lifeTime;
    [SerializeField] AudioClip clip;
   
    public void OnCreatedInPool()
    {
        audioSource = GetComponent<AudioSource>();
        myTransform = transform;
    }

    public void OnGettingFromPool()
    {
        
    }

    public void Play(SoundEventSO  soundData, Vector3 initPos)
    {
        if(soundData==null)
        {
            SoundManager.Instance.Return(this);
            return;
        }
        
        // 세팅
        myTransform.position = initPos;
        // audioSource.priority = defaultPriority + soundData.rank;

        //After Setting
        audioSource.PlayOneShot(soundData.clip);
        float lifeTime = soundData.clip.length+ 0.1f;
        
        StartCoroutine( DelayedDestroy( lifeTime ));

        this.lifeTime = lifeTime;
        this.clip = soundData.clip;

    }

    IEnumerator DelayedDestroy(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        SoundManager.Instance.Return(this);
    }
}
