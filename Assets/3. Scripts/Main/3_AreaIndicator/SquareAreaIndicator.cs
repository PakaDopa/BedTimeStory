using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareAreaIndicator : AreaIndicator
{
    float width => param_0;
    float height => param_1;
    
    protected override void DrawOutline()
    {
        // LineRenderer가 연결되지 않았다면 종료
        if (outlineRenderer == null)
        {
            return;
        }

        // 꼭짓점이 4개인 사각형, 마지막 점과 첫 점을 이어주려면 loop = true
        outlineRenderer.positionCount = 4;
        outlineRenderer.loop = true;

        outlineRenderer.startColor = outlineColor;
        outlineRenderer.endColor   = outlineColor;

        // world 기준이 아니라 local 기준 좌표로 그리려면 false
        outlineRenderer.useWorldSpace = false;

        // 폭과 높이의 절반을 구함
        float halfW = width  * 0.5f;

        // 사각형 꼭짓점 4개를 시계(또는 반시계) 방향으로 지정
        // 바닥에서 X방향이 가로, Z방향이 세로(길이)라고 가정
        outlineRenderer.SetPosition(0, new Vector3(-halfW, 0f, 0));
        outlineRenderer.SetPosition(1, new Vector3(-halfW, 0f, height));
        outlineRenderer.SetPosition(2, new Vector3( halfW, 0f, height));
        outlineRenderer.SetPosition(3, new Vector3( halfW, 0f, 0));
    }

    protected override void GenerateSectorMesh(Mesh targetMesh, float progress)
    {
        if (targetMesh == null)
        {
            Debug.LogWarning("targetMesh가 존재하지 않습니다.");
            return;
        }

        // 좌우 폭의 절반
        float halfW = width * 0.5f;

        float currW = halfW * progress;
        float currH = height* progress;


        // -----------------------
        // 1) 정점 (Vertices) 생성
        // -----------------------
        // 사각형 꼭짓점 총 4개
        // 문제에서 원하는 좌표:
        // ( -halfW, 0f, 0f ), ( -halfW, 0f, height ), ( halfW, 0f, height ), ( halfW, 0f, 0f )
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-halfW, 0f, 0f);    
        vertices[1] = new Vector3(-halfW, 0f, currH );
        vertices[2] = new Vector3( halfW, 0f, currH );
        vertices[3] = new Vector3( halfW, 0f, 0f);

        // -----------------------
        // 2) 삼각형 인덱스 (Triangles) 생성
        // -----------------------
        // 사각형을 구성하기 위해서는 삼각형 2개가 필요
        // (0,1,2), (2,3,0)
        int[] triangles = new int[6]
        {
            0, 1, 2,
            2, 3, 0
        };

        // -----------------------
        // 3) Mesh에 적용
        // -----------------------
        targetMesh.Clear();
        targetMesh.vertices  = vertices;
        targetMesh.triangles = triangles;

        // 노멀 및 경계 재계산
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }
}
