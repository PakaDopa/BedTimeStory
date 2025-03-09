using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // [SerializeField] public float moveSpeed = 3f;
    [HideInInspector] public Vector3 dir;
    float hInput, vInput;
    CharacterController controller;

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    [Header("WalkSound")]
    [SerializeField] SoundEventSO[] soundEventSOs;
    [Header("DashSound")]
    [SerializeField] SoundEventSO dashSoundSO;
    private Coroutine walkSoundCoroutine;
    private int soundIndex = 0;
    private bool isWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        //Ŀ�� �����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        walkSoundCoroutine = StartCoroutine(PlayerWalkSound());
    }
    private void GetDirectionAndMove()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        
        dir = transform.forward * vInput + transform.right * hInput;
        
        if(hInput == 0 && vInput == 0)
        {
            isWalking = false;
            PlayerStats.Instance.playerStatus = PlayerStats.Status.Idle;
        }
        else if(IsRun())
        {
            isWalking = true;
            PlayerStats.Instance.playerStatus = PlayerStats.Status.Run;

            float movementSpeed = PlayerStats.Instance.MoveSpeed;
            controller.Move(dir * (movementSpeed * 2f) * Time.deltaTime);
        }
        else
        {
            isWalking = true;
            PlayerStats.Instance.playerStatus = PlayerStats.Status.Walk;

            float movementSpeed = PlayerStats.Instance.MoveSpeed;
            controller.Move(dir * movementSpeed* Time.deltaTime);
        }
    }
    // Update is called once per frame
    void Update()
    {

        Gravity();
        
        if(GamePlayManager.isGamePlaying==false)
            return;
        

        //첫대쉬 판정
        if (Input.GetKeyDown(KeyCode.LeftShift))
            dashSoundSO.Raise();
            
        GetDirectionAndMove();
        
    }
    private bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask))
            return true;
        return false;
    }
    private bool IsRun() => Input.GetKey(KeyCode.LeftShift);
    private void Gravity()
    {
        if (!IsGrounded())
            velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0)
            velocity.y = -2;

        controller.Move(velocity * Time.deltaTime);
    }
    IEnumerator PlayerWalkSound()
    {
        yield return new WaitUntil(() => isWalking && GamePlayManager.isGamePlaying == true);

        soundEventSOs[soundIndex++].Raise();
        if (soundEventSOs.Length <= soundIndex)
            soundIndex = 0;

        if (PlayerStats.Instance.playerStatus == PlayerStats.Status.Walk)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else if (PlayerStats.Instance.playerStatus == PlayerStats.Status.Run)
        {
            yield return new WaitForSeconds(0.25f);
        }
        else
            yield return null;

        StartCoroutine(PlayerWalkSound());
    }
}
