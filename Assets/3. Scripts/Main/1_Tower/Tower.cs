using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;

public class Tower : DestroyableSingleton<Tower>
{
    public Transform t;
    
    public bool initialized {get;private set; }
    [SerializeField] private SoundEventSO soundSO;

    public Collider towerCollider;  //거리를 정확하게 측정하기 위함. 


    public float hp;
    public float maxHp = 5000;


    public UnityEvent<float,float> onHpChanged = new();


    public void Init()
    {
        t = transform;
        
        hp = maxHp;
        towerCollider = GetComponent<Collider>();

        initialized = true;
    }


    public void GetDamaged(float dmg)
    {
        hp -= dmg;

        if (hp<= 0)
        {
            DestroyTower();
        }

        onHpChanged.Invoke(hp, maxHp);
        SoundManager.Instance.OnTowerDamaged(t.position);
    }   


    public void DestroyTower()
    {
        // Destroy(gameObject);
        //soundSO.Raise();
        GamePlayManager.Instance.GameOver();
        Debug.LogError("패배!!!!!!!!!!");
    }   

}
