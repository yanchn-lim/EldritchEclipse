Shader "Hidden/EDGE DETECTION"
{
  
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off

        Pass
        {
            Name "DifferenceOfGaussian"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            //SAMPLER(sampler_BlitTexture);
            //SAMPLER(sampler_CameraDepthTexture);
            // 
            //declare your variables here
            
            float3 GaussianBlur(float2 pos) 
            {

            }

            half4 frag(Varyings input) : SV_Target
            {
                //_CameraNormalsTexture
                //_CameraDepthTexture
                //samples the texture
                float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord).rgb;

                //logic here

                return float4(color,1);
            }

            ENDHLSL
        }

        Pass
        {
            Name "DepthDifference"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

                //SAMPLER(sampler_BlitTexture);
                //SAMPLER(sampler_CameraDepthTexture);
                //declare your varaibles here

                half4 frag(Varyings input) : SV_Target
                {
                //_CameraNormalsTexture
                //_CameraDepthTexture
                //samples the texture
                float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord).rgb;

                //logic here

                return float4(color,1);
            }

            ENDHLSL
        }
    }
}
