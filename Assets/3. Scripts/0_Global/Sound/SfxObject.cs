using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SfxObject : MonoBehaviour, IPoolObject
{
    Transform myTransform;
    [SerializeField] AudioSource audioSource;
   
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
            return;
        }
        
        // 세팅
        myTransform.position = initPos;
        // audioSource.priority = defaultPriority + soundData.rank;

        //After Setting
        audioSource.PlayOneShot(soundData.clip);
        StartCoroutine( DelayedDestroy( soundData.clip.length+ 0.1f)); 
    }

    IEnumerator DelayedDestroy(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        SoundManager.Instance.Return(this);
    }
}
