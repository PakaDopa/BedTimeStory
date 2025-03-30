using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using TMPro;
using DG.Tweening;

public class CutSceneManager : MonoBehaviour
{
    

    [SerializeField] List<GameObject> pages= new();
    [SerializeField] List<Image> cuts;


    [SerializeField] GameObject imgParent;
    [SerializeField] TextMeshProUGUI text_howToStart;
    
    public int currPage;
    public int cutIdx;
    
    
    public bool isPlaying => cutIdx < cuts.Count;
    public bool isWaitingForInput;
    public bool isFinished;

    


    public void Init()
    {
        imgParent.SetActive(false);
        text_howToStart.gameObject.SetActive(false);
    }

    void Update()
    {


        #if UNITY_EDITOR
        if (isPlaying && Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("히든 커맨드");
            Time.timeScale = 5f;
        }
        #endif        


        if (isPlaying && Input.GetKeyDown(KeyCode.Escape)) //    강제 종료 
        {
            isFinished = true;
        }   
        else if (isWaitingForInput && Input.anyKeyDown)    // 컷씬 종료
        {
            // CloseCutSceneAndStartWave();
            isFinished = true;      // 이때 업적띄우기??
        }
    }


    public IEnumerator PlayCutScene()
    {
        Debug.Log("컷씬 시작");
        imgParent.SetActive(true);
        text_howToStart.gameObject.SetActive(false);
        foreach( Image img in cuts)
        {
            // Debug.Log(img.gameObject.name);
            img.gameObject.SetActive(false);
        }
        
    
        yield return new WaitForSeconds(1f);

        while(isPlaying)
        {
            if ( cutIdx == 5 )
            {
                pages[currPage].SetActive(false);
                currPage++;

            }
            
            ShowImage(cuts[cutIdx]);
            cutIdx ++;
            yield return new WaitForSeconds(1f);
        }
        
        
        isWaitingForInput = true;
        OnCutSceneFinish();

        Debug.Log("컷씬 끝");
    }


    void ShowImage(Image img)
    {
        img.gameObject.SetActive(true);
        img.color = new Color(1,1,1,0);

        img.DOFade(1f,0.2f).Play();
    }

    void OnCutSceneFinish()
    {
        text_howToStart.gameObject.SetActive(true);
        text_howToStart.color = new Color(1,1,1,0);
        
        DOTween.Sequence()
        .Append(text_howToStart.DOFade(1f,0.5f))
        .AppendInterval(0.3f)
        .Append(text_howToStart.DOFade(0f,0.5f))
        .AppendInterval(0.3f)
        .SetLoops(-1)
        // .SetUpdate(true) 
        .Play();
        
    
    }



    public void CloseCutSceneAndStartWave()
    {
        Debug.Log("컷씬 종료 입력");
        
        // gameObject.SetActive(false);
        // GamePlayManager.Instance.OnCutSceneFinish();
        
    }
}
