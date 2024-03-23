Shader "Hidden/SHARPEN"
{
  
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off

        Pass
        {
            Name "Sharpen Pass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            SAMPLER(sampler_BlitTexture);

            //declare your varaibles here
            float _Sharpness;
            float2 _TextureSize;

            half4 frag(Varyings input) : SV_Target
            {
                //samples the texture
                float3 colour = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord).rgb;
                
                float2 x = float2(1 / _TextureSize.x , 0);
                float2 y = float2(0 , 1 / _TextureSize.y);

                float3 topPix = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord + y).rgb * _Sharpness * -1;
                float3 btmPix = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord - y).rgb * _Sharpness * -1;
                float3 lftPix = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord + x).rgb * _Sharpness * -1;
                float3 rgtPix = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord - x).rgb * _Sharpness * -1;
                float3 neighbourPix = topPix + btmPix + lftPix + rgtPix;

                float3 currPixel = colour * 4 * _Sharpness + 1 + neighbourPix;

                return float4(clamp(colour * currPixel,0,1),0);
            }

            ENDHLSL
        }
    }
}
