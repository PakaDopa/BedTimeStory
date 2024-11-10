using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class PlayerHitEffect : MonoBehaviour
{
    [SerializeField] Image img_playerHitEffect;

    Sequence seq_playerHit;
    
    // Start is called before the first frame update
    void Start()
    {
        GameEventManager.Instance.onPlayerGetDamage.AddListener( OnPlayerGetDamage );
    }

    void OnPlayerGetDamage()
    {   
        if (seq_playerHit !=null && seq_playerHit.IsActive())
        {
            seq_playerHit.Kill();
        }
        
        img_playerHitEffect.color = new Color(1,1,1,0f);

        seq_playerHit = DOTween.Sequence()
        .OnKill(()=>img_playerHitEffect.color = new Color(1,1,1,0f) )

        .Append( img_playerHitEffect.DOFade(1f,0.1f))
        .Append( img_playerHitEffect.DOFade(0f,0.1f))

        .Append( img_playerHitEffect.DOFade(1f,0.1f))
        .Append( img_playerHitEffect.DOFade(0f,0.1f))

        .Play();
    }

    
}
