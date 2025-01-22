using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : DestroyableSingleton<Player>
{
    public Collider playerCollider;


    public void Init()
    {
        playerCollider = GetComponent<Collider>();
    }
}
