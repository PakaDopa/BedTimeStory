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
    public float maxHp = 10000;


    public UnityEvent<float,float,float> onHpChanged = new(); // p0: from, p1 : to  p2: max


    public void Init()
    {
        t = transform;
        
        hp = maxHp;
        towerCollider = GetComponent<Collider>();

        initialized = true;
    }


    public void GetDamaged(float dmg)
    {
        if( GamePlayManager.gameActiavated==false)
        {
            return;
        }
        float newValue = Mathf.Clamp(hp - dmg, 0, maxHp);
        onHpChanged.Invoke(hp, newValue, maxHp);
        hp = newValue;

        //
        if (hp<= 0)
        {
            DestroyTower();
        }
        SoundManager.Instance.OnTowerDamaged(t.position);
    }   


    public void DestroyTower()
    {
        GamePlayManager.Instance.GameOver();
        Debug.LogError("패배!!!!!!!!!!");
    }   

}
