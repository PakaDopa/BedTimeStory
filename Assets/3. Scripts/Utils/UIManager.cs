using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : DestroyableSingleton<UIManager>
{
    private enum PanelState
    {
        Option,     //옵션 열려있는 상태
        Upgrade,    //업그레이드 열려있는 상태
        None        //열려 있는게 없는 상태
    }

    [SerializeField] GameObject upgradePanel;
    [SerializeField] GameObject optionPanel;

    [SerializeField] KeyCode upgradePanelOpenKey;
    [SerializeField] KeyCode optionPanelOpenKey;
    [SerializeField] KeyCode closePanelKey;

    private PanelState panelState = PanelState.None;

    private void Update()
    {
        //패널 키 조작
        switch(panelState)
        {
            case PanelState.None:
                if(Input.GetKeyDown(upgradePanelOpenKey))
                {
                    EnablePanel(upgradePanel, true);
                    panelState = PanelState.Upgrade;
                }
                else if(Input.GetKeyDown(optionPanelOpenKey))
                {
                    EnablePanel(optionPanel, true);
                    panelState = PanelState.Option;
                }
                break;
            case PanelState.Option:
                //옵션 닫기
                if(Input.GetKeyDown(closePanelKey))
                {
                    EnablePanel(optionPanel, false);
                    panelState = PanelState.None;
                }
                break;
            case PanelState.Upgrade:
                //업그레이드 닫기
                if (Input.GetKeyDown(closePanelKey))
                {
                    EnablePanel(upgradePanel, false);
                    panelState = PanelState.None;
                }
                break;
            default:
                break;
        }
    }
    private void EnablePanel(GameObject panel, bool enable)
    {
        panel.SetActive(enable);

        if (panel.activeSelf)
        {
            Cursor.visible = true;
            GamePlayManager.isGamePlaying = false;
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.PauseGamePlay(true);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GamePlayManager.isGamePlaying = true;
            GameManager.Instance.PauseGamePlay(false);
        }
    }
}
