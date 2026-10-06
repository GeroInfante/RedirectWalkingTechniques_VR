Shader "Custom/PortalShaderURP_VR_Stereo2"
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
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
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

                // Usamos el UV real del mesh (no screen-space) para que la imagen
                // quede "pegada" geométricamente al plano del portal, como una
                // pantalla, en vez de deslizarse con el movimiento de cabeza.
                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Magia de VR: unity_StereoEyeIndex detecta qué pantalla de las gafas se está dibujando
                // 0 = Ojo Izquierdo | 1 = Ojo Derecho
                if (unity_StereoEyeIndex == 0)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                }
                else
                {
                    return SAMPLE_TEXTURE2D(_RightTex, sampler_RightTex, input.uv);
                }
            }
            ENDHLSL
        }
    }
}
