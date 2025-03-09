using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Unity.VisualScripting;
using DG.Tweening;
using VFX_SO;
using System.Security.Cryptography;

public class UpgradeSystem : MonoBehaviour
{
    //buttons
    [Header("버튼들")]
    [SerializeField] Button closeButton;
    [SerializeField] Button rerollButton;
    [SerializeField] Button hpRecoverButton;
    [SerializeField] Transform upgradeMenu;

    [SerializeField] NotEnoughMoneyUI notEnoughMoneyUI;

    // 구현해야함.
    [SerializeField] VFXData[] vfxDatas;

    List<UpgradeMenuItem> upgradeMenuItems = new List<UpgradeMenuItem>();
    List<Dictionary<string, object>> dataset;

    [HideInInspector] public UnityEvent onItemLocked;




    

    float hpRecoverRate = 100;
    int hpRecoverCost = 100;
    int rerollCost = 0; 


    [Header("Sound")]
    [SerializeField] SoundEventSO sfx_open;
    [SerializeField] SoundEventSO sfx_close;
    [SerializeField] SoundEventSO sfx_reroll;
    [SerializeField] SoundEventSO sfx_notEnoughGold;
    [SerializeField] SoundEventSO sfx_lock;
    [SerializeField] SoundEventSO[] sfxs_grade;
    
    void OnEnable()
    {
        SoundManager.Instance?.Play(sfx_open,transform.position);
    }

    void OnDisable()
    {
        SoundManager.Instance?.Play(sfx_close,transform.position);
    }

    public void Init()
    {
        
        //데이터베이스 불러오기
        dataset = CSVReader.Read("3. Database/UpgradeSystem");
        foreach(Transform child in upgradeMenu)
        {
            upgradeMenuItems.Add(child.GetComponent<UpgradeMenuItem>());
        }

        rerollButton.onClick.AddListener(Roll);






        // 원래 start에 있던거
        onItemLocked.AddListener(OnItemLocked);

        for (int i = 0; i < upgradeMenuItems.Count; i++)
        {
            var upgradeMenuItem = upgradeMenuItems[i];
            if (upgradeMenuItem.IsLocked)
                continue;

            //버그 일어날 수도 있음. 테스트 필요
            int index = (i * 9) + 6; //offset
    
            var row = dataset[index];

            int grade = Convert.ToInt32(row["Grade"]);
            int value = Convert.ToInt32(row["Value"]);

            upgradeMenuItem.Construct(i, grade, value);
            upgradeMenuItem.EnableVfx(false);
            upgradeMenuItem.OnSelected();
        }

        // notEnoughMoneyUI = GetComponentInChildren<NotEnoughMoneyUI>();
        // notEnoughMoneyUI.Init();


        ChangeRerollCost();
        // SetRecoverButtonText();
    }

    private void Roll()
    {
        if ( PlayerStats.Instance.CanUseGold( rerollCost) == false)
        {
            SoundManager.Instance.Play(sfx_notEnoughGold,transform.position);
            notEnoughMoneyUI.OnInsufficientGold();
            return;
        }

        SoundManager.Instance.Play(sfx_reroll,transform.position);
        PlayerStats.Instance.UseGold(rerollCost);

        int min_gradeOffset = 987654321;
        for (int i = 0; i < upgradeMenuItems.Count; i++)
        {

            //아이템 잠겨있으면 패스!
            var upgradeMenuItem = upgradeMenuItems[i];
            if (upgradeMenuItem.IsLocked)
                continue;
            // csv 파일에 의거
            int typeOffset = i * 9;

            int gradeOffset;
            float randomNumber = UnityEngine.Random.Range(0, 100);
            // 확률
            if (randomNumber < 1) //sss
                gradeOffset = 0;
            else if (randomNumber < 3) //ss
                gradeOffset = 1;
            else if (randomNumber < 6) //s
                gradeOffset = 2;
            else if (randomNumber < 16) //a
                gradeOffset = 3;
            else if (randomNumber < 41) //b
                gradeOffset = 4;
            else if (randomNumber < 66) //c
                gradeOffset = 5;
            else if (randomNumber < 81) //d
                gradeOffset = 6;
            else if (randomNumber < 91) //e
                gradeOffset = 7;
            else                        //f
                gradeOffset = 8;

            min_gradeOffset = (int)MathF.Min(gradeOffset, min_gradeOffset);

            var row = dataset[typeOffset + gradeOffset];
            int grade = Convert.ToInt32(row["Grade"]);
            int value = Convert.ToInt32(row["Value"]);

            upgradeMenuItem.Construct(i, grade, value);
            upgradeMenuItem.EnableVfx(false);
            if(gradeOffset <= 3)
                upgradeMenuItem.AdjustVfx(vfxDatas[gradeOffset]);
            upgradeMenuItem.OnSelected();
        }

        //
        if (min_gradeOffset<sfxs_grade.Length)
        {
            var sfx = sfxs_grade[min_gradeOffset];
            SoundManager.Instance.Play(sfx,transform.position);
        }


        ChangeRerollCost();
    }

    // private void SetRecoverButtonText()
    // {
    //     hpRecoverButton.GetComponentInChildren<TextMeshProUGUI>().
    //         SetText($"HP {hpRecoverRate} recover : {hpRecoverCost}");
    // }

    // public void OnRecoverButtonPressed()
    // {
    //     PlayerStats.Instance.UseGold(hpRecoverCost);
    //     PlayerStats.Instance.Recover(hpRecoverRate);

    //     hpRecoverCost *= 2;

    //     SetRecoverButtonText();
    //     SetButtonsInteractable();
    // }

    // public void OnRerollButtonPressed()
    // {
    //     Roll();

    //     SetButtonsInteractable();
    // }

    // private void SetButtonsInteractable()
    // {
    //     Debug.Log("button check");

    //     if(PlayerStats.Instance.CurrGold >= hpRecoverCost)
    //     {
    //         hpRecoverButton.interactable = true;
    //     }
    //     else
    //     {
    //         hpRecoverButton.interactable = false;
    //     }

    //     if (PlayerStats.Instance.CurrGold >= rerollCost)
    //     {
    //         rerollButton.interactable = true;
    //     }
    //     else
    //     {
    //         rerollButton.interactable = false;
    //     }
    // }

    private void ChangeRerollCost()
    {
        int count = 0;
        foreach(var item in upgradeMenuItems)
        {
            if (item.IsLocked)
                count++;
        }

        if (count == 4) // �� ��׸� reroll�� ���ϰ�
        {
            rerollButton.interactable = false;
            return;
        }
        else
        {
            rerollButton.interactable = true;
        }

        //확률 코스트
        if (count == 0)
        {
            rerollCost = 50;
        }
        else if (count == 1)
        {
            rerollCost = 75;
        }
        else if (count == 2)
        {
            rerollCost = 150;
        }
        else if (count == 3)
        {
            rerollCost = 300;
        }

        rerollButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"<sprite name=\"0\">{rerollCost}");
    }
    
    void OnItemLocked()
    {
        SoundManager.Instance.Play(sfx_lock,transform.position);
        ChangeRerollCost();
    }
}
