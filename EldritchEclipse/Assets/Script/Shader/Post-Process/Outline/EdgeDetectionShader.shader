Shader "Hidden/EDGE DETECTION"
{
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        #pragma shader_feature THRESHOLDING
        #pragma shader_feature TANH
        #pragma shader_feature INVERT

        Texture2D _GaussianTex;
        float2 _TexelSize;
        float _Spread, _K, _Tau, _Threshold,_Phi;
        int _GridSize;
        

        //generate gaussian value
        float gaussian(float sig,int x)
        {
            float sigmaSqu = sig * sig;
            //pow(E, -(x * x) / (2 * sigmaSqu))
            return (1 / sqrt(TWO_PI * sigmaSqu)) * exp(-(x * x) / (2 * sigmaSqu));
        }

        ENDHLSL

            Pass
        {
            Name "GaussianHorizontal"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                //NAMES FOR DIFFERENT TEXTURES IN UNITY
                //_CameraNormalsTexture
                //_CameraDepthTexture
                float2 col = 0;
                float gridSum = 0;
                float gridSum2 = 0;
                int upper = ((_GridSize - 1) / 2);
                int lower = -upper;

                for (int x = lower; x <= upper; x++)
                {
                    float gauss = gaussian(_Spread,x);
                    float gauss2 = gaussian(_Spread * _K, x);

                    gridSum += gauss;
                    gridSum2 += gauss2;
                    float2 uv = input.texcoord + float2(_TexelSize.x * x, 0.0f);
                    float3 texCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv).rgb;

                    col.r += gauss * length(texCol);
                    col.g += gauss2 * length(texCol);
                }

                col.r /= gridSum;
                col.g /= gridSum2;

                return float4(col.r,col.g,0,1);
            }

            ENDHLSL
    }

        Pass
            {
                Name "GaussianVertical"

                HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment frag


                half4 frag(Varyings input) : SV_Target
                {
                    //NAMES FOR DIFFERENT TEXTURES IN UNITY
                    //_CameraNormalsTexture
                    //_CameraDepthTexture

                    float2 col = 0;
                    float gridSum = 0;
                    float gridSum2 = 0;
                    int upper = ((_GridSize - 1) / 2);
                    int lower = -upper;

                    for (int x = lower; x <= upper; x++)
                    {
                        float gauss = gaussian(_Spread,x);
                        float gauss2 = gaussian(_Spread * _K, x);

                        gridSum += gauss;
                        gridSum2 += gauss2;
                        float2 uv = input.texcoord + float2(0.0f, _TexelSize.y * x);
                        float3 texCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv).rgb;
                        col.r += gauss * length(texCol);
                        col.g += gauss2 * length(texCol);
                    }

                    col.r /= gridSum;
                    col.g /= gridSum2;
                    return float4(col.r,col.g,0,1);
                }

                ENDHLSL
            }

                Pass
                {
                        Name "DoG"

                        HLSLPROGRAM
                        #pragma vertex Vert
                        #pragma fragment frag


                        half4 frag(Varyings input) : SV_Target
                        {
                        //NAMES FOR DIFFERENT TEXTURES IN UNITY
                        //_CameraNormalsTexture
                        //_CameraDepthTexture
                        float2 G = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord).rg;
                        
                        float4 D = (G.r - _Tau * G.g);

#if THRESHOLDING

#if TANH
                            D = (D >= _Threshold) ? 1 : 1 + tanh(_Phi * (D - _Threshold));
#else

                            D = (D > -_Threshold) ? 1 : 0;
#endif

#endif

#if INVERT
                        D = 1 - D;
#endif
                        return D;
                    }
                    ENDHLSL
            }
        
    }
}
