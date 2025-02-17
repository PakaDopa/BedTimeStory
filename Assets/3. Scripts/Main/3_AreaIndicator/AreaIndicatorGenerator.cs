using System.Collections;
using System.Collections.Generic;
using GameUtil;
using UnityEngine;

public class AreaIndicatorGenerator : DestroyableSingleton<AreaIndicatorGenerator>
{
    [SerializeField] ConeAreaIndicator prefab_cone;
    

    // IEnumerator Start()
    // {
        // while(true)
        // {
        //     Vector3 randomStartPos = new Vector3( Random.Range(-10,10) , 0.01f, Random.Range(-10,10));
        //     Vector3 randomEndPos  = new Vector3( Random.Range(-10,10) , 0.01f, Random.Range(-10,10));

        //     Vector3 dir = randomEndPos - randomStartPos;
            
        //     GetCone(null, randomStartPos, dir, 1f, 2, 30);

        //     yield return new WaitForSeconds(3f);
        // }

    // }


    public ConeAreaIndicator GetCone(Enemy enemy, Vector3 targetPos, Vector3 dir, float duration, float radius, float angle)
    {
        dir = dir.WithFloorHeight().normalized; // 회전 벡터는 보통 정규화해서 사용
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, dir);

        ConeAreaIndicator  cone = Instantiate(prefab_cone.gameObject, targetPos+ new Vector3(0,0.1f,0) , rot ).GetComponent<ConeAreaIndicator>();
        cone.Init(enemy, duration, radius, angle);

        return cone;

    }
}
