using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySceneManager : MonoBehaviour
{
    public bool isStroyCutSceneEnabled = true;                  // 컷씬대기, 디버그할 땐 플래그 false 해놓기        
    [SerializeField] CutSceneManager cutSceneManager;

    IEnumerator Start()
    {        
        cutSceneManager.Init();

        // 
        if( isStroyCutSceneEnabled )
        {
            StartCoroutine(cutSceneManager.PlayCutScene());
            yield return new WaitUntil( ()=>cutSceneManager.isFinished );
        }

        OnCutSceneFinish();
    }




    void OnCutSceneFinish()
    {
        SceneLoadManager.Instance.Load_Lobby();
    }
}
