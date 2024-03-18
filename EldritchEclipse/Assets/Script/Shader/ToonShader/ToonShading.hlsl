void ToonShading_float(
    in float3 Normal,
    in float ToonRampSmoothness,
    in float3 ClipSpacePos,
    in float3 WorldPos,
    in float4 ToonRampTinting,
    in float ToonRampOffset,
    in float ToonRampOffsetPoint,
    in float Ambient,
    out float3 ToonRampOutput,
    out float3 Direction
    )
{
    #ifdef SHADERGRAPH_PREVIEW
        //what is shown in the shader graph preview
        ToonRampOutput = float3(0.5,0.5,0);
        Direction = float3(0.5,0.5,0);
    #else
        //getting the shadows
        #if SHADOWS_SCREEN
            half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
        #else
            half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        #endif

        //getting the light information
        #if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
            Light light = GetMainLight(shadowCoord);
        #else
            Light light = GetMainLight();
        #endif

        //dot product to get shading
        half d = dot(Normal,light.direction) * 0.5 + 0.5;
        //smoothstep to cut off parts of the shading to achieve toon shade
        half toonRamp = smoothstep(ToonRampOffset, ToonRampOffset + ToonRampSmoothness, d);

        float3 extraLights = float3(0,0,0);
        //get the lights in the scene
        int pixelLightCount = GetAdditionalLightsCount();

        //loop through the lights
        for(int i = 0; i < pixelLightCount; i++)
        {
            //get additional lights in the scene
            Light aLight = GetAdditionalLight(i,WorldPos);

            //grab light, shadows and light colour
            float3 attenuatedLightColor = aLight.color * (aLight.distanceAttenuation * aLight.shadowAttenuation);

            //repeat the same toon shading on the additional lights
            half d = dot(Normal, aLight.direction) * 0.5 + 0.5;
            half toonRampExtra = smoothstep(ToonRampOffsetPoint, ToonRampOffsetPoint + ToonRampSmoothness, d);

            extraLights += (attenuatedLightColor * toonRampExtra);
        }


        toonRamp *= light.shadowAttenuation;

        ToonRampOutput = light.color * (toonRamp + ToonRampTinting) + Ambient;
        ToonRampOutput += saturate(extraLights);
        
        #if MAIN_LIGHT
            Direction = normalize(light.direction);
        #else
            Direction = float3(0.5,0.5,0);
        #endif
    #endif
}