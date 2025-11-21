Shader "Hidden/StampBrush_URP"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _BrushTex("Brush", 2D) = "white" {}
        _Center("Center", Vector) = (0.5,0.5,0,0)
        _BrushSize("BrushSize", Float) = 0.1
        _Color("Color", Color) = (1,0,0,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_BrushTex); SAMPLER(sampler_BrushTex);

            float4 _Center;
            float _BrushSize;
            float4 _Color;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float d = distance(i.uv, _Center.xy);

                if (d < _BrushSize)
                    return _Color;

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            }

            ENDHLSL
        }
    }
}
