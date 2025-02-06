using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class TouchEffectManager : MonoBehaviour
{
    [SerializeField] CanvasScaler canvasScalerRef;      //스케일 따라할 캔버스 
    [SerializeField] GameObject prefab_touchEffect;
    
    bool touchEffectEnabled => Cursor.lockState != CursorLockMode.Locked;
    
    // Start is called before the first frame update
    void Start()
    {
        InitCanvasScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && touchEffectEnabled)
        {
            // Debug.Log( $"터치이펙트 생성 {touchEffectEnabled} {Input.mousePosition}  ");
            GenerateTouchEffect();
        }
    }

    //============================================
    
    void InitCanvasScale()
    {
        if(canvasScalerRef==null)
        {
            return;
        }
        
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();

        canvasScaler.uiScaleMode = canvasScalerRef.uiScaleMode;
        canvasScaler.referenceResolution =  canvasScalerRef.referenceResolution;
        canvasScaler.referencePixelsPerUnit = canvasScalerRef.referencePixelsPerUnit;
        canvasScaler.screenMatchMode =  canvasScalerRef.screenMatchMode;
        canvasScaler.matchWidthOrHeight = canvasScalerRef.matchWidthOrHeight;
        
    }


    void GenerateTouchEffect()
    {
        //
        Image touchEffectImg = Instantiate(prefab_touchEffect,transform).GetComponent<Image>();
        //
        Vector2 initPos = Input.mousePosition;
        touchEffectImg.transform.position = initPos;
        //
        float randAngle = Random.Range(-30.0f,30.0f);                  
        touchEffectImg.transform.localRotation = Quaternion.Euler(0,0, randAngle);


        PlayeTouchEffectAnim(touchEffectImg);
    }


    void PlayeTouchEffectAnim(Image touchEffectImg)
    {
        Transform t_touchEffect = touchEffectImg.transform;
        // 
        Vector2 initScale = touchEffectImg.transform.localScale;
        float targetScaleMultiplier = 1.5f;
        //
        float initAlpha = 0.5f;             // 초기 투명도
        float targetAlpha = 0.9f;           // 가장 잘보일때 투명도
        touchEffectImg.color = new Color(1,1,1,initAlpha);

        //
        Sequence sequence = DOTween.Sequence()
        //
        .Append(t_touchEffect.DOScale(targetScaleMultiplier,0.1f))      // 커지기
        .Join(touchEffectImg.DOFade(targetAlpha, 0.1f))                 // 선명해지기
        //
        .Append(t_touchEffect.DOScale(initScale,0.4f))              // 작아지기
        .Join(touchEffectImg.DOFade(0,0.4f))                      // 투명해지기 
        //
        .OnKill(()=>Destroy(touchEffectImg.gameObject))                 //끝나고 파괴
        .SetUpdate(true)
        .Play();
    }


}
