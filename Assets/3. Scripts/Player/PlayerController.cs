using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using TMPro;

public enum PlayerLegState
{
    Idle,
    Walk,
    Run
}

public enum PlayerBodyState
{
    Default,
    Aim,
}


public class PlayerController : MonoBehaviour
{
    public PlayerLegState legState;
    public PlayerBodyState bodyState;
    
    
    PlayerInputManager input;
    PlayerStats stats;
    PlayerCamera playerCamera;
    PlayerMovement  playerMove;
    PlayerAnimation animationController;
    Weapon weapon;
    

    //
    public float mouseSense_min => 0.01f;
    public float mouseSense_max => 2f;

    [SerializeField] float mouseSense;
    public float currMouseSense => mouseSense;


    // UI
    [SerializeField] GameObject crossHairUI;


    // SFX
    [Header("SFX")]
    [SerializeField] SoundEventSO aimSfx;
    [SerializeField] SoundEventSO dashSoundSO;
    [SerializeField] SoundEventSO[] sfxs_footstep;
    [SerializeField] SoundEventSO skillUseSfx;
    [SerializeField] private SoundEventSO[] attackEventSOs;
    [SerializeField] private SoundEventSO reloadEventSo;

    private int footStepSfxIdx = 0;
    int shotSfxIdx = 0;


    //=======================================================================
    private void Start()
    {
        Init();
    }
    
    // 기능들의 초기화를 직접 제어한다. 
    public void Init()
    {
        input = GetComponent<PlayerInputManager>();
        stats = GetComponent<PlayerStats>();
        animationController  = GetComponent<PlayerAnimation>();
        animationController.Init();
        playerCamera = GetComponent<PlayerCamera>();
        playerCamera.Init();
        playerMove = GetComponent<PlayerMovement>();
        playerMove.Init();
        weapon = GetComponentInChildren<Weapon>();
        //
        mouseSense  = LocalDataManager.GetMouseSense();
    }

    // 기능들을 현재 상태에 따라 업데이트한다. 
    private void Update()
    {
        if(GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        
        // 플레이어 상태 설정
        SetBodyState(); // 조준 상태
        SetLegState();  // 인풋에 따라 하체 상태
        
        // 하위 기능 설정
        RotateAndMove(legState,bodyState);  // 움직임 & 애니메이션 
        ControlWeapon(bodyState);           // 무기
        stats.OnUpdate(legState);           // 스탯
        playerCamera.ControlCameraPosition(legState,bodyState); // 카메라
    }

    private void LateUpdate()
    {
        if(GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        playerCamera.ControlCameraRotation( input.mouseMoveVector, mouseSense);
    }

    //===========================================================================================================

    // 다리 상태 ( 가만히, 걷기, 뛰기 )
    void SetLegState()
    {
        // 입력이 있는 경우, 키를 입력한 방향으로 회전
        if (input.playerMoveVector != Vector2.zero)
        {
            if (input.dash && stats.CanRun())
            {
                legState = PlayerLegState.Run;
                
                //첫대쉬 판정
                if (input.firstDash)
                {
                    SoundManager.Instance.Play(dashSoundSO,Player.Instance.T.position);
                }
            }
            else
            {
                legState = PlayerLegState.Walk;
            }    
        }
        // 입력이 없는 경우, 속도 0
        else
        {
            legState = PlayerLegState.Idle;
        }
    }

    // 상체 상태 ( 기본, 조준 ) // 추후 사격도 추가할까 생각중
    void SetBodyState()
    {
        // 입력이 있는 경우, 키를 입력한 방향으로 회전
        if (input.aim)
        {
            bodyState = PlayerBodyState.Aim;
            if(input.firstAim)
            {
                EnterAimState();
            }
        }
        // 입력이 없는 경우, 속도 0
        else
        {
            bodyState = PlayerBodyState.Default;
            if (input.finishAim)
            {
                ExitAimState();
            }
        }

        float animSpeed = stats.currMoveSpeedRatio;
        animationController.OnSetBodyState(bodyState,animSpeed);
    }

    // 회전, 이동 - 애니메이션까지 
    private void RotateAndMove(PlayerLegState currLegState, PlayerBodyState currBodyState)
    {
        Vector2 moveVector = input.playerMoveVector;

        // 회전 적용
        playerMove.Rotate(currLegState, currBodyState, moveVector, playerCamera.t_mainCam.eulerAngles.y);

        //
        float targetSpeed = stats.GetMovementSpeed(currLegState,currBodyState); // 다리 상태, 조준 상테에 따라 이동속도가 다름. 
        playerMove.Move(currLegState, currBodyState, moveVector, targetSpeed );


        // 애니메이션 (상태별로)
        if( bodyState== PlayerBodyState.Default)
        {
            // Animation
            animationController.OnMove_Default(targetSpeed);
        }
        else if(bodyState == PlayerBodyState.Aim)
        {
            Vector2 normalizedVector = moveVector.normalized;
            animationController.OnMove_OnAim(normalizedVector);
        }
    }


    // 감도 설정  
    public void SetMouseSense(float value)
    {
        mouseSense = Mathf.Clamp(value, mouseSense_min, mouseSense_max);
        mouseSense = (float)System.Math.Round(mouseSense,2);

        LocalDataManager.SetSense( mouseSense );
    }


    #region ===Weapon===
    // 조준 시작
    void EnterAimState()
    {
        SoundManager.Instance.Play(aimSfx, Player.Instance.T.position); 

        crossHairUI.SetActive(true);    
        playerCamera.OnAim(true);
    }

    // 조준 해제
    void ExitAimState()
    {
        crossHairUI.SetActive(false);
        playerCamera.OnAim(false);
    }

    // 장전, 사격, 스킬 
    void ControlWeapon(PlayerBodyState bodyState )
    {  
        bool isAiming = bodyState == PlayerBodyState.Aim;
        bool toFire = input.fire;
        bool toUseSkill = input.useSkill;
        bool toReload = input.reload;


        if( weapon.TryShoot(isAiming,toFire))
        {
            playerCamera.ApplyRecoil();  //
            PlaySFX_fire();
        }

        if( weapon.TryUseSkill(isAiming,toUseSkill))
        {
            playerCamera.ApplyRecoil(1.5f);     // 사격보다 반동이 쏌
            SoundManager.Instance.Play(skillUseSfx, Player.Instance.T.position);   // 스킬 sfx
        }

        if( weapon.TryReload(isAiming,toReload,toFire))
        {
            SoundManager.Instance.Play(reloadEventSo, Player.Instance.T.position);  // 장전 sfx
        }
    }
    #endregion


    // 발자국 소리 - 애니메이션 이벤트에 의해 자동으로 호출 
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            var sfx = sfxs_footstep[footStepSfxIdx++];
            SoundManager.Instance.Play(sfx, Player.Instance.T.position);
            
            if (sfxs_footstep.Length <= footStepSfxIdx)
                footStepSfxIdx = 0;
        }
    }

    // 사격 sfx
    void PlaySFX_fire()
    {
        var soundData = attackEventSOs[shotSfxIdx ++];
        SoundManager.Instance.Play(soundData, Player.Instance.T.position);

        if (shotSfxIdx  >= attackEventSOs.Length)
            shotSfxIdx  = 0;

    }

}
