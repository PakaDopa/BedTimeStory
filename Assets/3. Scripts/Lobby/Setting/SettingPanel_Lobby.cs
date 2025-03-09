using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPanel_Lobby : MonoBehaviour
{
    bool initialized = false;
    
    

    [Header("Volume Setting")]
    [SerializeField] Slider slider_master;
    [SerializeField] Slider slider_bgm;
    [SerializeField] Slider slider_sfx;

    [SerializeField] TextMeshProUGUI text_masterValue;
    [SerializeField] TextMeshProUGUI text_bgmValue;
    [SerializeField] TextMeshProUGUI text_sfxValue;








    [Header("Etc")]
    [SerializeField] Button btn_close;


    
    //=======================================================================================================
    
    public void Init()
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

        
        
        
        
        
        
        //
        btn_close.onClick.AddListener(Close);
        initialized = true;
    }



    public void Open()
    {
        if( initialized ==false)
        {
            Init();
        }

        slider_master.value = SoundManager.Instance.GetSettingValue_Master();
        slider_bgm.value    = SoundManager.Instance.GetSettingValue_BGM();
        slider_sfx.value    = SoundManager.Instance.GetSettingValue_SFX();





        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }



    //=================================================================
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

}
