using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCirculartSector : MonoBehaviour
{    [Header("부채꼴(원형 섹터) 기본 설정")]
    [Range(0, 360)]
    public float sectorAngle = 90f;     // 총 부채꼴 각도(도 단위)
    public float radius      = 3f;      // 최대 반지름
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
        GenerateCircleMesh(_fullSectorMesh, radius);

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

            float curRadius = Mathf.Lerp(0f, radius, t);

            // 메시 갱신
            GenerateCircleMesh(_fillSectorMesh, curRadius);

            yield return null;
        }

        // 마지막에 radius로 확정
        GenerateCircleMesh(_fillSectorMesh, radius);
    }

    /// <summary>
    /// XZ 평면에 원형(Circle) 메쉬를 생성한다.
    /// </summary>
    /// <param name="targetMesh">생성할 메쉬를 할당할 Mesh 객체</param>
    /// <param name="radius">원형의 반지름</param>
    /// <param name="segmentCount">분할 개수(클수록 원에 가까워짐)</param>
    private void GenerateCircleMesh(Mesh targetMesh, float radius)
    {
        if (targetMesh == null || segmentCount < 3)
        {
            Debug.LogWarning("세그먼트가 3 이상이어야 원형을 만들 수 있습니다.");
            return;
        }

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
            float x = Mathf.Cos(angleRad) * radius;
            float z = Mathf.Sin(angleRad) * radius;

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


    // ------------------------------------------------------------
    // ------------------------------------------------------------
    // 2) 라인렌더러로 "닫힌" 외곽선 생성 (센터 → 호 → 센터).
    //    angle 범위: [-(sectorAngle/2), +(sectorAngle/2)]
    //    정중앙(0°)이 +Z 방향이 되도록.
    // ------------------------------------------------------------
    void DrawClosedSectorOutline()
    {
                // 세그먼트가 최소 3 이상이어야 원을 구성
        if (segmentCount < 3 || fullLineRenderer == null)
        {
            if (fullLineRenderer != null)
                fullLineRenderer.positionCount = 0;
            return;
        }
        fullLineRenderer.startColor = lineColor;
        fullLineRenderer.endColor   = lineColor;

        
        fullLineRenderer.useWorldSpace = false; // transform 로컬 기준으로 그릴지 여부

        fullLineRenderer.loop = true;
        fullLineRenderer.positionCount = segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float angleDeg = t * 360f;
            float angleRad = Mathf.Deg2Rad * angleDeg;

            float x = Mathf.Cos(angleRad) * radius;
            float z = Mathf.Sin(angleRad) * radius;

            fullLineRenderer.SetPosition(i, new Vector3(x, 0f, z));
        }
    }
}
