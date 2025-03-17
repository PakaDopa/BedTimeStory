using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameEventManager : Singleton<GameEventManager>
{
    public UnityEvent onGameStart = new();   // 메인씬 로드 직후, 필요 데이터가 초기화 된 후


    public UnityEvent onWaveStart = new();   // 웨이브 시작시.
    // public UnityEvent onWaveFinish = new();   // 웨이브 끝 시.




    public override void Init()
    {
        
    }
}
