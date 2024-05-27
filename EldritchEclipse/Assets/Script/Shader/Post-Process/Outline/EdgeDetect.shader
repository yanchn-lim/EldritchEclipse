Shader "Hidden/Edge Detection"
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
        #pragma shader_feature BLEND_NONE
        #pragma shader_feature BLEND_INTERPOLATE
        #pragma shader_feature BLEND_TWO_POINT_INTERPOLATE

        #pragma vertex Vert
        #pragma fragment frag

        float _Threshold;
        float _Sigma;
        float _K, _Tau,_Phi;
        float2 _TexelSize;
        int _GridSize;
        float _DoGStrength,_BlendStrength;
        float3 _MinColor,_MaxColor;
        sampler2D _GaussianTex,_GaussianTex2,_DoGTex;

        //generate gaussian value
        float gaussian(float sig,int x)
        {
            float sigmaSqu = sig * sig;
            return (1.0f / sqrt(TWO_PI * sigmaSqu)) * exp(-(x * x) / (2.0f * sigmaSqu));
        }

        float luminance(float3 col) {
            return dot(col, float3(0.299f, 0.587f, 0.114f));
        }

        float4 GetBlitTexCol(float2 uv){
            return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv);
        }
        ENDHLSL


        Pass{
            Name "GaussianHorizontal"

            HLSLPROGRAM

            half4 frag(Varyings input) : SV_Target
            {
                float2 col = 0;
                float gridSum = 0;
                float gridSum2 = 0;

                for (int x = -_GridSize; x <= _GridSize; x++)
                {
                    float gauss = gaussian(_Sigma,x);
                    float gauss2 = gaussian(_Sigma * _K, x);

                    gridSum += gauss;
                    gridSum2 += gauss2;
                    float2 uv = input.texcoord + float2(_TexelSize.x * x, 0.0f);
                    float lum = luminance(GetBlitTexCol(uv));

                    col.r += gauss * lum;
                    col.g += gauss2 * lum;
                }

                col.r /= gridSum;
                col.g /= gridSum2;
                
                return float4(col.r,col.g,0,1);            
            }

            ENDHLSL
        }

        Pass{
            Name "GaussianVertical"

            HLSLPROGRAM
            half4 frag(Varyings input) : SV_Target
            {              
                float2 col = 0;
                float gridSum = 0;
                float gridSum2 = 0;
                
                for (int x = -_GridSize; x <= _GridSize; x++)
                {
                    float gauss = gaussian(_Sigma,x);
                    float gauss2 = gaussian(_Sigma * _K, x);

                    gridSum += gauss;
                    gridSum2 += gauss2;
                    float2 uv = input.texcoord + float2(0.0f, _TexelSize.y * x);
                    float lum = luminance(GetBlitTexCol(uv));

                    col.r += gauss * lum;
                    col.g += gauss2 * lum;
                }
                
                col.r /= gridSum;
                col.g /= gridSum2;
                float2 texCol = tex2D(_GaussianTex, input.texcoord).rg;
                col = saturate(col + texCol);
                return float4(col.r,col.g,0,1);
                
            }
            ENDHLSL
        }

        Pass{
            Name "DoG"

            HLSLPROGRAM

            half4 frag(Varyings input) : SV_Target
            {         
                float2 G = tex2D(_GaussianTex2,input.texcoord).rg;
                float4 D = (1 + _Tau) * G.r - _Tau * G.g;

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
                return saturate(D);
                
             }
             ENDHLSL
            }

        //blend
        Pass{
            Name "Blend"

            HLSLPROGRAM
            half4 frag(Varyings input) : SV_Target{

                float4 D = tex2D(_DoGTex, input.texcoord) * _DoGStrength;
                float3 col = GetBlitTexCol(input.texcoord);

                float4 output = 0.0f;

                #if BLEND_NONE
                    output.rgb = lerp(_MinColor, _MaxColor, D.r);
                #endif

                #if BLEND_INTERPOLATE
                    output.rgb = lerp(_MinColor, col, D.r);
                #endif

                #if BLEND_TWO_POINT_INTERPOLATE
                    if (D.r < 0.5f)
                        output.rgb = lerp(_MinColor, col, D.r * 2.0f);
                    else
                        output.rgb = lerp(col, _MaxColor, (D.r - 0.5f) * 2.0f);
                #endif

                return saturate(float4(lerp(col, output, _BlendStrength), 1.0f));
            }
            ENDHLSL
        }       
    }
}
