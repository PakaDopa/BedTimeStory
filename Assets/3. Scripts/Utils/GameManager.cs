using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using DG.Tweening;


public enum Difficulty
{
    Easy,
    Normal,
    Hard
}





public class GameManager : Singleton<GameManager>
{
    public bool isPaused;

    public Difficulty currDifficulty{get;private set;}
    public override void Init()
    {
        currDifficulty = 0;
    }


    public void SetDifficulty(Difficulty targetDifficulty)
    {
        currDifficulty = targetDifficulty;
    }



    /// <summary>
    /// 게임 종료 - 현재 01_Lobby 의 Quit 버튼에서 호출됨.
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    public void PauseGamePlay(bool pause, float duration = 0f)
    {
        float targetTimeScale= pause? 0: 1f;
        isPaused = pause;
        if (duration == 0f)
        {
            Time.timeScale = targetTimeScale;
        }
        else
        {
            DOTween.To( ()=> Time.timeScale, x=> Time.timeScale = x ,targetTimeScale, duration ).SetUpdate(true).Play();
        }
       
        
    }


    public void LockCursor(bool flag)
    {

        if (flag)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }

    }

}
