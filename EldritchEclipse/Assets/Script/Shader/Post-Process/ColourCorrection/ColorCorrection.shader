Shader "Hidden/ColorCorrection"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float _Contrast;

    float4 _BlitTexture_TexelSize;

    float4 ApplyContrast(Varyings input) : SV_Target
    {
        float3 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp,input.texcoord).rgb;
        color = _Contrast * (color -0.5) + 0.5;
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
            #pragma fragment ApplyContrast

            ENDHLSL
        }
    }
}
