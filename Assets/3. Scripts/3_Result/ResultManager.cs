using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] VictoryPanel victoryPanel;
    [SerializeField] GameOverPanel gameOverPanel;




    [SerializeField] 
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>SceneLoadManager.Instance.initialized);
        CurrGamePlayInfo currGamePlayInfo = GameManager.Instance.currGamePlayInfo;

        if(currGamePlayInfo==null)
        {
            SceneLoadManager.Instance.Load_Lobby();
        }
        else
        {
            //
            if(currGamePlayInfo.cleared)
            {
                victoryPanel.Open(currGamePlayInfo.towerHpRatio);
            }
            else
            {
                gameOverPanel.Open();
            }
        }
    }





}
