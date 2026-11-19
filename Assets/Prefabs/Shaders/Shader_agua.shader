
Shader "Custom/AguaRio"
{
    Properties
    {
        [MainColor] _BaseColor("Color del agua", Color) = (0.035, 0.16, 0.12, 0.42)
        _FresnelStrength("Brillo en bordes", Range(0, 1)) = 0.15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "Water"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _FresnelStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionHCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);

                float3 viewDir = normalize(
                    GetWorldSpaceViewDir(IN.positionWS)
                );

                float fresnel = pow(
                    1.0 - saturate(abs(dot(normalWS, viewDir))),
                    3.0
                );

                half3 color = _BaseColor.rgb;
                color += fresnel * _FresnelStrength;

                half alpha = saturate(
                    _BaseColor.a + fresnel * 0.1
                );

                return half4(color, alpha);
            }

            ENDHLSL
        }
    }
}
