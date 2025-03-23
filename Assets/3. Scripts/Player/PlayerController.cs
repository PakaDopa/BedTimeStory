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
    private int sfxIdx = 0;


    //=======================================================================
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

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if(GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        
        // 플레이어 상태
        SetBodyState(); // 조준 설정
        SetLegState();  // 인풋에 따라 하체 상태 설정
        
        // 움직임 & 애니메이션 
        RotateAndMove(legState,bodyState);

        // 무기
        ControlWeapon(bodyState);       

        // 스탯
        stats.OnUpdate(legState);

        // 카메라
        playerCamera.ControlCameraPosition(legState,bodyState);
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


    private void RotateAndMove(PlayerLegState currLegState, PlayerBodyState currBodyState)
    {
        Vector2 moveVector = input.playerMoveVector;

        // 회전 적용
        playerMove.Rotate(currLegState, currBodyState, moveVector, playerCamera.t_mainCam.eulerAngles.y);

        //
        float targetSpeed = stats.GetMovementSpeed(currLegState,currBodyState);
        playerMove.Move(currLegState, currBodyState, moveVector, targetSpeed );


        // 애니메이션 
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


    // 
    public void SetMouseSense(float value)
    {
        mouseSense = Mathf.Clamp(value, mouseSense_min, mouseSense_max);
        mouseSense = (float)System.Math.Round(mouseSense,2);

        LocalDataManager.SetSense( mouseSense );
    }


    #region ===Weapon===
    void EnterAimState()
    {
        SoundManager.Instance.Play(aimSfx, Player.Instance.T.position);

        crossHairUI.SetActive(true);
        playerCamera.OnAim(true);
    }

    void ExitAimState()
    {
        crossHairUI.SetActive(false);
        playerCamera.OnAim(false);
    }


    void ControlWeapon(PlayerBodyState bodyState )
    {  
        bool isAiming = bodyState == PlayerBodyState.Aim;
        bool toFire = input.fire;
        bool toUseSkill = input.useSkill;
        bool toReload = input.reload;


        if( weapon.TryShoot(isAiming,toFire))
        {
            playerCamera.ApplyRecoil();  //
        }

        if( weapon.TryUseSkill(isAiming,toUseSkill))
        {
            playerCamera.ApplyRecoil(1.5f);
        }

        if( weapon.TryReload(isAiming,toReload,toFire))
        {

        }
    }
    #endregion



    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            var sfx = sfxs_footstep[sfxIdx++];
            SoundManager.Instance.Play(sfx, Player.Instance.T.position);
            
            if (sfxs_footstep.Length <= sfxIdx)
                sfxIdx = 0;
        }
    }


}
