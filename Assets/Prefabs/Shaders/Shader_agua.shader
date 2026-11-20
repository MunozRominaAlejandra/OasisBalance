
Shader "Custom/AguaRio"
{
    Properties
    {
        [MainColor] _BaseColor("Color del agua", Color) = (0.035, 0.13, 0.095, 0.55)

        _Smoothness("Suavidad de reflejos", Range(0, 1)) = 0.85
        _SpecularStrength("Intensidad de reflejos", Range(0, 2)) = 0.35
        _FresnelStrength("Brillo en bordes", Range(0, 1)) = 0.2
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

            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
                float _Smoothness;
                float _SpecularStrength;
                float _FresnelStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionHCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.normalWS =
                    TransformObjectToWorldNormal(IN.normalOS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);

                float3 viewDir = normalize(
                    GetWorldSpaceViewDir(IN.positionWS)
                );

                // Orientar la normal hacia la cámara
                normalWS *=
                    (dot(normalWS, viewDir) < 0) ? -1.0 : 1.0;

                // Luz principal
                Light mainLight = GetMainLight();

                float3 lightDir = normalize(mainLight.direction);

                float NdotL = saturate(
                    dot(normalWS, lightDir)
                );

                // Iluminación ambiental
                half3 ambient = SampleSH(normalWS);

                // Iluminación difusa
                half3 diffuse =
                    _BaseColor.rgb *
                    (ambient + mainLight.color * NdotL * 0.65);

                // Reflejos especulares
                float3 halfDir = normalize(lightDir + viewDir);

                float NdotH = saturate(
                    dot(normalWS, halfDir)
                );

                float specularPower = lerp(
                    8.0, 128.0, _Smoothness
                );

                float specular = pow(
                    NdotH, specularPower
                ) * _SpecularStrength;

                half3 specularColor =
                    mainLight.color * specular;

                // Fresnel
                float fresnel = pow(
                    1.0 - saturate(dot(normalWS, viewDir)),
                    4.0
                );

                half3 fresnelColor =
                    _BaseColor.rgb * fresnel * _FresnelStrength;

                // Resultado final
                half3 finalColor =
                    diffuse + specularColor + fresnelColor;

                return half4(
                    finalColor,
                    saturate(_BaseColor.a)
                );
            }

            ENDHLSL
        }
    }
}
