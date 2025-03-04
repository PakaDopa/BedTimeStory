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


    private void Start()
    {

    }

    private void Update()
    {
        bool isRunning = PlayerStats.Instance.playerStatus == PlayerStats.Status.Run;
        
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
