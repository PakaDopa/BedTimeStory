using System.Collections;
using System.Collections.Generic;
using GameUtil;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
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




    public Status playerStatus { get; set; }
    public AimState aimState {get;set;}


    [SerializeField] public float currHP;
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

    [HideInInspector] public UnityEvent<float,float> onHpChanged = new();

    //=============================================================================

    public void TakeDamage(float amount)
    {
        if( isAlive == false)
        {
            return;
        }
        
        currHP = Mathf.Clamp(currHP - amount, 0, maxHP);

        onHpChanged.Invoke(currHP,maxHP);
        
        GameManager.Instance.currGamePlayInfo.totalDamageTaken +=amount; 
        SoundManager.Instance.OnPlayerDamaged(Player.Instance.T.position);
        
        if (currHP <= 0)
        {
            Die();
        } 
    }

    public void Recover(float amount)
    {
        currHP = Mathf.Clamp(currHP + amount, 0, maxHP);
        
        onHpChanged.Invoke(currHP,maxHP);
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
        currGold = 0;
        // attackPower = 5;        //  UpgradeSystem에 의해 세팅
        // moveSpeed = 3;          //  UpgradeSystem에 의해 세팅
        // reloadSpeed = 3;        //  UpgradeSystem에 의해 세팅
        // skillCooltime = 30;     //  UpgradeSystem에 의해 세팅
    }

    void Die()
    {
        Player.Instance.playerCollider.enabled = false;
        GamePlayManager.Instance.GameOver();
    }
}
