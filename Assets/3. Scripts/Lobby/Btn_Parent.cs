using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Btn_Parent : MonoBehaviour
{
 

    // [SerializeField] float originScale =1f;
    // [SerializeField] float targetScale = 1.2f;

    // public bool isMouseOver;
    
    // Start is called before the first frame update
    void Start()
    {
        // originScale = transform.position.x;
        Init();
    }

    public abstract void Init();

    
    // void Update()
    // {
    //     if( isMouseOver )
    //     {
    //         transform.localScale = Vector3.one * targetScale;
    //     }
    //     else
    //     {
    //         transform.localScale = Vector3.one * originScale;
    //     }

    // }


    // public void OnPointerEnter(PointerEventData eventData)
    // {

    //     isMouseOver = true;
    // }

    // public void OnPointerExit(PointerEventData eventData)
    // {
   
    //     isMouseOver = false;
    // }
    

}
