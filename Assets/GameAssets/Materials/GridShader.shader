Shader "Custom/GridShader3DVertical"
{
    Properties
    {
        _GridSize ("Grid Size", Range(0.1, 10)) = 1.0
        _LineWidth ("Line Width", Range(0.001, 0.1)) = 0.01
        _GridColorCenter ("Grid Color (Center)", Color) = (1, 1, 1, 1)
        _GridColorEdge ("Grid Color (Edge)", Color) = (0.5, 0.5, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _GradientRadius ("Gradient Radius", Range(0.01, 10)) = 5.0
        _GradientSmoothness ("Gradient Smoothness", Range(0.001, 1.0)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 worldPos : TEXCOORD0;
                float2 objPosXZ : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _GridSize;
            float _LineWidth;
            float4 _GridColorCenter;
            float4 _GridColorEdge;
            float4 _BackgroundColor;
            float _GradientRadius;
            float _GradientSmoothness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objPosXZ = v.vertex.xz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 gridUV = i.worldPos / _GridSize;
                float3 grid = frac(gridUV);
                float3 distToLine = min(grid, 1.0 - grid);

                float minDist = min(min(distToLine.x, distToLine.y), distToLine.z);

                float onGridLine = smoothstep(_LineWidth * 0.5, _LineWidth, minDist);

                float distanceFromCenter = length(i.objPosXZ);
                float normalizedDist = saturate(distanceFromCenter / _GradientRadius);

                float edgePos = 1.0 - _GradientSmoothness * 0.5;
                float gradient = smoothstep(edgePos - _GradientSmoothness, edgePos, normalizedDist);

                float4 currentGridColor = lerp(_GridColorCenter, _GridColorEdge, gradient);

                fixed4 color = lerp(currentGridColor, _BackgroundColor, onGridLine);
                return color;
            }
            ENDCG
        }
    }
}