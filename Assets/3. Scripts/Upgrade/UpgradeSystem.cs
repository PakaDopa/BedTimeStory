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

public class UpgradeSystem : MonoBehaviour
{
    //buttons
    [Header("버튼들")]
    [SerializeField] Button closeButton;
    [SerializeField] Button rerollButton;
    [SerializeField] Button hpRecoverButton;
    [SerializeField] Transform upgradeMenu;

    // 구현해야함.
    [SerializeField] VFXData[] vfxDatas;

    List<UpgradeMenuItem> upgradeMenuItems = new List<UpgradeMenuItem>();
    List<Dictionary<string, object>> dataset;

    [HideInInspector] public UnityEvent onItemLocked;

    float hpRecoverRate = 100;
    int hpRecoverCost = 100;
    int rerollCost = 0; 

    private void Awake()
    {
        //데이터베이스 불러오기
        dataset = CSVReader.Read("3. Database/UpgradeSystem");
        foreach(Transform child in upgradeMenu)
        {
            upgradeMenuItems.Add(child.GetComponent<UpgradeMenuItem>());
        }
    }

    private void Start()
    {
        onItemLocked.AddListener(ChangeRerollCost);

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

        ChangeRerollCost();
        SetRecoverButtonText();
    }
    private void Roll()
    {
        PlayerStats.Instance.UseGold(rerollCost);

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

            var row = dataset[typeOffset + gradeOffset];
            int grade = Convert.ToInt32(row["Grade"]);
            int value = Convert.ToInt32(row["Value"]);

            upgradeMenuItem.Construct(i, grade, value);
            upgradeMenuItem.EnableVfx(false);
            if(gradeOffset <= 3)
                upgradeMenuItem.AdjustVfx(vfxDatas[gradeOffset]);
            upgradeMenuItem.OnSelected();
        }

        ChangeRerollCost();
    }

    private void SetRecoverButtonText()
    {
        hpRecoverButton.GetComponentInChildren<TextMeshProUGUI>().
            SetText($"HP {hpRecoverRate} recover : {hpRecoverCost}");
    }

    public void OnRecoverButtonPressed()
    {
        PlayerStats.Instance.UseGold(hpRecoverCost);
        PlayerStats.Instance.Recover(hpRecoverRate);

        hpRecoverCost *= 2;

        SetRecoverButtonText();
        SetButtonsInteractable();
    }

    public void OnRerollButtonPressed()
    {
        Roll();

        SetButtonsInteractable();
    }

    private void SetButtonsInteractable()
    {
        Debug.Log("button check");

        if(PlayerStats.Instance.CurrGold >= hpRecoverCost)
        {
            hpRecoverButton.interactable = true;
        }
        else
        {
            hpRecoverButton.interactable = false;
        }

        if (PlayerStats.Instance.CurrGold >= rerollCost)
        {
            rerollButton.interactable = true;
        }
        else
        {
            rerollButton.interactable = false;
        }
    }

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

        rerollButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"Reroll <sprite name=\"0\">{rerollCost}");
    }
}
