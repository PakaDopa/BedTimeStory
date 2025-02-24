using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Text_GoldIncrement : MonoBehaviour
{
    TextMeshProUGUI text_increment;
    [SerializeField] Color defaultColor = Color.green;

    Sequence seq_text;

    void Awake()
    {
        text_increment = GetComponent<TextMeshProUGUI>();
        text_increment.color = defaultColor;
    }

    void Start()
    {
        PlayerStats.Instance.onGoldChanged.AddListener(onGoldChanged);
        gameObject.SetActive(false);
    }


    void onGoldChanged(int amount,int before, int after)
    {
        if( after <=before )
        {
            return;
        }
        if( seq_text != null && seq_text.IsActive())
        {
            seq_text.Kill();
        }
        
        gameObject.SetActive(true);
        text_increment.color = defaultColor;
        text_increment.SetText($"+{amount}");
        Transform t_text = text_increment.transform;

        seq_text = DOTween.Sequence()
        .Append(t_text.DOScale(1.5f,0.1f))
        .Append(t_text.DOScale(1f,0.1f))
        .Join(text_increment.DOFade(0,0.2f))
        .AppendCallback(()=> {gameObject.SetActive(false);})
        .SetUpdate(true)
        .Play();
    }
}
