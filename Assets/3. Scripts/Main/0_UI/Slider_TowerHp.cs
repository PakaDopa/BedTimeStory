using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slider_TowerHp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_hpRatio; 

    [SerializeField]Slider slider_hp;

    [SerializeField] Transform t_handle;
    Sequence seq_hpChanged;
    
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

        if( seq_hpChanged !=null && seq_hpChanged.IsActive())
        {
            seq_hpChanged.Kill();
        }

        float originScale = t_handle.localScale.x;
        seq_hpChanged = DOTween.Sequence()
        .OnKill( ()=>{t_handle.localScale = originScale *Vector3.one;} )
        .Append( t_handle.DOScale(1.3f,0.2f))
        .Append( t_handle.DOScale(originScale,0.2f))
        .Play();

    }




    
}
