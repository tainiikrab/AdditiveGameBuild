Shader "Custom/URPHeightDissolve"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        _DissolveAmount("Dissolve Amount", Range(0,1)) = 0
        _MinHeight("Min Height", Float) = 0
        _MaxHeight("Max Height", Float) = 1
        _Threshold("Threshold", Float) = 0.02
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _EdgeColor("Edge Color", Color) = (1,0.5,0,1)
        _EdgeWidth("Edge Width", Range(0,0.2)) = 0.05
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _EdgeColor;
                float _DissolveAmount;
                float _MinHeight;
                float _MaxHeight;
                float _Threshold;
                float _Smoothness;
                float _Metallic;
                float _EdgeWidth;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float worldY : TEXCOORD3;
                float fogCoord : TEXCOORD4;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);
                
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.normalWS = normalInputs.normalWS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.worldY = positionInputs.positionWS.y;
                OUT.fogCoord = ComputeFogFactor(positionInputs.positionCS.z);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                float normHeight = 1.0 - saturate((IN.worldY - _MinHeight) / (_MaxHeight - _MinHeight));
                float dissolveAlpha = smoothstep(_DissolveAmount - _Threshold, _DissolveAmount + _Threshold, normHeight);
                
                clip(dissolveAlpha - 0.001);
                
                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 albedo = baseTex.rgb * _BaseColor.rgb;
                
                half3 finalColor = albedo;
                float edgeFactor = smoothstep(dissolveAlpha, dissolveAlpha + _EdgeWidth, dissolveAlpha + _EdgeWidth * 2.0);
                if (edgeFactor > 0.1)
                {
                    finalColor = lerp(albedo, _EdgeColor.rgb, edgeFactor * _EdgeColor.a);
                }
                
                InputData inputData;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS = normalize(IN.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                inputData.fogCoord = IN.fogCoord;
                inputData.bakedGI = SampleSH(inputData.normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);
                
                SurfaceData surfaceData;
                surfaceData.albedo = finalColor;
                surfaceData.metallic = _Metallic;
                surfaceData.specular = 0.5h;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = half3(0, 0, 1);
                surfaceData.emission = half3(0, 0, 0);
                surfaceData.occlusion = 1.0;
                surfaceData.alpha = dissolveAlpha;
                surfaceData.clearCoatMask = 0.0h;
                surfaceData.clearCoatSmoothness = 0.0h;
                
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                color.rgb = MixFog(color.rgb, IN.fogCoord);
                
                if (edgeFactor > 0.1)
                {
                    color.rgb += _EdgeColor.rgb * edgeFactor * 0.5;
                }
                
                return color;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            float3 _LightDirection;
            float3 _LightPosition;
            
            CBUFFER_START(UnityPerMaterial)
                float _DissolveAmount;
                float _MinHeight;
                float _MaxHeight;
                float _Threshold;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                
                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection;
                #endif
                
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
                
                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif
                
                OUT.positionCS = positionCS;
                OUT.positionWS = positionWS;
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_TARGET
            {
                float normHeight = 1.0 - saturate((IN.positionWS.y - _MinHeight) / (_MaxHeight - _MinHeight));
                float dissolveAlpha = smoothstep(_DissolveAmount - _Threshold, _DissolveAmount + _Threshold, normHeight);
                
                clip(dissolveAlpha - 0.001);
                
                return 0;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ColorMask 0
            ZWrite On
            ZTest LEqual
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float _DissolveAmount;
                float _MinHeight;
                float _MaxHeight;
                float _Threshold;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_TARGET
            {
                float normHeight = 1.0 - saturate((IN.positionWS.y - _MinHeight) / (_MaxHeight - _MinHeight));
                float dissolveAlpha = smoothstep(_DissolveAmount - _Threshold, _DissolveAmount + _Threshold, normHeight);
                clip(dissolveAlpha - 0.001);
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}