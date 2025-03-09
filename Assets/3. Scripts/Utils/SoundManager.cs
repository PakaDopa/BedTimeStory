using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region 싱글톤 세팅 
    
    public bool initialized; 
    private static SoundManager  instance;
    public static SoundManager  Instance
    {
        get
        {
            if (instance == null) 
            {
                instance = FindObjectOfType<SoundManager> ();
            }
            return instance;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad (gameObject);
        if (instance == null ) 
        {
            instance = this;
            Init();
        } 
        else 
        {
            if (instance != this) 
            {
                Destroy (gameObject);
            }
        }
    }


    public void Init()
    {
        t=transform;
        initialized = true;
        
        InitSoundSetting();
        InitPool();
    }

    #endregion

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
        master = ChangeToMixerValue(LocalDataManager.GetMaster());        
        bgm = ChangeToMixerValue(LocalDataManager.GetBgm());    
        sfx = ChangeToMixerValue(LocalDataManager.GetSfx());    

        mixer?.SetFloat(nameof(master), master);
        mixer?.SetFloat(nameof(bgm), bgm);
        mixer?.SetFloat(nameof(sfx), sfx);
        
    
        // mixer?.GetFloat(nameof(master), out master);
        // mixer?.GetFloat(nameof(bgm), out bgm);
        // mixer?.GetFloat(nameof(sfx), out sfx);
    }


    public void SetMaster(float settingValue)
    {
        float mixerValue = ChangeToMixerValue(settingValue);

        CheckMute();
        
        master = mixerValue;
        mixer?.SetFloat(nameof(master), master);

        LocalDataManager.SetMaster(settingValue);
    }

    public void SetBGM(float settingValue)
    {
        CheckMute();
        
        float mixerValue = ChangeToMixerValue(settingValue);

        bgm = mixerValue;
        mixer?.SetFloat(nameof(bgm), bgm);

        LocalDataManager.SetBgm(settingValue);
    }

    public void SetSFX(float settingValue)
    {
        CheckMute();
        
        float mixerValue = ChangeToMixerValue(settingValue);

        sfx = mixerValue;
        mixer?.SetFloat(nameof(sfx), sfx);


        LocalDataManager.SetSfx(settingValue);
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




    #region sound 재생 관련
    

    [SerializeField] SoundEventSO[] sfxs_enemyHit;
    public void OnEnemyHit( Vector3 initPos )
    {
        if ( sfxs_enemyHit.Length <=0)
        {
            return;
        }
        int  randIdx = Random.Range(0, sfxs_enemyHit.Length);

        SoundEventSO soundEvent = sfxs_enemyHit[randIdx];

        var sfx = Get();
        sfx?.Play(soundEvent, initPos);

    }




    #endregion 






    #region Pool
    [Header("Pool Setting")]
    [SerializeField] SfxObject prfab_sfx;
    Transform t;
    [SerializeField] Pool<SfxObject> sfxPools;     // 풀

    void InitPool()
    {

        PoolData poolData = new();
        poolData._name = $"sfx";
        poolData._component = prfab_sfx;
        
        var pool = Pool<SfxObject>.Create( (SfxObject)poolData.Component, poolData.Count, t);
        sfxPools  = pool;

        if (poolData.NonLazy)
        {
            pool.NonLazy();
        }
    }

    SfxObject Get()
    {   
        var sfx = sfxPools.Get();
        return sfx;
    }


    public void Return(SfxObject sfx)
    {
        var pool = sfxPools;
        sfxPools.Take(sfx);
    }



    #endregion
}
