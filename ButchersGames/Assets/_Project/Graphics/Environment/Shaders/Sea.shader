Shader "ButchersGames/Environment/Sea"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _DistortionScale ("Distortion Cell Size (m)", Float) = 6
        _DistortionStrength ("Distortion Strength (tiles)", Range(0, 0.5)) = 0.06
        _DistortionSpeed ("Distortion Speed", Float) = 0.6
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

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float _DistortionScale;
            float _DistortionStrength;
            float _DistortionSpeed;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            static const float TwoPi = 6.2831853;
            static const float2 SecondLayerShift = float2(17.3, 5.7);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            float2 CellHash(float2 cell)
            {
                float2 projected = float2(dot(cell, float2(127.1, 311.7)), dot(cell, float2(269.5, 183.3)));

                return frac(sin(projected) * 43758.5453);
            }

            float AnimatedVoronoi(float2 position, float time)
            {
                float2 cell = floor(position);
                float2 local = frac(position);
                float nearest = 8.0;

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 neighbour = float2(x, y);
                        float2 cellPoint = 0.5 + 0.5 * sin(time + TwoPi * CellHash(cell + neighbour));

                        nearest = min(nearest, length(neighbour + cellPoint - local));
                    }
                }

                return nearest;
            }

            Varyings Vertex(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = positions.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.positionWS = positions.positionWS;

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float time = _Time.y * _DistortionSpeed;
                float2 distortionPosition = input.positionWS.xz / _DistortionScale;

                float2 distortion = float2(
                    AnimatedVoronoi(distortionPosition, time),
                    AnimatedVoronoi(distortionPosition + SecondLayerShift, time)) - 0.5;

                float2 uv = input.uv + distortion * _DistortionStrength;
                half3 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb;
                half fogFactor = InitializeInputDataFog(float4(input.positionWS, 1.0), 0.0);

                color = MixFog(color, fogFactor);

                return half4(color, 1.0);
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
    }

    FallBack Off
}
