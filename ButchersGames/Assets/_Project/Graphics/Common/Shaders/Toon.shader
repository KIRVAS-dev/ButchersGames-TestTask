Shader "ButchersGames/Toon"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (1, 1, 1, 1)
        _RampThreshold ("Ramp Threshold", Range(0, 1)) = 0.5
        _RampSmoothing ("Ramp Smoothing", Range(0.001, 1)) = 0.1
        _SpecularColor ("Specular Color", Color) = (0, 0, 0, 1)
        _SpecularGloss ("Specular Gloss", Range(1, 256)) = 48
        _SpecularSmoothing ("Specular Smoothing", Range(0.001, 1)) = 0.1
        _SpecularLift ("Specular Lift", Range(-1, 1)) = 0.3
        _RimColor ("Rim Color", Color) = (0, 0, 0, 1)
        _RimSize ("Rim Size", Range(0, 1)) = 0.35
        _RimSmoothing ("Rim Smoothing", Range(0.001, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _BaseColor;
            half4 _HighlightColor;
            half4 _ShadowColor;
            half _RampThreshold;
            half _RampSmoothing;
            half4 _SpecularColor;
            half _SpecularGloss;
            half _SpecularSmoothing;
            half _SpecularLift;
            half4 _RimColor;
            half _RimSize;
            half _RimSmoothing;
        CBUFFER_END

        half ToonStep(half edge, half smoothing, half value)
        {
            half halfWidth = smoothing * 0.5;

            return smoothstep(edge - halfWidth, edge + halfWidth, value);
        }
        ENDHLSL

        Pass
        {
            Name "ForwardToon"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = positions.positionCS;
                output.positionWS = positions.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                half3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).rgb * _BaseColor.rgb;

                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));

                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

                half halfLambert = dot(normalWS, mainLight.direction) * 0.5 + 0.5;
                half ramp = ToonStep(_RampThreshold, _RampSmoothing, halfLambert) * mainLight.shadowAttenuation;

                half3 highlight = mainLight.color * _HighlightColor.rgb;
                half3 shadow = SampleSH(normalWS) * _ShadowColor.rgb;
                half3 color = albedo * lerp(shadow, highlight, ramp);

                float3 specularDirWS = normalize(viewDirWS + float3(0.0, _SpecularLift, 0.0));
                half normalDotSpecular = saturate(dot(normalWS, specularDirWS));
                half specular = ToonStep(0.5, _SpecularSmoothing, pow(normalDotSpecular, _SpecularGloss));

                half rimFactor = 1.0 - saturate(dot(normalWS, viewDirWS));
                half rim = ToonStep(1.0 - _RimSize, _RimSmoothing, rimFactor) * halfLambert;

                color += specular * _SpecularColor.rgb + rim * _RimColor.rgb;

                half fogFactor = InitializeInputDataFog(float4(input.positionWS, 1.0), 0.0);

                return half4(MixFog(color, fogFactor), 1.0);
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

            HLSLPROGRAM
            #pragma vertex ShadowVertex
            #pragma fragment ShadowFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            float3 _LightDirection;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            float4 ShadowVertex(Attributes input) : SV_POSITION
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

                #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }

            half4 ShadowFragment() : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R

            HLSLPROGRAM
            #pragma vertex DepthVertex
            #pragma fragment DepthFragment

            float4 DepthVertex(float4 positionOS : POSITION) : SV_POSITION
            {
                return TransformObjectToHClip(positionOS.xyz);
            }

            half DepthFragment() : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormals" }

            ZWrite On

            HLSLPROGRAM
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            Varyings DepthNormalsVertex(Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);

                return output;
            }

            half4 DepthNormalsFragment(Varyings input) : SV_Target
            {
                return half4(NormalizeNormalPerPixel(input.normalWS), 0.0);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
