using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeAreaIndicator : AreaIndicator
{
    [Space(40)]
    [Header("Cone Setting")]
    [SerializeField] int segmentCount  = 18;  
    float radius => param_0;
    float sectorAngle => param_1;
    
    //==========================================================================================

    protected override void DrawOutline()
    {
        if (segmentCount < 1 || sectorAngle <= 0f)
        {
            outlineRenderer.positionCount = 0;
            return;
        }

        // 라인렌더러 기본 세팅
        outlineRenderer.useWorldSpace = false; // 로컬 좌표 사용
        outlineRenderer.startColor = outlineColor;
        outlineRenderer.endColor   = outlineColor;
        outlineRenderer.loop       = false;    

        // 총 (segmentCount + 3)개 위치:
        //   0: center
        //   1..(segmentCount+1): 호
        //   (segmentCount+2): center
        outlineRenderer.positionCount = segmentCount + 3;

        // 0번: 중심
        outlineRenderer.SetPosition(0, Vector3.zero);

        float halfAngle = sectorAngle * 0.5f;
        float startAngle = -halfAngle;
        float endAngle   =  halfAngle;

        // segmentCount+1개 점으로 "호"를 만든다.
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float currentAngleDeg = Mathf.Lerp(startAngle, endAngle, t);
            float currentAngleRad = Mathf.Deg2Rad * currentAngleDeg;

            float x = Mathf.Sin(currentAngleRad) * radius;
            float z = Mathf.Cos(currentAngleRad) * radius;

            // lineRenderer에서 i+1 인덱스에 세팅
            outlineRenderer.SetPosition(i + 1, new Vector3(x, 0f, z));
        }

        // 마지막 점: 다시 중앙
        outlineRenderer.SetPosition(segmentCount + 2, Vector3.zero);

    }

    protected override void GenerateSectorMesh(Mesh targetMesh, float progress)
    {
        if (targetMesh == null || segmentCount < 1 || sectorAngle <= 0f)
        {
            return;
        }

        float currRadius = Mathf.Lerp(0f, radius, progress);



        Vector3[] vertices = new Vector3[segmentCount + 2];
        vertices[0] = Vector3.zero; // 중심

        float halfAngle = sectorAngle * 0.5f;
        float startAngle = -halfAngle;
        float endAngle   =  halfAngle;

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float angleDeg = Mathf.Lerp(startAngle, endAngle, t);
            float rad = Mathf.Deg2Rad * angleDeg;

            float x = Mathf.Sin(rad) * currRadius;
            float z = Mathf.Cos(rad) * currRadius;

            vertices[i + 1] = new Vector3(x, 0f, z);
        }

        int[] triangles = new int[segmentCount * 3];
        for (int i = 0; i < segmentCount; i++)
        {
            triangles[i * 3]     = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        targetMesh.Clear();
        targetMesh.vertices  = vertices;
        targetMesh.triangles = triangles;
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }

    
}
