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

        sampler2D _GaussianTex,_GaussianTex2,_GaussianTex3,_TempTex;
        float2 _TexelSize;
        float _Spread, _K, _Tau, _Threshold,_Phi;
        int _GridSize;
        float4 _Colour;

        //generate gaussian value
        float gaussian(float sig,int x)
        {
            float sigmaSqu = sig * sig;
            //pow(E, -(x * x) / (2 * sigmaSqu))
            return (1 / sqrt(TWO_PI * sigmaSqu)) * exp(-(x * x) / (2 * sigmaSqu));
        }

        float luminance(float3 col) {
            return dot(col, float3(0.299f, 0.587f, 0.114f));
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

                    col.r += gauss * luminance(texCol);
                    col.g += gauss2 * luminance(texCol);
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
                        col.r += gauss * luminance(texCol);
                        col.g += gauss2 * luminance(texCol);
                    }

                    col.r /= gridSum;
                    col.g /= gridSum2;
                    float2 texCol = tex2D(_GaussianTex, input.texcoord).rg;
                    col = saturate(col + texCol);
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
                float2 G = tex2D(_GaussianTex2, input.texcoord).rg;
                
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

        Pass
        {
            Name "Overlay"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                //NAMES FOR DIFFERENT TEXTURES IN UNITY
                //_CameraNormalsTexture
                //_CameraDepthTexture
                float3 G = tex2D(_GaussianTex3, input.texcoord).rgb;
                float3 C = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord).rgb;
               
                
                G *= _Colour / C;

                return float4(G,1);
            }
            ENDHLSL
            
        }

        
    }
}
