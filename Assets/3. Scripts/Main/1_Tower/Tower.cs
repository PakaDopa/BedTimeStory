using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;

public class Tower : DestroyableSingleton<Tower>
{
    public bool initialized {get;private set; }
    [SerializeField] private SoundEventSO soundSO;

    public Collider towerCollider;  //거리를 정확하게 측정하기 위함. 


    public float hp;
    public float maxHp = 500;


    public UnityEvent<float,float> onHpChanged = new();


    public void Init()
    {
        hp = maxHp;
        towerCollider = GetComponent<Collider>();

        initialized = true;
    }


    public void GetDamaged(float dmg)
    {
        hp -= dmg;

        GameEventManager.Instance.onChange_towerHp.Invoke();

        if (hp<= 0)
        {
            DestroyTower();
        }

        onHpChanged.Invoke(hp, maxHp);
    }   


    public void DestroyTower()
    {
        // Destroy(gameObject);
        //soundSO.Raise();
        GamePlayManager.Instance.GameOver();
        Debug.LogError("패배!!!!!!!!!!");
    }   

}
