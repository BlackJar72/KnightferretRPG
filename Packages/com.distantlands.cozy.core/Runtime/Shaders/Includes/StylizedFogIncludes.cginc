
#ifndef COZY_INCLUDED
#define COZY_INCLUDED

uniform float4 CZY_FogColor1;
uniform float4 CZY_FogColor2;
uniform float4 CZY_FogColor3;
uniform float4 CZY_FogColor4;
uniform float4 CZY_FogColor5;

uniform float CZY_FogColorStart1;
uniform float CZY_FogColorStart2;
uniform float CZY_FogColorStart3;
uniform float CZY_FogColorStart4;

uniform half CZY_FogDepthMultiplier;

uniform float4 CZY_LightColor;
uniform float3 CZY_SunDirection;
uniform half CZY_LightIntensity;
uniform half CZY_LightFalloff;

uniform float CZY_FogSmoothness;
uniform float CZY_FogOffset;
uniform float CZY_FogIntensity;
uniform sampler2D _FogVariationTexture;
uniform	float3 CZY_VariationWindDirection;
uniform	float CZY_VariationScale;
uniform	float CZY_VariationAmount;
uniform	float CZY_VariationDistance;
uniform	float3 CZY_MoonDirection;
uniform	float4 CZY_FogMoonFlareColor;
uniform	float CZY_LightFlareSquish;
uniform	float CZY_FilterSaturation;
uniform	float CZY_FilterValue;
uniform	float4 CZY_FilterColor;
uniform	float4 CZY_SunFilterColor;


inline float4 ASE_ComputeGrabScreenPos(float4 pos)
{
#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
#else
	float scale = 1.0;
#endif
	float4 o = pos;
	o.y = pos.w * 0.5f;
	o.y = (pos.y - o.y) * _ProjectionParams.x * scale + o.y;
	return o;
}


float3 HSVToRGB(float3 c)
{
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}


