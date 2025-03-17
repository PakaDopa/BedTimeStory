using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{

    [Header("HP")]
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI text_hp;
    
    [Header("RP")]  // running point
    [SerializeField] Slider staminaSlider;
    [SerializeField] TextMeshProUGUI text_stamina;


    //===========================================================================================
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance.initialized);
        
        PlayerStats.Instance.onHpChanged.AddListener( UpdateHpValue ); 
        PlayerStats.Instance.onRpChanged.AddListener( UpdateStaminaValue );
        
        float currHp = PlayerStats.Instance.currHP;
        float maxHp = PlayerStats.Instance.maxHP;
        UpdateHpValue(0, currHp, maxHp);

        float currRP = PlayerStats.Instance.currStamina;
        float maxRp = PlayerStats.Instance.maxStamina;
        UpdateStaminaValue(0, currRP, maxRp);
    }

    //=====================================================
    public void UpdateHpValue(float from, float to,float maxValue)
    {
        text_hp.SetText($"{to} | {maxValue}");

        hpSlider.maxValue = maxValue;
        hpSlider.value = to;   
    }

    //=====================================================
    public void UpdateStaminaValue(float from, float to, float maxValue)
    {
        float ratio = to/maxValue * 100;
        text_stamina.SetText($"{ratio:0}%");

        staminaSlider.maxValue = maxValue;
        staminaSlider.value = to;
    }

}
