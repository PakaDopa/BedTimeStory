using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldInfo : MonoBehaviour
{
    private void Start()
    {
        PlayerStats.Instance.onGoldChanged.AddListener(ChangeGoldUI);
        ChangeGoldUI();
    }

    void ChangeGoldUI()
    {
        GetComponentInChildren<TextMeshProUGUI>().SetText(PlayerStats.Instance.CurrGold.ToString());
    }
}
