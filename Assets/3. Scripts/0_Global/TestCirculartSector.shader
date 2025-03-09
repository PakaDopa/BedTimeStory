Shader "Custom/ProceduralSectorWithOutline"
{
    Properties
    {
        _MainColor      ("Main Color", Color) = (1, 0, 0, 1)
        _OutlineColor   ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth   ("Outline Width (in UV space)", Range(0,0.1)) = 0.02
        _Radius         ("Radius (0~0.5 recommended)", Range(0,0.5)) = 0.4
        _SectorAngleDeg ("Sector Angle (0~360)", Range(0,360)) = 180
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            //===========================================
            // 구조체 정의
            //===========================================
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            //===========================================
            // 셰이더 프로퍼티 (CG 변수)
            //===========================================
            float4 _MainColor;
            float4 _OutlineColor;
            float  _OutlineWidth;
            float  _Radius;           // 섹터 반지름 (UV 기준)
            float  _SectorAngleDeg;   // 섹터 각도 (도 단위)

            //===========================================
            // 버텍스 셰이더
            //===========================================
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //===========================================
            // 프래그먼트 셰이더
            //===========================================
            fixed4 frag (v2f i) : SV_Target
            {
                // [0,1] 범위의 UV 가운데 (0.5, 0.5)가 "원점"이 되도록 보정
                float2 centerUV = float2(0.5, 0.5);
                float2 pos = i.uv - centerUV;  // (-0.5 ~ +0.5)

                // 반지름 거리와 각도 계산 (atan2)
                float dist = length(pos);
                float angleRad = atan2(pos.y, pos.x); // -π ~ +π
                // 도 단위로 변환 후, 0 ~ 360 범위로 조정
                float angleDeg = degrees(angleRad);
                if (angleDeg < 0) angleDeg += 360;

                // --------------------------
                // 1) 현재 픽셀이 섹터 내부인지 확인
                //    - 거리 dist <= _Radius
                //    - 각도 angleDeg <= _SectorAngleDeg
                // --------------------------
                float insideRadius = step(dist, _Radius);                          // 반지름 안쪽이면 1
                float insideAngle  = step(angleDeg, _SectorAngleDeg);              // 각도 범위 안쪽이면 1
                float insideSector = insideRadius * insideAngle;                   // 둘 다 1이면 '부채꼴 내부'

                // --------------------------
                // 2) 외곽선 여부를 판단
                //    - "원둘레" 근처 여부: |dist - _Radius| < _OutlineWidth
                //    - "각도 경계" 근처 여부: angleDeg ~ 0 또는 ~ _SectorAngleDeg
                // --------------------------
                // 2-1) 원둘레
                //     smoothstep(a, b, x)는 x가 a일 때 0, b일 때 1; 그 사이에서 부드럽게 보간
                //     => dist가 [(_Radius - _OutlineWidth), _Radius] 구간이면 0~1로 상승
                float nearCircle = smoothstep(_Radius - _OutlineWidth, _Radius, dist);

                // 2-2) 각도 시작(0도) 근처
                //     angleDeg가 [0, _OutlineWidth] 범위 내에 있으면 외곽선
                float nearAngleStart = smoothstep(0, _OutlineWidth, angleDeg);

                // 2-3) 각도 끝(_SectorAngleDeg) 근처
                //     만약 섹터 각도가 360이면 '끝 경계'가 의미 없음
                float nearAngleEnd = 0;
                if(_SectorAngleDeg < 360)
                {
                    float startVal = _SectorAngleDeg - _OutlineWidth;
                    float endVal   = _SectorAngleDeg;
                    nearAngleEnd = smoothstep(startVal, endVal, angleDeg);
                }

                // 2-4) nearCircle, nearAngleStart, nearAngleEnd 중 하나라도 "1"이면 외곽선
                //     하지만 섹터 '안'에 있을 때만 (insideSector=1 일 때만) 유효
                float outlineFactor = max(nearCircle, max(nearAngleStart, nearAngleEnd));
                outlineFactor *= insideSector; // 섹터 밖이면 outline=0

                // 이 값 outlineFactor는 0~1 사이 부드러운 값이 될 수 있음.
                // - 0일 때 : 외곽선 아님, (안 or 밖)
                // - 1에 가까울수록 '외곽선' 측에 가깝다.

                // --------------------------
                // 최종 색상 결정
                // --------------------------
                // - 메인색과 외곽선을 lerp로 섞어볼 수도 있고,
                // - 외곽선 부분은 무조건 _OutlineColor로 하기 원한다면 step(0.5, outlineFactor) 등으로 처리
                //   예: float isOutline = step(0.5, outlineFactor);
                //   fixed4 col = lerp(_MainColor, _OutlineColor, isOutline);

                // 여기는 그냥 부드럽게 섞어보겠습니다.
                fixed4 col = lerp(_MainColor, _OutlineColor, outlineFactor);

                // 섹터 밖인 경우(alpha=0) => 완전히 안보이게
                // 섹터 밖이면 insideSector=0 이므로 col.a=0. (안에만 보이게)
                if (insideSector < 0.5)
                {
                    col.a = 0;
                }

                return col;
            }
            ENDCG
        }
    }
}
