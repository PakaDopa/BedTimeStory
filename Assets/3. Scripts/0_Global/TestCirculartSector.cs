using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCirculartSector : MonoBehaviour
{    [Header("부채꼴(원형 섹터) 기본 설정")]
    [Range(0, 360)]
    public float sectorAngle = 90f;     // 총 부채꼴 각도(도 단위)
    public float radius      = 3f;      // 최대 반지름

    public float width = 3f;
    public float height = 5f;
    public int segmentCount  = 18;      // 분할 개수 (값이 높을수록 곡면이 부드러워짐)

    [Range(0.1f, 5f)]
    public float fillDuration = 1.5f;   // 몇 초 동안 0→100%로 채워질지

    [Header("=== '풀 범위' (최대)용 ===")]
    // (1) 메시 (최대 범위)
    public MeshFilter   fullSectorFilter;
    public MeshRenderer fullSectorRenderer;

    // (2) 라인렌더러 (최대 범위 외곽선)
    public LineRenderer fullLineRenderer;

    [Header("=== '채워지는' 용 ===")]
    // (3) 메시 (0→radius로 애니메이션)
    public MeshFilter   fillSectorFilter;
    public MeshRenderer fillSectorRenderer;
    // 라인은 없음

    [Header("=== 공통 머티리얼(Shader) ===")]
    [Tooltip("모든 Renderer(LineRenderer 포함)가 사용할 공통 머티리얼 (URP/Lit, Standard 등)")]
    public Material baseMaterial;

    [Header("=== 각 오브젝트 색상 (MPB) ===")]
    public Color fullMeshColor = new Color(0, 0, 1, 0.3f); // 풀 범위 메쉬: 반투명 파랑
    public Color lineColor = Color.blue;               // 풀 범위 외곽선
    public Color fillMeshColor = Color.red;                // 채워지는 범위(메쉬)

    [Header("머티리얼 컬러 프로퍼티 이름")]
    [Tooltip("URP/Lit은 _BaseColor, Legacy Standard는 _Color 등이 일반적")]
    public string colorPropertyName = "_Color";

    // 내부적으로 관리할 메쉬
    private Mesh _fullSectorMesh;   // 최대 범위용
    private Mesh _fillSectorMesh;   // 0→radius 애니메이션용

    // MPB
    private MaterialPropertyBlock _mpbFullMesh;
    private MaterialPropertyBlock _mpbFillMesh;

    void Start()
    {
        
        DrawClosedSectorOutline();
        // ---------------------------------------------------
        // 1) 풀 범위 메쉬 생성 (항상 radius)
        // ---------------------------------------------------
        _fullSectorMesh = new Mesh { name = "FullSectorMesh" };
        GenerateCircleMesh(_fullSectorMesh, 1f);

        if (fullSectorFilter)
            fullSectorFilter.mesh = _fullSectorMesh;

        // ---------------------------------------------------
        // 2) 풀 범위 라인 (항상 radius)
        // ---------------------------------------------------
        // if (fullLineRenderer)
        // {
        //     DrawClosedSectorOutline(fullLineRenderer, radius);
        // }

        // ---------------------------------------------------
        // 3) 채워지는(애니메이션) 메쉬 (초기 0%)
        // ---------------------------------------------------
        _fillSectorMesh = new Mesh { name = "FillSectorMesh" };
        GenerateCircleMesh(_fillSectorMesh, 0f);

        if (fillSectorFilter)
            fillSectorFilter.mesh = _fillSectorMesh;

        // ---------------------------------------------------
        // 4) 공통 머티리얼 할당
        // ---------------------------------------------------
        if (baseMaterial != null)
        {
            // 풀 범위 메쉬
            if (fullSectorRenderer)
                fullSectorRenderer.sharedMaterial = baseMaterial;


            // 채워지는 메쉬
            if (fillSectorRenderer)
                fillSectorRenderer.sharedMaterial = baseMaterial;
        }

        // ---------------------------------------------------
        // 5) MPB로 각 렌더러 색상 설정
        // ---------------------------------------------------
        // (a) 풀 범위 메쉬
        _mpbFullMesh = new MaterialPropertyBlock();
        if (fullSectorRenderer)
        {
            _mpbFullMesh.SetColor(colorPropertyName, fullMeshColor);
            fullSectorRenderer.SetPropertyBlock(_mpbFullMesh);
        }


        // (c) 채워지는 메쉬
        _mpbFillMesh = new MaterialPropertyBlock();
        if (fillSectorRenderer)
        {
            _mpbFillMesh.SetColor(colorPropertyName, fillMeshColor);
            fillSectorRenderer.SetPropertyBlock(_mpbFillMesh);
        }

        // ---------------------------------------------------
        // 6) 코루틴으로 0→100% 진행
        // ---------------------------------------------------
        StartCoroutine(FillSectorOverTime());
    }

    // ---------------------------------------------------------
    // (코루틴) 채워지는 범위를 0→radius로 확장
    // ---------------------------------------------------------
    private IEnumerator FillSectorOverTime()
    {
        float elapsed = 0f;
        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fillDuration);


            // 메시 갱신
            GenerateCircleMesh(_fillSectorMesh, t);

            yield return null;
        }

        // 마지막에 radius로 확정
        GenerateCircleMesh(_fillSectorMesh, 1);
    }

    /// <summary>
    /// XZ 평면에 원형(Circle) 메쉬를 생성한다.
    /// </summary>
    /// <param name="targetMesh">생성할 메쉬를 할당할 Mesh 객체</param>
    /// <param name="radius">원형의 반지름</param>
    /// <param name="segmentCount">분할 개수(클수록 원에 가까워짐)</param>
    private void GenerateCircleMesh(Mesh targetMesh, float progress)
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


    // ------------------------------------------------------------
    // ------------------------------------------------------------
    // 2) 라인렌더러로 "닫힌" 외곽선 생성 (센터 → 호 → 센터).
    //    angle 범위: [-(sectorAngle/2), +(sectorAngle/2)]
    //    정중앙(0°)이 +Z 방향이 되도록.
    // ------------------------------------------------------------
    void DrawClosedSectorOutline()
    {
        // LineRenderer가 연결되지 않았다면 종료
        if (fullLineRenderer == null)
        {
            return;
        }

        // 꼭짓점이 4개인 사각형, 마지막 점과 첫 점을 이어주려면 loop = true
        fullLineRenderer.positionCount = 4;
        fullLineRenderer.loop = true;

        fullLineRenderer.startColor = lineColor;
        fullLineRenderer.endColor   = lineColor;

        // world 기준이 아니라 local 기준 좌표로 그리려면 false
        fullLineRenderer.useWorldSpace = false;

        // 폭과 높이의 절반을 구함
        float halfW = width  * 0.5f;

        // 사각형 꼭짓점 4개를 시계(또는 반시계) 방향으로 지정
        // 바닥에서 X방향이 가로, Z방향이 세로(길이)라고 가정
        fullLineRenderer.SetPosition(0, new Vector3(-halfW, 0f, 0));
        fullLineRenderer.SetPosition(1, new Vector3(-halfW, 0f, height));
        fullLineRenderer.SetPosition(2, new Vector3( halfW, 0f, height));
        fullLineRenderer.SetPosition(3, new Vector3( halfW, 0f, 0));
    }
}
