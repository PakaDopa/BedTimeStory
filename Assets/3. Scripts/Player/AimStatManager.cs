using Cinemachine;
using DG.Tweening;
// using System;
using System.Collections;
using UnityEngine;

public class AimStatManager : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;

    public AxisState xAxis, yAxis;

    [Header(" ? ")]
    [SerializeField] Transform camFollowPos;

    [Header(" ? ?? ")]
    [SerializeField] Transform camFollowPosTarget;
    [SerializeField] Transform camFollowPosRunTarget;
    [SerializeField] Transform camFollowPosAimTarget;

    public float mouseSense_min => 0.01f;
    public float mouseSense_max => 2f;


    [SerializeField] float mouseSense;
    public float currMouseSense => mouseSense;

    [Header(" ?  ? ")]
    // [SerializeField] RectTransform crosshairImg;
    [SerializeField] GameObject crossHairUI;

    Sequence seq_shoot;
    [SerializeField] CinemachineVirtualCamera vCam;


    [SerializeField] float originFOV{get;set;} 


    public void Init()
    {
        //setting default position
        camFollowPos.position = camFollowPosTarget.position;


        Manager.EventManager.Instance.AddListener(MEventType.OnShoot, OnShoot);

        mouseSense  = LocalDataManager.GetMouseSense();

        originFOV = vCam.m_Lens.FieldOfView;
    }


    // Update is called once per frame
    void Update()
    {
        //State
        if(GamePlayManager.isGamePlaying ==false)
        {
            return;
        }


        AimMode();

        xAxis.Value += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis.Value -= Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis.Value = Mathf.Clamp(yAxis.Value, yAxis.m_MinValue, yAxis.m_MaxValue);
    }

    public void SetMouseSense(float value)
    {
        mouseSense = Mathf.Clamp(value, mouseSense_min, mouseSense_max);
        mouseSense = (float)System.Math.Round(mouseSense,2);

        LocalDataManager.SetSense( mouseSense );
    }


    private void LateUpdate()
    {
        camFollowPos.localEulerAngles = new Vector3(yAxis.Value, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis.Value, transform.eulerAngles.z);
    }
    private void AimMode()
    {
        // ???? ??? ????
        if (Input.GetKey(KeyCode.Mouse1))
        {
            
            if( (seq_shoot ==null && seq_shoot.IsActive() )==false ) 
            {
                camFollowPos.DOLocalMove(camFollowPosAimTarget.localPosition, 0.25f, false);
            }
            
            OnAim(true);
            return;
        }
        else if(PlayerStats.Instance.playerStatus == PlayerStats.Status.Run)
            camFollowPos.DOLocalMove(camFollowPosRunTarget.localPosition, 0.25f, false);
        else
            camFollowPos.DOLocalMove(camFollowPosTarget.localPosition, 0.25f, false).SetEase(Ease.OutCubic);
        OnAim(false);
    }


    void OnAim(bool isOn)
    {
        PlayerStats.Instance.aimState = isOn? PlayerStats.AimState.Aim : PlayerStats.AimState.None ;
        crossHairUI.SetActive(isOn);
    }


    void OnShoot(MEventType MEventType, Component Sender, System.EventArgs args = null)
    {
        TransformEventArgs tArgs = args as TransformEventArgs;
        float recoil_camera = (float)tArgs.value[0];
        
        
        float recoil_Y= 0.2f;
        float recoil_X= Random.Range(-0.2f,0.2f);
        float recoil_Z = -0.2f;
        
        Vector3 originPos = camFollowPos.position;
        Vector3 targetPos =  originPos + new Vector3( recoil_X,recoil_Y,recoil_Z);

        if(seq_shoot !=null && seq_shoot.IsActive())
        {
            seq_shoot.Kill();
        }
        // Sequence seq = DOTween.Sequence()
        // .Append( camFollowPos.DOMove( targetPos,0.1f))
        // .Append( camFollowPos.DOMove( originPos,0.1f))
        // .Play();

        // float recoil_camera = 0.5f;
        
        float targetFOV = originFOV + recoil_camera ;

        // vCam.m_Lens.FieldOfView = targetFOV;

        Sequence seq = DOTween.Sequence()
        .Append(DOTween.To(() => vCam.m_Lens.FieldOfView, x =>vCam.m_Lens.FieldOfView= x, targetFOV, 0.05f))
        // .Join( camFollowPos.DOMove( targetPos,0.1f))
        .Append(DOTween.To(() => vCam.m_Lens.FieldOfView, x =>vCam.m_Lens.FieldOfView= x, originFOV, 0.05f))
        // .Join( camFollowPos.DOMove( originPos,0.1f))
        .Play();

        seq_shoot = seq;
    }
}
