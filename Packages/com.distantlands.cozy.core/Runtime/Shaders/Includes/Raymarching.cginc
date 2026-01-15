TEXTURE2D_SHADOW(_DirectionalShadowAtlas);
#define SHADOW_SAMPLER sampler_linear_clamp_compare
SAMPLER_CMP(SHADOW_SAMPLER);

half GetShadowAttenuation(half3 RayPos)
{
	half4 shadowPos = TransformWorldToShadowCoord(RayPos);
    half4 shadowParams = GetMainLightShadowParams();
    float shadowStrength = shadowParams.x;
    float attenuation = _MainLightShadowmapTexture.SampleCmpLevelZero(SHADOW_SAMPLER, shadowPos.xy, shadowPos.z);
	return saturate(lerp(BEYOND_SHADOW_FAR(shadowPos) ? 1.0 : attenuation, 1.0, GetMainLightShadowFade(RayPos)));
}

float Raymarching( inout float3 position, float3 direction, int maxSteps, float stepSize, float depth, float sensitivity, float densityMultiplier, out float density )
{
	density = 0;
	float shadowAtten = 0;
	for (int i = 0; i < maxSteps; i++) {
	if (density + stepSize < depth) {
		position+=direction*stepSize;
		density += stepSize;
		shadowAtten += GetShadowAttenuation(position) * sensitivity;
	} else
	return shadowAtten;
	}
	return shadowAtten;
}