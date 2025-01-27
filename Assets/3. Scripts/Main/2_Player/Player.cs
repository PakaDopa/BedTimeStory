using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : DestroyableSingleton<Player>
{
    public Collider playerCollider;
    public Transform T;

    public void Init()
    {
        playerCollider = GetComponent<Collider>();

        T=transform;
        T.position = Stage.Instance.playerInitPos;      // 스테이지에 설정된 위치에서 시작
        
    }
}
