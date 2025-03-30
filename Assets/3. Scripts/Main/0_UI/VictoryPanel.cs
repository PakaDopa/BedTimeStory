using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] GameObject[] fillStars;


    public void Open(float rate)
    {
        int starCount = 1;
        
        Debug.Log($" 타워 남은체력 {rate}" );

        if( 0.61f<= rate)
        {
            starCount = 3;
        }
        else if( 0.31f<= rate)
        {
            starCount = 2;
            
        }       
        
        for(int i=0;i<fillStars.Length;i++)
        {
            fillStars[i].gameObject.SetActive(i<starCount);
        }


        gameObject.SetActive(true);
    }
}
