using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button btn; 
    [SerializeField] float originScale =1f;
    [SerializeField] float targetScale = 1.2f;

    public bool isMouseOver;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>SoundManager.Instance.initialized);

        btn = GetComponent<Button>();
        btn.onClick.AddListener(SoundManager.Instance.OnBtnClick);
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
        SoundManager.Instance.OnBtnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
   
        isMouseOver = false;
    }


}
    

