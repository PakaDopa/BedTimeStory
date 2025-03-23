using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    //


    private float _animationBlend;      // 현재 애니메이션 진행도
    Vector2 blendedVector;                  // 
    public float SpeedChangeRate = 10.0f;       // 애니메이션 교체 속도








    //
    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    int _animIDhSpeed;
    int _animIDvSpeed;
    int _animIDIsAiming;
    int _animIDanimSpeed;

    private Animator _animator;
    private bool _hasAnimator;
    //
    // Update is called once per frame
    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);
    }

    public void Init()
    {
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDhSpeed = Animator.StringToHash("hSpeed");
        _animIDvSpeed = Animator.StringToHash("vSpeed");
        _animIDIsAiming = Animator.StringToHash("isAiming");
        _animIDanimSpeed = Animator.StringToHash("animSpeed");
    }

    //==================================================================================

    public void OnMove_Default(float targetSpeed)
    {
        if (_hasAnimator==false)
        {
            return;

        }
            
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

        if (_animationBlend < 0.01f)
        {
            _animationBlend = 0f;
        } 

        _animator.SetFloat(_animIDSpeed, _animationBlend);
        // _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }


    public void OnMove_OnAim(Vector2 normalizedInput)
    {
        if (_hasAnimator==false)
        {
            return;

        }

        blendedVector = Vector2.Lerp(blendedVector , normalizedInput ,  Time.deltaTime * SpeedChangeRate);

        _animator.SetFloat(_animIDhSpeed , blendedVector.x);
        _animator.SetFloat(_animIDvSpeed ,  blendedVector.y);

    }


    public void OnSetBodyState(PlayerBodyState bodyState, float animSpeed)
    {
        bool isAim = bodyState == PlayerBodyState.Aim;
        if(isAim)
        {
            _animator.SetLayerWeight(1, 1);
            _animator.SetBool(_animIDIsAiming, true);
            _animator.SetFloat(_animIDanimSpeed, animSpeed);
            
        }
        else
        {
            _animator.SetLayerWeight(1, 0);
            _animator.SetBool(_animIDIsAiming, false);
        }
    }
}
