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
        Aim,        //���ӻ��� (���ӻ���->���� ���� �Ļ�)
        Attack,     //���ݻ���
        Hitted,     //�ǰݻ���
        Dead,       //��� -> ���ӳ�
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



    public Status playerStatus { get; set; }
    public AimState aimState {get;set;}


    
    private int currGold = 100000;
    public int CurrGold => currGold;

    public bool isAlive => currHP>0;

    private float attackPower = 5;
    private float moveSpeed = 3;
    private float reloadSpeed = 3;
    private float skillCooltime = 30;

    public float AttackPower => attackPower;
    public float MoveSpeed => moveSpeed;
    public float ReloadSpeed => reloadSpeed;
    public float SkillCooltime => skillCooltime;

    [HideInInspector] public UnityEvent<int,int,int> onGoldChanged = new();   //p0 : amount , p1: before, p2: after

    [HideInInspector] public UnityEvent<float,float, float> onHpChanged = new();    // p0: before, p1 :after, p2 max
    [HideInInspector] public UnityEvent<float,float, float> onRpChanged = new();    // p0: before, p1 :after, p2 max

    //=============================================================================

    public bool CanRun()
    {
        if (currStamina >= sps * Time.deltaTime)
        {
            return true;
        }

        //
        return false;
    }

    public void SetRun()
    {
        playerStatus = Status.Run;
        lastRunTime = Time.time;
    }

    IEnumerator StaminaRoutine()
    {
        WaitForFixedUpdate wffu = new();
        WaitUntil wu = new(()=>GamePlayManager.isGamePlaying);

        yield return new WaitUntil(()=>isAlive);
        while( GamePlayManager.gameFinished ==false)
        {
            //
            if(GamePlayManager.isGamePlaying ==false)
            {
                yield return wu;
            }
            
            // 달리기 상태일때는 스태미나 감소
            if( playerStatus == Status.Run)
            {
                UpdateStamina(-sps * Time.fixedDeltaTime);
            }
            // 휴식하면 게이지 참
            else
            {
                //
                if ( canRegenStamina )
                {
                    UpdateStamina(staminaRegenPerSeconds * Time.fixedDeltaTime);
                }
            }

            yield return wffu;
        }
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

    public void SetAttackPower(float value)
    {
        attackPower = value;
    }

    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }
    public void SetReloadSpeed(float value)
    {
        reloadSpeed = value;
    }
    public void SetSkillCooltime(float value)
    {
        skillCooltime = value;
    }

    public void Init()
    {
        ResetInfo();
    }
    void ResetInfo()
    {
        playerStatus = Status.Idle;

        currHP = maxHP;
        currStamina = maxStamina;
        currGold = 0;
        // attackPower = 5;        //  UpgradeSystem에 의해 세팅
        // moveSpeed = 3;          //  UpgradeSystem에 의해 세팅
        // reloadSpeed = 3;        //  UpgradeSystem에 의해 세팅
        // skillCooltime = 30;     //  UpgradeSystem에 의해 세팅

        StartCoroutine(StaminaRoutine());
    }

    void Die()
    {
        Player.Instance.playerCollider.enabled = false;
        GamePlayManager.Instance.GameOver();
    }
}
