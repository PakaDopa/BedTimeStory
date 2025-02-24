using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenuItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt_grade;
    [SerializeField] TextMeshProUGUI txt_value;
    [SerializeField] Button LockButton;

    [SerializeField] Sprite lockedImage;
    [SerializeField] Sprite unlockedImage;

    [SerializeField] UpgradeSystem upgradeSystem;

    private int type;
    private int value;

    private bool isLocked = false;
    public bool IsLocked=>isLocked;

    void SetGradeTxt(int grade)
    {
        

        switch(grade)
        {
            case 0:
                txt_grade.SetText("Legendary");
                break;
            case 1:
                txt_grade.SetText("Epic");
                break;
            case 2:
                txt_grade.SetText("Unique");
                break;
            case 3:
                txt_grade.SetText("Normal");
                break;
            case 4:
                txt_grade.SetText("UnCommon");
                break;
            case 5:
                txt_grade.SetText("Common");
                break;
        }
    }
    void SetValueTxt(int value)
    {
        if(type == 0)
        {
            txt_value.SetText(value.ToString());
        }
        if (type == 1)
        {
            txt_value.SetText(value.ToString());
        }
        if (type == 2)
        {
            txt_value.SetText(value.ToString() + " sec");
        }
        if (type == 3)
        {
            txt_value.SetText((value*10).ToString() + "%");
        }

    }

    public void Construct(int type, int grade, int value)
    {
        this.type = type;
        this.value = value;

        SetGradeTxt(grade);
        SetValueTxt(value);
    }

    public void OnLockButtonPressed()
    {
        isLocked = !isLocked;
        Debug.Log("Lock : " + isLocked);

        if(isLocked)
        {
            LockButton.GetComponent<Image>().sprite = lockedImage;
            upgradeSystem.onItemLocked.Invoke();
        }
        else
        {
            LockButton.GetComponent<Image>().sprite = unlockedImage;
            upgradeSystem.onItemLocked.Invoke();
        }

        LockButton.GetComponent<Image>().SetNativeSize();
    }

    public void OnSelected()
    {
        Debug.Log($"type {type} ,value {value}");

        if (type == 0)
        {
            PlayerStats.Instance.SetAttackPower(value);
            
        }
        else if (type == 1)
        {
            PlayerStats.Instance.SetMoveSpeed(value);
        }
        else if (type == 2)
        {
            PlayerStats.Instance.SetReloadSpeed(value);
        }
        else if (type == 3)
        {
            PlayerStats.Instance.SetSkillCooltime(value);
        }
    }
    public void AdjustVfx(VFXData data)
    {
        //enable controll
        EnableVfx(true);

        //settings
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(data.color, 0.0f),
                new GradientColorKey(data.color, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),  // 처음에는 불투명
            }
        );
        
        lightParticleImage.colorOverLifetime = new ParticleSystem.MinMaxGradient(gradient);
        lightParticleImage.GetComponent<RectTransform>().localScale = data.lightSize;

        Gradient gradient2 = new Gradient();
        gradient2.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(data.color, 0.0f),
                new GradientColorKey(data.color, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),  // 처음에는 불투명
                new GradientAlphaKey(0.0f, 1.0f),
            }
        );
        impactParticleImage.colorOverLifetime = new ParticleSystem.MinMaxGradient(gradient2);
        impactParticleImage.Play();
        
        ringParticleImage.colorOverLifetime = new ParticleSystem.MinMaxGradient(gradient2);
        ringParticleImage.GetComponent<RectTransform>().localScale = data.ringSize;

        Gradient gradient3 = new Gradient();
        gradient3.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(data.color, 0.0f),
                new GradientColorKey(data.color, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),  // 처음에는 불투명
                new GradientAlphaKey(0.0f, 1.0f),
            }
        );

        starParticleImage.colorOverLifetime = new ParticleSystem.MinMaxGradient(gradient2);
        starParticleImage.GetComponent<RectTransform>().localScale = data.starSize;
    }
    public void EnableVfx(bool enable)
    {
        lightParticleImage.enabled = enable;
        ringParticleImage.enabled = enable;
        starParticleImage.enabled = enable;
    }
}
