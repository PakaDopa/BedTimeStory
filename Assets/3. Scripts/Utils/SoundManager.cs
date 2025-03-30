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

    float minMixerValue = -25;
    float maxMixerValue = 0;
    float diff_mixerValue => maxMixerValue - minMixerValue;

    public float maxSettingValue = 10;
    public float minSettingValue = 0;
    

    [Range(-80,0)] public float master = 0;
    [Range(-80,0)] public float bgm = 0;
    [Range(-80,0)] public float sfx = 0;
    
    [SerializeField] bool isMute_bgm;
    [SerializeField] bool isMute_sfx;

    //====================================

    void Start()
    {
        Init();     // audiomixer가 awake에서는 초기화가 안됨.
    }
    void InitSoundSetting()
    {
        float localMaster = LocalDataManager.GetMaster();
        float localBgm = LocalDataManager.GetBgm();
        float localSfx = LocalDataManager.GetSfx();
        // Debug.Log($" {localMaster} / {localBgm} / {localSfx}");
         
        
        master = ChangeToMixerValue(localMaster);        
        bgm = ChangeToMixerValue(localBgm);    
        sfx = ChangeToMixerValue(localSfx);    

        // Debug.Log($" {master} / {bgm} / {sfx}");

        mixer.SetFloat(nameof(master), master);
        mixer.SetFloat(nameof(bgm), bgm);
        mixer.SetFloat(nameof(sfx), sfx);

        // Debug.Log(mixer);
        
    
        // mixer?.GetFloat(nameof(master), out master);
        // mixer?.GetFloat(nameof(bgm), out bgm);
        // mixer?.GetFloat(nameof(sfx), out sfx);
    }


    public void SetMaster(float settingValue)
    {
        float mixerValue = ChangeToMixerValue(settingValue);

        
        master = mixerValue;
        mixer?.SetFloat(nameof(master), master);

        LocalDataManager.SetMaster(settingValue);
        CheckMute();


        if( currBgmSource!=null)
        {
            currBgmSource.mute = isMute_bgm;
        }
    }

    public void SetBGM(float settingValue)
    {
        
        
        float mixerValue = ChangeToMixerValue(settingValue);

        bgm = mixerValue;
        mixer?.SetFloat(nameof(bgm), bgm);

        LocalDataManager.SetBgm(settingValue);
        CheckMute();
        if( currBgmSource!=null)
        {
            currBgmSource.mute = isMute_bgm;
        }
    }

    public void SetSFX(float settingValue)
    {
        float mixerValue = ChangeToMixerValue(settingValue);

        sfx = mixerValue;
        mixer?.SetFloat(nameof(sfx), sfx);


        LocalDataManager.SetSfx(settingValue);
        CheckMute();
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

    void PlayRandomSFX(SoundEventSO[] targetArr, Vector3 initPos)
    {
        if (targetArr.Length<=0)
        {
            return ;
        }
        int  randIdx = Random.Range(0, targetArr.Length);

        SoundEventSO soundEvent = targetArr[randIdx];

        var sfx = Get();
        sfx?.Play(soundEvent, initPos);

        return ;
    }


    
    // 적
    [SerializeField] SoundEventSO[] sfxs_enemyHit;
    public void OnEnemyDamaged( Vector3 initPos )
    {
        PlayRandomSFX(sfxs_enemyHit, initPos);
    }

    [SerializeField] SoundEventSO[] sfxs_enemyDeath;
    [SerializeField] SoundEventSO sfx_enemyDeath_add;
    public void OnEnemyDeath( Vector3 initPos )
    {
        PlayRandomSFX(sfxs_enemyDeath, initPos);
        Play(sfx_enemyDeath_add,initPos);
    }

    // 타워
    [SerializeField] SoundEventSO[] sfxs_towerHit;

    public void OnTowerDamaged(Vector3 initPos)
    {
        PlayRandomSFX(sfxs_towerHit, initPos);
    }

    //플레이어 피격
    [SerializeField] SoundEventSO[] sfxs_playerHit;
    public void OnPlayerDamaged(Vector3 initPos)
    {
        PlayRandomSFX(sfxs_playerHit, initPos);
    }

    // 아이템
    [SerializeField] SoundEventSO sfx_coin;
    [SerializeField] SoundEventSO sfx_heart;
    public void OnPickupCoin(Vector3 initPos)
    {
        Play(sfx_coin,initPos);
    }

    public void OnPickupHeart(Vector3 initPos)
    {
        Play(sfx_heart,initPos);
    }



    //UI
    [SerializeField] SoundEventSO sfx_hover;
    [SerializeField] SoundEventSO sfx_click;


    public void OnBtnClick()
    {
        Play(sfx_click,transform.position);
    }

    public void OnBtnHover()
    {
        Play(sfx_hover,transform.position);
    }



    #endregion 






    #region Pool
    [Header("Pool Setting")]
    [SerializeField] SfxObject prfab_sfx;
    [SerializeField] SfxObject prfab_bgm;
    AudioSource currBgmSource;
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

    public void Play( SoundEventSO soundData, Vector3 initPos)
    {
        if( isMute_sfx || soundData == null)
        {
            return;
        }
        
        var sfx = Get();
        sfx?.Play(soundData, initPos);
    }

    public void PlayBgm(SoundEventSO soundData)
    {
        currBgmSource = Instantiate(prfab_bgm).GetComponent<AudioSource>();
        currBgmSource.clip = soundData.clip;
        currBgmSource.loop = true;
        currBgmSource.Play();
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
