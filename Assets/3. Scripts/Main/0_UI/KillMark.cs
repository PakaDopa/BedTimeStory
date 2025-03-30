using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class KillMark : MonoBehaviour
{
    Transform _t;
    Sequence seq_activation;
    Image img;


    public void Init()
    {
        _t=transform;
        img = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        if( seq_activation!=null && seq_activation.IsActive())
        {
            seq_activation.Kill();
        }

        gameObject.SetActive(true);


        _t.localScale= new Vector2(0.5f,0.5f);
        img.color = new Color(1,1,1,0.8f);

        seq_activation = DOTween.Sequence()
        .OnComplete(()=>gameObject.SetActive(false))
        .Append(_t.DOScale(1.5f,0.1f))
        .Append(_t.DOScale(1f,0.1f))
        .AppendInterval(0.2f)
        .Append(img.DOFade(0,0.3f))
        .Play();
    }
}
