Shader "Hidden/SHADER_NAME_HERE"
{
  
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off

        Pass
        {
            Name "PASS_NAME_HERE"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            SAMPLER(sampler_BlitTexture);

            //declare your varaibles here
            int _NumberOfColour;

            half4 frag(Varyings input) : SV_Target
            {
                //samples the texture
                float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord).rgb;
                color *= _NumberOfColour - 1;
                color += 0.5f;
                color = floor(color);
                color /= _NumberOfColour - 1;

                return float4(saturate(color),1);
            }

            ENDHLSL
        }
    }
}
