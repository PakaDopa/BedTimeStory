using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.Rendering;

public class ComboToastUI : MonoBehaviour
{
    [SerializeField] List<int> killList;        // 150, 300,600, 1020
    [SerializeField] List<Sprite> spriteList;
    
    [SerializeField]Image img;
    

    Sequence seq_activate;


    [SerializeField] float duration_fadeIn = 0.2f;
    [SerializeField] float duration_stay = 0.5f;
    [SerializeField] float duration_fadeOut = 0.3f;

    void OnValidate()
    {
        int killListCount = killList.Count;
        int spriteListCount = spriteList.Count;

        int diff = killListCount  - spriteListCount; 
        // sprite 채우기
        if ( diff>0)
        {
            for(int i=0;i<diff;i++)
            {
                spriteList.Add(null);
            }   
        }
        // sprite 줄이기
        else if( diff<0)
        {
            for(int i=0;i<-diff;i++)
            {
                spriteList.RemoveAt( spriteList.Count-1 );
            }   
        }
    }




    public void Init()
    {
        img.color = new Color(1,1,1,0);
    }

    public void Activate(int killCount)
    {
        if(seq_activate !=null && seq_activate.IsActive())
        {
            seq_activate.Kill();
        }


        img.color = new Color(1,1,1,0);

        img.sprite = GetCurrSprite(killCount);
        
        Transform t_img = img.transform;
        // t_img.localScale = new Vector3(1.5f,1.5f,1.5f);

        seq_activate = DOTween.Sequence()
        .Append( img.DOFade(1f,duration_fadeIn))
        .AppendInterval(duration_stay)
        .Append( img.DOFade(0f,duration_fadeOut))
        .Play();

    }

    Sprite GetCurrSprite(int killCount)
    {
        Sprite ret = null;  
        for(int i=killList.Count-1;i>=0;i--)
        {
            int currValue = killList[i];
            if( killCount >= currValue )
            {
                ret = spriteList[i];
                break;
            }
        }

        return ret;
    }   





}
