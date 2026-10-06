Shader "Custom/PortalShaderURP_VR_Stereo"
{
    Properties
    {
        _MainTex ("Ojo Izquierdo", 2D) = "white" {}
        _RightTex ("Ojo Derecho", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        LOD 100

        Pass
        {
            Name "Unlit"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing 

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos  : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Declaramos ambas texturas
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_RightTex);
            SAMPLER(sampler_RightTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                // Magia de VR: unity_StereoEyeIndex detecta qué pantalla de las gafas se está dibujando
                // 0 = Ojo Izquierdo | 1 = Ojo Derecho
                if (unity_StereoEyeIndex == 0)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, screenUV);
                }
                else
                {
                    return SAMPLE_TEXTURE2D(_RightTex, sampler_RightTex, screenUV);
                }
            }
            ENDHLSL
        }
    }
}