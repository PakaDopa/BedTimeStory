using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillInfo : MonoBehaviour
{
    TMP_Text text;
    Weapon weapon;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        weapon = FindObjectOfType<Weapon>();
        if(weapon)
        {
            Debug.Log("SUCCESS");
        }
    }

    private void Update()
    {
        if(weapon.CurrSkillCooltime > 0)
        {
            text.enabled = true;
            text.SetText(((int)(weapon.CurrSkillCooltime)).ToString());
        }
            
        else
        {
            text.enabled = false;

        }
    }
}
