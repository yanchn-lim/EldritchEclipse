Shader "Hidden/EDGE DETECTION"
{
  
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off

        Pass
        {
            Name "Gaussian"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #define E 2.71828f

            #pragma vertex Vert
            #pragma fragment frag

            float2 _TexelSize;
            float _Spread;
            int _GridSize;

            float _K;
            float _Scalar;
            
            //generate gaussian value
            float gaussian(float sig,int x) 
            {
                float sigmaSqu = sig * sig;
                return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
            }

            float3 blur(float sig,float2 input) 
            {
                float3 col = float3(0, 0, 0);
                float gridSum = 0;
                int upper = ((_GridSize - 1) / 2);
                int lower = -upper;

                for (int x = lower; x <= upper; x++)
                {
                    float gauss = gaussian(sig,x);
                    gridSum += gauss;
                    float2 uv = input + float2(_TexelSize.x * x, 0.0f);
                    col += gauss * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv).rgb;
                }

                for (int y = lower; y <= upper; y++)
                {
                    float gauss = gaussian(sig,y);
                    gridSum += gauss;
                    float2 uv = input + float2(0.0f, _TexelSize.y * y);
                    col += gauss * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv).rgb;
                }

                col /= gridSum;
                return col;
            }

            float3 diffGaussian(float2 input) 
            {
                return /*(1 + _Scalar) * */blur(_Spread, input) - (_Scalar * blur(_Spread * _K, input));
            }

            half4 frag(Varyings input) : SV_Target
            { 
                //NAMES FOR DIFFERENT TEXTURES IN UNITY
                //_CameraNormalsTexture
                //_CameraDepthTexture
                
                //samples the texture
                float3 sampleCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord).rgb;

                return float4(diffGaussian(input.texcoord),1);
            }

            ENDHLSL
        }

        
    }
}
