using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAreaIndicator : AreaIndicator
{
    [Space(40)]
    [Header("Cone Setting")]
    [SerializeField] int segmentCount  = 36;  
    float radius => param_0;
    // float sectorAngle => param_1;
    
    
    
    
    
    
    protected override void DrawOutline()
    {
        // 세그먼트가 최소 3 이상이어야 원을 구성
        if (segmentCount < 3 || outlineRenderer == null)
        {
            if (outlineRenderer != null)
                outlineRenderer.positionCount = 0;
            return;
        }
        outlineRenderer.startColor = outlineColor;
        outlineRenderer.endColor   = outlineColor;

        outlineRenderer.useWorldSpace = false; // transform 로컬 기준으로 그릴지 여부
        outlineRenderer.loop = true;
        outlineRenderer.positionCount = segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float angleDeg = t * 360f;
            float angleRad = Mathf.Deg2Rad * angleDeg;

            float x = Mathf.Cos(angleRad) * radius;
            float z = Mathf.Sin(angleRad) * radius;

            outlineRenderer.SetPosition(i, new Vector3(x, 0f, z));
        }

    }

    protected override void GenerateSectorMesh(Mesh targetMesh, float progress)
    {
        if (targetMesh == null || segmentCount < 3)
        {
            Debug.LogWarning("세그먼트가 3 이상이어야 원형을 만들 수 있습니다.");
            return;
        }

        float currRadius = Mathf.Lerp(0f, radius, progress);


        // -----------------------
        // 1) 정점 (Vertices) 생성
        // -----------------------
        // 중심점(인덱스 0) + 세그먼트 개수만큼 (원 주위)
        // 총 segmentCount + 1개의 정점
        Vector3[] vertices = new Vector3[segmentCount + 1];
        
        // 중심점
        vertices[0] = Vector3.zero;

        // 0 ~ 2파이(360도)를 segmentCount로 나누어 vertex 생성
        for (int i = 0; i < segmentCount; i++)
        {
            // i번째 세그먼트가 몇 도(라디안)인지
            float t = i / (float)segmentCount;
            float angleRad = t * Mathf.PI * 2f; // 0 ~ 2PI

            // XZ 평면에서 원 둘레 점
            float x = Mathf.Cos(angleRad) * currRadius;
            float z = Mathf.Sin(angleRad) * currRadius;

            // 정점 저장
            vertices[i + 1] = new Vector3(x, 0f, z);
        }

        // -----------------------
        // 2) 삼각형 인덱스 (Triangles) 생성
        // -----------------------
        // 각 세그먼트마다 (중심점, 현재점, 다음점)으로 삼각형 하나씩
        // => segmentCount * 3개의 인덱스
        int[] triangles = new int[segmentCount * 3];

        for (int i = 0; i < segmentCount; i++)
        {
            // 중심점은 0번
            int current = i + 1;         // 현재 꼭짓점
            int next = (i + 1) + 1;      // 다음 꼭짓점
            
            // 마지막 꼭짓점 처리 (세그먼트 wrap)
            // i == segmentCount-1 인 경우, next가 (segmentCount+1)이 되어야 하는데,
            // 배열 밖이므로 next = 1로 순환
            if (i == segmentCount - 1)
            {
                next = 1;
            }

            int triIndex = i * 3;
            triangles[triIndex + 0] = 0;      // 중심
            triangles[triIndex + 1] = current;
            triangles[triIndex + 2] = next;
        }

        // -----------------------
        // 3) Mesh에 적용
        // -----------------------
        targetMesh.Clear();
        targetMesh.vertices = vertices;
        targetMesh.triangles = triangles;

        // 노말 및 경계 재계산
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }
}
