using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class Lobby : DestroyableSingleton<Lobby>
{
    [Header("Dashboard")]
    [SerializeField] Button btn_dashboard;
    [SerializeField] DashboardPanel dashboardPanel;
    

    [Header("Guide")]
    [SerializeField] Button btn_guide;
    [SerializeField] GuidePanel guidePanel;


    [Header("Setting")]
    [SerializeField] Button btn_setting;
    [SerializeField] SettingPanel_Lobby settingPanel;


    
    // Start is called before the first frame update
    void Start()
    {
        Init();
                
    }


    public void Init()
    {
        GameManager.Instance.PauseGamePlay(false);
    
        btn_dashboard?.onClick.AddListener(()=>dashboardPanel?.Open());
        btn_guide?.onClick.AddListener(()=>guidePanel?.Open());
        btn_setting?.onClick.AddListener(()=>settingPanel?.Open());
    
    
    }


}
