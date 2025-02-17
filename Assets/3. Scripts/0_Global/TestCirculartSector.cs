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
    public Color fullLineColor = Color.blue;               // 풀 범위 외곽선
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
        GenerateSectorMesh(_fullSectorMesh, radius);

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
        GenerateSectorMesh(_fillSectorMesh, 0f);

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
            GenerateSectorMesh(_fillSectorMesh, curRadius);

            yield return null;
        }

        // 마지막에 radius로 확정
        GenerateSectorMesh(_fillSectorMesh, radius);
    }

    // ---------------------------------------------------------
    // (A) 부채꼴 메시 생성 함수 (XZ 평면, +Z가 0도)
    // ---------------------------------------------------------
    private void GenerateSectorMesh(Mesh targetMesh, float currentRadius)
    {
        if (targetMesh == null || segmentCount < 1 || sectorAngle <= 0f)
        {
            return;
        }

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

            float x = Mathf.Sin(rad) * currentRadius;
            float z = Mathf.Cos(rad) * currentRadius;

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



    // ------------------------------------------------------------
    // ------------------------------------------------------------
    // 2) 라인렌더러로 "닫힌" 외곽선 생성 (센터 → 호 → 센터).
    //    angle 범위: [-(sectorAngle/2), +(sectorAngle/2)]
    //    정중앙(0°)이 +Z 방향이 되도록.
    // ------------------------------------------------------------
    void DrawClosedSectorOutline()
    {
        if (segmentCount < 1 || sectorAngle <= 0f)
        {
            fullLineRenderer.positionCount = 0;
            return;
        }


        // 라인렌더러 기본 세팅
        fullLineRenderer.useWorldSpace = false; // 로컬 좌표 사용
        fullLineRenderer.startColor = fullLineColor;
        fullLineRenderer.endColor   = fullLineColor;
        fullLineRenderer.loop       = false;    

        // 총 (segmentCount + 3)개 위치:
        //   0: center
        //   1..(segmentCount+1): 호
        //   (segmentCount+2): center
        fullLineRenderer.positionCount = segmentCount + 3;

        // 0번: 중심
        fullLineRenderer.SetPosition(0, Vector3.zero);

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
            fullLineRenderer.SetPosition(i + 1, new Vector3(x, 0f, z));
        }

        // 마지막 점: 다시 중앙
        fullLineRenderer.SetPosition(segmentCount + 2, Vector3.zero);


    }
}
