using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;


public class PlayerCamera : MonoBehaviour
{
    Transform _t_mainCam;
    public  Transform t_mainCam => _t_mainCam;
    [SerializeField] CinemachineVirtualCamera _vCam;


    [SerializeField] float originFOV{get;set;} 


    [Header("Camera rotation Setting")]
    public Transform t_CinemachineCameraTarget;
    public float TopClamp = 3f;          
    public float BottomClamp = -3f;
    
    public bool LockCameraPosition = false;
    private const float _threshold = 0.01f;

    [SerializeField] private float _cinemachineTargetYaw;
    [SerializeField] private float _cinemachineTargetPitch;


    [Header("Cemera Pos Setting")]
    [SerializeField] Cinemachine3rdPersonFollow _3rdPersonFollow;
    [SerializeField] Vector3 currCamOffset; 
    [SerializeField] Vector3 targetCamOffset;
    [SerializeField]Vector3 vCamOffset_default = new Vector3(0.4f,0,-1.4f);
    [SerializeField]Vector3 vCamOffset_aim = new Vector3(0.6f,0,0.6f);
    [SerializeField]Vector3 camOffsetModifier_dash = new Vector3(0, 0,-1.6f);
    [SerializeField] float switchDuration = 0.15f;




    Sequence seq_shoot;
    WaitForEndOfFrame wfuf = new();

    //===================================================================================================

    public void Init()
    {
        _t_mainCam =  GameObject.FindGameObjectWithTag("MainCamera").transform;
        _3rdPersonFollow = _vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        
        originFOV = _vCam.m_Lens.FieldOfView;
        _cinemachineTargetYaw = t_CinemachineCameraTarget.rotation.eulerAngles.y;
        
        

        Manager.EventManager.Instance.AddListener(MEventType.OnShoot, OnShoot);
    }

    /// <summary>
    /// 마우스 움직임에 따라 카메라 회전 
    /// </summary>
    public void ControlCameraRotation(Vector2 mouseMoveVector,float mouseSense)
    {

            // if there is an input and camera position is not fixed
        if (mouseMoveVector.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;

            _cinemachineTargetYaw += mouseMoveVector.x *  mouseSense ;
            _cinemachineTargetPitch -= mouseMoveVector.y  *  mouseSense ;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        t_CinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch,  _cinemachineTargetYaw, 0.0f);
    }

    /// <summary>
    /// 현재 상태에 따라 카메라 위치 조정 - 기본, 대쉬, 에임 
    /// </summary>
    public void ControlCameraPosition(PlayerLegState legState, PlayerBodyState bodyState)
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



    /// <summary>
    /// 사격시 반동 ( 카메라에 임의의 힘 추가 )
    /// </summary>
    public void ApplyRecoil(float power = 0.5f)
    {
        
        // _cinemachineTargetYaw += Random.Range(-0.5f,0.5f);
        // _cinemachineTargetPitch -= 0.5f ;
        StartCoroutine(  RecoilRoutine( Random.Range(-power,power),power ,0.1f) );
    }



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

    public void OnAim(bool isOn)
    {
        if( isOn)
        {
            targetCamOffset = vCamOffset_aim;
        }
        else
        {
            targetCamOffset = vCamOffset_default;
        }
        
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










    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
