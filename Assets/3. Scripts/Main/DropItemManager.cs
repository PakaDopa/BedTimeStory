using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : DestroyableSingleton<DropItemManager>
{
    public GameObject prefab_pouch;
    public GameObject prefab_potion;

    float yHeight = 1.11f;


    //금화주머니
    public DropItem GetItem_Pouch(Vector3 pos)
    {
        pos = new Vector3 (pos.x, yHeight, pos.z);
        
        return Instantiate( prefab_pouch, pos ,Quaternion.identity).GetComponent<DropItem>();
    }


    
    //포션 
    public DropItem GetItem_Potion(Vector3 pos)
    {
        pos = new Vector3 (pos.x, yHeight, pos.z);
        return Instantiate( prefab_potion, pos ,Quaternion.identity).GetComponent<DropItem>();
    }




}
