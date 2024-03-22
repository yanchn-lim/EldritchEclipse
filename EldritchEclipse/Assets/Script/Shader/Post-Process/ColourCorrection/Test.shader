Shader "Hidden/Test"
{
  
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off

        Pass
        {
            Name "TestPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            //TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);


            half4 frag(Varyings input) : SV_Target
            {
                float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_BlitTexture,input.texcoord).rgb;

                color += 0.1f;

                return float4(color,1);
            }

            ENDHLSL
        }
    }
}
