using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using DG.Tweening;
using System;


public enum Difficulty
{
    Easy,
    Normal,
    Hard
}


/// <summary>
/// 현재 플레이하는 게임의 정보. 난이도와, 각종 통계를 포함한다.  - 대시보드 제작을 위함. 
/// </summary>
[Serializable]
public class CurrGamePlayInfo
{
    public string id;
    public Difficulty currDifficulty;
    public string startDate;
    
    // 실시간 통계 - 추후 명중률 이런 것도 추가하면 좋겠다. 
    public int killCount;               // 쓰러뜨린 적의 수
    public int totalGold;               // 획득한 골드 
    public float totalDamageTaken;      // 받은 피해량(플레이어)
    public float totalHealingDone;      // 총 회복량 


    // 클리어 통계
    public bool cleared;
    public string clearDate;
    public float towerHp;               // 클리어시 남은 타워 체력 

    // 새 게임이 시작될 때, 
    public CurrGamePlayInfo(Difficulty targetDifficulty)
    {
        currDifficulty = targetDifficulty;
        startDate = DateTime.Now.ToString("yyyyMMddHHmmss");
    }


    public void OnVictory()
    {
        cleared   = true;
        clearDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        towerHp   = Tower.Instance.hp;

        LocalDataManager.SaveClearedGamePlayInfo(this);
    }
}

[Serializable]
public class RankingData
{
    public List<CurrGamePlayInfo> list_easy;
    public List<CurrGamePlayInfo> list_normal;
    public List<CurrGamePlayInfo> list_hard;
}


/// <summary>
/// 게임 매니저
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public bool isPaused;

    // public Difficulty currDifficulty{get;private set;}

    public CurrGamePlayInfo currGamePlayInfo;
    public override void Init()
    {
        
    }


    public void StartNewGame(Difficulty targetDifficulty)
    {
        currGamePlayInfo = new(targetDifficulty);
        SceneLoadManager.Instance.Load_MainScene();
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
