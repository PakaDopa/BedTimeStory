using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VFX_SO;
using AssetKits.ParticleImage;

public class UpgradeMenuItem : MonoBehaviour
{
    [Header("파티클 관련 컴포넌트")]
    [SerializeField] ParticleImage lightParticleImage;
    [SerializeField] ParticleImage ringParticleImage;
    [SerializeField] ParticleImage starParticleImage;
    //[SerializeField] ParticleImage impactParticleImage;
    [SerializeField] ShineEffectTest shineEffect;

    [Header("텍스트 관련 컴포넌트")]
    [SerializeField] TextMeshProUGUI txt_grade;
    [SerializeField] TextMeshProUGUI txt_value;

    [Header("버튼 관련 컴포넌트")]
    [SerializeField] Button LockButton;

    [Header("이미지 관련 컴포넌트")]
    [SerializeField] Sprite lockedImage;
    [SerializeField] Sprite unlockedImage;

    [Header("시스템 스크립트")]
    [SerializeField] UpgradeSystem upgradeSystem;

    private int type;
    private float value;

    private bool isLocked = false;
    public bool IsLocked=>isLocked;

    private Sequence seq;

    void Start()
    {
        seq = DOTween.Sequence();
        seq
            .SetAutoKill(false)
            .Append(transform.DOPunchScale(new Vector3(0.125f, 0.125f, 0.125f), 0.25f))
            .SetUpdate(true);
    }

    void SetGradeTxt(int grade)
    {
        switch(grade)
        {
            case 0: //sss
                txt_grade.SetText("SSS");
                break;
            case 1: //ss
                txt_grade.SetText("SS");
                break;
            case 2: //s
                txt_grade.SetText("S");
                break;
            case 3: //a
                txt_grade.SetText("A");
                break;
            case 4: //b
                txt_grade.SetText("B");
                break;
            case 5: //c
                txt_grade.SetText("C");
                break;
            case 6: //d
                txt_grade.SetText("D");
                break;
            case 7: //e
                txt_grade.SetText("E");
                break;
            case 8: //f
                txt_grade.SetText("F");
                break;
        }
    }
    void SetValueTxt(float value)
    {
        if(type == 0)
        {
            txt_value.SetText($"{value}");
        }
        if (type == 1)
        {
            txt_value.SetText($"{value}");
        }
        if (type == 2)
        {
            txt_value.SetText($"{value}sec");
        }
        if (type == 3)
        {
            txt_value.SetText($"{value}sec");
        }

    }

    public void Construct(int type, int grade, float value)
    {
        seq.Restart();
        
        Debug.Log("Construct!");
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

    public void OnSelected(bool isInitSetting=false)
    {
        Debug.Log($"type {type} ,value {value}");

        if (type == 0)
        {
            PlayerStats.Instance.SetAttackPower(value,isInitSetting);
        }
        else if (type == 1)
        {
            PlayerStats.Instance.SetMoveSpeed(value,isInitSetting);
        }
        else if (type == 2)
        {
            PlayerStats.Instance.SetReloadSpeed(value,isInitSetting);
        }
        else if (type == 3)
        {
            PlayerStats.Instance.SetSkillCooltime(value,isInitSetting);
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
        //impactParticleImage.colorOverLifetime = new ParticleSystem.MinMaxGradient(gradient2);
        //impactParticleImage.Play();
        
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

        shineEffect.EnableEffect();
    }
    public void EnableVfx(bool enable)
    {
        lightParticleImage.enabled = enable;
        ringParticleImage.enabled = enable;
        starParticleImage.enabled = enable;
    }
}
