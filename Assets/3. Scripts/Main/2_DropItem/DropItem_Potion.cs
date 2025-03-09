using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DropItem_Potion : DropItem
{
    public override void Get()
    {
        SoundManager.Instance.OnPickupHeart(transform.position);
        PlayerStats.Instance.Recover(30);
    }


}
