using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


using DG.Tweening;




public class TowerStateToast : MonoBehaviour
{
    [SerializeField] List<ToastAppearanceInfo> toastAppearanceInfoList;
    [SerializeField] Image img_toast;

    DG.Tweening.Sequence seq_toast;



    //=================================================================================
    void OnValidate()
    {
        toastAppearanceInfoList = toastAppearanceInfoList.OrderByDescending(x=>x.targetHp).ToList();
    }
    
    
    IEnumerator Start()
    {
        img_toast.gameObject.SetActive(false);
        yield return new WaitUntil(()=>Tower.Instance.initialized);  
        Init();
    }

    void Init()
    {
        Tower.Instance.onHpChanged.AddListener( OnUpdateTowerHp );
    }

    //

    void OnUpdateTowerHp(float from, float currValue, float maxValue)
    {
        float ratio = currValue / maxValue * 100f;

        for(int i=0;i<toastAppearanceInfoList.Count;i++)
        {
            ToastAppearanceInfo toastAppearanceInfo = toastAppearanceInfoList[i];

            if ( ratio <= toastAppearanceInfo.targetHp && toastAppearanceInfo.used == false  )
            {
                if ( toastAppearanceInfo.canRepeat)
                {
                    toastAppearanceInfo.used = true;
                }
                
                Toast(toastAppearanceInfo);
                return;
            }
        }
    }


    void Toast(ToastAppearanceInfo toastAppearanceInfo)
    {
        if (seq_toast !=null && seq_toast.IsActive())
        {
            seq_toast.Kill();
        }
        
        
        img_toast.gameObject.SetActive( true );
        img_toast.sprite = toastAppearanceInfo.sprite;  
        img_toast.color = new Color(1,1,1,0);
        img_toast.transform.localScale = Vector3.one;
        
        

        Transform t_img = img_toast.transform;

        seq_toast = DOTween.Sequence()
        .Append( t_img.DOScale(1.5f, 0.25f))
        .Append( t_img.DOScale(1f, 0.25f))
        .AppendInterval(0.5f)
        .Append( img_toast.DOFade(0, 1f))
        .AppendCallback( ()=> img_toast.gameObject.SetActive(false))
        .Play();
    }




    [System.Serializable]
    class ToastAppearanceInfo 
    {
        public float targetHp;
        public Sprite sprite;
        public bool canRepeat;
        public bool used;
    }
}