float3 RGBToHSV(float3 c)
{
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float4 Debug(float4 input) {

	return CZY_FogColor3;

}

float4 BlendStylizedFog(float3 worldPos, float4 inColor) {

	//Get Depth
	float eyeDepth = distance(worldPos, _WorldSpaceCameraPos);
	float fogDepth = (CZY_FogDepthMultiplier * sqrt(eyeDepth));

	//5 COLOR FOG
	float4 c1toc2 = lerp( CZY_FogColor1 , CZY_FogColor2 , saturate( ( fogDepth / CZY_FogColorStart1 ) ));
	float4 c2toc3 = lerp( c1toc2 , CZY_FogColor3 , saturate( ( ( CZY_FogColorStart1 - fogDepth ) / ( CZY_FogColorStart1 - CZY_FogColorStart2 ) ) ));
	float4 c3toc4 = lerp( c2toc3 , CZY_FogColor4 , saturate( ( ( CZY_FogColorStart2 - fogDepth ) / ( CZY_FogColorStart2 - CZY_FogColorStart3 ) ) ));
	float4 tempFogColor = lerp( c3toc4 , CZY_FogColor5 , saturate( ( ( CZY_FogColorStart3 - fogDepth ) / ( CZY_FogColorStart3 - CZY_FogColorStart4 ) ) ));
	float3 fogHSV = RGBToHSV( tempFogColor.rgb );
	//VARIATION
	float variatedFogDepth = lerp(fogDepth, (fogDepth * ((1.0 - CZY_VariationAmount) + (tex2D( _FogVariationTexture, ((worldPos).xz + ((CZY_VariationWindDirection).xz * _TimeParameters.x ))*( 0.1 / CZY_VariationScale))).r) * (1.0 - ( 1.0 - CZY_VariationAmount)) / (1.0)), (1.0 - saturate(fogDepth / CZY_VariationDistance)));
	float newFogAlpha = (tempFogColor).a * saturate((CZY_FogDepthMultiplier * sqrt(variatedFogDepth)));


	//SUN FLARE
	float3 directionAndLengthToClipPlane = normalize(worldPos - _WorldSpaceCameraPos) * _ProjectionParams.z;
	float sunFlare = dot( normalize( ( ( directionAndLengthToClipPlane * (float3(1.0 , CZY_LightFlareSquish , 1.0)) ) - _WorldSpaceCameraPos ) ) , CZY_SunDirection );
	half modifiedSunFlare = saturate( pow( abs( ( (sunFlare*0.5 + 0.5) * CZY_LightIntensity ) ) , CZY_LightFalloff ) );
	float3 hsvTorgb2_g56 = RGBToHSV( ( CZY_LightColor * fogHSV.z * saturate( ( modifiedSunFlare * ( 1.5 * newFogAlpha ) ) ) ).rgb );
	float3 filteredFogColor = ( float4( HSVToRGB( float3(hsvTorgb2_g56.x,saturate( ( hsvTorgb2_g56.y + CZY_FilterSaturation ) ),( hsvTorgb2_g56.z + CZY_FilterValue )) ) , 0.0 ) * CZY_FilterColor );
	float3 viewDirection = ( worldPos - _WorldSpaceCameraPos );
	half moonMask = saturate( pow( abs( ( saturate( (dot( normalize( viewDirection ) , normalize( CZY_MoonDirection ) )*1.0 + 0.0) ) * CZY_LightIntensity ) ) , ( CZY_LightFalloff * 3.0 ) ) );
	float3 moonColorHSV = RGBToHSV( ( tempFogColor + ( fogHSV.z * saturate( ( newFogAlpha * moonMask ) ) * CZY_FogMoonFlareColor ) ).rgb );
	float3 moonColorRGB = HSVToRGB(float3(moonColorHSV.x,saturate((moonColorHSV.y + CZY_FilterSaturation)),(moonColorHSV.z + CZY_FilterValue ))) * CZY_FilterColor;
					
	float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
					
	float4 fogColor = float4(( ( filteredFogColor * CZY_SunFilterColor.rgb ) + moonColorRGB ).rgb, 1);
	// float finalAlpha = ( newFogAlpha * saturate( ( ( 1.0 - saturate( ( ( ( viewDirection.y * 0.1 ) * ( 1.0 / ( ( CZY_FogSmoothness * length( ase_objectScale ) ) * 10.0 ) ) ) + ( 1.0 - CZY_FogOffset ) ) ) ) * CZY_FogIntensity ) ) );
	float finalAlpha = newFogAlpha;

	return lerp(inColor, fogColor, finalAlpha);


}

float GetStylizedFogDensity(float3 worldPos) {



	//float4 ase_screenPos = float4(i.screenPos.xyz, i.screenPos.w + 0.00000000001);
	//float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
	//ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
	float eyeDepth = distance(worldPos, _WorldSpaceCameraPos);


	float fogDepth = (CZY_FogDepthMultiplier * sqrt(eyeDepth));

	float4 lerpResult28_g2 = lerp(CZY_FogColor1, CZY_FogColor2, saturate((fogDepth / CZY_FogColorStart1)));
	float4 lerpResult41_g2 = lerp(saturate(lerpResult28_g2), CZY_FogColor3, saturate(((CZY_FogColorStart1 - fogDepth) / (CZY_FogColorStart1 - CZY_FogColorStart2))));
	float4 lerpResult35_g2 = lerp(lerpResult41_g2, CZY_FogColor4, saturate(((CZY_FogColorStart2 - fogDepth) / (CZY_FogColorStart2 - CZY_FogColorStart3))));
	float4 lerpResult113_g2 = lerp(lerpResult35_g2, CZY_FogColor5, saturate(((CZY_FogColorStart3 - fogDepth) / (CZY_FogColorStart3 - CZY_FogColorStart4))));

	float4 tempFogColor = lerpResult113_g2;
	float3 hsvTorgb31_g1 = RGBToHSV(CZY_LightColor.rgb);
	float3 hsvTorgb32_g1 = RGBToHSV(tempFogColor.rgb);
	float3 hsvTorgb39_g1 = HSVToRGB(float3(hsvTorgb31_g1.x, hsvTorgb31_g1.y, (hsvTorgb31_g1.z * hsvTorgb32_g1.z)));
	float3 normalizeResult5_g1 = normalize((worldPos - _WorldSpaceCameraPos));
	float dotResult6_g1 = dot(normalizeResult5_g1, CZY_SunDirection);
	half lightMask = saturate(pow(abs(((dotResult6_g1 * 0.5 + 0.5) * CZY_LightIntensity)), CZY_LightFalloff));

	float alpha = saturate(((1.0 - saturate(((((worldPos - _WorldSpaceCameraPos).y * 0.1) * (1.0 / CZY_FogSmoothness)) + (1.0 - CZY_FogOffset)))) * CZY_FogIntensity));
	float finalFogDensity = (tempFogColor.a * saturate(fogDepth)) * alpha;



	return finalFogDensity;


}
#endif