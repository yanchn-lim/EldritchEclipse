
#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

static float2 sobelSamplePoints[9] =
{
    float2(-1, 1), float2(0, 1), float2(1, 1),
    float2(-1, 0), float2(0, 0), float2(1, 0),
    float2(-1, -1), float2(0, -1), float2(1, -1),
};

static float sobelXMatrix[9] =
{
    1, 0, -1,
    2, 0, -2,
    1, 0, -1
};

static float sobelYMatrix[9] =
{
    1, 2, 1,
    0, 0, 0,
    - 1, -2, -1
};

void DepthSobel_float(
    in float4 LT, in float4 T, in float4 RT, 
    in float4 LM, in float4 M, in float4 RM, 
    in float4 LB, in float4 B, in float4 RB, 
    out float Color)
{
    Color = 0;
    Color += (LT * 1) + (T * 0) + (RT * -1);
    Color += (LM * 2) + (M * 0) + (RM * -2);
    Color += (LB * 1) + (B * 0) + (RB * -1);
    
    Color += (LT *  1) + (T *  2) + (RT *  1);
    Color += (LM *  0) + (M *  0) + (RM *  0);
    Color += (LB * -1) + (B * -2) + (RB * -1); 
    
    //Color = saturate(Color);

}

#endif