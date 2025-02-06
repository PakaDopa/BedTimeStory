using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DashboardPanel : MonoBehaviour
{
    bool initialized = false;
    
    [SerializeField] Button btn_close;

    [SerializeField] List<TableRow> tableData;

    [SerializeField] SerializableDictionary<Difficulty,Button> difficultyBtns;


    //
    Difficulty selectedDifficulty;
    RankingData rankingData;



    //=========================================================================

    public void Init()
    {
        btn_close.onClick.AddListener(Close);

        foreach( var kv in difficultyBtns)
        {
            Difficulty difficulty = kv.Key;
            Button btn = kv.Value;

            btn.onClick.AddListener(()=>OnClick_difficultyBtn(difficulty));
        
        }


        // 데이터 채우기. 
        rankingData = LocalDataManager.LoadRankingData();


        initialized = true;
    }



    public void Open()
    {
        if( initialized ==false)
        {
            Init();
        }
        OnClick_difficultyBtn(Difficulty.Easy);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    //============================================================
    void OnClick_difficultyBtn(Difficulty targetDifficulty)
    {
        selectedDifficulty = targetDifficulty;
        
        // btn 꾸미기
        foreach( var kv in difficultyBtns)
        {
            Difficulty difficulty = kv.Key;
            Button btn = kv.Value;

            bool isTarget = difficulty == targetDifficulty;
            
            Color targetColor = isTarget?Color.white: Color.white.WithAlpha(0.5f);
            
            btn.GetComponent<Image>().color = targetColor;
        }


        // 테이블 보여주기 
        InitTable(targetDifficulty);
    }




    void InitTable(Difficulty targetDifficulty)
    {
        List<CurrGamePlayInfo> targetList = new();

        if (targetDifficulty == Difficulty.Easy)
        {
            targetList = rankingData.list_easy;
        }
        else if (targetDifficulty == Difficulty.Normal)
        {
            targetList = rankingData.list_normal;
        }
        else if (targetDifficulty==Difficulty.Hard)
        {
            targetList = rankingData.list_hard;
        }

        //
        for(int i=0;i<tableData.Count;i++)
        {
            CurrGamePlayInfo data = i< targetList.Count?  targetList[i]: null;
            tableData[i].Init(i, data);
        }
    }




}
