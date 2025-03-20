using System.Collections;
using System.Collections.Generic;
using GameUtil;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerStats : DestroyableSingleton<PlayerStats>
{
    public enum Status
    {
        Idle,       //��������
        Walk,       //�ȴ»���
        Run,        //�޸��� ���� (�ȴ»��� -> �޸��� ���� �Ļ�)
    }

    public enum AimState
    {
        None,
        Aim
    }


    [Header("InitValue")]
    public float maxHP = 100;
    public float currHP {get;private set;}
    public float maxStamina = 100;
    public float currStamina {get;private set;}
    [SerializeField] float sps = 33;    // 초당 stamina 사용량
    float lastRunTime;
    [SerializeField] float staminaRegenWaitingTime = 1.5f;  // stamina 충전 대기시간
    [SerializeField] float staminaRegenPerSeconds = 20f; // 스테미나 초당 충전량.

    bool canRegenStamina => currStamina < maxStamina && Time.time >= lastRunTime + staminaRegenWaitingTime ;

    public float dashMultiplier = 2f;       // 대시시 이동속도 배수

    public Status playerStatus { get; set; }
    public AimState aimState {get;set;}


    
    private int currGold = 100000;
    public int CurrGold => currGold;

    public bool isAlive => currHP>0;

    [SerializeField] private float attackPower = 5;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float reloadSpeed = 3;
    [SerializeField] private float skillCooltime = 30;

    float attackPower_init ;
    float moveSpeed_init ;
    float reloadSpeed_init;
    float skillCooltime_init;

    public float currMoveSpeedRatio;



    public float AttackPower => attackPower;
    // public float MoveSpeed => moveSpeed;
    public float ReloadSpeed => reloadSpeed;
    public float SkillCooltime => skillCooltime;

    [HideInInspector] public UnityEvent<int,int,int> onGoldChanged = new();   //p0 : amount , p1: before, p2: after

    [HideInInspector] public UnityEvent<float,float, float> onHpChanged = new();    // p0: before, p1 :after, p2 max
    [HideInInspector] public UnityEvent<float,float, float> onRpChanged = new();    // p0: before, p1 :after, p2 max

    //=============================================================================
    public void OnUpdate(PlayerLegState legState)
    {
        // 달리기 상태일때는 스태미나 감소
        if (legState == PlayerLegState.Run)
        {
            lastRunTime = Time.time;

            UpdateStamina(-sps * Time.deltaTime);
        }
        else
        {
            //
            if ( canRegenStamina )
            {
                UpdateStamina(staminaRegenPerSeconds * Time.deltaTime);
            }
        }
    }


    public float GetMovementSpeed(PlayerLegState legState, PlayerBodyState bodyState)
    {
        float multiplier = 1f;
        if( bodyState == PlayerBodyState.Aim)
        {
            multiplier = 0.5f;
        }
        
        
        if (legState == PlayerLegState.Walk)
        {
            float v = moveSpeed * multiplier;
            currMoveSpeedRatio = v /moveSpeed_init;
            return v;
        }
        else if (legState == PlayerLegState.Run)
        {
            float v = moveSpeed * dashMultiplier * multiplier;
            currMoveSpeedRatio = v / moveSpeed_init;
            return v;
        }

        currMoveSpeedRatio = 1;
        return 0;
    }

    public bool CanRun()
    {
        if (currStamina >= sps * Time.deltaTime)
        {
            return true;
        }

        //
        return false;
    }

    void UpdateStamina(float amount)
    {
        float oldValue = currStamina;
        currStamina = Mathf.Clamp( currStamina +=  amount, 0, maxStamina);
        onRpChanged.Invoke(oldValue, currStamina, maxStamina);
    }



    public void TakeDamage(float amount)
    {
        if( isAlive == false)
        {
            return;
        }
        float newValue = Mathf.Clamp(currHP - amount, 0, maxHP);
        onHpChanged.Invoke(currHP,newValue,maxHP);
        currHP = newValue;

        GameManager.Instance.currGamePlayInfo.totalDamageTaken +=amount; 
        SoundManager.Instance.OnPlayerDamaged(Player.Instance.T.position);
        
        if (currHP <= 0)
        {
            Die();
        } 
    }

    public void Recover(float amount)
    {
        float newValue  = Mathf.Clamp(currHP + amount, 0, maxHP);
        onHpChanged.Invoke(currHP,newValue,maxHP);
        currHP = newValue;
        
        GameManager.Instance.currGamePlayInfo.totalHealingDone +=amount; 
        EffectPoolManager.Instance.GetHealAmountText(transform.position.WithPlayerHeadHeight(), amount );
    }

    public void GetGold(int amount)
    {
        int origin = currGold;
        currGold += amount;

        onGoldChanged.Invoke(amount, origin, currGold);
        GameManager.Instance.currGamePlayInfo.totalGold += amount;
    }

    public bool CanUseGold(int amount)
    {
        if (currGold >= amount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UseGold(int amount)
    {
        int origin = currGold;
        if (currGold >= amount)
        {
            currGold -= amount;
            onGoldChanged.Invoke(amount, origin, currGold);
        }
        else
            throw new System.Exception("invalid use of gold!");

        

        //Debug.Log("Currgold : " + currGold);
    }

    public void SetAttackPower(float value,bool isInitSetting)
    {
        attackPower = value;
        if(isInitSetting)
        {
            attackPower_init = value;
        }
    }

    public void SetMoveSpeed(float value,bool isInitSetting)
    {
        moveSpeed = value;
        if(isInitSetting)
        {
            moveSpeed_init = value;
        }
    }
    public void SetReloadSpeed(float value,bool isInitSetting)
    {
        reloadSpeed = value;
        if(isInitSetting)
        {
            reloadSpeed_init = value;
        }
    }
    public void SetSkillCooltime(float value,bool isInitSetting)
    {
        skillCooltime = value;
        if(isInitSetting)
        {
            skillCooltime_init = value;
        }
    }

    public void Init()
    {
        ResetInfo();
    }
    void ResetInfo()
    {
        // playerStatus = Status.Idle;

        currHP = maxHP;
        currStamina = maxStamina;
        currGold = 0;
        // attackPower = 5;        //  UpgradeSystem에 의해 세팅
        // moveSpeed = 3;          //  UpgradeSystem에 의해 세팅
        // reloadSpeed = 3;        //  UpgradeSystem에 의해 세팅
        // skillCooltime = 30;     //  UpgradeSystem에 의해 세팅

        // StartCoroutine(StaminaRoutine());
    }

    void Die()
    {
        Player.Instance.playerCollider.enabled = false;
        GamePlayManager.Instance.GameOver();
    }
}
