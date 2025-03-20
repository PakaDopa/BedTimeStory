using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    //
    public Vector2 playerMoveVector;

    // 
    public Vector2 mouseMoveVector;

    //
    public bool dash { get; private set;}
    public bool firstDash {get;private set;}

    //
    public bool aim {get; private set;}
    public bool firstAim {get; private set;}

    //
    public bool fire { get; private set;}
    public bool firstFire {get; private set;}


    //
    public bool reload {get; private set;}

    //
    public bool useSkill {get;private set;}


    //================================================================
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        
        
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        playerMoveVector = new Vector2(hInput, vInput);


        float hMouse = Input.GetAxisRaw("Mouse X");
        float vMouse = Input.GetAxisRaw("Mouse Y");
        mouseMoveVector = new Vector2(hMouse, vMouse);


        dash = Input.GetKey(KeyCode.LeftShift);
        firstDash = Input.GetKeyDown(KeyCode.LeftShift);

        aim = Input.GetKey(KeyCode.Mouse1);
        firstAim = Input.GetKeyDown(KeyCode.Mouse1);

        fire = Input.GetKey(KeyCode.Mouse0);
        firstFire = Input.GetKeyDown(KeyCode.Mouse0);

        reload = Input.GetKey(KeyCode.R);

        useSkill = Input.GetKey(KeyCode.Q);

        
    }


}
