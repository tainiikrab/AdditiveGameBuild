Shader "SlicerUnlit"
{
    Properties
    {
        _LayerHeight("Layer Height", Float) = 0.2
        _SliceGap("Inter-layer gap visual (0..1)", Range(0,1)) = 0.08
        _SliceSmooth("Slice edge smooth", Range(0.0001,0.05)) = 0.01

        _DistortionAmount("Distortion Amount", Range(0,1)) = 0.1
        _DistortionScale("Distortion Scale", Float) = 2.0
        _DistortionSpeed("Distortion Speed", Float) = 0.5

        _LayerColor("Layer Color", Color) = (0.95,0.95,0.95,1)
        _GlobalOpacity("Global Opacity", Range(0,1)) = 1.0

        _PrintProgress("Print Progress Height", Float) = 9999.0
        _ProgressFade("Progress Fade Smoothness", Range(0.001,0.2)) = 0.05

        _HighlightColor("Highlight Color", Color) = (1,0.3,0.3,1)
        _HighlightStrength("Highlight Strength", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _LayerHeight;
            float _SliceGap;
            float _SliceSmooth;

            float _DistortionAmount;
            float _DistortionScale;
            float _DistortionSpeed;

            fixed4 _LayerColor;
            float _GlobalOpacity;

            float _PrintProgress;
            float _ProgressFade;

            fixed4 _HighlightColor;
            float _HighlightStrength;

            float noise3D(float3 p)
            {
                return (sin(p.x) + cos(p.y) + sin(p.z)) * 0.33;
            }

            float distortion(float3 worldPos, float t)
            {
                float3 p = worldPos * _DistortionScale + float3(t, t * 0.5, t * 0.25);
                return noise3D(p) * _DistortionAmount;
            }

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 wpos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                o.worldPos = wpos.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float layerH = max(_LayerHeight, 1e-4);

                float t = _Time.y * _DistortionSpeed;
                float d = distortion(i.worldPos, t);

                float y = i.worldPos.y + d;
                float layerIndex = floor(y / layerH);
                float within = frac(y / layerH) * layerH;

                float fade = 1.0 - smoothstep(_PrintProgress - _ProgressFade, _PrintProgress, y);
                if (fade <= 0.001) return fixed4(0, 0, 0, 0);

                float gap = saturate(_SliceGap);
                float gapStart = layerH * (1.0 - gap);
                float isGap = smoothstep(gapStart - _SliceSmooth, gapStart + _SliceSmooth, within);

                float gapMask = 1.0 - isGap;
                float alpha = lerp(_GlobalOpacity * 0.2, _GlobalOpacity, gapMask);
                alpha *= fade;

                float currentLayerIndex = floor(_PrintProgress / layerH);
                float highlight = 1.0 - saturate(abs(layerIndex - currentLayerIndex));
                fixed3 col = lerp(_LayerColor.rgb, _HighlightColor.rgb, highlight * _HighlightStrength);

                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}