using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvent_Attack : MonoBehaviour
{
    public bool AbilityActivationTime;
    public bool animationFinished;
    
    // public bool AbilityActivationTime   {get; private set;}
    // public bool animationFinished       {get; private set;}

    
    public void OnStart()
    {
        AbilityActivationTime = false;
        animationFinished = false;
    }

    public void Apply()
    {
        AbilityActivationTime = true;
    }

    public void OnFinish()
    {
        animationFinished = true;
    }

}