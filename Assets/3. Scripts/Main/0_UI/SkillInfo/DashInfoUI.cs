using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DashInfoUI : MonoBehaviour
{
    [SerializeField] Image img_icon;
    [SerializeField] Image img_fill;


    bool readyEffectActivated  = false;
    bool initialized;

    PlayerController playerController;

    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance.initialized);
        playerController = Player.Instance.playerController;
        initialized = true;
    }

    private void Update()
    {
        if( initialized == false)
        {
            return;
        }
        bool isRunning = playerController.legState == PlayerLegState.Run;
        
        if (isRunning)
        {
            // 칠하기
            img_fill.fillAmount = 1;

            if ( readyEffectActivated )
            {
                readyEffectActivated = false;
            }
        }
        else
        {
            img_fill.fillAmount = 0;
        }
    }
}
