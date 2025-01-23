using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


public enum TextEffectType
{
    NormalDamage,
    CriticalDamage
}


public class TextEffect : Effect
{
    public override EffectEvent effectEvent => EffectEvent.Text;

    TextMeshPro text;


    Transform t_camera;
    Transform t_text;

    //============================================================================================================
    void Update()
    {
        Billboard();
    }

    
    public void Init(Vector3 initPos, float lifeTime, string value, TextEffectType type = TextEffectType.NormalDamage)
    {   
        t_camera = Camera.main.transform;
        t_text = transform;
        t_text.localScale = Vector3.one;        // 풀에서 나오면 리셋
        
        text = GetComponent<TextMeshPro>();
        text.SetText(value);
        

        initPos = GetFixedInitPos(initPos);
        Init(initPos, lifeTime);

        

        // 애니메이션 재생 
        if( type == TextEffectType.NormalDamage)
        {
            PlayAnim_NormalDamage();
        }
        else if ( type == TextEffectType.CriticalDamage)
        {
            PlayAnim_CriticalDamage();
        }
    }


    Vector3 GetFixedInitPos(Vector3 initPos)
    {
        float xNoise = Random.Range(-1.5f,1.5f);
        float yNoise = Random.Range(-0.5f,0.5f);
        float zOffset = -1;

        return initPos + new Vector3( xNoise, yNoise, zOffset);
    }

    

    /// <summary>
    /// 글자가 카메라를 보는 기능
    /// </summary>
    void Billboard()
    {
        // 텍스트가 항상 카메라를 바라보게 설정
        t_text.LookAt(t_camera );
        // 텍스트가 반대로 회전하지 않도록 Z축으로 180도 회전
        t_text.Rotate(0, 180, 0);
    }


    void PlayAnim_NormalDamage()
    {
        text.color = new Color(1,1,0.25f,1);    // 약간 노란색
        
        float deltaY = 0.5f;
        float interval = lifeTime * 0.3f;
        float duration = (lifeTime - interval)* 0.95f;

        Sequence sequence  = DOTween.Sequence()
        //
        .Append( t_text.DOScale(2f,interval*0.5f))
        .Append( t_text.DOScale(1f,interval*0.5f))
        .Append(t_text.DOMoveY(t_text.position.y + deltaY, duration ))
        .Join( text.DOFade(0,duration * 0.8f ).SetDelay(duration * 0.2f) )
        .Play();
    }

    void PlayAnim_CriticalDamage()
    {
        
    }

}
