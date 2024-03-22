Shader "Hidden/ColorCorrection"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float _Contrast;
    float _Brightness;
    float _Saturation;
    float _Gamma;

    float4 _BlitTexture_TexelSize;

    float4 ColorCorrection(Varyings input) : SV_Target
    {
        float3 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp,input.texcoord).rgb;
        color = _Contrast * (color -0.5) + 0.5 + _Brightness; //applying contrast + brightness
        color = clamp(color.rgb,0,1); //clamp values before doing more processing

        //apply saturation
        float grayScale = dot(color,float3(0.299,0.587,0.114));
        color = lerp(grayScale,color,_Saturation);
        color = clamp(color.rgb, 0, 1); //clamp again

        //apply gamma correction
        color = pow(color, _Gamma);
        return float4(color.rgb,1);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off

        Pass
        {
            Name "ContrastPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment ColorCorrection

            ENDHLSL
        }
    }
}
