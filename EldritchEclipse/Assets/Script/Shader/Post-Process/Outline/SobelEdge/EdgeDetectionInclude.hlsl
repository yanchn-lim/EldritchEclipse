
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

void DepthSobel_float(float2)

#endif