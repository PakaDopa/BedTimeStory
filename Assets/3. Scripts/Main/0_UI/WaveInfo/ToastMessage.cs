using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using DG.Tweening;

public class ToastMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_toast;


    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void OnWaveStart()
    {
        text_toast.SetText($"Wave {Stage.Instance.clearedWaveCount+1}" );
        
        Sequence seqeunce = DOTween.Sequence()
        .AppendCallback( ()=>gameObject.SetActive(true) )
        .AppendInterval(1f)
        .AppendCallback( ()=>gameObject.SetActive(false))
        .Play();
    }
}
