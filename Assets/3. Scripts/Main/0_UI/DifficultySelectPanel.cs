using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectPanel : MonoBehaviour
{
    [SerializeField] List<EnemyDataSO> enemyDataSOs = new();

    public void SelectEasyMode()
    {
        foreach(var ed in enemyDataSOs)
            ed.maxHp = 65;
        SceneLoadManager.Instance.Load_MainScene();
    }

    public void SelectNormalMode()
    {
        foreach (var ed in enemyDataSOs)
            ed.maxHp = 100;
        SceneLoadManager.Instance.Load_MainScene();
    }

    public void SelectHardMode()
    {
        foreach (var ed in enemyDataSOs)
            ed.maxHp = 135;
        SceneLoadManager.Instance.Load_MainScene();
    }
}
