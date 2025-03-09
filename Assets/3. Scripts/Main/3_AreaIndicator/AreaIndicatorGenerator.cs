using System.Collections;
using System.Collections.Generic;
using GameUtil;
using UnityEngine;

public class AreaIndicatorGenerator : DestroyableSingleton<AreaIndicatorGenerator>
{
    [SerializeField] ConeAreaIndicator prefab_cone;
    [SerializeField] CircleAreaIndicator prefab_circle;
    [SerializeField] SquareAreaIndicator prefab_square;


    Vector3 offset =  new Vector3(0,0.11f,0);

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

    Vector3 InitPos(Vector3 inputPos)
    {
        return inputPos.WithFloorHeight() + offset;
    }


    public ConeAreaIndicator GetCone(Enemy enemy, Vector3 initPos, Vector3 dir, float duration, float radius, float angle)
    {
        dir = dir.WithFloorHeight().normalized; // 회전 벡터는 보통 정규화해서 사용
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, dir);

        ConeAreaIndicator  cone = Instantiate(prefab_cone.gameObject, InitPos( initPos) , rot ).GetComponent<ConeAreaIndicator>();
        cone.Init(enemy, duration, radius, angle);

        return cone;

    }

    public CircleAreaIndicator GetCircle(Enemy enemy, Vector3 initPos, float duration, float radius)
    {
        
        CircleAreaIndicator  circle = Instantiate(prefab_circle.gameObject, InitPos( initPos) , Quaternion.identity ).GetComponent<CircleAreaIndicator>();
        circle.Init(enemy, duration, radius);

        return circle;
    }

    public SquareAreaIndicator GetSquare(Enemy enemy, Vector3 initPos, Vector3 dir, float duration, float width, float height)
    {
        dir = dir.WithFloorHeight().normalized; // 회전 벡터는 보통 정규화해서 사용
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, dir);


        SquareAreaIndicator  square = Instantiate(prefab_square.gameObject, InitPos( initPos) , rot ).GetComponent<SquareAreaIndicator>();
        square.Init(enemy, duration, width, height);

        return square;
    }
}
