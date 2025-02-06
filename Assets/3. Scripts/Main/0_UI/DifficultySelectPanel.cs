using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectPanel : MonoBehaviour
{
    public void SelectEasyMode()
    {
        SetDifficultyAndLoadMainScene(Difficulty.Easy);
    }

    public void SelectNormalMode()
    {
        SetDifficultyAndLoadMainScene(Difficulty.Normal);
    }

    public void SelectHardMode()
    {
        SetDifficultyAndLoadMainScene(Difficulty.Hard);
    }


    void SetDifficultyAndLoadMainScene(Difficulty targetDifficulty)
    {
        GameManager.Instance.SetDifficulty( targetDifficulty );
        SceneLoadManager.Instance.Load_MainScene();
    }
}
