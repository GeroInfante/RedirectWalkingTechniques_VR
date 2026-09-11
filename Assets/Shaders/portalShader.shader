Shader "Custom/PortalShaderURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // Estas etiquetas le avisan a Unity que este material es exclusivo de URP
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry" 
        }
        LOD 100

        Pass
        {
            Name "Unlit"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Importamos la librería principal de matemáticas y renderizado de URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos  : TEXCOORD0;
            };

            // Declaración moderna de texturas en URP
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                // Transformamos la posición del objeto a la vista de la cámara
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                // Calculamos las coordenadas exactas de la pantalla
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Dividimos por 'w' para mantener la perspectiva 3D correcta
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                // Dibujamos la textura respetando al 100% el color original
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, screenUV);
            }
            ENDHLSL
        }
    }
}