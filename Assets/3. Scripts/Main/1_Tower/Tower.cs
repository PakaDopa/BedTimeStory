using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Tower : DestroyableSingleton<Tower>
{
    [SerializeField] private SoundEventSO soundSO;

    public Collider towerCollider;  //거리를 정확하게 측정하기 위함. 


    public float hp;
    public float maxHp = 500;


    public void Init()
    {
        hp = maxHp;
        towerCollider = GetComponent<Collider>();
        
    }


    public void GetDamaged(float dmg)
    {
        hp -= dmg;

        GameEventManager.Instance.onChange_towerHp.Invoke();

        if (hp<= 0)
        {
            DestroyTower();
        }



    }


    public void DestroyTower()
    {
        // Destroy(gameObject);
        //soundSO.Raise();
        GamePlayManager.Instance.GameOver();
        Debug.LogError("패배!!!!!!!!!!");
    }   

}
