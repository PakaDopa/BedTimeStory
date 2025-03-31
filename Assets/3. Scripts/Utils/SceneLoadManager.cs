using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using DG.Tweening;

public class SceneLoadManager : DestroyableSingleton<SceneLoadManager>
{
    public static readonly string DummySceneName = "Dummy";
    public static readonly string storySceneName = "0_Story";
    public static readonly string lobbySceneName = "1_Lobby";
    public static readonly string mainSceneName = "2_Main";
    public static readonly string resultSceneName = "3_Result";

    bool isLoading;

    [SerializeField] bool isCompleted_sceneLoaded;

    [Header("Fade")]
    [SerializeField] Canvas fadeCanvas;
    [SerializeField] Image fade;
    [SerializeField] Color fadeColor = new Color(0,0,0,1);

    public bool isCompleted_fade;

    public bool initialized{get;private set;}

    //==============================================================
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        initialized = true;
    }


    //====================
    // 비동기 씬 호출 : sceneName에 해당하는 씬을 비동기적으로 로드한다. 
    //===================
    public void LoadScene(string sceneName)
    {
        GameManager.Instance?.PauseGamePlay(false);
        if( isLoading )
        {
            return;
        }
        isLoading = true;



        StartCoroutine(LoadScene_async(sceneName)); // 씬 전환 작업
    }

    IEnumerator LoadScene_async(string sceneName)
    {            
        //
        isLoading = false;
        isCompleted_sceneLoaded = false;


         //페이드 인 실행

        // 더미씬 실행 
        AsyncOperation dummyLoad = SceneManager.LoadSceneAsync(DummySceneName);
        dummyLoad.allowSceneActivation = false;     
        yield return FadeIn().WaitForCompletion();
        yield return new WaitUntil(()=> dummyLoad.progress >= 0.9f);           // 페이드 인 대기
        dummyLoad.allowSceneActivation = true;     
        
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        // 비동기 씬호출
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;         
        yield return new WaitUntil(()=> asyncLoad.progress >= 0.9f) ;      // 메모리에 해당 씬 리소스 모두 올릴 때까지 대기  
        asyncLoad.allowSceneActivation = true;      // 이거 하면 씬 넘어감

    
        // 씬 로드 이후 작업 
        yield return new WaitUntil(()=>asyncLoad.isDone);
        
        FadeOut();  // 페이드 아웃 실행
        isLoading = false;
        isCompleted_sceneLoaded = true;
        
        OnSceneChanged();
    }

    void OnSceneChanged()
    {

    }

    //================================
    public void Load_Story()
    {
        LoadScene(lobbySceneName);
    }



    public void Load_Lobby()
    {
        LoadScene(lobbySceneName);
    }


    public void Load_MainScene()
    {
        LoadScene(mainSceneName);
    }

    public void Load_Result()
    {
        LoadScene(resultSceneName);
    }

    //=====================================================================


    /// <summary>
    ///  페이드인 : 화면 까매짐
    /// </summary>
    public Sequence FadeIn()
    {
        isCompleted_fade =false;
        fadeCanvas.gameObject.SetActive(true);
        fade.gameObject.SetActive(true);
        fade.color = new Color(0,0,0,0);

        return DOTween.Sequence()
        .OnComplete( ()=>{isCompleted_fade= true;})
        .Append(fade.DOFade(1,0.5f))
        .SetUpdate(true)
        .Play();
    }

    /// <summary>s
    /// 페이드 아웃 : 화면 밝아짐 - 씬 전환되고 
    /// </summary>
    public void FadeOut()
    {
        DOTween.Sequence()
        .OnComplete( ()=>{fadeCanvas.gameObject.SetActive(false);  fade.gameObject.SetActive(false);})
        .Append(fade.DOFade(0,0.5f))
        .Play();
    }

}
