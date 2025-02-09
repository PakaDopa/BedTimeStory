using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuidePanel : MonoBehaviour
{
    bool initialized = false;
    
    [Header("Contents control")]
    [SerializeField] int currPage;

    [SerializeField] List<GuideContent> contents;
    
    [SerializeField] Button btn_left;
    [SerializeField] Button btn_right;






    [Header("etc")]

    [SerializeField] Button btn_close;
    

    
    //=======================================================================================================
    
    public void Init()
    {
        btn_close.onClick.AddListener(Close);


        btn_left.onClick.AddListener(OnClick_LeftBtn);
        btn_right.onClick.AddListener(OnClick_RightBtn);

        initialized = true;
    }



    public void Open()
    {
        if( initialized ==false)
        {
            Init();
        }
        currPage = 0;
        SetPage(currPage);
        SetControlBtnState();

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }


    //=========================================

    void OnClick_LeftBtn()
    {
        currPage = Mathf.Clamp( --currPage, 0, contents.Count-1);
        SetPage(currPage);

        SetControlBtnState();
    }


    void OnClick_RightBtn()
    {
        currPage = Mathf.Clamp( ++currPage, 0, contents.Count-1);
        SetPage(currPage);

        SetControlBtnState();
    }

    void SetControlBtnState()
    {
        btn_left.gameObject.SetActive( currPage >0  );
        btn_right.gameObject.SetActive( currPage < contents.Count-1  );
    }

    void SetPage(int currPage)
    {
        for(int i=0;i<contents.Count;i++)
        {
            contents[i].gameObject.SetActive(i == currPage);
        }
    }
}
