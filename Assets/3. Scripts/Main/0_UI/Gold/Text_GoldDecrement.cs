using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Text_GoldDecrement : MonoBehaviour
{
    TextMeshProUGUI text_decrement;

    [SerializeField] Color defaultColor = Color.red;

    Sequence seq_text;


    void Awake()
    {
        text_decrement = GetComponent<TextMeshProUGUI>();
        text_decrement.color = defaultColor;
        
    }

    void Start()
    {
        PlayerStats.Instance.onGoldChanged.AddListener(onGoldChanged);
        gameObject.SetActive(false);
    }

    void onGoldChanged(int amount,int before, int after)
    {
        if( after >= before )
        {
            return;
        }
        
        if( seq_text != null && seq_text.IsActive())
        {
            seq_text.Kill();
        }
        
        gameObject.SetActive(true);
        text_decrement.color = defaultColor;
        text_decrement.SetText($"-{amount}");
        Transform t_text = text_decrement.transform;

        seq_text = DOTween.Sequence()
        .Append(t_text.DOScale(1.5f,0.1f))
        .Append(t_text.DOScale(1f,0.1f))
        .Join(text_decrement.DOFade(0,0.2f))
        .AppendCallback(()=> {gameObject.SetActive(false);})
        .SetUpdate(true)
        .Play();
    }
}
