using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
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

    public void OnMove_Default(float animationBlend, float inputMagnitude)
    {
        if (_hasAnimator==false)
        {
            return;

        }
            

        if (animationBlend < 0.01f)
        {
            animationBlend = 0f;
        } 

        _animator.SetFloat(_animIDSpeed, animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }


    public void OnMove_OnAim(Vector2 normalizedInput)
    {
        if (_hasAnimator==false)
        {
            return;

        }


        _animator.SetFloat(_animIDhSpeed , normalizedInput.x);
        _animator.SetFloat(_animIDvSpeed ,  normalizedInput.y);

    }


    public void OnSetBodyState(PlayerBodyState bodyState, float animSpeed)
    {
        bool isAim = bodyState == PlayerBodyState.Aim;
        _animator.SetBool(_animIDIsAiming, isAim);
        _animator.SetFloat(_animIDanimSpeed, animSpeed);

        if(isAim)
        {
            _animator.SetLayerWeight(1, 1);
            
        }
        else
        {
            _animator.SetLayerWeight(1, 0);
        }
    }
}
