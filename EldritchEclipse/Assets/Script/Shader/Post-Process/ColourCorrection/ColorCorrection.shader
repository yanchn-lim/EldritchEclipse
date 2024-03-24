Shader "Hidden/ColorCorrection"
{
  
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off

        Pass
        {
            Name "ContrastPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            SAMPLER(sampler_BlitTexture);

            float _Contrast;
            float _Brightness;
            float _Saturation;
            float _Gamma;

            half4 frag(Varyings input) : SV_Target
            {
                //float3 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture,sampler_CameraOpaqueTexture,input.texcoord).rgb;
                float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord).rgb;

                //apply contrast + brightness
                color = _Contrast * (color - 0.5) + 0.5 + _Brightness;
                color = saturate(color) //clamp values before doing more processing

                //apply saturation
                float grayScale = dot(color,float3(0.299,0.587,0.114));
                color = lerp(grayScale,color,_Saturation);
                color = saturate(color); //clamp again

                //apply gamma correction
                color = pow(color, _Gamma);
                return float4(color,1);
            }

            ENDHLSL
        }
    }
}
