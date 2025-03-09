using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;


    void Start()
    {
        playerMovement = GetComponentInChildren<PlayerMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        var status = PlayerStats.Instance.playerStatus;
        float animationSpeed = 1/ playerMovement .behaivourSpeedMultiplier;

        // Debug.Log(animationSpeed    );
        switch(status)
        {
            case PlayerStats.Status.Idle:
                animator.SetBool("isRunning", false);
                animator.speed = 1;
                break;  
            case PlayerStats.Status.Walk:
                animator.SetBool("isRunning", true);
                animator.speed = animationSpeed ;
                break;
            case PlayerStats.Status.Run:
                animator.SetBool("isRunning", true);
                animator.speed = animationSpeed ;
                break;
            default:
                //�������� ���߿�
                break;
        }
    }
}
