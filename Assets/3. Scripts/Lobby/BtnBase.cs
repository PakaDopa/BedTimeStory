using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
 

    [SerializeField] float originScale =1f;
    [SerializeField] float targetScale = 1.2f;

    public bool isMouseOver;
    
    // Start is called before the first frame update
    void Start()
    {

    }


    void Update()
    {
        if( isMouseOver )
        {
            transform.localScale = Vector3.one * targetScale;
        }
        else
        {
            transform.localScale = Vector3.one * originScale;
        }

    }


    public void OnPointerEnter(PointerEventData eventData)
    {

        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
   
        isMouseOver = false;
    }
}
    

