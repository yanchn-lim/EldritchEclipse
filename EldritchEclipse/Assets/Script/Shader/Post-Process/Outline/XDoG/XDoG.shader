Shader "Hidden/XDoG"
{
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        #pragma shader_feature CALCDIFFBEFORECONVOLUTION
        #pragma shader_feature THRESHOLDING_1
        #pragma shader_feature THRESHOLDING_2
        #pragma shader_feature THRESHOLDING_3
        #pragma shader_feature THRESHOLDING_DEFAULT
        #pragma shader_feature BLEND_NONE
        #pragma shader_feature BLEND_INTERPOLATE
        #pragma shader_feature BLEND_TWO_POINT_INTERPOLATE
        #pragma shader_feature INVERT

        #pragma vertex Vert
        #pragma fragment frag

        Texture2D _TFM;
        sampler2D _DoGTex;
        SamplerState point_clamp_sampler;

        float _Threshold,_Threshold2,_Threshold3,_Threshold4,_Thresholds;
        float _SigmaC, _SigmaE, _SigmaA,_SigmaM;
        float _K, _Tau,_Phi;
        float _DoGStrength,_BlendStrength;
        float2 _TexelSize;
        float3 _MinColor,_MaxColor;
        float4 _IntegralConvolutionStepSizes;

        sampler2D _GaussianTex,_GaussianTex2,_GaussianTex3,_TempTex;

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

        //color conversion
        float3 rgb2xyz(float3 c) {
            float3 tmp;

            tmp.x = (c.r > 0.04045) ? pow((c.r + 0.055) / 1.055, 2.4) : c.r / 12.92;
            tmp.y = (c.g > 0.04045) ? pow((c.g + 0.055) / 1.055, 2.4) : c.g / 12.92,
            tmp.z = (c.b > 0.04045) ? pow((c.b + 0.055) / 1.055, 2.4) : c.b / 12.92;
            
            const float3x3 mat = float3x3(
                0.4124, 0.3576, 0.1805,
                0.2126, 0.7152, 0.0722,
                0.0193, 0.1192, 0.9505 
            );

            return 100.0 * mul(tmp, mat);
        }

        float3 xyz2lab(float3 c) {
            float3 n = c / float3(95.047, 100, 108.883);
            float3 v;

            v.x = (n.x > 0.008856) ? pow(n.x, 1.0 / 3.0) : (7.787 * n.x) + (16.0 / 116.0);
            v.y = (n.y > 0.008856) ? pow(n.y, 1.0 / 3.0) : (7.787 * n.y) + (16.0 / 116.0);
            v.z = (n.z > 0.008856) ? pow(n.z, 1.0 / 3.0) : (7.787 * n.z) + (16.0 / 116.0);

            return float3((116.0 * v.y) - 16.0, 500.0 * (v.x - v.y), 200.0 * (v.y - v.z));
        }

        float3 rgb2lab(float3 c) {
            float3 lab = xyz2lab(rgb2xyz(c));

            return float3(lab.x / 100.0f, 0.5 + 0.5 * (lab.y / 127.0), 0.5 + 0.5 * (lab.z / 127.0));
        }
        ENDHLSL

        //pass to find the eigenvector
        Pass{
            Name "Sobel"

            HLSLPROGRAM
            half4 frag(Varyings input) : SV_Target
            {
                float2 d = _TexelSize;

            
                //apply sobel operator
                float3 Sx = float3(
                    1.0f * GetBlitTexCol(input.texcoord + float2(-d.x, -d.y)).rgb +
                    2.0f * GetBlitTexCol(input.texcoord + float2(-d.x, 0)).rgb  +
                    1.0f * GetBlitTexCol(input.texcoord + float2(-d.x, d.y)).rgb  +

                    -1.0f * GetBlitTexCol(input.texcoord + float2(d.x, -d.y)).rgb  +
                    -2.0f * GetBlitTexCol(input.texcoord + float2(d.x, 0)).rgb  +
                    -1.0f * GetBlitTexCol(input.texcoord + float2(d.x, d.y)).rgb 
                )/4.0f;

                float Sy = float3(
                    1.0f * GetBlitTexCol(input.texcoord + float2(-d.x, -d.y)).rgb  +
                    2.0f * GetBlitTexCol(input.texcoord + float2(0, -d.y)).rgb  +
                    1.0f * GetBlitTexCol(input.texcoord + float2(d.x, -d.y)).rgb  +

                    -1.0f * GetBlitTexCol(input.texcoord + float2(-d.x, d.y)).rgb  +
                    -2.0f * GetBlitTexCol(input.texcoord + float2(0, d.y)).rgb  +
                    -1.0f * GetBlitTexCol(input.texcoord + float2(d.x, d.y)).rgb 
                )/4.0f;

                return float4(dot(Sx,Sx),dot(Sy,Sy),dot(Sx,Sy),1);
            }

            ENDHLSL
        }

        //horizontal gaussian pass on eigenvector
        Pass{
            Name "GaussianHorizontal"

            HLSLPROGRAM

            half4 frag(Varyings input) : SV_Target
            {
                //gauss blur the "Smooth Structure Tensor"?
                int kernelRadius = max(1.0f,floor(_SigmaC * 2.45f));

                float4 col = 0;
                float kernelSum = 0;

                for(int x = -kernelRadius; x<= kernelRadius; x++){
                    float4 c = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord + float2(x,0) * _TexelSize);
                    float gauss = gaussian(_SigmaC,x);

                    col += c * gauss;
                    kernelSum += gauss;
                }

                return col / kernelSum;
            }

            ENDHLSL
        }

        //vertical gaussian pass on eigenvector
        Pass{
            Name "GaussianVertical"
            HLSLPROGRAM
            half4 frag(Varyings input) : SV_Target
            {
                //gauss blur the "Smooth Structure Tensor"?
                //same as pass 1 but do it vertically
                int kernelRadius = max(1.0f,floor(_SigmaC * 2.45f));

                float4 col = 0;
                float kernelSum = 0;

                for(int x = -kernelRadius; x<= kernelRadius; x++){
                    float4 c = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord + float2(0,x) * _TexelSize);
                    float gauss = gaussian(_SigmaC,x);

                    col += c * gauss;
                    kernelSum += gauss;
                }

                return col / kernelSum;
            }
            ENDHLSL
        }

        //use the blurred eigen to get the a blur based off flow
        Pass{
            Name "FDoG Blur 1"

            HLSLPROGRAM

            half4 frag(Varyings input) : SV_Target
            {
                //apply "Flow-based Difference of Gaussian" FDoG
                float2 t = _TFM.Sample(point_clamp_sampler,input.texcoord).xy;
                float2 n = float2(t.y,-t.x);
                float2 nabs = abs(n);
                float ds = 1.0/ ((nabs.x > nabs.y) ? nabs.x : nabs.y);
                n *= _TexelSize;

                float2 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord).rr;
                float2 kernelSum = 1.0f;

                int kernelSize = (_SigmaE * 2 > 1) ? floor(_SigmaE * 2) : 1;

                [loop]
                for(int x = ds; x <= kernelSize; x++){
                    float gauss1  = gaussian(_SigmaE,x);
                    float gauss2  = gaussian(_SigmaE * _K, x);

                    float c1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord - x * n).r;
                    float c2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord + x * n).r;

                    col.r += (c1 + c2) * gauss1;
                    kernelSum.x += 2.0f * gauss1;

                    col.g += (c1 + c2) * gauss2;
                    kernelSum.y += 2.0f * gauss2;
                }

                col /= kernelSum;

                return float4(col,(1 + _Tau) * (col.r * 100.0f) - _Tau * (col.g * 100.0f),1.0f);
             }
             ENDHLSL
        }

        //2nd FDoG blur
        Pass{
            Name "FDoG Blur 2"

            HLSLPROGRAM
            half4 frag(Varyings input) : SV_Target
            {
                float kernelSize = _SigmaM * 2;

                float2 w = 1.0f;
                float3 c = GetBlitTexCol(input.texcoord);

                float2 G = 0;
                //change to keyword
                #if CALCDIFFBEFORECONVOLUTION
                    G = float2(c.b,0.0f);
                #else
                    G = c.rg;
                #endif

                float2 v = _TFM.Sample(point_clamp_sampler,input.texcoord).xy * _TexelSize;

                float2 st0 = input.texcoord;
                float2 v0 = v;

                [loop]
                for(int d = 1; d <= kernelSize; d++){
                    st0 += v0 * _IntegralConvolutionStepSizes.x;
                    float3 c = GetBlitTexCol(st0).rgb;
                    float gauss1  = gaussian(_SigmaM,d);

                    #if CALCDIFFBEFORECONVOLUTION
                        G.r += gauss1 * c.b;
                        w.x += gauss1;
                    #else
                        float2 gauss2 = gaussian(_SigmaM * _K ,d);
                        G.r += gauss1 * c.r;
                        w.x += gauss1;
                        G.g += gauss2 * c.g;
                        w.y += gauss2;
                    #endif
                    v0 = _TFM.Sample(point_clamp_sampler,st0).xy * _TexelSize;
                }

                float2 st1 = input.texcoord;
                float2 v1= v;

                [loop]
                for(int d = 1; d <= kernelSize; d++){
                    st1 += v1 * _IntegralConvolutionStepSizes.y;
                    float3 c = GetBlitTexCol(st1).rgb;

                    float gauss1  = gaussian(_SigmaM,d);

                    #if CALCDIFFBEFORECONVOLUTION
                        G.r += gauss1 * c.b;
                        w.x += gauss1;
                    #else
                        float2 gauss2 = gaussian(_SigmaM * _K ,d);
                        G.r += gauss1 * c.r;
                        w.x += gauss1;
                        G.g += gauss2 * c.g;
                        w.y += gauss2;
                    #endif
                    v1 = _TFM.Sample(point_clamp_sampler,st1).xy * _TexelSize;
                }

                G /= w;

                float4 D = 0.0f;
                #if CALCDIFFBEFORECONVOLUTION
                    D = G.x;
                #else
                    D = (1 + _Tau) * (G.r * 100.0f) - _Tau * (G.g * 100.0f);
                #endif

                float4 output = 0.0f;

                #if THRESHOLDING_1
                    output.r = (D >= _Threshold) ? 1 : 1 + tanh(_Phi * (D - _Threshold));
                    output.g = (D >= _Threshold2) ? 1 : 1 + tanh(_Phi * (D - _Threshold2));
                    output.b = (D >= _Threshold3) ? 1 : 1 + tanh(_Phi * (D - _Threshold3));
                    output.a = (D >= _Threshold4) ? 1 : 1 + tanh(_Phi * (D - _Threshold4));
                #endif

                #if THRESHOLDING_2
                    float a = 1.0f / _Thresholds;
                    float b = _Threshold / 100.0f;
                    float x = D / 100.0f;

                    output = (x >= b) ? 1 : a * floor((pow(x, _Phi) - (a * b / 2.0f)) / (a * b) + 0.5f);
                #endif

                #if THRESHOLDING_3
                    float x = D / 100.0f;
                    float qn = floor(x * float(_Thresholds) + 0.5f) / float(_Thresholds);
                    float qs = smoothstep(-2.0, 2.0, _Phi * (x - qn) * 10.0f) - 0.5f;
                    
                    output = qn + qs / float(_Thresholds);
                #endif

                #if THRESHOLDING_DEFAULT
                    output = D /100.0f;
                #endif

                #if INVERT
                    output = 1- output;
                #endif

                return saturate(output);
            }
            ENDHLSL
        }

        //anti-alias
        Pass{
            Name "Anti-Alias"
            
            HLSLPROGRAM

            half4 frag(Varyings input) : SV_Target
            {
                float kernelSize = _SigmaA * 2;

                float4 G = GetBlitTexCol(input.texcoord);
                float w = 1.0f;

                float2 v = _TFM.Sample(point_clamp_sampler, input.texcoord).xy * _TexelSize;

                float2 st0 = input.texcoord;
                float2 v0 = v;

                [loop]
                for (int d = 1; d < kernelSize; ++d) {
                    st0 += v0 * _IntegralConvolutionStepSizes.z;
                    float4 c = GetBlitTexCol(st0);
                    float gauss1 = gaussian(_SigmaA, d);

                    G += gauss1 * c;
                    w += gauss1;

                    v0 = _TFM.Sample(point_clamp_sampler, st0).xy * _TexelSize;
                }

                float2 st1 = input.texcoord;
                float2 v1 = v;

                [loop]
                for (int d = 1; d < kernelSize; ++d) {
                    st1 -= v1 * _IntegralConvolutionStepSizes.w;
                    float4 c = GetBlitTexCol(st1);
                    float gauss1 = gaussian(_SigmaA, d);

                    G += gauss1 * c;
                    w += gauss1;

                    v1 = _TFM.Sample(point_clamp_sampler, st1).xy * _TexelSize;
                }

                return G / w;
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
        
        //color convert
        Pass{
            Name "Color Conversion"

            HLSLPROGRAM

            half4 frag(Varyings input) : SV_Target
            {
                return float4(rgb2lab(GetBlitTexCol(input.texcoord)), 1.0f);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Overlay"

            HLSLPROGRAM

            half4 frag(Varyings input) : SV_Target
            {
                //NAMES FOR DIFFERENT TEXTURES IN UNITY
                //_CameraNormalsTexture
                //_CameraDepthTexture
                float3 G = tex2D(_GaussianTex3, input.texcoord).rgb;
                float3 C = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord).rgb;
               
                
                G = G + C;
                G = saturate(G);
                return float4(G,1);
            }
            ENDHLSL
            
        }

        
    }
}
