using UnityEngine;
using UnityEngine.Playables;

public class SkipCutScene : MonoBehaviour
{
    [SerializeField] PlayableDirector cutScene;
   
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            cutScene.Stop();
    }
}