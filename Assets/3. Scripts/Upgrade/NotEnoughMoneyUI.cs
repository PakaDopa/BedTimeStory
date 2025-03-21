using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NotEnoughMoneyUI : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color textcolor;

    DG.Tweening.Sequence seq_notEnoughGold;

    bool initialized;

    public void Init()
    {
        if( initialized )
        {
            return;
        }
        
        // img = GetComponent<Image>();
        // text = GetComponentInChildren<TextMeshProUGUI>();
        
        gameObject.SetActive(false);

        initialized = true;
    }

    public void OnInsufficientGold()
    {
        if (seq_notEnoughGold !=null && seq_notEnoughGold.IsActive())
        {
            seq_notEnoughGold.Kill();
        }

        // Debug.Log("건하");
        gameObject.SetActive(true);
        img.color = Color.white;
        text.color = textcolor;
        text.transform.localScale = Vector3.one;

        //
        seq_notEnoughGold= DOTween.Sequence()
        .Append( text.transform.DOShakeScale(0.3f))
        .AppendInterval(0.5f)
        .Append( text.DOFade(0f,0.5f))
        .Join(img.DOFade(0f,0.5f))
        .AppendCallback(()=>gameObject.SetActive(false))    
        .SetUpdate(true)
        .Play();
        
    }



}
