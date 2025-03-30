using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : DestroyableSingleton<Player>
{

    public PlayerController playerController;
    public AimStatManager aimStat;
    public Collider playerCollider;
    public PlayerStats stats;


    public Transform T;

    public bool initialized {get; private set;}


    public void Init()
    {
        playerController  = GetComponent<PlayerController>();
        
        
        playerCollider = GetComponent<Collider>();

        // aimStat = GetComponent<AimStatManager>();
        // aimStat.Init();
        
        stats = GetComponent<PlayerStats>();
        stats.Init();


        T=transform;
        T.position = Stage.Instance.playerInitPos;      // 스테이지에 설정된 위치에서 시작

        initialized = true;
    }


    public void OnVictory()
    {
        playerController.OnVictory();
    }

    public void OnDefeated()
    {
        playerController.OnDefeated();
    }
}
