using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Unity.VisualScripting;

public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button rerollButton;
    [SerializeField] Button hpRecoverButton;
    [SerializeField] Transform upgradeMenu;

    [SerializeField] RectTransform[] effectGroup;

    List<UpgradeMenuItem> upgradeMenuItems = new List<UpgradeMenuItem>();
    List<Dictionary<string, object>> dataset;

    [HideInInspector] public UnityEvent onItemLocked;

    float hpRecoverRate = 100;
    int hpRecoverCost = 100;
    int rerollCost = 0; // ó�� roll�� 0��, reroll �ÿ��� �ڽ�Ʈ�� ��

    private void Awake()
    {
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

            int index = -1;
            if(i == 0)
                index = 5;
            else if(i==1)
                index = 9;
            else if (i == 2)
                index = 15;
            else if (i == 3)
                index = 23;

            var row = dataset[index];

            int grade = Convert.ToInt32(row["Grade"]);
            int value = Convert.ToInt32(row["Value"]);

            upgradeMenuItem.Construct(i, grade, value);
            upgradeMenuItem.OnSelected();
        }

        ChangeRerollCost();


        SetRecoverButtonText();
    }
    private void EnableVFX(int offset, int index)
    {
        if (offset > 2)
            return;
        Debug.Log(offset);
        effectGroup[index].GetChild(offset).GetComponent<ParticleSystem>().Play();
        Debug.Log(effectGroup[index].GetChild(offset).GetComponent<ParticleSystem>().isPlaying);
    }
    private void DisenableVFX()
    {
        for(int i = 0; i < effectGroup.Length; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                effectGroup[i].GetChild(j).GetComponent<ParticleSystem>().Stop();
            }
        }
    }
    private void Roll()
    {
        DisenableVFX();
        PlayerStats.Instance.UseGold(rerollCost);

        for (int i = 0; i < upgradeMenuItems.Count; i++)
        {
            var upgradeMenuItem = upgradeMenuItems[i];
            if (upgradeMenuItem.IsLocked)
                continue;

            // Ÿ�� �з� ������
            int typeOffset = i * 6; // A~F���� 6���� ���

            // ��� �з� ������
            int gradeOffset;
            float randomNumber = UnityEngine.Random.Range(0, 100);
            if (randomNumber < 2.5) //A
                gradeOffset = 0;
            else if (randomNumber < 8) //B
                gradeOffset = 1;
            else if (randomNumber < 20) //C
                gradeOffset = 2;
            else if (randomNumber < 60) // D
                gradeOffset = 3;
            else if (randomNumber < 80) //E
                gradeOffset = 4;
            else // (randomNumber < 100)
                gradeOffset = 5;

            var row = dataset[typeOffset + gradeOffset];
            EnableVFX(gradeOffset, i);
            int grade = Convert.ToInt32(row["Grade"]);
            int value = Convert.ToInt32(row["Value"]);

            upgradeMenuItem.Construct(i, grade, value);
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

        rerollButton.GetComponentInChildren<TextMeshProUGUI>().SetText($"Reroll : {rerollCost}");
    }
}
