using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    void Awake()
    {
        Init();
    }
    


    public override void Init()
    {
        InitSoundSetting();
    }



    //===================================================================================================================
    #region 사운드 세팅 
    
    [Space(40)]
    [Header("볼륨 세팅")]
    [SerializeField]  AudioMixer mixer;

    float minMixerValue = -40;
    float maxMixerValue = 0;
    float diff_mixerValue => maxMixerValue - minMixerValue;

    public float maxSettingValue = 10;
    public float minSettingValue = 0;
    

    [Range(-80,0)] public float master = 0;
    [Range(-80,0)] public float bgm = 0;
    [Range(-80,0)] public float sfx = 0;
    
    bool isMute_bgm;
    bool isMute_sfx;

    //====================================
    void InitSoundSetting()
    {
        // soundSetting = GameManager.Instance.playerData.soundSetting;
        mixer?.GetFloat(nameof(master), out master);
        mixer?.GetFloat(nameof(bgm), out bgm);
        mixer?.GetFloat(nameof(sfx), out sfx);
    }


    public void SetMaster(float settingValue)
    {
        float mixerValue = ChangeToMixerValue(settingValue);

        CheckMute();
        
        master = mixerValue;
        mixer?.SetFloat(nameof(master), master);
    }

    public void SetBGM(float settingValue)
    {
        CheckMute();
        
        float mixerValue = ChangeToMixerValue(settingValue);

        bgm = mixerValue;
        mixer?.SetFloat(nameof(bgm), bgm);
    }

    public void SetSFX(float settingValue)
    {
        CheckMute();
        
        float mixerValue = ChangeToMixerValue(settingValue);

        sfx = mixerValue;
        mixer?.SetFloat(nameof(sfx), sfx);
    }

    public float GetSettingValue_Master()
    {
        return ChangeToSettingValue( master);
    }
    public float GetSettingValue_BGM()
    {
        return ChangeToSettingValue( bgm );
    }

    public float GetSettingValue_SFX()
    {
        return ChangeToSettingValue( sfx );
    }

    //=========================================================================

    float ChangeToMixerValue(float settingValue)
    {
        settingValue = System.Math.Clamp(settingValue, minSettingValue, maxSettingValue);

        float ret = minMixerValue  + diff_mixerValue * settingValue / maxSettingValue;
        return ret;        
    }

    float ChangeToSettingValue(float mixerValue)
    {
        float ratio = (mixerValue - minMixerValue) / diff_mixerValue;
        
        float ret = ratio * maxSettingValue;
        return ret;  
    }


    void CheckMute()
    {
        isMute_bgm  = master == minMixerValue || bgm == minMixerValue;
        isMute_sfx  = master == minMixerValue || sfx == minMixerValue;
    }





    #endregion
}
