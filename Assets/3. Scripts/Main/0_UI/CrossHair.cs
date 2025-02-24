using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CrossHair : MonoBehaviour
{
    [SerializeField] Slider ammoSlide;
    [SerializeField] private TMP_Text text_reload;
    [SerializeField] private TMP_Text text_ammo;
    
    public bool isReloading {get;set;}

    static WaitForEndOfFrame  wfef = new();

    [Header("Slide Setting")]
    [SerializeField] Image img_ammoSlideFill;
    [SerializeField] Color color_ammo = new Color(1,1,0.6f,0.4f);
    [SerializeField] Color color_reload = new Color(0,1,1,0.9f);
    //========================================================


    public void UpdateAmmoRemain(float currValue, float maxValue)
    {
        if (isReloading)
        {
            return;
        }

        img_ammoSlideFill.color = color_ammo;


        Transform t_text = text_ammo.transform;
        
        DOTween.Sequence()
            .Append(t_text.DOScale(1.5f, 0.05f))
            .Append(t_text.DOScale(0.8f, 0.05f))
            .Append(t_text.DOScale(1f,0.05f))
            .Play();


        text_ammo.text = $"{currValue}";
        ammoSlide.maxValue = maxValue;
        ammoSlide.value = currValue;
    }

    public void OnReload(bool isOn, float duration)
    {
        if ( isOn )
        {
            img_ammoSlideFill.color = color_reload;
            StartCoroutine(UIActivationRoutine(duration));
        }
        else
        {
            img_ammoSlideFill.color = color_ammo;
            OnReload(false);
        }
    }

    void OnReload(bool isOn)
    {
        isReloading = false;
        text_reload.gameObject.SetActive(isOn);
    }

    IEnumerator UIActivationRoutine(float duration)
    {
        OnReload(true);
        
        float elapsed = 0;
        while (elapsed < duration)
        {
            float ratio = elapsed/duration;
            text_ammo.SetText($"{ratio * 100:0}%");
            
            ammoSlide.maxValue = duration;
            ammoSlide.value = elapsed;

            elapsed += Time.deltaTime;
            yield return wfef;
        }
    }

}