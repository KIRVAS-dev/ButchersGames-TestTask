Shader "ButchersGames/Environment/Skybox"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Panorama (Lat-Long)", 2D) = "grey" {}
        _FogBottom ("Fog Bottom (direction Y)", Range(-1, 1)) = 0
        _FogTop ("Fog Top (direction Y)", Range(-1, 1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Background"
            "Queue" = "Background"
            "PreviewType" = "Skybox"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        ZWrite Off

        Pass
        {
            Name "Skybox"

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _FogBottom;
                float _FogTop;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            float2 LatLongUV(float3 direction)
            {
                float longitude = atan2(direction.z, direction.x);
                float latitude = acos(direction.y);

                return float2(0.5, 1.0) - float2(longitude, latitude) * float2(0.5 * INV_PI, INV_PI);
            }

            Varyings Vertex(Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.direction = input.positionOS.xyz;

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float3 direction = normalize(input.direction);
                half3 skyColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, LatLongUV(direction)).rgb;
                half skyFactor = saturate((direction.y - _FogBottom) / max(_FogTop - _FogBottom, 1e-4));
                half3 color = lerp(unity_FogColor.rgb, skyColor, skyFactor);

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
