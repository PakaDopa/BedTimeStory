using System.Collections;
using Unity.VisualScripting;
using UnityEngine;






public  enum PanelState
{
    Option,     //옵션 열려있는 상태
    Upgrade,    //업그레이드 열려있는 상태
    None        //열려 있는게 없는 상태
}

public class UIManager : DestroyableSingleton<UIManager>
{


    [SerializeField] UpgradeSystem upgradePanel;
    [SerializeField] GameObject optionPanel;

    [SerializeField] KeyCode upgradePanelOpenKey;
    [SerializeField] KeyCode optionPanelOpenKey;
    [SerializeField] KeyCode closePanelKey;

    private PanelState panelState = PanelState.None;

    IEnumerator Start()
    {
        yield return new WaitUntil(()=> Player.Instance.initialized);
        
        upgradePanel.Init();
    }

    private void Update()
    {
        //패널 키 조작
        switch(panelState)
        {
            case PanelState.None:
                if(Input.GetKeyDown(upgradePanelOpenKey))
                {
                    EnablePanel(upgradePanel.gameObject, true, PanelState.Upgrade);
                }
                else if(Input.GetKeyDown(optionPanelOpenKey))
                {
                    EnablePanel(optionPanel, true, PanelState.Option);
                }
                break;
            case PanelState.Option:
                //옵션 닫기
                if(Input.GetKeyDown(closePanelKey))
                {
                    EnablePanel(optionPanel, false, PanelState.None);
                }
                break;
            case PanelState.Upgrade:
                //업그레이드 닫기
                if (Input.GetKeyDown(closePanelKey) || Input.GetKeyDown(upgradePanelOpenKey))
                {
                    EnablePanel(upgradePanel.gameObject, false, PanelState.None);
                }
                break;
            default:
                break;
        }
    }
    public void EnablePanel(GameObject panel, bool enable, PanelState targetPanelState)
    {
        panel.SetActive(enable);
        
        if (panel.activeSelf)
        {
            panelState = targetPanelState;

            Cursor.visible = true;
            GamePlayManager.isGamePlaying = false;
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.PauseGamePlay(true);
        }
        else
        {
            panelState = PanelState.None;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GamePlayManager.isGamePlaying = true;
            GameManager.Instance.PauseGamePlay(false);
        }
    }
}
