using DG.Tweening.Core.Easing;
using Manager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] SoundEventSO[] soundSO;
    public GameObject hitPrefab;
    public List<GameObject> trails;
    private void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(2);

        Explode();
    }

    protected void Explode()
    {
        var hitVFX = Instantiate(hitPrefab, transform.position, Quaternion.identity) as GameObject;
        hitVFX.transform.position = transform.position;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Damage Enemy
        if( other.TryGetComponent(out Enemy e))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 damageEffectPos = new Vector3(hitPoint.x, e.damageEffectPos , hitPoint.z);
            float damage = PlayerStats.Instance.AttackPower;

            e.GetDamaged(damage);
            EffectPoolManager.Instance.GetNormalDamageText(damageEffectPos, damage);


            EventManager.Instance.PostNotification(MEventType.EnemyHitted, this, new TransformEventArgs(transform, true));
            soundSO[Random.Range(0, soundSO.Length)].Raise();
            Explode();
        }
        //StartCoroutine(DestroyParticle(0f));
    }


    public IEnumerator DestroyParticle(float waitTime)
    {

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
