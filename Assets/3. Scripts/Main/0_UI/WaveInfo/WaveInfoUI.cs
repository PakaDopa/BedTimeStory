using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using DG.Tweening;

public class WaveInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_waveNum;
    [SerializeField] TextMeshProUGUI text_waveTime;

    [SerializeField] ToastMessage toastMessage;  // 웨이브 시작시 토스트 메시지 - 추후 디테일수정
     
    


    
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>GamePlayManager.Instance.initialized);
        Stage.Instance.onWaveStart += OnWaveStart; 
        InitUI();
        
        StartCoroutine( UpdateRoutine());
    }


    void InitUI()
    {
        toastMessage.Init();
    }

    void OnWaveStart()
    {
        text_waveNum.SetText($"<sprite name=\"0\">{Stage.Instance.clearedWaveCount + 1:00}/{Stage.Instance.totalWaveCount:00}");

        toastMessage.OnWaveStart();// 토스트 메시지도 업데이트 
    }




    IEnumerator UpdateRoutine()
    {
        WaitForSeconds wfs = new (0.49f);
        
        while ( true)
        {
            if (Stage.Instance.isWavePlaying )
            {   
                float waveTime = Stage.Instance.wavePlayTime;

                int min  = (int)waveTime/60;
                int sec = (int)waveTime%60;
                
                text_waveTime.SetText($"<sprite name=\"0\">{min:00}:{sec:00}");
            }
            
            yield return wfs;
        }
        
        
        

    }
}
