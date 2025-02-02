using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectPanel : MonoBehaviour
{
    public void SelectEasyMode()
    {
        GameManager.Instance.StartNewGame( Difficulty.Easy);
    }

    public void SelectNormalMode()
    {
        GameManager.Instance.StartNewGame( Difficulty.Normal);
    }

    public void SelectHardMode()
    {
        GameManager.Instance.StartNewGame( Difficulty.Hard);
    }
}
