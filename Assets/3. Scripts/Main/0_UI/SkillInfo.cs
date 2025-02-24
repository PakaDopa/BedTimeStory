using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    [SerializeField] Image img_icon;
    [SerializeField] Image img_fill;

    [SerializeField] TMP_Text text_cooltime;
    Weapon weapon;

    bool readyEffectActivated  = false;
    // private void Awake()
    // {
    //     text = GetComponentInChildren<TextMeshProUGUI>();
    // }

    private void Start()
    {
        weapon = FindObjectOfType<Weapon>();
        if(weapon)
        {
            Debug.Log("SUCCESS");
        }
    }

    private void Update()
    {
        float currCooltime = weapon.CurrSkillCooltime;
        
        if(currCooltime > 0)
        {
            float maxCooltime = PlayerStats.Instance.SkillCooltime;
            float ratio = maxCooltime>0? currCooltime / maxCooltime:0;



            text_cooltime.enabled = true;
            text_cooltime.SetText($"{currCooltime:0}");

            img_fill.fillAmount = ratio;
            
            if ( readyEffectActivated )
            {
                readyEffectActivated = false;
            }
        }
        else
        {
            

            img_fill.fillAmount = 0;
            text_cooltime.enabled = false;
            
            if(readyEffectActivated == false)
            {
                OnSkillReady();
                readyEffectActivated = true;
            }
        }
    }

    void OnSkillReady()
    {
        Image img_effect = new GameObject("ReadyEffect").AddComponent<Image>();
        img_effect.sprite = img_icon.sprite;
        img_effect.transform.SetParent(img_icon.transform);
        img_effect.transform.localPosition = Vector3.zero;
    

        Sequence seq = DOTween.Sequence()
        .OnComplete(()=>Destroy(img_effect.gameObject))
        .Append(img_effect.transform.DOScale(1.5f,0.3f))
        .Join(img_effect.DOFade(0,0.3f))
        .Play();
    }
}
