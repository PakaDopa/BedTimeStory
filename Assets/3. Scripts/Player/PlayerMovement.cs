using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    Transform t;
    private CharacterController _controller;

    //
    
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;





    [SerializeField] private float _targetRotation = 0.0f;
    [SerializeField] private float _rotationVelocity;
    private float _verticalVelocity;



    //========================================================================================

    public void Init()
    {
        t = transform;
        _controller = GetComponent<CharacterController>(); 
    }



    public void Rotate(PlayerLegState currLegState, PlayerBodyState currBodyState, Vector2 moveVector, float cameraRotationH)
    {
        // 기본 상태에서는 플레이어를 이동방향으로 회전
        if( currBodyState == PlayerBodyState.Default)
        {
            if (moveVector != Vector2.zero)
            {
                // normalise input direction
                Vector3 inputDirection = new Vector3(moveVector.x, 0.0f, moveVector.y).normalized;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +  cameraRotationH;
                float rotation = Mathf.SmoothDampAngle(t.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                t.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
        // 조준 상태의 경우 카메라 방향으로 회전 
        else if (currBodyState == PlayerBodyState.Aim)
        {
            _targetRotation = cameraRotationH;
            float rotation = Mathf.SmoothDampAngle(t.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            t.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    public void Move(PlayerLegState currLegState, PlayerBodyState currBodyState, Vector2 moveVector, float targetSpeed)
    {
        // move the player
        Vector3 targetDirection= Vector3.zero;
        if( currBodyState== PlayerBodyState.Default)
        {
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        }
        else if(currBodyState == PlayerBodyState.Aim)
        {
            targetDirection = t.forward *  moveVector.y + t.right * moveVector.x;
        }
        _controller.Move(targetDirection.normalized * (targetSpeed  * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }




}
