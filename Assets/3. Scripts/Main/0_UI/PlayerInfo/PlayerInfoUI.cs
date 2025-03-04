using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI text_hp;
    
    //===========================================================================================
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance.initialized);
        
        PlayerStats.Instance.onHpChanged.AddListener( UpdateHpValue ); 
        
        float currHp = PlayerStats.Instance.currHP;
        float maxHp = PlayerStats.Instance.maxHP;
        UpdateHpValue(currHp, maxHp);
    }

    //=====================================================
    public void UpdateHpValue(float currValue,float maxValue)
    {
        text_hp.SetText($"{currValue} | {maxValue}");

        hpSlider.maxValue = maxValue;
        hpSlider.value = currValue;   
    }

}
