using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TableRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_rank;
    [SerializeField] TextMeshProUGUI text_date;
    [SerializeField] TextMeshProUGUI text_towerHp;
    [SerializeField] TextMeshProUGUI text_killCount;
    
    [SerializeField] TextMeshProUGUI text_nan;
    


    //===================================================================
    
    public void Init(int idx, CurrGamePlayInfo currGamePlayInfo)
    {
        bool isEmptyData = currGamePlayInfo==null;
        
        text_rank.gameObject.SetActive(isEmptyData == false);
        text_date.gameObject.SetActive(isEmptyData == false);
        text_towerHp.gameObject.SetActive(isEmptyData == false);
        text_killCount.gameObject.SetActive(isEmptyData == false);

        text_nan.gameObject.SetActive(isEmptyData);

        //
        if ( isEmptyData==false )
        {
            text_rank.SetText($"{idx+1}.");
            text_date.SetText($"{currGamePlayInfo.clearDate}");
            text_towerHp.SetText($"{currGamePlayInfo.towerHp}");
            text_killCount.SetText($"{currGamePlayInfo.killCount}");
        }

        
    }
}
