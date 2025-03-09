using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem_Pouch : DropItem
{

    public override void Get()
    {
        SoundManager.Instance.OnPickupCoin(transform.position);
        PlayerStats.Instance.GetGold(100);
    }

}
