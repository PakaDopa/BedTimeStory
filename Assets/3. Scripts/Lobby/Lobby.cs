using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : DestroyableSingleton<Lobby>
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.PauseGamePlay(false);        
    }


}
