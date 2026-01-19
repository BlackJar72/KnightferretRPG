#ifndef KFCUSTOM_LIGHTING_INCLUDED
#define KFCUSTOM_LIGHTING_INCLUDED


void GetMeshPos_float(out float3 MeshPosition){
    MeshPosition = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
}


void GetBrightestLight_float(float3 MeshPosition, out float3 LightColor) {
    LightColor = float3(0, 0, 0);
#ifndef SHADERGRAPH_PREVIEW
    int numLights = GetAdditionalLightsCount();
    for(int i = 0; i < numLights; i++) {
        Light light = GetAdditionalPerObjectLight(i, MeshPosition);
        LightColor = max(LightColor, light.color * sqrt(light.distanceAttenuation) * light.shadowAttenuation);
    }
    LightColor = saturate(LightColor);
#endif
}


void MainLightAttenuation_float(float3 pos, out float ShadowAttenuation) {
    ShadowAttenuation = 1;
#if defined(UNIVERSAL_LIGHTING_INCLUDED)
    #if SHADOWS_SCREEN
        half4 clipPos = TransformWorldToHClip(pos);
        half4 shadowCoord = ComputeScreenPos(clipPos);
    #else
        half4 shadowCoord = TransformWorldToShadowCoord(pos);
    #endif
    Light light = GetMainLight(shadowCoord);
    ShadowAttenuation = light.shadowAttenuation;
#endif
}



#endif
