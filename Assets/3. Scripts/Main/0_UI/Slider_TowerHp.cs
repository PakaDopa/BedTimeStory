using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slider_TowerHp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_hpRatio; 

    [SerializeField]Slider slider_hp;
    
    //

    IEnumerator Start()
    {
        yield return new WaitUntil( ()=> Tower.Instance.initialized);
        Tower.Instance.onHpChanged.AddListener(OnUpdateTowerHp);

        slider_hp = GetComponent<Slider>();
        text_hpRatio = GetComponentInChildren<TextMeshProUGUI>();
        float currValue = Tower.Instance.hp;
        float maxValue = Tower.Instance.maxHp;
        OnUpdateTowerHp(currValue,maxValue);
    }




    void OnUpdateTowerHp(float currValue, float maxValue)
    {
        slider_hp.maxValue = maxValue;
        slider_hp.value = currValue;
        

        float ratio = currValue/ maxValue * 100;

        text_hpRatio.SetText($"{ratio:00}%");   

    }




    
}
