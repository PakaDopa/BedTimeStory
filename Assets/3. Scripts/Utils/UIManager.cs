using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : DestroyableSingleton<UIManager>
{
    [SerializeField] GameObject upgradePanel;
    [SerializeField] GameObject optionPanel;

    private void Update()
    {
        if (GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        
        
        UseUpgradePanel();
        UseOptionPanel();
    }

    private void UseOptionPanel()
    { 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionPanel.SetActive(!optionPanel.activeSelf);

            if(optionPanel.activeSelf)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GameManager.Instance.PauseGamePlay(true);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                GameManager.Instance.PauseGamePlay(false);
            }
        }
    }

    private void UseUpgradePanel()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            upgradePanel.SetActive(!upgradePanel.activeSelf);

            if (upgradePanel.activeSelf)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GameManager.Instance.PauseGamePlay(true);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                GameManager.Instance.PauseGamePlay(false);
            }
        }
    }
}
