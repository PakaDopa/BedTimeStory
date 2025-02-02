using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : DestroyableSingleton<Lobby>
{
    [SerializeField] Button btn_dashboard;
    [SerializeField] DashboardPanel dashboardPanel;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
                
    }


    public void Init()
    {
        GameManager.Instance.PauseGamePlay(false);
    
        btn_dashboard.onClick.AddListener(dashboardPanel.Open);
    
    
    
    }


}
