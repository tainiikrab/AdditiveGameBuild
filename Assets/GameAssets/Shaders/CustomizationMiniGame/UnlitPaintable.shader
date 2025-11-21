Shader "Custom/UnlitPaintable_URP"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _PaintTex("PaintTex", 2D) = "black" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_PaintTex); SAMPLER(sampler_PaintTex);
            float4 _BaseColor;

            struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionHCS:SV_POSITION; float2 uv:TEXCOORD0; };

            Varyings vert(Attributes v){
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(Varyings i):SV_Target{
                float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 paint = SAMPLE_TEXTURE2D(_PaintTex, sampler_PaintTex, i.uv);
                float3 mul = baseCol.rgb * _BaseColor.rgb;
                float3 outRgb = lerp(mul, paint.rgb, paint.a);
                return float4(outRgb, 1.0);
            }
            ENDHLSL
        }
    }
}
