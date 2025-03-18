using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using TMPro;

public class OptionPanel : MonoBehaviour
{
    bool initialized = false;
    
    
    [Header("Volume Setting")]
    [SerializeField] Slider slider_master;
    [SerializeField] Slider slider_bgm;
    [SerializeField] Slider slider_sfx;

    [SerializeField] TextMeshProUGUI text_masterValue;
    [SerializeField] TextMeshProUGUI text_bgmValue;
    [SerializeField] TextMeshProUGUI text_sfxValue;



    
    [Header("Mouse Setting")]
    [SerializeField] Slider slider_sensitivity;
    [SerializeField] TextMeshProUGUI text_sensitivity;





    [Header("btns")]
    [SerializeField] Button btn_close;
    [SerializeField] Button btn_lobby;
    [SerializeField] Button btn_retry;

    
    public void Init()
    {
        InitSoundSetting();
        InitMouseSensitivity();
        
        
        //
        btn_close.onClick.AddListener(Close);
        btn_lobby.onClick.AddListener(ToMainMenu);
        btn_retry.onClick.AddListener(Retry);


        initialized = true;
    }

    //===========================================================

    void OnEnable()
    {
        if( initialized ==false)
        {
            Init();
        }

        OnOpen_Sound();
        OnOpen_Mouse();
    }


    public void Close()
    {
        UIManager.Instance.EnablePanel(gameObject, false, PanelState.None);
    }
    
    //==================================================================================================
    #region Sound Setting 

    void InitSoundSetting()
    {
        float minValue = SoundManager.Instance.minSettingValue;
        float maxValue = SoundManager.Instance.maxSettingValue;
        
        slider_master.minValue = minValue;
        slider_bgm.minValue = minValue;
        slider_sfx.minValue = minValue;

        slider_master.maxValue = maxValue;
        slider_bgm.maxValue = maxValue;
        slider_sfx.maxValue = maxValue;

        slider_master.onValueChanged.AddListener(SetMaster);
        slider_bgm.onValueChanged.AddListener(SetBGM);
        slider_sfx.onValueChanged.AddListener(SetSFX);

        text_masterValue.SetText($"{slider_master.value}");
        text_bgmValue.SetText($"{slider_bgm.value}");
        text_sfxValue.SetText($"{slider_sfx.value}");    
    }

    void OnOpen_Sound()
    {
        slider_master.value = SoundManager.Instance.GetSettingValue_Master();
        slider_bgm.value    = SoundManager.Instance.GetSettingValue_BGM();
        slider_sfx.value    = SoundManager.Instance.GetSettingValue_SFX();
    }





    void SetMaster(float value)
    {
        SoundManager.Instance.SetMaster(value);
        text_masterValue.SetText($"{slider_master.value}");
    }

    void SetBGM(float value)
    {
        SoundManager.Instance.SetBGM(value);
        text_bgmValue.SetText($"{slider_bgm.value}");
    }

    void SetSFX(float value)
    {
        SoundManager.Instance.SetSFX(value);
        text_sfxValue.SetText($"{slider_sfx.value}");
    }
    
    
    
    

    #endregion

    //=========================================
    #region Mouse Setting
    void InitMouseSensitivity()
    {
        float minValue = Player.Instance.aimStat.mouseSense_min;
        float maxValue = Player.Instance.aimStat.mouseSense_max;
        
        

        slider_sensitivity.minValue = minValue;
        slider_sensitivity.maxValue = maxValue;

        slider_sensitivity.onValueChanged.AddListener(SetSensitivity);

    }

    void OnOpen_Mouse()
    {
        float currValue = Player.Instance.aimStat.currMouseSense;
        slider_sensitivity.value = currValue;
    }



    void SetSensitivity(float value)
    {
        Player.Instance.aimStat.SetMouseSense(value);
        text_sensitivity.SetText($"{slider_sensitivity.value:0.00}");
    }

    #endregion
    
    //================================
    
    
    public void ToMainMenu()
    {
        SceneLoadManager.Instance.Load_Lobby();
    }

    public void Retry()
    {
        GameManager.Instance.ReTryThisGame();
    }
}
