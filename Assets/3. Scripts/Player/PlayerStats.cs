using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : Singleton<PlayerStats>
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
    public Status playerStatus { get; set; }
    public float maxHP = 100;
    [SerializeField] public float currHP;
    private int currGold = 100000;
    public int CurrGold => currGold;

    private float attackPower = 5;
    private float moveSpeed = 3;
    private float reloadSpeed = 3;
    private float skillCooltime = 30;

    public float AttackPower => attackPower;
    public float MoveSpeed => moveSpeed;
    public float ReloadSpeed => reloadSpeed;
    public float SkillCooltime => skillCooltime;

    public UnityEvent<int,int,int> onGoldChanged = new();   //p0 : amount , p1: before, p2: after

    public UnityEvent<float,float> onHpChanged = new();

    //=============================================================================
    private void Awake()
    {
        playerStatus = Status.Idle;
        currHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currHP = Mathf.Clamp(currHP - amount, 0, maxHP);

        GameManager.Instance.currGamePlayInfo.totalDamageTaken +=amount; 
        GameEventManager.Instance.onPlayerGetDamage.Invoke();
        if (currHP <= 0)
        {
            Die();
        } 

        onHpChanged.Invoke(currHP,maxHP);
    }

    public void Recover(float amount)
    {
        currHP = Mathf.Clamp(currHP + amount, 0, maxHP);
        GameManager.Instance.currGamePlayInfo.totalHealingDone +=amount; 

        onHpChanged.Invoke(currHP,maxHP);
    }

    public void GetGold(int amount)
    {
        int origin = currGold;
        currGold += amount;

        onGoldChanged.Invoke(amount, origin, currGold);


        GameManager.Instance.currGamePlayInfo.totalGold += amount;
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

    public override void Init()
    {
        ResetInfo();
    }
    void ResetInfo()
    {
        currHP = maxHP;
        currGold = 0;
        attackPower = 5;
        moveSpeed = 3;
        reloadSpeed = 3;
        skillCooltime = 30;
    }

    void Die()
    {
        ResetInfo();
        GamePlayManager.Instance.GameOver();
    }
}
