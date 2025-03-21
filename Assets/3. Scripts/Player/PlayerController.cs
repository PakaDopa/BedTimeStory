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
    
    
    
    Transform t;
    PlayerInputManager input;
    PlayerStats stats;
    PlayerAnimation animationController;
    Weapon weapon;



    [Header("Player")]

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;


    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public Transform t_CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;


    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    [SerializeField] private float _cinemachineTargetYaw;
    [SerializeField] private float _cinemachineTargetPitch;

    // player
    // private float _speed;
    private float _animationBlend;
    Vector2 blendedVector;
    [SerializeField] private float _targetRotation = 0.0f;
    [SerializeField] private float _rotationVelocity;
    private float _verticalVelocity;


    
    private CharacterController _controller;
    private const float _threshold = 0.01f;

    



    //
    [SerializeField] Cinemachine3rdPersonFollow _3rdPersonFollow;
    [SerializeField] Vector3 currCamOffset; 
    [SerializeField] Vector3 targetCamOffset;
    [SerializeField]Vector3 vCamOffset_default = new Vector3(0.4f,0,-1.4f);
    [SerializeField]Vector3 vCamOffset_aim = new Vector3(0.6f,0,0.6f);
    [SerializeField]Vector3 camOffsetModifier_dash = new Vector3(-0.4f, 0,-1.6f);
    [SerializeField] float switchDuration = 0.2f;

    public float mouseSense_min => 0.01f;
    public float mouseSense_max => 2f;

    [SerializeField] float mouseSense;
    public float currMouseSense => mouseSense;

    [SerializeField] GameObject crossHairUI;

    Sequence seq_shoot;
    [SerializeField] Transform t_mainCam;
    [SerializeField] CinemachineVirtualCamera _vCam;


    [SerializeField] float originFOV{get;set;} 

    [SerializeField] SoundEventSO aimSfx;

    
    public void Init()
    {
        t=transform;
        input = GetComponent<PlayerInputManager>();
        stats = GetComponent<PlayerStats>();
        animationController  = GetComponent<PlayerAnimation>();
        animationController.Init();

        weapon = GetComponentInChildren<Weapon>();
    }

    private void Start()
    {
        Init();
        InitCameraSetting();
        
        
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if(GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        


        // 캐릭터
        SetBodyState(); // 조준 설정
        SetLegState();  // 인풋에 따라 하체 상태 설정
        RotateAndMove(legState,bodyState);

        // 무기
        ControlWeapon(bodyState);       

        // 스탯
        stats.OnUpdate(legState);


        // 카메라 ()
        ControlCamera();
    }

    private void LateUpdate()
    {
        if(GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        CameraRotation(bodyState);
    }




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

            ExitAimState();
        }

        float animSpeed = stats.currMoveSpeedRatio;
        animationController.OnSetBodyState(bodyState,animSpeed);
    }


    void Rotate(PlayerLegState currLegState, PlayerBodyState currBodyState)
    {
        // 기본 상태에서는 플레이어를 이동방향으로 회전
        if( currBodyState == PlayerBodyState.Default)
        {
            if (input.playerMoveVector != Vector2.zero)
            {
                // normalise input direction
                Vector3 inputDirection = new Vector3(input.playerMoveVector.x, 0.0f, input.playerMoveVector.y).normalized;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +  t_mainCam.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
        // 조준 상태의 경우 카메라 방향으로 회전 
        else if (currBodyState == PlayerBodyState.Aim)
        {
            _targetRotation = t_mainCam.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }


    private void RotateAndMove(PlayerLegState currLegState, PlayerBodyState currBodyState)
    {
        //
        float targetSpeed = stats.GetMovementSpeed(currLegState,currBodyState);
        // Debug.Log(targetSpeed);
        float inputMagnitude = input.playerMoveVector.magnitude;


        // 회전 적용
        Rotate(currLegState, currBodyState );

        // move the player
        Vector3 targetDirection= Vector3.zero;
        if( currBodyState== PlayerBodyState.Default)
        {
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            // Animation
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            animationController.OnMove_Default(_animationBlend, inputMagnitude);
        }
        else if(currBodyState == PlayerBodyState.Aim)
        {
            targetDirection = transform.forward *  input.playerMoveVector.y + transform.right * input.playerMoveVector.x;
            Vector2 normalizedVector = input.playerMoveVector.normalized;

            blendedVector = Vector2.Lerp(blendedVector , normalizedVector ,  Time.deltaTime * SpeedChangeRate);
            animationController.OnMove_OnAim(blendedVector);
        }
        _controller.Move(targetDirection.normalized * (targetSpeed  * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }





    
    [Header("WalkSound")]
    [SerializeField] SoundEventSO[] soundEventSOs;
    [Header("DashSound")]
    [SerializeField] SoundEventSO dashSoundSO;

    private int soundIndex = 0;

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            var sfx = soundEventSOs[soundIndex++];
            SoundManager.Instance.Play(sfx, Player.Instance.T.position);
            
            if (soundEventSOs.Length <= soundIndex)
                soundIndex = 0;
        }
    }





    #region ===Camera===
    void InitCameraSetting()
    {
        t_mainCam =  GameObject.FindGameObjectWithTag("MainCamera").transform;

        _3rdPersonFollow = _vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        
        originFOV = _vCam.m_Lens.FieldOfView;
        _cinemachineTargetYaw = t_CinemachineCameraTarget.rotation.eulerAngles.y;
        
        mouseSense  = LocalDataManager.GetMouseSense();

        Manager.EventManager.Instance.AddListener(MEventType.OnShoot, OnShoot);
    }

    public void SetMouseSense(float value)
    {
        mouseSense = Mathf.Clamp(value, mouseSense_min, mouseSense_max);
        mouseSense = (float)System.Math.Round(mouseSense,2);

        LocalDataManager.SetSense( mouseSense );
    }

    void OnShoot(MEventType MEventType, Component Sender, System.EventArgs args = null)
    {
        TransformEventArgs tArgs = args as TransformEventArgs;
        float recoil_camera = (float)tArgs.value[0];
        
        if(seq_shoot !=null && seq_shoot.IsActive())
        {
            seq_shoot.Kill();
        }
        float targetFOV = originFOV + recoil_camera ;

        Sequence seq = DOTween.Sequence()
        .Append(DOTween.To(() => _vCam.m_Lens.FieldOfView, x =>_vCam.m_Lens.FieldOfView= x, targetFOV, 0.05f))
        .Append(DOTween.To(() => _vCam.m_Lens.FieldOfView, x =>_vCam.m_Lens.FieldOfView= x, originFOV, 0.05f))
        .Play();

        seq_shoot = seq;
    }

    void ControlCamera()
    {
        if (_3rdPersonFollow == null)
        {
            return;
        }
        
        // shoulder offset 적용
        Vector3 newTargetOffset = vCamOffset_default;
        if( bodyState == PlayerBodyState.Aim)
        {
            newTargetOffset = vCamOffset_aim;
        }

        Vector3 offsetModifier = Vector3.zero;
        if( legState == PlayerLegState.Run )
        {
            offsetModifier = camOffsetModifier_dash;
        }

        targetCamOffset = newTargetOffset + offsetModifier;


        // 카메라 적용
        _3rdPersonFollow.ShoulderOffset= Vector3.SmoothDamp( _3rdPersonFollow.ShoulderOffset, targetCamOffset, ref currCamOffset, switchDuration);
    }

    private void CameraRotation(PlayerBodyState bodyState)
    {

            // if there is an input and camera position is not fixed
        if (input.mouseMoveVector.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
                //Don't multiply mouse input by Time.deltaTime;

                _cinemachineTargetYaw += input.mouseMoveVector.x *  mouseSense ;
                _cinemachineTargetPitch -= input.mouseMoveVector.y  *  mouseSense ;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        t_CinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch,  _cinemachineTargetYaw, 0.0f);
    }

    // 카메라 반동 적용 
    void ApplyRecoil(float power = 0.5f)
    {
        
        // _cinemachineTargetYaw += Random.Range(-0.5f,0.5f);
        // _cinemachineTargetPitch -= 0.5f ;
        StartCoroutine(  RecoilRoutine( Random.Range(-power,power),power ,0.1f) );
    }


    WaitForEndOfFrame wfuf = new();

    IEnumerator RecoilRoutine(float recoilX, float recoliY ,float duration)
    {

        float startYaw = 0f;
        float startPitch = 0f;
        float elapsed = 0;
        while(elapsed < duration)
        {
            float t = elapsed / duration;
            // 선형 보간 (부드럽게 변화)
            float deltaYaw = Mathf.Lerp(0f, recoilX, t);
            float deltaPitch = Mathf.Lerp(0f, recoliY, t);

            _cinemachineTargetYaw += deltaYaw - startYaw;
            _cinemachineTargetPitch -= deltaPitch - startPitch;

            startYaw = deltaYaw;
            startPitch = deltaPitch;

            elapsed += Time.deltaTime;
            yield return wfuf;
        }

        // 마지막 프레임 보정
        _cinemachineTargetYaw += recoilX - startYaw;
        _cinemachineTargetPitch -= recoliY - startPitch;
    }

    #endregion


    #region ===Weapon===
    void EnterAimState()
    {
        SoundManager.Instance.Play(aimSfx, Player.Instance.T.position);

        crossHairUI.SetActive(true);
        targetCamOffset = vCamOffset_aim;
    }

    void ExitAimState()
    {
        crossHairUI.SetActive(false);
        targetCamOffset = vCamOffset_default;
    }


    void ControlWeapon(PlayerBodyState bodyState )
    {  
        bool isAiming = bodyState == PlayerBodyState.Aim;
        bool toFire = input.fire;
        bool toUseSkill = input.useSkill;
        bool toReload = input.reload;


        if( weapon.TryShoot(isAiming,toFire))
        {
            ApplyRecoil();  //
        }

        if( weapon.TryUseSkill(isAiming,toUseSkill))
        {
            ApplyRecoil(1.5f);
        }

        if( weapon.TryReload(isAiming,toReload,toFire))
        {

        }
    }
    #endregion
}
