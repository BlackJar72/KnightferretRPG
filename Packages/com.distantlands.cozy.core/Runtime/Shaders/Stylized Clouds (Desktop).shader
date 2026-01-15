// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/URP/Stylized Clouds (COZY Desktop)"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent-50" "UniversalMaterialType"="Unlit" }

		Cull Front
		AlphaToMask Off

		Stencil
		{
			Ref 221
			Comp Always
			Pass Zero
			Fail Keep
			ZFail Keep
		}

		HLSLINCLUDE
		#pragma target 3.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			

			#pragma multi_compile_fragment _ALPHATEST_ON
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 140010


			

			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

			

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD2;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD3;
				#endif
				#if defined(LIGHTMAP_ON)
					float4 lightmapUVOrVertexSH : TEXCOORD4;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD5;
				#endif
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			float4 CZY_CloudColor;
			float CZY_FilterSaturation;
			float CZY_FilterValue;
			float4 CZY_FilterColor;
			float4 CZY_CloudFilterColor;
			float4 CZY_CloudHighlightColor;
			float4 CZY_SunFilterColor;
			float CZY_WindSpeed;
			float CZY_MainCloudScale;
			float CZY_CumulusCoverageMultiplier;
			float3 CZY_SunDirection;
			half CZY_SunFlareFalloff;
			float3 CZY_MoonDirection;
			half CZY_CloudMoonFalloff;
			float4 CZY_CloudMoonColor;
			float CZY_DetailScale;
			float CZY_DetailAmount;
			float CZY_BorderHeight;
			float CZY_BorderVariation;
			float CZY_BorderEffect;
			float3 CZY_StormDirection;
			float CZY_NimbusHeight;
			float CZY_NimbusMultiplier;
			float CZY_NimbusVariation;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_ClippingThreshold;
			float4 CZY_AltoCloudColor;
			sampler2D CZY_AltocumulusTexture;
			float2 CZY_AltocumulusWindSpeed;
			float CZY_AltocumulusScale;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float4 CZY_LightColor;
			float4 CZY_FogColor5;
			float CZY_LightFlareSquish;
			half CZY_LightIntensity;
			half CZY_LightFalloff;
			float CZY_CloudsFogLightAmount;
			float4 CZY_FogMoonFlareColor;
			float CZY_CloudsFogAmount;
			float CZY_FogSmoothness;
			float CZY_FogOffset;
			float CZY_FogIntensity;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash81_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi81_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash81_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash88_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi88_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash88_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash200_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi200_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash200_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash232_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi232_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash232_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash84_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi84_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash84_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			float HLSL20_g367( bool enabled, bool submerged, float textureSample )
			{
				if(enabled)
				{
					if(submerged) return 1.0;
					else return textureSample;
				}
				else
				{
					return 0.0;
				}
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord6.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord6.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						output.fogFactorAndVertexLight.x = ComputeFogFactor(vertexInput.positionCS.z);
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				float3 WorldPosition = input.positionWS;
				float3 WorldViewDirection = GetWorldSpaceNormalizeViewDir( WorldPosition );
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 hsvTorgb2_g364 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g364 = HSVToRGB( float3(hsvTorgb2_g364.x,saturate( ( hsvTorgb2_g364.y + CZY_FilterSaturation ) ),( hsvTorgb2_g364.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g364 = ( float4( hsvTorgb3_g364 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor41_g363 = ( temp_output_10_0_g364 * CZY_CloudFilterColor );
				float3 hsvTorgb2_g368 = RGBToHSV( CZY_CloudHighlightColor.rgb );
				float3 hsvTorgb3_g368 = HSVToRGB( float3(hsvTorgb2_g368.x,saturate( ( hsvTorgb2_g368.y + CZY_FilterSaturation ) ),( hsvTorgb2_g368.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g368 = ( float4( hsvTorgb3_g368 , 0.0 ) * CZY_FilterColor );
				float4 CloudHighlightColor55_g363 = ( temp_output_10_0_g368 * CZY_SunFilterColor );
				float2 texCoord31_g363 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos33_g363 = texCoord31_g363;
				float mulTime29_g363 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme30_g363 = mulTime29_g363;
				float simplePerlin2D409_g363 = snoise( ( Pos33_g363 + ( TIme30_g363 * float2( 0.2,-0.4 ) ) )*( 100.0 / CZY_MainCloudScale ) );
				simplePerlin2D409_g363 = simplePerlin2D409_g363*0.5 + 0.5;
				float SimpleCloudDensity153_g363 = simplePerlin2D409_g363;
				float time81_g363 = 0.0;
				float2 voronoiSmoothId81_g363 = 0;
				float2 temp_output_94_0_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) );
				float2 coords81_g363 = temp_output_94_0_g363 * ( 140.0 / CZY_MainCloudScale );
				float2 id81_g363 = 0;
				float2 uv81_g363 = 0;
				float voroi81_g363 = voronoi81_g363( coords81_g363, time81_g363, id81_g363, uv81_g363, 0, voronoiSmoothId81_g363 );
				float time88_g363 = 0.0;
				float2 voronoiSmoothId88_g363 = 0;
				float2 coords88_g363 = temp_output_94_0_g363 * ( 500.0 / CZY_MainCloudScale );
				float2 id88_g363 = 0;
				float2 uv88_g363 = 0;
				float voroi88_g363 = voronoi88_g363( coords88_g363, time88_g363, id88_g363, uv88_g363, 0, voronoiSmoothId88_g363 );
				float2 appendResult95_g363 = (float2(voroi81_g363 , voroi88_g363));
				float2 VoroDetails109_g363 = appendResult95_g363;
				float CumulusCoverage34_g363 = CZY_CumulusCoverageMultiplier;
				float ComplexCloudDensity141_g363 = (0.0 + (min( SimpleCloudDensity153_g363 , ( 1.0 - VoroDetails109_g363.x ) ) - ( 1.0 - CumulusCoverage34_g363 )) * (1.0 - 0.0) / (1.0 - ( 1.0 - CumulusCoverage34_g363 )));
				float4 lerpResult315_g363 = lerp( CloudHighlightColor55_g363 , CloudColor41_g363 , saturate( (2.0 + (ComplexCloudDensity141_g363 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
				float3 normalizeResult40_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult42_g363 = dot( normalizeResult40_g363 , CZY_SunDirection );
				float temp_output_49_0_g363 = abs( (dotResult42_g363*0.5 + 0.5) );
				half LightMask56_g363 = saturate( pow( temp_output_49_0_g363 , CZY_SunFlareFalloff ) );
				float time200_g363 = 0.0;
				float2 voronoiSmoothId200_g363 = 0;
				float mulTime163_g363 = _TimeParameters.x * 0.003;
				float2 coords200_g363 = (Pos33_g363*1.0 + ( float2( 1,-2 ) * mulTime163_g363 )) * 10.0;
				float2 id200_g363 = 0;
				float2 uv200_g363 = 0;
				float voroi200_g363 = voronoi200_g363( coords200_g363, time200_g363, id200_g363, uv200_g363, 0, voronoiSmoothId200_g363 );
				float time232_g363 = ( 10.0 * mulTime163_g363 );
				float2 voronoiSmoothId232_g363 = 0;
				float2 coords232_g363 = input.ase_texcoord6.xy * 10.0;
				float2 id232_g363 = 0;
				float2 uv232_g363 = 0;
				float voroi232_g363 = voronoi232_g363( coords232_g363, time232_g363, id232_g363, uv232_g363, 0, voronoiSmoothId232_g363 );
				float temp_output_242_0_g363 = ( ( ( 1.0 - 0.0 ) - (1.0 + (voroi200_g363 - 0.0) * (-0.5 - 1.0) / (1.0 - 0.0)) ) - voroi232_g363 );
				float AltoCumulusPlacement376_g363 = temp_output_242_0_g363;
				float CloudThicknessDetails286_g363 = ( VoroDetails109_g363.y * saturate( ( AltoCumulusPlacement376_g363 - 0.26 ) ) );
				float3 normalizeResult43_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult46_g363 = dot( normalizeResult43_g363 , CZY_MoonDirection );
				half MoonlightMask57_g363 = saturate( pow( abs( (dotResult46_g363*0.5 + 0.5) ) , CZY_CloudMoonFalloff ) );
				float3 hsvTorgb2_g365 = RGBToHSV( CZY_CloudMoonColor.rgb );
				float3 hsvTorgb3_g365 = HSVToRGB( float3(hsvTorgb2_g365.x,saturate( ( hsvTorgb2_g365.y + CZY_FilterSaturation ) ),( hsvTorgb2_g365.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g365 = ( float4( hsvTorgb3_g365 , 0.0 ) * CZY_FilterColor );
				float4 MoonlightColor60_g363 = ( temp_output_10_0_g365 * CZY_CloudFilterColor );
				float4 lerpResult338_g363 = lerp( ( lerpResult315_g363 + ( LightMask56_g363 * CloudHighlightColor55_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) + ( MoonlightMask57_g363 * MoonlightColor60_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) ) , ( CloudColor41_g363 * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , CloudThicknessDetails286_g363);
				float time84_g363 = 0.0;
				float2 voronoiSmoothId84_g363 = 0;
				float2 coords84_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) ) * ( 100.0 / CZY_DetailScale );
				float2 id84_g363 = 0;
				float2 uv84_g363 = 0;
				float fade84_g363 = 0.5;
				float voroi84_g363 = 0;
				float rest84_g363 = 0;
				for( int it84_g363 = 0; it84_g363 <3; it84_g363++ ){
				voroi84_g363 += fade84_g363 * voronoi84_g363( coords84_g363, time84_g363, id84_g363, uv84_g363, 0,voronoiSmoothId84_g363 );
				rest84_g363 += fade84_g363;
				coords84_g363 *= 2;
				fade84_g363 *= 0.5;
				}//Voronoi84_g363
				voroi84_g363 /= rest84_g363;
				float temp_output_173_0_g363 = ( (0.0 + (( 1.0 - voroi84_g363 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * CZY_DetailAmount );
				float DetailedClouds252_g363 = saturate( ( ComplexCloudDensity141_g363 + temp_output_173_0_g363 ) );
				float CloudDetail179_g363 = temp_output_173_0_g363;
				float2 texCoord79_g363 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_161_0_g363 = ( texCoord79_g363 - float2( 0.5,0.5 ) );
				float dotResult212_g363 = dot( temp_output_161_0_g363 , temp_output_161_0_g363 );
				float BorderHeight154_g363 = ( 1.0 - CZY_BorderHeight );
				float temp_output_151_0_g363 = ( -2.0 * ( 1.0 - CZY_BorderVariation ) );
				float clampResult247_g363 = clamp( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( BorderHeight154_g363 * temp_output_151_0_g363 ) + (dotResult212_g363 - 0.0) * (( temp_output_151_0_g363 * -4.0 ) - ( BorderHeight154_g363 * temp_output_151_0_g363 )) / (0.5 - 0.0)) ) ) * 10.0 * CZY_BorderEffect ) , -1.0 , 1.0 );
				float BorderLightTransport278_g363 = clampResult247_g363;
				float3 normalizeResult116_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float3 normalizeResult146_g363 = normalize( CZY_StormDirection );
				float dotResult150_g363 = dot( normalizeResult116_g363 , normalizeResult146_g363 );
				float2 texCoord98_g363 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_124_0_g363 = ( texCoord98_g363 - float2( 0.5,0.5 ) );
				float dotResult125_g363 = dot( temp_output_124_0_g363 , temp_output_124_0_g363 );
				float temp_output_140_0_g363 = ( -2.0 * ( 1.0 - ( CZY_NimbusVariation * 0.9 ) ) );
				float NimbusLightTransport269_g363 = saturate( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 ) + (( dotResult150_g363 + ( CZY_NimbusHeight * 4.0 * dotResult125_g363 ) ) - 0.5) * (( temp_output_140_0_g363 * -4.0 ) - ( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 )) / (7.0 - 0.5)) ) ) * 10.0 ) );
				float mulTime104_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D143_g363 = snoise( (Pos33_g363*1.0 + mulTime104_g363)*2.0 );
				float mulTime93_g363 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos97_g363 = cos( ( mulTime93_g363 * 0.01 ) );
				float sin97_g363 = sin( ( mulTime93_g363 * 0.01 ) );
				float2 rotator97_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos97_g363 , -sin97_g363 , sin97_g363 , cos97_g363 )) + float2( 0.5,0.5 );
				float cos131_g363 = cos( ( mulTime93_g363 * -0.02 ) );
				float sin131_g363 = sin( ( mulTime93_g363 * -0.02 ) );
				float2 rotator131_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos131_g363 , -sin131_g363 , sin131_g363 , cos131_g363 )) + float2( 0.5,0.5 );
				float mulTime107_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D147_g363 = snoise( (Pos33_g363*1.0 + mulTime107_g363)*4.0 );
				float4 ChemtrailsPattern210_g363 = ( ( saturate( simplePerlin2D143_g363 ) * tex2D( CZY_ChemtrailsTexture, (rotator97_g363*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator131_g363 ) * saturate( simplePerlin2D147_g363 ) ) );
				float2 texCoord139_g363 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_162_0_g363 = ( texCoord139_g363 - float2( 0.5,0.5 ) );
				float dotResult207_g363 = dot( temp_output_162_0_g363 , temp_output_162_0_g363 );
				float ChemtrailsFinal248_g363 = ( ( ChemtrailsPattern210_g363 * saturate( (0.4 + (dotResult207_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - ( CZY_ChemtrailsMultiplier * 0.5 ) ) ? 1.0 : 0.0 );
				float mulTime80_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D126_g363 = snoise( (Pos33_g363*1.0 + mulTime80_g363)*2.0 );
				float mulTime75_g363 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos101_g363 = cos( ( mulTime75_g363 * 0.01 ) );
				float sin101_g363 = sin( ( mulTime75_g363 * 0.01 ) );
				float2 rotator101_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos101_g363 , -sin101_g363 , sin101_g363 , cos101_g363 )) + float2( 0.5,0.5 );
				float cos112_g363 = cos( ( mulTime75_g363 * -0.02 ) );
				float sin112_g363 = sin( ( mulTime75_g363 * -0.02 ) );
				float2 rotator112_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos112_g363 , -sin112_g363 , sin112_g363 , cos112_g363 )) + float2( 0.5,0.5 );
				float mulTime135_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D122_g363 = snoise( (Pos33_g363*1.0 + mulTime135_g363) );
				simplePerlin2D122_g363 = simplePerlin2D122_g363*0.5 + 0.5;
				float4 CirrusPattern137_g363 = ( ( saturate( simplePerlin2D126_g363 ) * tex2D( CZY_CirrusTexture, (rotator101_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator112_g363*1.0 + 0.0) ) * saturate( simplePerlin2D122_g363 ) ) );
				float2 texCoord134_g363 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_164_0_g363 = ( texCoord134_g363 - float2( 0.5,0.5 ) );
				float dotResult157_g363 = dot( temp_output_164_0_g363 , temp_output_164_0_g363 );
				float4 temp_output_217_0_g363 = ( CirrusPattern137_g363 * saturate( (0.0 + (dotResult157_g363 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) );
				float Clipping208_g363 = CZY_ClippingThreshold;
				float CirrusAlpha250_g363 = ( ( temp_output_217_0_g363 * ( CZY_CirrusMultiplier * 10.0 ) ).r > Clipping208_g363 ? 1.0 : 0.0 );
				float SimpleRadiance268_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + NimbusLightTransport269_g363 + ChemtrailsFinal248_g363 + CirrusAlpha250_g363 ) );
				float4 lerpResult342_g363 = lerp( CloudColor41_g363 , lerpResult338_g363 , ( 1.0 - SimpleRadiance268_g363 ));
				float CloudbreakLightDir426_g363 = saturate( pow( temp_output_49_0_g363 , ( CZY_SunFlareFalloff * 0.5 ) ) );
				float lerpResult316_g363 = lerp( -0.4 , 1.0 , ( saturate( ( ComplexCloudDensity141_g363 - 0.0 ) ) * CloudDetail179_g363 * CloudbreakLightDir426_g363 ));
				float SunThroughClouds399_g363 = saturate( lerpResult316_g363 );
				float3 hsvTorgb2_g366 = RGBToHSV( CZY_AltoCloudColor.rgb );
				float3 hsvTorgb3_g366 = HSVToRGB( float3(hsvTorgb2_g366.x,saturate( ( hsvTorgb2_g366.y + CZY_FilterSaturation ) ),( hsvTorgb2_g366.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g366 = ( float4( hsvTorgb3_g366 , 0.0 ) * CZY_FilterColor );
				float4 CirrusCustomLightColor350_g363 = ( CloudColor41_g363 * ( temp_output_10_0_g366 * CZY_CloudFilterColor ) );
				float temp_output_391_0_g363 = ( AltoCumulusPlacement376_g363 * (0.0 + (tex2D( CZY_AltocumulusTexture, ((Pos33_g363*1.0 + ( CZY_AltocumulusWindSpeed * TIme30_g363 ))*( 1.0 / CZY_AltocumulusScale ) + 0.0) ).r - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) * CZY_AltocumulusMultiplier );
				float AltoCumulusLightTransport393_g363 = temp_output_391_0_g363;
				float ACCustomLightsClipping387_g363 = ( AltoCumulusLightTransport393_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float mulTime193_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D224_g363 = snoise( (Pos33_g363*1.0 + mulTime193_g363)*2.0 );
				float mulTime178_g363 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos138_g363 = cos( ( mulTime178_g363 * 0.01 ) );
				float sin138_g363 = sin( ( mulTime178_g363 * 0.01 ) );
				float2 rotator138_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos138_g363 , -sin138_g363 , sin138_g363 , cos138_g363 )) + float2( 0.5,0.5 );
				float cos198_g363 = cos( ( mulTime178_g363 * -0.02 ) );
				float sin198_g363 = sin( ( mulTime178_g363 * -0.02 ) );
				float2 rotator198_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos198_g363 , -sin198_g363 , sin198_g363 , cos198_g363 )) + float2( 0.5,0.5 );
				float mulTime184_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D216_g363 = snoise( (Pos33_g363*10.0 + mulTime184_g363)*4.0 );
				float4 CirrostratPattern261_g363 = ( ( saturate( simplePerlin2D224_g363 ) * tex2D( CZY_CirrostratusTexture, (rotator138_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator198_g363*1.5 + 0.75) ) * saturate( simplePerlin2D216_g363 ) ) );
				float2 texCoord234_g363 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_243_0_g363 = ( texCoord234_g363 - float2( 0.5,0.5 ) );
				float dotResult238_g363 = dot( temp_output_243_0_g363 , temp_output_243_0_g363 );
				float clampResult264_g363 = clamp( ( CZY_CirrostratusMultiplier * 0.5 ) , 0.0 , 0.98 );
				float CirrostratLightTransport281_g363 = ( ( CirrostratPattern261_g363 * saturate( (0.4 + (dotResult238_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - clampResult264_g363 ) ? 1.0 : 0.0 );
				float CSCustomLightsClipping309_g363 = ( CirrostratLightTransport281_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float CustomRadiance340_g363 = saturate( ( ACCustomLightsClipping387_g363 + CSCustomLightsClipping309_g363 ) );
				float4 lerpResult331_g363 = lerp( ( lerpResult342_g363 + SunThroughClouds399_g363 ) , CirrusCustomLightColor350_g363 , CustomRadiance340_g363);
				float FinalAlpha375_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + AltoCumulusLightTransport393_g363 + ChemtrailsFinal248_g363 + CirrostratLightTransport281_g363 + CirrusAlpha250_g363 + NimbusLightTransport269_g363 ) );
				float4 appendResult420_g363 = (float4((lerpResult331_g363).rgb , FinalAlpha375_g363));
				float4 FinalCloudColor325_g363 = appendResult420_g363;
				float3 hsvTorgb69_g369 = RGBToHSV( CZY_FogColor5.rgb );
				float3 normalizeResult54_g369 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float3 temp_output_56_0_g369 = ( normalizeResult54_g369 * _ProjectionParams.z );
				float3 appendResult25_g369 = (float3(1.0 , CZY_LightFlareSquish , 1.0));
				float3 normalizeResult13_g369 = normalize( ( ( temp_output_56_0_g369 * appendResult25_g369 ) - _WorldSpaceCameraPos ) );
				float dotResult16_g369 = dot( normalizeResult13_g369 , CZY_SunDirection );
				half LightMask35_g369 = saturate( pow( abs( ( (dotResult16_g369*0.5 + 0.5) * CZY_LightIntensity ) ) , CZY_LightFalloff ) );
				float temp_output_91_0_g369 = CZY_CloudsFogLightAmount;
				float3 hsvTorgb2_g371 = RGBToHSV( ( CZY_LightColor * hsvTorgb69_g369.z * saturate( ( LightMask35_g369 * ( 1.5 * CZY_FogColor5.a ) * temp_output_91_0_g369 ) ) ).rgb );
				float3 hsvTorgb3_g371 = HSVToRGB( float3(hsvTorgb2_g371.x,saturate( ( hsvTorgb2_g371.y + CZY_FilterSaturation ) ),( hsvTorgb2_g371.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g371 = ( float4( hsvTorgb3_g371 , 0.0 ) * CZY_FilterColor );
				float3 direction88_g369 = ( temp_output_56_0_g369 - _WorldSpaceCameraPos );
				float3 normalizeResult32_g369 = normalize( direction88_g369 );
				float3 normalizeResult30_g369 = normalize( CZY_MoonDirection );
				float dotResult28_g369 = dot( normalizeResult32_g369 , normalizeResult30_g369 );
				half MoonMask18_g369 = saturate( pow( abs( ( saturate( (dotResult28_g369*1.0 + 0.0) ) * CZY_LightIntensity ) ) , ( CZY_LightFalloff * 3.0 ) ) );
				float3 hsvTorgb2_g370 = RGBToHSV( ( CZY_FogColor5 + ( hsvTorgb69_g369.z * saturate( ( CZY_FogColor5.a * MoonMask18_g369 * temp_output_91_0_g369 ) ) * CZY_FogMoonFlareColor ) ).rgb );
				float3 hsvTorgb3_g370 = HSVToRGB( float3(hsvTorgb2_g370.x,saturate( ( hsvTorgb2_g370.y + CZY_FilterSaturation ) ),( hsvTorgb2_g370.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g370 = ( float4( hsvTorgb3_g370 , 0.0 ) * CZY_FilterColor );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float temp_output_34_0_g369 = ( CZY_CloudsFogAmount * saturate( ( ( 1.0 - saturate( ( ( ( direction88_g369.y * 0.1 ) * ( 1.0 / ( ( CZY_FogSmoothness * length( ase_objectScale ) ) * 10.0 ) ) ) + ( 1.0 - CZY_FogOffset ) ) ) ) * CZY_FogIntensity ) ) );
				float4 lerpResult90_g369 = lerp( FinalCloudColor325_g363 , ( ( temp_output_10_0_g371 * CZY_SunFilterColor ) + temp_output_10_0_g370 ) , temp_output_34_0_g369);
				
				bool enabled20_g367 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g367 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g367 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g367 = HLSL20_g367( enabled20_g367 , submerged20_g367 , textureSample20_g367 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = lerpResult90_g369.rgb;
				float Alpha = ( ( (FinalCloudColor325_g363).w * ( 1.0 - localHLSL20_g367 ) ) > Clipping208_g363 ? 1.0 : 0.0 );
				float AlphaClipThreshold = Clipping208_g363;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;

				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(input.positionCS, Color);
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						Color.rgb = MixFogColor(Color.rgb, half3(0,0,0), inputData.fogCoord);
					#else
						Color.rgb = MixFog(Color.rgb, inputData.fogCoord);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			

			#pragma multi_compile _ALPHATEST_ON
			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 140010


			

			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			float4 CZY_CloudColor;
			float CZY_FilterSaturation;
			float CZY_FilterValue;
			float4 CZY_FilterColor;
			float4 CZY_CloudFilterColor;
			float4 CZY_CloudHighlightColor;
			float4 CZY_SunFilterColor;
			float CZY_WindSpeed;
			float CZY_MainCloudScale;
			float CZY_CumulusCoverageMultiplier;
			float3 CZY_SunDirection;
			half CZY_SunFlareFalloff;
			float3 CZY_MoonDirection;
			half CZY_CloudMoonFalloff;
			float4 CZY_CloudMoonColor;
			float CZY_DetailScale;
			float CZY_DetailAmount;
			float CZY_BorderHeight;
			float CZY_BorderVariation;
			float CZY_BorderEffect;
			float3 CZY_StormDirection;
			float CZY_NimbusHeight;
			float CZY_NimbusMultiplier;
			float CZY_NimbusVariation;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_ClippingThreshold;
			float4 CZY_AltoCloudColor;
			sampler2D CZY_AltocumulusTexture;
			float2 CZY_AltocumulusWindSpeed;
			float CZY_AltocumulusScale;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash81_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi81_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash81_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash88_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi88_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash88_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash200_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi200_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash200_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash232_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi232_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash232_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash84_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi84_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash84_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			float HLSL20_g367( bool enabled, bool submerged, float textureSample )
			{
				if(enabled)
				{
					if(submerged) return 1.0;
					else return textureSample;
				}
				else
				{
					return 0.0;
				}
			}
			

			float3 _LightDirection;
			float3 _LightPosition;

			PackedVaryings VertexFunction( Attributes input )
			{
				PackedVaryings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir(input.normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = positionCS;
				output.clipPosV = positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 hsvTorgb2_g364 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g364 = HSVToRGB( float3(hsvTorgb2_g364.x,saturate( ( hsvTorgb2_g364.y + CZY_FilterSaturation ) ),( hsvTorgb2_g364.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g364 = ( float4( hsvTorgb3_g364 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor41_g363 = ( temp_output_10_0_g364 * CZY_CloudFilterColor );
				float3 hsvTorgb2_g368 = RGBToHSV( CZY_CloudHighlightColor.rgb );
				float3 hsvTorgb3_g368 = HSVToRGB( float3(hsvTorgb2_g368.x,saturate( ( hsvTorgb2_g368.y + CZY_FilterSaturation ) ),( hsvTorgb2_g368.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g368 = ( float4( hsvTorgb3_g368 , 0.0 ) * CZY_FilterColor );
				float4 CloudHighlightColor55_g363 = ( temp_output_10_0_g368 * CZY_SunFilterColor );
				float2 texCoord31_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos33_g363 = texCoord31_g363;
				float mulTime29_g363 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme30_g363 = mulTime29_g363;
				float simplePerlin2D409_g363 = snoise( ( Pos33_g363 + ( TIme30_g363 * float2( 0.2,-0.4 ) ) )*( 100.0 / CZY_MainCloudScale ) );
				simplePerlin2D409_g363 = simplePerlin2D409_g363*0.5 + 0.5;
				float SimpleCloudDensity153_g363 = simplePerlin2D409_g363;
				float time81_g363 = 0.0;
				float2 voronoiSmoothId81_g363 = 0;
				float2 temp_output_94_0_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) );
				float2 coords81_g363 = temp_output_94_0_g363 * ( 140.0 / CZY_MainCloudScale );
				float2 id81_g363 = 0;
				float2 uv81_g363 = 0;
				float voroi81_g363 = voronoi81_g363( coords81_g363, time81_g363, id81_g363, uv81_g363, 0, voronoiSmoothId81_g363 );
				float time88_g363 = 0.0;
				float2 voronoiSmoothId88_g363 = 0;
				float2 coords88_g363 = temp_output_94_0_g363 * ( 500.0 / CZY_MainCloudScale );
				float2 id88_g363 = 0;
				float2 uv88_g363 = 0;
				float voroi88_g363 = voronoi88_g363( coords88_g363, time88_g363, id88_g363, uv88_g363, 0, voronoiSmoothId88_g363 );
				float2 appendResult95_g363 = (float2(voroi81_g363 , voroi88_g363));
				float2 VoroDetails109_g363 = appendResult95_g363;
				float CumulusCoverage34_g363 = CZY_CumulusCoverageMultiplier;
				float ComplexCloudDensity141_g363 = (0.0 + (min( SimpleCloudDensity153_g363 , ( 1.0 - VoroDetails109_g363.x ) ) - ( 1.0 - CumulusCoverage34_g363 )) * (1.0 - 0.0) / (1.0 - ( 1.0 - CumulusCoverage34_g363 )));
				float4 lerpResult315_g363 = lerp( CloudHighlightColor55_g363 , CloudColor41_g363 , saturate( (2.0 + (ComplexCloudDensity141_g363 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
				float3 normalizeResult40_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult42_g363 = dot( normalizeResult40_g363 , CZY_SunDirection );
				float temp_output_49_0_g363 = abs( (dotResult42_g363*0.5 + 0.5) );
				half LightMask56_g363 = saturate( pow( temp_output_49_0_g363 , CZY_SunFlareFalloff ) );
				float time200_g363 = 0.0;
				float2 voronoiSmoothId200_g363 = 0;
				float mulTime163_g363 = _TimeParameters.x * 0.003;
				float2 coords200_g363 = (Pos33_g363*1.0 + ( float2( 1,-2 ) * mulTime163_g363 )) * 10.0;
				float2 id200_g363 = 0;
				float2 uv200_g363 = 0;
				float voroi200_g363 = voronoi200_g363( coords200_g363, time200_g363, id200_g363, uv200_g363, 0, voronoiSmoothId200_g363 );
				float time232_g363 = ( 10.0 * mulTime163_g363 );
				float2 voronoiSmoothId232_g363 = 0;
				float2 coords232_g363 = input.ase_texcoord3.xy * 10.0;
				float2 id232_g363 = 0;
				float2 uv232_g363 = 0;
				float voroi232_g363 = voronoi232_g363( coords232_g363, time232_g363, id232_g363, uv232_g363, 0, voronoiSmoothId232_g363 );
				float temp_output_242_0_g363 = ( ( ( 1.0 - 0.0 ) - (1.0 + (voroi200_g363 - 0.0) * (-0.5 - 1.0) / (1.0 - 0.0)) ) - voroi232_g363 );
				float AltoCumulusPlacement376_g363 = temp_output_242_0_g363;
				float CloudThicknessDetails286_g363 = ( VoroDetails109_g363.y * saturate( ( AltoCumulusPlacement376_g363 - 0.26 ) ) );
				float3 normalizeResult43_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult46_g363 = dot( normalizeResult43_g363 , CZY_MoonDirection );
				half MoonlightMask57_g363 = saturate( pow( abs( (dotResult46_g363*0.5 + 0.5) ) , CZY_CloudMoonFalloff ) );
				float3 hsvTorgb2_g365 = RGBToHSV( CZY_CloudMoonColor.rgb );
				float3 hsvTorgb3_g365 = HSVToRGB( float3(hsvTorgb2_g365.x,saturate( ( hsvTorgb2_g365.y + CZY_FilterSaturation ) ),( hsvTorgb2_g365.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g365 = ( float4( hsvTorgb3_g365 , 0.0 ) * CZY_FilterColor );
				float4 MoonlightColor60_g363 = ( temp_output_10_0_g365 * CZY_CloudFilterColor );
				float4 lerpResult338_g363 = lerp( ( lerpResult315_g363 + ( LightMask56_g363 * CloudHighlightColor55_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) + ( MoonlightMask57_g363 * MoonlightColor60_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) ) , ( CloudColor41_g363 * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , CloudThicknessDetails286_g363);
				float time84_g363 = 0.0;
				float2 voronoiSmoothId84_g363 = 0;
				float2 coords84_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) ) * ( 100.0 / CZY_DetailScale );
				float2 id84_g363 = 0;
				float2 uv84_g363 = 0;
				float fade84_g363 = 0.5;
				float voroi84_g363 = 0;
				float rest84_g363 = 0;
				for( int it84_g363 = 0; it84_g363 <3; it84_g363++ ){
				voroi84_g363 += fade84_g363 * voronoi84_g363( coords84_g363, time84_g363, id84_g363, uv84_g363, 0,voronoiSmoothId84_g363 );
				rest84_g363 += fade84_g363;
				coords84_g363 *= 2;
				fade84_g363 *= 0.5;
				}//Voronoi84_g363
				voroi84_g363 /= rest84_g363;
				float temp_output_173_0_g363 = ( (0.0 + (( 1.0 - voroi84_g363 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * CZY_DetailAmount );
				float DetailedClouds252_g363 = saturate( ( ComplexCloudDensity141_g363 + temp_output_173_0_g363 ) );
				float CloudDetail179_g363 = temp_output_173_0_g363;
				float2 texCoord79_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_161_0_g363 = ( texCoord79_g363 - float2( 0.5,0.5 ) );
				float dotResult212_g363 = dot( temp_output_161_0_g363 , temp_output_161_0_g363 );
				float BorderHeight154_g363 = ( 1.0 - CZY_BorderHeight );
				float temp_output_151_0_g363 = ( -2.0 * ( 1.0 - CZY_BorderVariation ) );
				float clampResult247_g363 = clamp( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( BorderHeight154_g363 * temp_output_151_0_g363 ) + (dotResult212_g363 - 0.0) * (( temp_output_151_0_g363 * -4.0 ) - ( BorderHeight154_g363 * temp_output_151_0_g363 )) / (0.5 - 0.0)) ) ) * 10.0 * CZY_BorderEffect ) , -1.0 , 1.0 );
				float BorderLightTransport278_g363 = clampResult247_g363;
				float3 normalizeResult116_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float3 normalizeResult146_g363 = normalize( CZY_StormDirection );
				float dotResult150_g363 = dot( normalizeResult116_g363 , normalizeResult146_g363 );
				float2 texCoord98_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_124_0_g363 = ( texCoord98_g363 - float2( 0.5,0.5 ) );
				float dotResult125_g363 = dot( temp_output_124_0_g363 , temp_output_124_0_g363 );
				float temp_output_140_0_g363 = ( -2.0 * ( 1.0 - ( CZY_NimbusVariation * 0.9 ) ) );
				float NimbusLightTransport269_g363 = saturate( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 ) + (( dotResult150_g363 + ( CZY_NimbusHeight * 4.0 * dotResult125_g363 ) ) - 0.5) * (( temp_output_140_0_g363 * -4.0 ) - ( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 )) / (7.0 - 0.5)) ) ) * 10.0 ) );
				float mulTime104_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D143_g363 = snoise( (Pos33_g363*1.0 + mulTime104_g363)*2.0 );
				float mulTime93_g363 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos97_g363 = cos( ( mulTime93_g363 * 0.01 ) );
				float sin97_g363 = sin( ( mulTime93_g363 * 0.01 ) );
				float2 rotator97_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos97_g363 , -sin97_g363 , sin97_g363 , cos97_g363 )) + float2( 0.5,0.5 );
				float cos131_g363 = cos( ( mulTime93_g363 * -0.02 ) );
				float sin131_g363 = sin( ( mulTime93_g363 * -0.02 ) );
				float2 rotator131_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos131_g363 , -sin131_g363 , sin131_g363 , cos131_g363 )) + float2( 0.5,0.5 );
				float mulTime107_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D147_g363 = snoise( (Pos33_g363*1.0 + mulTime107_g363)*4.0 );
				float4 ChemtrailsPattern210_g363 = ( ( saturate( simplePerlin2D143_g363 ) * tex2D( CZY_ChemtrailsTexture, (rotator97_g363*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator131_g363 ) * saturate( simplePerlin2D147_g363 ) ) );
				float2 texCoord139_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_162_0_g363 = ( texCoord139_g363 - float2( 0.5,0.5 ) );
				float dotResult207_g363 = dot( temp_output_162_0_g363 , temp_output_162_0_g363 );
				float ChemtrailsFinal248_g363 = ( ( ChemtrailsPattern210_g363 * saturate( (0.4 + (dotResult207_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - ( CZY_ChemtrailsMultiplier * 0.5 ) ) ? 1.0 : 0.0 );
				float mulTime80_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D126_g363 = snoise( (Pos33_g363*1.0 + mulTime80_g363)*2.0 );
				float mulTime75_g363 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos101_g363 = cos( ( mulTime75_g363 * 0.01 ) );
				float sin101_g363 = sin( ( mulTime75_g363 * 0.01 ) );
				float2 rotator101_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos101_g363 , -sin101_g363 , sin101_g363 , cos101_g363 )) + float2( 0.5,0.5 );
				float cos112_g363 = cos( ( mulTime75_g363 * -0.02 ) );
				float sin112_g363 = sin( ( mulTime75_g363 * -0.02 ) );
				float2 rotator112_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos112_g363 , -sin112_g363 , sin112_g363 , cos112_g363 )) + float2( 0.5,0.5 );
				float mulTime135_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D122_g363 = snoise( (Pos33_g363*1.0 + mulTime135_g363) );
				simplePerlin2D122_g363 = simplePerlin2D122_g363*0.5 + 0.5;
				float4 CirrusPattern137_g363 = ( ( saturate( simplePerlin2D126_g363 ) * tex2D( CZY_CirrusTexture, (rotator101_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator112_g363*1.0 + 0.0) ) * saturate( simplePerlin2D122_g363 ) ) );
				float2 texCoord134_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_164_0_g363 = ( texCoord134_g363 - float2( 0.5,0.5 ) );
				float dotResult157_g363 = dot( temp_output_164_0_g363 , temp_output_164_0_g363 );
				float4 temp_output_217_0_g363 = ( CirrusPattern137_g363 * saturate( (0.0 + (dotResult157_g363 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) );
				float Clipping208_g363 = CZY_ClippingThreshold;
				float CirrusAlpha250_g363 = ( ( temp_output_217_0_g363 * ( CZY_CirrusMultiplier * 10.0 ) ).r > Clipping208_g363 ? 1.0 : 0.0 );
				float SimpleRadiance268_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + NimbusLightTransport269_g363 + ChemtrailsFinal248_g363 + CirrusAlpha250_g363 ) );
				float4 lerpResult342_g363 = lerp( CloudColor41_g363 , lerpResult338_g363 , ( 1.0 - SimpleRadiance268_g363 ));
				float CloudbreakLightDir426_g363 = saturate( pow( temp_output_49_0_g363 , ( CZY_SunFlareFalloff * 0.5 ) ) );
				float lerpResult316_g363 = lerp( -0.4 , 1.0 , ( saturate( ( ComplexCloudDensity141_g363 - 0.0 ) ) * CloudDetail179_g363 * CloudbreakLightDir426_g363 ));
				float SunThroughClouds399_g363 = saturate( lerpResult316_g363 );
				float3 hsvTorgb2_g366 = RGBToHSV( CZY_AltoCloudColor.rgb );
				float3 hsvTorgb3_g366 = HSVToRGB( float3(hsvTorgb2_g366.x,saturate( ( hsvTorgb2_g366.y + CZY_FilterSaturation ) ),( hsvTorgb2_g366.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g366 = ( float4( hsvTorgb3_g366 , 0.0 ) * CZY_FilterColor );
				float4 CirrusCustomLightColor350_g363 = ( CloudColor41_g363 * ( temp_output_10_0_g366 * CZY_CloudFilterColor ) );
				float temp_output_391_0_g363 = ( AltoCumulusPlacement376_g363 * (0.0 + (tex2D( CZY_AltocumulusTexture, ((Pos33_g363*1.0 + ( CZY_AltocumulusWindSpeed * TIme30_g363 ))*( 1.0 / CZY_AltocumulusScale ) + 0.0) ).r - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) * CZY_AltocumulusMultiplier );
				float AltoCumulusLightTransport393_g363 = temp_output_391_0_g363;
				float ACCustomLightsClipping387_g363 = ( AltoCumulusLightTransport393_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float mulTime193_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D224_g363 = snoise( (Pos33_g363*1.0 + mulTime193_g363)*2.0 );
				float mulTime178_g363 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos138_g363 = cos( ( mulTime178_g363 * 0.01 ) );
				float sin138_g363 = sin( ( mulTime178_g363 * 0.01 ) );
				float2 rotator138_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos138_g363 , -sin138_g363 , sin138_g363 , cos138_g363 )) + float2( 0.5,0.5 );
				float cos198_g363 = cos( ( mulTime178_g363 * -0.02 ) );
				float sin198_g363 = sin( ( mulTime178_g363 * -0.02 ) );
				float2 rotator198_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos198_g363 , -sin198_g363 , sin198_g363 , cos198_g363 )) + float2( 0.5,0.5 );
				float mulTime184_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D216_g363 = snoise( (Pos33_g363*10.0 + mulTime184_g363)*4.0 );
				float4 CirrostratPattern261_g363 = ( ( saturate( simplePerlin2D224_g363 ) * tex2D( CZY_CirrostratusTexture, (rotator138_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator198_g363*1.5 + 0.75) ) * saturate( simplePerlin2D216_g363 ) ) );
				float2 texCoord234_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_243_0_g363 = ( texCoord234_g363 - float2( 0.5,0.5 ) );
				float dotResult238_g363 = dot( temp_output_243_0_g363 , temp_output_243_0_g363 );
				float clampResult264_g363 = clamp( ( CZY_CirrostratusMultiplier * 0.5 ) , 0.0 , 0.98 );
				float CirrostratLightTransport281_g363 = ( ( CirrostratPattern261_g363 * saturate( (0.4 + (dotResult238_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - clampResult264_g363 ) ? 1.0 : 0.0 );
				float CSCustomLightsClipping309_g363 = ( CirrostratLightTransport281_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float CustomRadiance340_g363 = saturate( ( ACCustomLightsClipping387_g363 + CSCustomLightsClipping309_g363 ) );
				float4 lerpResult331_g363 = lerp( ( lerpResult342_g363 + SunThroughClouds399_g363 ) , CirrusCustomLightColor350_g363 , CustomRadiance340_g363);
				float FinalAlpha375_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + AltoCumulusLightTransport393_g363 + ChemtrailsFinal248_g363 + CirrostratLightTransport281_g363 + CirrusAlpha250_g363 + NimbusLightTransport269_g363 ) );
				float4 appendResult420_g363 = (float4((lerpResult331_g363).rgb , FinalAlpha375_g363));
				float4 FinalCloudColor325_g363 = appendResult420_g363;
				bool enabled20_g367 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g367 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g367 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g367 = HLSL20_g367( enabled20_g367 , submerged20_g367 , textureSample20_g367 );
				

				float Alpha = ( ( (FinalCloudColor325_g363).w * ( 1.0 - localHLSL20_g367 ) ) > Clipping208_g363 ? 1.0 : 0.0 );
				float AlphaClipThreshold = Clipping208_g363;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			

			#pragma multi_compile _ALPHATEST_ON
			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 140010


			

			#pragma vertex vert
			#pragma fragment frag

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			float4 CZY_CloudColor;
			float CZY_FilterSaturation;
			float CZY_FilterValue;
			float4 CZY_FilterColor;
			float4 CZY_CloudFilterColor;
			float4 CZY_CloudHighlightColor;
			float4 CZY_SunFilterColor;
			float CZY_WindSpeed;
			float CZY_MainCloudScale;
			float CZY_CumulusCoverageMultiplier;
			float3 CZY_SunDirection;
			half CZY_SunFlareFalloff;
			float3 CZY_MoonDirection;
			half CZY_CloudMoonFalloff;
			float4 CZY_CloudMoonColor;
			float CZY_DetailScale;
			float CZY_DetailAmount;
			float CZY_BorderHeight;
			float CZY_BorderVariation;
			float CZY_BorderEffect;
			float3 CZY_StormDirection;
			float CZY_NimbusHeight;
			float CZY_NimbusMultiplier;
			float CZY_NimbusVariation;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_ClippingThreshold;
			float4 CZY_AltoCloudColor;
			sampler2D CZY_AltocumulusTexture;
			float2 CZY_AltocumulusWindSpeed;
			float CZY_AltocumulusScale;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash81_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi81_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash81_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash88_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi88_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash88_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash200_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi200_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash200_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash232_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi232_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash232_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash84_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi84_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash84_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			float HLSL20_g367( bool enabled, bool submerged, float textureSample )
			{
				if(enabled)
				{
					if(submerged) return 1.0;
					else return textureSample;
				}
				else
				{
					return 0.0;
				}
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 hsvTorgb2_g364 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g364 = HSVToRGB( float3(hsvTorgb2_g364.x,saturate( ( hsvTorgb2_g364.y + CZY_FilterSaturation ) ),( hsvTorgb2_g364.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g364 = ( float4( hsvTorgb3_g364 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor41_g363 = ( temp_output_10_0_g364 * CZY_CloudFilterColor );
				float3 hsvTorgb2_g368 = RGBToHSV( CZY_CloudHighlightColor.rgb );
				float3 hsvTorgb3_g368 = HSVToRGB( float3(hsvTorgb2_g368.x,saturate( ( hsvTorgb2_g368.y + CZY_FilterSaturation ) ),( hsvTorgb2_g368.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g368 = ( float4( hsvTorgb3_g368 , 0.0 ) * CZY_FilterColor );
				float4 CloudHighlightColor55_g363 = ( temp_output_10_0_g368 * CZY_SunFilterColor );
				float2 texCoord31_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos33_g363 = texCoord31_g363;
				float mulTime29_g363 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme30_g363 = mulTime29_g363;
				float simplePerlin2D409_g363 = snoise( ( Pos33_g363 + ( TIme30_g363 * float2( 0.2,-0.4 ) ) )*( 100.0 / CZY_MainCloudScale ) );
				simplePerlin2D409_g363 = simplePerlin2D409_g363*0.5 + 0.5;
				float SimpleCloudDensity153_g363 = simplePerlin2D409_g363;
				float time81_g363 = 0.0;
				float2 voronoiSmoothId81_g363 = 0;
				float2 temp_output_94_0_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) );
				float2 coords81_g363 = temp_output_94_0_g363 * ( 140.0 / CZY_MainCloudScale );
				float2 id81_g363 = 0;
				float2 uv81_g363 = 0;
				float voroi81_g363 = voronoi81_g363( coords81_g363, time81_g363, id81_g363, uv81_g363, 0, voronoiSmoothId81_g363 );
				float time88_g363 = 0.0;
				float2 voronoiSmoothId88_g363 = 0;
				float2 coords88_g363 = temp_output_94_0_g363 * ( 500.0 / CZY_MainCloudScale );
				float2 id88_g363 = 0;
				float2 uv88_g363 = 0;
				float voroi88_g363 = voronoi88_g363( coords88_g363, time88_g363, id88_g363, uv88_g363, 0, voronoiSmoothId88_g363 );
				float2 appendResult95_g363 = (float2(voroi81_g363 , voroi88_g363));
				float2 VoroDetails109_g363 = appendResult95_g363;
				float CumulusCoverage34_g363 = CZY_CumulusCoverageMultiplier;
				float ComplexCloudDensity141_g363 = (0.0 + (min( SimpleCloudDensity153_g363 , ( 1.0 - VoroDetails109_g363.x ) ) - ( 1.0 - CumulusCoverage34_g363 )) * (1.0 - 0.0) / (1.0 - ( 1.0 - CumulusCoverage34_g363 )));
				float4 lerpResult315_g363 = lerp( CloudHighlightColor55_g363 , CloudColor41_g363 , saturate( (2.0 + (ComplexCloudDensity141_g363 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
				float3 normalizeResult40_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult42_g363 = dot( normalizeResult40_g363 , CZY_SunDirection );
				float temp_output_49_0_g363 = abs( (dotResult42_g363*0.5 + 0.5) );
				half LightMask56_g363 = saturate( pow( temp_output_49_0_g363 , CZY_SunFlareFalloff ) );
				float time200_g363 = 0.0;
				float2 voronoiSmoothId200_g363 = 0;
				float mulTime163_g363 = _TimeParameters.x * 0.003;
				float2 coords200_g363 = (Pos33_g363*1.0 + ( float2( 1,-2 ) * mulTime163_g363 )) * 10.0;
				float2 id200_g363 = 0;
				float2 uv200_g363 = 0;
				float voroi200_g363 = voronoi200_g363( coords200_g363, time200_g363, id200_g363, uv200_g363, 0, voronoiSmoothId200_g363 );
				float time232_g363 = ( 10.0 * mulTime163_g363 );
				float2 voronoiSmoothId232_g363 = 0;
				float2 coords232_g363 = input.ase_texcoord3.xy * 10.0;
				float2 id232_g363 = 0;
				float2 uv232_g363 = 0;
				float voroi232_g363 = voronoi232_g363( coords232_g363, time232_g363, id232_g363, uv232_g363, 0, voronoiSmoothId232_g363 );
				float temp_output_242_0_g363 = ( ( ( 1.0 - 0.0 ) - (1.0 + (voroi200_g363 - 0.0) * (-0.5 - 1.0) / (1.0 - 0.0)) ) - voroi232_g363 );
				float AltoCumulusPlacement376_g363 = temp_output_242_0_g363;
				float CloudThicknessDetails286_g363 = ( VoroDetails109_g363.y * saturate( ( AltoCumulusPlacement376_g363 - 0.26 ) ) );
				float3 normalizeResult43_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult46_g363 = dot( normalizeResult43_g363 , CZY_MoonDirection );
				half MoonlightMask57_g363 = saturate( pow( abs( (dotResult46_g363*0.5 + 0.5) ) , CZY_CloudMoonFalloff ) );
				float3 hsvTorgb2_g365 = RGBToHSV( CZY_CloudMoonColor.rgb );
				float3 hsvTorgb3_g365 = HSVToRGB( float3(hsvTorgb2_g365.x,saturate( ( hsvTorgb2_g365.y + CZY_FilterSaturation ) ),( hsvTorgb2_g365.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g365 = ( float4( hsvTorgb3_g365 , 0.0 ) * CZY_FilterColor );
				float4 MoonlightColor60_g363 = ( temp_output_10_0_g365 * CZY_CloudFilterColor );
				float4 lerpResult338_g363 = lerp( ( lerpResult315_g363 + ( LightMask56_g363 * CloudHighlightColor55_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) + ( MoonlightMask57_g363 * MoonlightColor60_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) ) , ( CloudColor41_g363 * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , CloudThicknessDetails286_g363);
				float time84_g363 = 0.0;
				float2 voronoiSmoothId84_g363 = 0;
				float2 coords84_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) ) * ( 100.0 / CZY_DetailScale );
				float2 id84_g363 = 0;
				float2 uv84_g363 = 0;
				float fade84_g363 = 0.5;
				float voroi84_g363 = 0;
				float rest84_g363 = 0;
				for( int it84_g363 = 0; it84_g363 <3; it84_g363++ ){
				voroi84_g363 += fade84_g363 * voronoi84_g363( coords84_g363, time84_g363, id84_g363, uv84_g363, 0,voronoiSmoothId84_g363 );
				rest84_g363 += fade84_g363;
				coords84_g363 *= 2;
				fade84_g363 *= 0.5;
				}//Voronoi84_g363
				voroi84_g363 /= rest84_g363;
				float temp_output_173_0_g363 = ( (0.0 + (( 1.0 - voroi84_g363 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * CZY_DetailAmount );
				float DetailedClouds252_g363 = saturate( ( ComplexCloudDensity141_g363 + temp_output_173_0_g363 ) );
				float CloudDetail179_g363 = temp_output_173_0_g363;
				float2 texCoord79_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_161_0_g363 = ( texCoord79_g363 - float2( 0.5,0.5 ) );
				float dotResult212_g363 = dot( temp_output_161_0_g363 , temp_output_161_0_g363 );
				float BorderHeight154_g363 = ( 1.0 - CZY_BorderHeight );
				float temp_output_151_0_g363 = ( -2.0 * ( 1.0 - CZY_BorderVariation ) );
				float clampResult247_g363 = clamp( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( BorderHeight154_g363 * temp_output_151_0_g363 ) + (dotResult212_g363 - 0.0) * (( temp_output_151_0_g363 * -4.0 ) - ( BorderHeight154_g363 * temp_output_151_0_g363 )) / (0.5 - 0.0)) ) ) * 10.0 * CZY_BorderEffect ) , -1.0 , 1.0 );
				float BorderLightTransport278_g363 = clampResult247_g363;
				float3 normalizeResult116_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float3 normalizeResult146_g363 = normalize( CZY_StormDirection );
				float dotResult150_g363 = dot( normalizeResult116_g363 , normalizeResult146_g363 );
				float2 texCoord98_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_124_0_g363 = ( texCoord98_g363 - float2( 0.5,0.5 ) );
				float dotResult125_g363 = dot( temp_output_124_0_g363 , temp_output_124_0_g363 );
				float temp_output_140_0_g363 = ( -2.0 * ( 1.0 - ( CZY_NimbusVariation * 0.9 ) ) );
				float NimbusLightTransport269_g363 = saturate( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 ) + (( dotResult150_g363 + ( CZY_NimbusHeight * 4.0 * dotResult125_g363 ) ) - 0.5) * (( temp_output_140_0_g363 * -4.0 ) - ( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 )) / (7.0 - 0.5)) ) ) * 10.0 ) );
				float mulTime104_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D143_g363 = snoise( (Pos33_g363*1.0 + mulTime104_g363)*2.0 );
				float mulTime93_g363 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos97_g363 = cos( ( mulTime93_g363 * 0.01 ) );
				float sin97_g363 = sin( ( mulTime93_g363 * 0.01 ) );
				float2 rotator97_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos97_g363 , -sin97_g363 , sin97_g363 , cos97_g363 )) + float2( 0.5,0.5 );
				float cos131_g363 = cos( ( mulTime93_g363 * -0.02 ) );
				float sin131_g363 = sin( ( mulTime93_g363 * -0.02 ) );
				float2 rotator131_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos131_g363 , -sin131_g363 , sin131_g363 , cos131_g363 )) + float2( 0.5,0.5 );
				float mulTime107_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D147_g363 = snoise( (Pos33_g363*1.0 + mulTime107_g363)*4.0 );
				float4 ChemtrailsPattern210_g363 = ( ( saturate( simplePerlin2D143_g363 ) * tex2D( CZY_ChemtrailsTexture, (rotator97_g363*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator131_g363 ) * saturate( simplePerlin2D147_g363 ) ) );
				float2 texCoord139_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_162_0_g363 = ( texCoord139_g363 - float2( 0.5,0.5 ) );
				float dotResult207_g363 = dot( temp_output_162_0_g363 , temp_output_162_0_g363 );
				float ChemtrailsFinal248_g363 = ( ( ChemtrailsPattern210_g363 * saturate( (0.4 + (dotResult207_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - ( CZY_ChemtrailsMultiplier * 0.5 ) ) ? 1.0 : 0.0 );
				float mulTime80_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D126_g363 = snoise( (Pos33_g363*1.0 + mulTime80_g363)*2.0 );
				float mulTime75_g363 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos101_g363 = cos( ( mulTime75_g363 * 0.01 ) );
				float sin101_g363 = sin( ( mulTime75_g363 * 0.01 ) );
				float2 rotator101_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos101_g363 , -sin101_g363 , sin101_g363 , cos101_g363 )) + float2( 0.5,0.5 );
				float cos112_g363 = cos( ( mulTime75_g363 * -0.02 ) );
				float sin112_g363 = sin( ( mulTime75_g363 * -0.02 ) );
				float2 rotator112_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos112_g363 , -sin112_g363 , sin112_g363 , cos112_g363 )) + float2( 0.5,0.5 );
				float mulTime135_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D122_g363 = snoise( (Pos33_g363*1.0 + mulTime135_g363) );
				simplePerlin2D122_g363 = simplePerlin2D122_g363*0.5 + 0.5;
				float4 CirrusPattern137_g363 = ( ( saturate( simplePerlin2D126_g363 ) * tex2D( CZY_CirrusTexture, (rotator101_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator112_g363*1.0 + 0.0) ) * saturate( simplePerlin2D122_g363 ) ) );
				float2 texCoord134_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_164_0_g363 = ( texCoord134_g363 - float2( 0.5,0.5 ) );
				float dotResult157_g363 = dot( temp_output_164_0_g363 , temp_output_164_0_g363 );
				float4 temp_output_217_0_g363 = ( CirrusPattern137_g363 * saturate( (0.0 + (dotResult157_g363 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) );
				float Clipping208_g363 = CZY_ClippingThreshold;
				float CirrusAlpha250_g363 = ( ( temp_output_217_0_g363 * ( CZY_CirrusMultiplier * 10.0 ) ).r > Clipping208_g363 ? 1.0 : 0.0 );
				float SimpleRadiance268_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + NimbusLightTransport269_g363 + ChemtrailsFinal248_g363 + CirrusAlpha250_g363 ) );
				float4 lerpResult342_g363 = lerp( CloudColor41_g363 , lerpResult338_g363 , ( 1.0 - SimpleRadiance268_g363 ));
				float CloudbreakLightDir426_g363 = saturate( pow( temp_output_49_0_g363 , ( CZY_SunFlareFalloff * 0.5 ) ) );
				float lerpResult316_g363 = lerp( -0.4 , 1.0 , ( saturate( ( ComplexCloudDensity141_g363 - 0.0 ) ) * CloudDetail179_g363 * CloudbreakLightDir426_g363 ));
				float SunThroughClouds399_g363 = saturate( lerpResult316_g363 );
				float3 hsvTorgb2_g366 = RGBToHSV( CZY_AltoCloudColor.rgb );
				float3 hsvTorgb3_g366 = HSVToRGB( float3(hsvTorgb2_g366.x,saturate( ( hsvTorgb2_g366.y + CZY_FilterSaturation ) ),( hsvTorgb2_g366.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g366 = ( float4( hsvTorgb3_g366 , 0.0 ) * CZY_FilterColor );
				float4 CirrusCustomLightColor350_g363 = ( CloudColor41_g363 * ( temp_output_10_0_g366 * CZY_CloudFilterColor ) );
				float temp_output_391_0_g363 = ( AltoCumulusPlacement376_g363 * (0.0 + (tex2D( CZY_AltocumulusTexture, ((Pos33_g363*1.0 + ( CZY_AltocumulusWindSpeed * TIme30_g363 ))*( 1.0 / CZY_AltocumulusScale ) + 0.0) ).r - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) * CZY_AltocumulusMultiplier );
				float AltoCumulusLightTransport393_g363 = temp_output_391_0_g363;
				float ACCustomLightsClipping387_g363 = ( AltoCumulusLightTransport393_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float mulTime193_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D224_g363 = snoise( (Pos33_g363*1.0 + mulTime193_g363)*2.0 );
				float mulTime178_g363 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos138_g363 = cos( ( mulTime178_g363 * 0.01 ) );
				float sin138_g363 = sin( ( mulTime178_g363 * 0.01 ) );
				float2 rotator138_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos138_g363 , -sin138_g363 , sin138_g363 , cos138_g363 )) + float2( 0.5,0.5 );
				float cos198_g363 = cos( ( mulTime178_g363 * -0.02 ) );
				float sin198_g363 = sin( ( mulTime178_g363 * -0.02 ) );
				float2 rotator198_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos198_g363 , -sin198_g363 , sin198_g363 , cos198_g363 )) + float2( 0.5,0.5 );
				float mulTime184_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D216_g363 = snoise( (Pos33_g363*10.0 + mulTime184_g363)*4.0 );
				float4 CirrostratPattern261_g363 = ( ( saturate( simplePerlin2D224_g363 ) * tex2D( CZY_CirrostratusTexture, (rotator138_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator198_g363*1.5 + 0.75) ) * saturate( simplePerlin2D216_g363 ) ) );
				float2 texCoord234_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_243_0_g363 = ( texCoord234_g363 - float2( 0.5,0.5 ) );
				float dotResult238_g363 = dot( temp_output_243_0_g363 , temp_output_243_0_g363 );
				float clampResult264_g363 = clamp( ( CZY_CirrostratusMultiplier * 0.5 ) , 0.0 , 0.98 );
				float CirrostratLightTransport281_g363 = ( ( CirrostratPattern261_g363 * saturate( (0.4 + (dotResult238_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - clampResult264_g363 ) ? 1.0 : 0.0 );
				float CSCustomLightsClipping309_g363 = ( CirrostratLightTransport281_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float CustomRadiance340_g363 = saturate( ( ACCustomLightsClipping387_g363 + CSCustomLightsClipping309_g363 ) );
				float4 lerpResult331_g363 = lerp( ( lerpResult342_g363 + SunThroughClouds399_g363 ) , CirrusCustomLightColor350_g363 , CustomRadiance340_g363);
				float FinalAlpha375_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + AltoCumulusLightTransport393_g363 + ChemtrailsFinal248_g363 + CirrostratLightTransport281_g363 + CirrusAlpha250_g363 + NimbusLightTransport269_g363 ) );
				float4 appendResult420_g363 = (float4((lerpResult331_g363).rgb , FinalAlpha375_g363));
				float4 FinalCloudColor325_g363 = appendResult420_g363;
				bool enabled20_g367 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g367 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g367 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g367 = HLSL20_g367( enabled20_g367 , submerged20_g367 , textureSample20_g367 );
				

				float Alpha = ( ( (FinalCloudColor325_g363).w * ( 1.0 - localHLSL20_g367 ) ) > Clipping208_g363 ? 1.0 : 0.0 );
				float AlphaClipThreshold = Clipping208_g363;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 140010


			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			float4 CZY_CloudColor;
			float CZY_FilterSaturation;
			float CZY_FilterValue;
			float4 CZY_FilterColor;
			float4 CZY_CloudFilterColor;
			float4 CZY_CloudHighlightColor;
			float4 CZY_SunFilterColor;
			float CZY_WindSpeed;
			float CZY_MainCloudScale;
			float CZY_CumulusCoverageMultiplier;
			float3 CZY_SunDirection;
			half CZY_SunFlareFalloff;
			float3 CZY_MoonDirection;
			half CZY_CloudMoonFalloff;
			float4 CZY_CloudMoonColor;
			float CZY_DetailScale;
			float CZY_DetailAmount;
			float CZY_BorderHeight;
			float CZY_BorderVariation;
			float CZY_BorderEffect;
			float3 CZY_StormDirection;
			float CZY_NimbusHeight;
			float CZY_NimbusMultiplier;
			float CZY_NimbusVariation;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_ClippingThreshold;
			float4 CZY_AltoCloudColor;
			sampler2D CZY_AltocumulusTexture;
			float2 CZY_AltocumulusWindSpeed;
			float CZY_AltocumulusScale;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash81_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi81_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash81_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash88_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi88_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash88_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash200_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi200_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash200_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash232_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi232_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash232_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash84_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi84_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash84_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			float HLSL20_g367( bool enabled, bool submerged, float textureSample )
			{
				if(enabled)
				{
					if(submerged) return 1.0;
					else return textureSample;
				}
				else
				{
					return 0.0;
				}
			}
			

			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				output.ase_texcoord1.xyz = ase_positionWS;
				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord2 = screenPos;
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;
				output.ase_texcoord1.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 hsvTorgb2_g364 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g364 = HSVToRGB( float3(hsvTorgb2_g364.x,saturate( ( hsvTorgb2_g364.y + CZY_FilterSaturation ) ),( hsvTorgb2_g364.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g364 = ( float4( hsvTorgb3_g364 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor41_g363 = ( temp_output_10_0_g364 * CZY_CloudFilterColor );
				float3 hsvTorgb2_g368 = RGBToHSV( CZY_CloudHighlightColor.rgb );
				float3 hsvTorgb3_g368 = HSVToRGB( float3(hsvTorgb2_g368.x,saturate( ( hsvTorgb2_g368.y + CZY_FilterSaturation ) ),( hsvTorgb2_g368.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g368 = ( float4( hsvTorgb3_g368 , 0.0 ) * CZY_FilterColor );
				float4 CloudHighlightColor55_g363 = ( temp_output_10_0_g368 * CZY_SunFilterColor );
				float2 texCoord31_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos33_g363 = texCoord31_g363;
				float mulTime29_g363 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme30_g363 = mulTime29_g363;
				float simplePerlin2D409_g363 = snoise( ( Pos33_g363 + ( TIme30_g363 * float2( 0.2,-0.4 ) ) )*( 100.0 / CZY_MainCloudScale ) );
				simplePerlin2D409_g363 = simplePerlin2D409_g363*0.5 + 0.5;
				float SimpleCloudDensity153_g363 = simplePerlin2D409_g363;
				float time81_g363 = 0.0;
				float2 voronoiSmoothId81_g363 = 0;
				float2 temp_output_94_0_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) );
				float2 coords81_g363 = temp_output_94_0_g363 * ( 140.0 / CZY_MainCloudScale );
				float2 id81_g363 = 0;
				float2 uv81_g363 = 0;
				float voroi81_g363 = voronoi81_g363( coords81_g363, time81_g363, id81_g363, uv81_g363, 0, voronoiSmoothId81_g363 );
				float time88_g363 = 0.0;
				float2 voronoiSmoothId88_g363 = 0;
				float2 coords88_g363 = temp_output_94_0_g363 * ( 500.0 / CZY_MainCloudScale );
				float2 id88_g363 = 0;
				float2 uv88_g363 = 0;
				float voroi88_g363 = voronoi88_g363( coords88_g363, time88_g363, id88_g363, uv88_g363, 0, voronoiSmoothId88_g363 );
				float2 appendResult95_g363 = (float2(voroi81_g363 , voroi88_g363));
				float2 VoroDetails109_g363 = appendResult95_g363;
				float CumulusCoverage34_g363 = CZY_CumulusCoverageMultiplier;
				float ComplexCloudDensity141_g363 = (0.0 + (min( SimpleCloudDensity153_g363 , ( 1.0 - VoroDetails109_g363.x ) ) - ( 1.0 - CumulusCoverage34_g363 )) * (1.0 - 0.0) / (1.0 - ( 1.0 - CumulusCoverage34_g363 )));
				float4 lerpResult315_g363 = lerp( CloudHighlightColor55_g363 , CloudColor41_g363 , saturate( (2.0 + (ComplexCloudDensity141_g363 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
				float3 ase_positionWS = input.ase_texcoord1.xyz;
				float3 normalizeResult40_g363 = normalize( ( ase_positionWS - _WorldSpaceCameraPos ) );
				float dotResult42_g363 = dot( normalizeResult40_g363 , CZY_SunDirection );
				float temp_output_49_0_g363 = abs( (dotResult42_g363*0.5 + 0.5) );
				half LightMask56_g363 = saturate( pow( temp_output_49_0_g363 , CZY_SunFlareFalloff ) );
				float time200_g363 = 0.0;
				float2 voronoiSmoothId200_g363 = 0;
				float mulTime163_g363 = _TimeParameters.x * 0.003;
				float2 coords200_g363 = (Pos33_g363*1.0 + ( float2( 1,-2 ) * mulTime163_g363 )) * 10.0;
				float2 id200_g363 = 0;
				float2 uv200_g363 = 0;
				float voroi200_g363 = voronoi200_g363( coords200_g363, time200_g363, id200_g363, uv200_g363, 0, voronoiSmoothId200_g363 );
				float time232_g363 = ( 10.0 * mulTime163_g363 );
				float2 voronoiSmoothId232_g363 = 0;
				float2 coords232_g363 = input.ase_texcoord.xy * 10.0;
				float2 id232_g363 = 0;
				float2 uv232_g363 = 0;
				float voroi232_g363 = voronoi232_g363( coords232_g363, time232_g363, id232_g363, uv232_g363, 0, voronoiSmoothId232_g363 );
				float temp_output_242_0_g363 = ( ( ( 1.0 - 0.0 ) - (1.0 + (voroi200_g363 - 0.0) * (-0.5 - 1.0) / (1.0 - 0.0)) ) - voroi232_g363 );
				float AltoCumulusPlacement376_g363 = temp_output_242_0_g363;
				float CloudThicknessDetails286_g363 = ( VoroDetails109_g363.y * saturate( ( AltoCumulusPlacement376_g363 - 0.26 ) ) );
				float3 normalizeResult43_g363 = normalize( ( ase_positionWS - _WorldSpaceCameraPos ) );
				float dotResult46_g363 = dot( normalizeResult43_g363 , CZY_MoonDirection );
				half MoonlightMask57_g363 = saturate( pow( abs( (dotResult46_g363*0.5 + 0.5) ) , CZY_CloudMoonFalloff ) );
				float3 hsvTorgb2_g365 = RGBToHSV( CZY_CloudMoonColor.rgb );
				float3 hsvTorgb3_g365 = HSVToRGB( float3(hsvTorgb2_g365.x,saturate( ( hsvTorgb2_g365.y + CZY_FilterSaturation ) ),( hsvTorgb2_g365.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g365 = ( float4( hsvTorgb3_g365 , 0.0 ) * CZY_FilterColor );
				float4 MoonlightColor60_g363 = ( temp_output_10_0_g365 * CZY_CloudFilterColor );
				float4 lerpResult338_g363 = lerp( ( lerpResult315_g363 + ( LightMask56_g363 * CloudHighlightColor55_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) + ( MoonlightMask57_g363 * MoonlightColor60_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) ) , ( CloudColor41_g363 * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , CloudThicknessDetails286_g363);
				float time84_g363 = 0.0;
				float2 voronoiSmoothId84_g363 = 0;
				float2 coords84_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) ) * ( 100.0 / CZY_DetailScale );
				float2 id84_g363 = 0;
				float2 uv84_g363 = 0;
				float fade84_g363 = 0.5;
				float voroi84_g363 = 0;
				float rest84_g363 = 0;
				for( int it84_g363 = 0; it84_g363 <3; it84_g363++ ){
				voroi84_g363 += fade84_g363 * voronoi84_g363( coords84_g363, time84_g363, id84_g363, uv84_g363, 0,voronoiSmoothId84_g363 );
				rest84_g363 += fade84_g363;
				coords84_g363 *= 2;
				fade84_g363 *= 0.5;
				}//Voronoi84_g363
				voroi84_g363 /= rest84_g363;
				float temp_output_173_0_g363 = ( (0.0 + (( 1.0 - voroi84_g363 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * CZY_DetailAmount );
				float DetailedClouds252_g363 = saturate( ( ComplexCloudDensity141_g363 + temp_output_173_0_g363 ) );
				float CloudDetail179_g363 = temp_output_173_0_g363;
				float2 texCoord79_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_161_0_g363 = ( texCoord79_g363 - float2( 0.5,0.5 ) );
				float dotResult212_g363 = dot( temp_output_161_0_g363 , temp_output_161_0_g363 );
				float BorderHeight154_g363 = ( 1.0 - CZY_BorderHeight );
				float temp_output_151_0_g363 = ( -2.0 * ( 1.0 - CZY_BorderVariation ) );
				float clampResult247_g363 = clamp( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( BorderHeight154_g363 * temp_output_151_0_g363 ) + (dotResult212_g363 - 0.0) * (( temp_output_151_0_g363 * -4.0 ) - ( BorderHeight154_g363 * temp_output_151_0_g363 )) / (0.5 - 0.0)) ) ) * 10.0 * CZY_BorderEffect ) , -1.0 , 1.0 );
				float BorderLightTransport278_g363 = clampResult247_g363;
				float3 normalizeResult116_g363 = normalize( ( ase_positionWS - _WorldSpaceCameraPos ) );
				float3 normalizeResult146_g363 = normalize( CZY_StormDirection );
				float dotResult150_g363 = dot( normalizeResult116_g363 , normalizeResult146_g363 );
				float2 texCoord98_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_124_0_g363 = ( texCoord98_g363 - float2( 0.5,0.5 ) );
				float dotResult125_g363 = dot( temp_output_124_0_g363 , temp_output_124_0_g363 );
				float temp_output_140_0_g363 = ( -2.0 * ( 1.0 - ( CZY_NimbusVariation * 0.9 ) ) );
				float NimbusLightTransport269_g363 = saturate( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 ) + (( dotResult150_g363 + ( CZY_NimbusHeight * 4.0 * dotResult125_g363 ) ) - 0.5) * (( temp_output_140_0_g363 * -4.0 ) - ( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 )) / (7.0 - 0.5)) ) ) * 10.0 ) );
				float mulTime104_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D143_g363 = snoise( (Pos33_g363*1.0 + mulTime104_g363)*2.0 );
				float mulTime93_g363 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos97_g363 = cos( ( mulTime93_g363 * 0.01 ) );
				float sin97_g363 = sin( ( mulTime93_g363 * 0.01 ) );
				float2 rotator97_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos97_g363 , -sin97_g363 , sin97_g363 , cos97_g363 )) + float2( 0.5,0.5 );
				float cos131_g363 = cos( ( mulTime93_g363 * -0.02 ) );
				float sin131_g363 = sin( ( mulTime93_g363 * -0.02 ) );
				float2 rotator131_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos131_g363 , -sin131_g363 , sin131_g363 , cos131_g363 )) + float2( 0.5,0.5 );
				float mulTime107_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D147_g363 = snoise( (Pos33_g363*1.0 + mulTime107_g363)*4.0 );
				float4 ChemtrailsPattern210_g363 = ( ( saturate( simplePerlin2D143_g363 ) * tex2D( CZY_ChemtrailsTexture, (rotator97_g363*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator131_g363 ) * saturate( simplePerlin2D147_g363 ) ) );
				float2 texCoord139_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_162_0_g363 = ( texCoord139_g363 - float2( 0.5,0.5 ) );
				float dotResult207_g363 = dot( temp_output_162_0_g363 , temp_output_162_0_g363 );
				float ChemtrailsFinal248_g363 = ( ( ChemtrailsPattern210_g363 * saturate( (0.4 + (dotResult207_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - ( CZY_ChemtrailsMultiplier * 0.5 ) ) ? 1.0 : 0.0 );
				float mulTime80_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D126_g363 = snoise( (Pos33_g363*1.0 + mulTime80_g363)*2.0 );
				float mulTime75_g363 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos101_g363 = cos( ( mulTime75_g363 * 0.01 ) );
				float sin101_g363 = sin( ( mulTime75_g363 * 0.01 ) );
				float2 rotator101_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos101_g363 , -sin101_g363 , sin101_g363 , cos101_g363 )) + float2( 0.5,0.5 );
				float cos112_g363 = cos( ( mulTime75_g363 * -0.02 ) );
				float sin112_g363 = sin( ( mulTime75_g363 * -0.02 ) );
				float2 rotator112_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos112_g363 , -sin112_g363 , sin112_g363 , cos112_g363 )) + float2( 0.5,0.5 );
				float mulTime135_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D122_g363 = snoise( (Pos33_g363*1.0 + mulTime135_g363) );
				simplePerlin2D122_g363 = simplePerlin2D122_g363*0.5 + 0.5;
				float4 CirrusPattern137_g363 = ( ( saturate( simplePerlin2D126_g363 ) * tex2D( CZY_CirrusTexture, (rotator101_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator112_g363*1.0 + 0.0) ) * saturate( simplePerlin2D122_g363 ) ) );
				float2 texCoord134_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_164_0_g363 = ( texCoord134_g363 - float2( 0.5,0.5 ) );
				float dotResult157_g363 = dot( temp_output_164_0_g363 , temp_output_164_0_g363 );
				float4 temp_output_217_0_g363 = ( CirrusPattern137_g363 * saturate( (0.0 + (dotResult157_g363 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) );
				float Clipping208_g363 = CZY_ClippingThreshold;
				float CirrusAlpha250_g363 = ( ( temp_output_217_0_g363 * ( CZY_CirrusMultiplier * 10.0 ) ).r > Clipping208_g363 ? 1.0 : 0.0 );
				float SimpleRadiance268_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + NimbusLightTransport269_g363 + ChemtrailsFinal248_g363 + CirrusAlpha250_g363 ) );
				float4 lerpResult342_g363 = lerp( CloudColor41_g363 , lerpResult338_g363 , ( 1.0 - SimpleRadiance268_g363 ));
				float CloudbreakLightDir426_g363 = saturate( pow( temp_output_49_0_g363 , ( CZY_SunFlareFalloff * 0.5 ) ) );
				float lerpResult316_g363 = lerp( -0.4 , 1.0 , ( saturate( ( ComplexCloudDensity141_g363 - 0.0 ) ) * CloudDetail179_g363 * CloudbreakLightDir426_g363 ));
				float SunThroughClouds399_g363 = saturate( lerpResult316_g363 );
				float3 hsvTorgb2_g366 = RGBToHSV( CZY_AltoCloudColor.rgb );
				float3 hsvTorgb3_g366 = HSVToRGB( float3(hsvTorgb2_g366.x,saturate( ( hsvTorgb2_g366.y + CZY_FilterSaturation ) ),( hsvTorgb2_g366.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g366 = ( float4( hsvTorgb3_g366 , 0.0 ) * CZY_FilterColor );
				float4 CirrusCustomLightColor350_g363 = ( CloudColor41_g363 * ( temp_output_10_0_g366 * CZY_CloudFilterColor ) );
				float temp_output_391_0_g363 = ( AltoCumulusPlacement376_g363 * (0.0 + (tex2D( CZY_AltocumulusTexture, ((Pos33_g363*1.0 + ( CZY_AltocumulusWindSpeed * TIme30_g363 ))*( 1.0 / CZY_AltocumulusScale ) + 0.0) ).r - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) * CZY_AltocumulusMultiplier );
				float AltoCumulusLightTransport393_g363 = temp_output_391_0_g363;
				float ACCustomLightsClipping387_g363 = ( AltoCumulusLightTransport393_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float mulTime193_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D224_g363 = snoise( (Pos33_g363*1.0 + mulTime193_g363)*2.0 );
				float mulTime178_g363 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos138_g363 = cos( ( mulTime178_g363 * 0.01 ) );
				float sin138_g363 = sin( ( mulTime178_g363 * 0.01 ) );
				float2 rotator138_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos138_g363 , -sin138_g363 , sin138_g363 , cos138_g363 )) + float2( 0.5,0.5 );
				float cos198_g363 = cos( ( mulTime178_g363 * -0.02 ) );
				float sin198_g363 = sin( ( mulTime178_g363 * -0.02 ) );
				float2 rotator198_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos198_g363 , -sin198_g363 , sin198_g363 , cos198_g363 )) + float2( 0.5,0.5 );
				float mulTime184_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D216_g363 = snoise( (Pos33_g363*10.0 + mulTime184_g363)*4.0 );
				float4 CirrostratPattern261_g363 = ( ( saturate( simplePerlin2D224_g363 ) * tex2D( CZY_CirrostratusTexture, (rotator138_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator198_g363*1.5 + 0.75) ) * saturate( simplePerlin2D216_g363 ) ) );
				float2 texCoord234_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_243_0_g363 = ( texCoord234_g363 - float2( 0.5,0.5 ) );
				float dotResult238_g363 = dot( temp_output_243_0_g363 , temp_output_243_0_g363 );
				float clampResult264_g363 = clamp( ( CZY_CirrostratusMultiplier * 0.5 ) , 0.0 , 0.98 );
				float CirrostratLightTransport281_g363 = ( ( CirrostratPattern261_g363 * saturate( (0.4 + (dotResult238_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - clampResult264_g363 ) ? 1.0 : 0.0 );
				float CSCustomLightsClipping309_g363 = ( CirrostratLightTransport281_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float CustomRadiance340_g363 = saturate( ( ACCustomLightsClipping387_g363 + CSCustomLightsClipping309_g363 ) );
				float4 lerpResult331_g363 = lerp( ( lerpResult342_g363 + SunThroughClouds399_g363 ) , CirrusCustomLightColor350_g363 , CustomRadiance340_g363);
				float FinalAlpha375_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + AltoCumulusLightTransport393_g363 + ChemtrailsFinal248_g363 + CirrostratLightTransport281_g363 + CirrusAlpha250_g363 + NimbusLightTransport269_g363 ) );
				float4 appendResult420_g363 = (float4((lerpResult331_g363).rgb , FinalAlpha375_g363));
				float4 FinalCloudColor325_g363 = appendResult420_g363;
				bool enabled20_g367 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g367 =(bool)_FullySubmerged;
				float4 screenPos = input.ase_texcoord2;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g367 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g367 = HLSL20_g367( enabled20_g367 , submerged20_g367 , textureSample20_g367 );
				

				surfaceDescription.Alpha = ( ( (FinalCloudColor325_g363).w * ( 1.0 - localHLSL20_g367 ) ) > Clipping208_g363 ? 1.0 : 0.0 );
				surfaceDescription.AlphaClipThreshold = Clipping208_g363;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19801
			#define ASE_SRP_VERSION 140010


			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT

			#define SHADERPASS SHADERPASS_DEPTHONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			float4 CZY_CloudColor;
			float CZY_FilterSaturation;
			float CZY_FilterValue;
			float4 CZY_FilterColor;
			float4 CZY_CloudFilterColor;
			float4 CZY_CloudHighlightColor;
			float4 CZY_SunFilterColor;
			float CZY_WindSpeed;
			float CZY_MainCloudScale;
			float CZY_CumulusCoverageMultiplier;
			float3 CZY_SunDirection;
			half CZY_SunFlareFalloff;
			float3 CZY_MoonDirection;
			half CZY_CloudMoonFalloff;
			float4 CZY_CloudMoonColor;
			float CZY_DetailScale;
			float CZY_DetailAmount;
			float CZY_BorderHeight;
			float CZY_BorderVariation;
			float CZY_BorderEffect;
			float3 CZY_StormDirection;
			float CZY_NimbusHeight;
			float CZY_NimbusMultiplier;
			float CZY_NimbusVariation;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_ClippingThreshold;
			float4 CZY_AltoCloudColor;
			sampler2D CZY_AltocumulusTexture;
			float2 CZY_AltocumulusWindSpeed;
			float CZY_AltocumulusScale;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash81_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi81_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash81_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash88_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi88_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash88_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash200_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi200_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash200_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash232_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi232_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash232_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash84_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi84_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash84_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			float HLSL20_g367( bool enabled, bool submerged, float textureSample )
			{
				if(enabled)
				{
					if(submerged) return 1.0;
					else return textureSample;
				}
				else
				{
					return 0.0;
				}
			}
			

			float4 _SelectionID;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				output.ase_texcoord1.xyz = ase_positionWS;
				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord2 = screenPos;
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;
				output.ase_texcoord1.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				output.positionCS = TransformWorldToHClip(positionWS);
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 hsvTorgb2_g364 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g364 = HSVToRGB( float3(hsvTorgb2_g364.x,saturate( ( hsvTorgb2_g364.y + CZY_FilterSaturation ) ),( hsvTorgb2_g364.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g364 = ( float4( hsvTorgb3_g364 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor41_g363 = ( temp_output_10_0_g364 * CZY_CloudFilterColor );
				float3 hsvTorgb2_g368 = RGBToHSV( CZY_CloudHighlightColor.rgb );
				float3 hsvTorgb3_g368 = HSVToRGB( float3(hsvTorgb2_g368.x,saturate( ( hsvTorgb2_g368.y + CZY_FilterSaturation ) ),( hsvTorgb2_g368.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g368 = ( float4( hsvTorgb3_g368 , 0.0 ) * CZY_FilterColor );
				float4 CloudHighlightColor55_g363 = ( temp_output_10_0_g368 * CZY_SunFilterColor );
				float2 texCoord31_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos33_g363 = texCoord31_g363;
				float mulTime29_g363 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme30_g363 = mulTime29_g363;
				float simplePerlin2D409_g363 = snoise( ( Pos33_g363 + ( TIme30_g363 * float2( 0.2,-0.4 ) ) )*( 100.0 / CZY_MainCloudScale ) );
				simplePerlin2D409_g363 = simplePerlin2D409_g363*0.5 + 0.5;
				float SimpleCloudDensity153_g363 = simplePerlin2D409_g363;
				float time81_g363 = 0.0;
				float2 voronoiSmoothId81_g363 = 0;
				float2 temp_output_94_0_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) );
				float2 coords81_g363 = temp_output_94_0_g363 * ( 140.0 / CZY_MainCloudScale );
				float2 id81_g363 = 0;
				float2 uv81_g363 = 0;
				float voroi81_g363 = voronoi81_g363( coords81_g363, time81_g363, id81_g363, uv81_g363, 0, voronoiSmoothId81_g363 );
				float time88_g363 = 0.0;
				float2 voronoiSmoothId88_g363 = 0;
				float2 coords88_g363 = temp_output_94_0_g363 * ( 500.0 / CZY_MainCloudScale );
				float2 id88_g363 = 0;
				float2 uv88_g363 = 0;
				float voroi88_g363 = voronoi88_g363( coords88_g363, time88_g363, id88_g363, uv88_g363, 0, voronoiSmoothId88_g363 );
				float2 appendResult95_g363 = (float2(voroi81_g363 , voroi88_g363));
				float2 VoroDetails109_g363 = appendResult95_g363;
				float CumulusCoverage34_g363 = CZY_CumulusCoverageMultiplier;
				float ComplexCloudDensity141_g363 = (0.0 + (min( SimpleCloudDensity153_g363 , ( 1.0 - VoroDetails109_g363.x ) ) - ( 1.0 - CumulusCoverage34_g363 )) * (1.0 - 0.0) / (1.0 - ( 1.0 - CumulusCoverage34_g363 )));
				float4 lerpResult315_g363 = lerp( CloudHighlightColor55_g363 , CloudColor41_g363 , saturate( (2.0 + (ComplexCloudDensity141_g363 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
				float3 ase_positionWS = input.ase_texcoord1.xyz;
				float3 normalizeResult40_g363 = normalize( ( ase_positionWS - _WorldSpaceCameraPos ) );
				float dotResult42_g363 = dot( normalizeResult40_g363 , CZY_SunDirection );
				float temp_output_49_0_g363 = abs( (dotResult42_g363*0.5 + 0.5) );
				half LightMask56_g363 = saturate( pow( temp_output_49_0_g363 , CZY_SunFlareFalloff ) );
				float time200_g363 = 0.0;
				float2 voronoiSmoothId200_g363 = 0;
				float mulTime163_g363 = _TimeParameters.x * 0.003;
				float2 coords200_g363 = (Pos33_g363*1.0 + ( float2( 1,-2 ) * mulTime163_g363 )) * 10.0;
				float2 id200_g363 = 0;
				float2 uv200_g363 = 0;
				float voroi200_g363 = voronoi200_g363( coords200_g363, time200_g363, id200_g363, uv200_g363, 0, voronoiSmoothId200_g363 );
				float time232_g363 = ( 10.0 * mulTime163_g363 );
				float2 voronoiSmoothId232_g363 = 0;
				float2 coords232_g363 = input.ase_texcoord.xy * 10.0;
				float2 id232_g363 = 0;
				float2 uv232_g363 = 0;
				float voroi232_g363 = voronoi232_g363( coords232_g363, time232_g363, id232_g363, uv232_g363, 0, voronoiSmoothId232_g363 );
				float temp_output_242_0_g363 = ( ( ( 1.0 - 0.0 ) - (1.0 + (voroi200_g363 - 0.0) * (-0.5 - 1.0) / (1.0 - 0.0)) ) - voroi232_g363 );
				float AltoCumulusPlacement376_g363 = temp_output_242_0_g363;
				float CloudThicknessDetails286_g363 = ( VoroDetails109_g363.y * saturate( ( AltoCumulusPlacement376_g363 - 0.26 ) ) );
				float3 normalizeResult43_g363 = normalize( ( ase_positionWS - _WorldSpaceCameraPos ) );
				float dotResult46_g363 = dot( normalizeResult43_g363 , CZY_MoonDirection );
				half MoonlightMask57_g363 = saturate( pow( abs( (dotResult46_g363*0.5 + 0.5) ) , CZY_CloudMoonFalloff ) );
				float3 hsvTorgb2_g365 = RGBToHSV( CZY_CloudMoonColor.rgb );
				float3 hsvTorgb3_g365 = HSVToRGB( float3(hsvTorgb2_g365.x,saturate( ( hsvTorgb2_g365.y + CZY_FilterSaturation ) ),( hsvTorgb2_g365.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g365 = ( float4( hsvTorgb3_g365 , 0.0 ) * CZY_FilterColor );
				float4 MoonlightColor60_g363 = ( temp_output_10_0_g365 * CZY_CloudFilterColor );
				float4 lerpResult338_g363 = lerp( ( lerpResult315_g363 + ( LightMask56_g363 * CloudHighlightColor55_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) + ( MoonlightMask57_g363 * MoonlightColor60_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) ) , ( CloudColor41_g363 * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , CloudThicknessDetails286_g363);
				float time84_g363 = 0.0;
				float2 voronoiSmoothId84_g363 = 0;
				float2 coords84_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) ) * ( 100.0 / CZY_DetailScale );
				float2 id84_g363 = 0;
				float2 uv84_g363 = 0;
				float fade84_g363 = 0.5;
				float voroi84_g363 = 0;
				float rest84_g363 = 0;
				for( int it84_g363 = 0; it84_g363 <3; it84_g363++ ){
				voroi84_g363 += fade84_g363 * voronoi84_g363( coords84_g363, time84_g363, id84_g363, uv84_g363, 0,voronoiSmoothId84_g363 );
				rest84_g363 += fade84_g363;
				coords84_g363 *= 2;
				fade84_g363 *= 0.5;
				}//Voronoi84_g363
				voroi84_g363 /= rest84_g363;
				float temp_output_173_0_g363 = ( (0.0 + (( 1.0 - voroi84_g363 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * CZY_DetailAmount );
				float DetailedClouds252_g363 = saturate( ( ComplexCloudDensity141_g363 + temp_output_173_0_g363 ) );
				float CloudDetail179_g363 = temp_output_173_0_g363;
				float2 texCoord79_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_161_0_g363 = ( texCoord79_g363 - float2( 0.5,0.5 ) );
				float dotResult212_g363 = dot( temp_output_161_0_g363 , temp_output_161_0_g363 );
				float BorderHeight154_g363 = ( 1.0 - CZY_BorderHeight );
				float temp_output_151_0_g363 = ( -2.0 * ( 1.0 - CZY_BorderVariation ) );
				float clampResult247_g363 = clamp( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( BorderHeight154_g363 * temp_output_151_0_g363 ) + (dotResult212_g363 - 0.0) * (( temp_output_151_0_g363 * -4.0 ) - ( BorderHeight154_g363 * temp_output_151_0_g363 )) / (0.5 - 0.0)) ) ) * 10.0 * CZY_BorderEffect ) , -1.0 , 1.0 );
				float BorderLightTransport278_g363 = clampResult247_g363;
				float3 normalizeResult116_g363 = normalize( ( ase_positionWS - _WorldSpaceCameraPos ) );
				float3 normalizeResult146_g363 = normalize( CZY_StormDirection );
				float dotResult150_g363 = dot( normalizeResult116_g363 , normalizeResult146_g363 );
				float2 texCoord98_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_124_0_g363 = ( texCoord98_g363 - float2( 0.5,0.5 ) );
				float dotResult125_g363 = dot( temp_output_124_0_g363 , temp_output_124_0_g363 );
				float temp_output_140_0_g363 = ( -2.0 * ( 1.0 - ( CZY_NimbusVariation * 0.9 ) ) );
				float NimbusLightTransport269_g363 = saturate( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 ) + (( dotResult150_g363 + ( CZY_NimbusHeight * 4.0 * dotResult125_g363 ) ) - 0.5) * (( temp_output_140_0_g363 * -4.0 ) - ( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 )) / (7.0 - 0.5)) ) ) * 10.0 ) );
				float mulTime104_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D143_g363 = snoise( (Pos33_g363*1.0 + mulTime104_g363)*2.0 );
				float mulTime93_g363 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos97_g363 = cos( ( mulTime93_g363 * 0.01 ) );
				float sin97_g363 = sin( ( mulTime93_g363 * 0.01 ) );
				float2 rotator97_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos97_g363 , -sin97_g363 , sin97_g363 , cos97_g363 )) + float2( 0.5,0.5 );
				float cos131_g363 = cos( ( mulTime93_g363 * -0.02 ) );
				float sin131_g363 = sin( ( mulTime93_g363 * -0.02 ) );
				float2 rotator131_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos131_g363 , -sin131_g363 , sin131_g363 , cos131_g363 )) + float2( 0.5,0.5 );
				float mulTime107_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D147_g363 = snoise( (Pos33_g363*1.0 + mulTime107_g363)*4.0 );
				float4 ChemtrailsPattern210_g363 = ( ( saturate( simplePerlin2D143_g363 ) * tex2D( CZY_ChemtrailsTexture, (rotator97_g363*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator131_g363 ) * saturate( simplePerlin2D147_g363 ) ) );
				float2 texCoord139_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_162_0_g363 = ( texCoord139_g363 - float2( 0.5,0.5 ) );
				float dotResult207_g363 = dot( temp_output_162_0_g363 , temp_output_162_0_g363 );
				float ChemtrailsFinal248_g363 = ( ( ChemtrailsPattern210_g363 * saturate( (0.4 + (dotResult207_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - ( CZY_ChemtrailsMultiplier * 0.5 ) ) ? 1.0 : 0.0 );
				float mulTime80_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D126_g363 = snoise( (Pos33_g363*1.0 + mulTime80_g363)*2.0 );
				float mulTime75_g363 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos101_g363 = cos( ( mulTime75_g363 * 0.01 ) );
				float sin101_g363 = sin( ( mulTime75_g363 * 0.01 ) );
				float2 rotator101_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos101_g363 , -sin101_g363 , sin101_g363 , cos101_g363 )) + float2( 0.5,0.5 );
				float cos112_g363 = cos( ( mulTime75_g363 * -0.02 ) );
				float sin112_g363 = sin( ( mulTime75_g363 * -0.02 ) );
				float2 rotator112_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos112_g363 , -sin112_g363 , sin112_g363 , cos112_g363 )) + float2( 0.5,0.5 );
				float mulTime135_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D122_g363 = snoise( (Pos33_g363*1.0 + mulTime135_g363) );
				simplePerlin2D122_g363 = simplePerlin2D122_g363*0.5 + 0.5;
				float4 CirrusPattern137_g363 = ( ( saturate( simplePerlin2D126_g363 ) * tex2D( CZY_CirrusTexture, (rotator101_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator112_g363*1.0 + 0.0) ) * saturate( simplePerlin2D122_g363 ) ) );
				float2 texCoord134_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_164_0_g363 = ( texCoord134_g363 - float2( 0.5,0.5 ) );
				float dotResult157_g363 = dot( temp_output_164_0_g363 , temp_output_164_0_g363 );
				float4 temp_output_217_0_g363 = ( CirrusPattern137_g363 * saturate( (0.0 + (dotResult157_g363 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) );
				float Clipping208_g363 = CZY_ClippingThreshold;
				float CirrusAlpha250_g363 = ( ( temp_output_217_0_g363 * ( CZY_CirrusMultiplier * 10.0 ) ).r > Clipping208_g363 ? 1.0 : 0.0 );
				float SimpleRadiance268_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + NimbusLightTransport269_g363 + ChemtrailsFinal248_g363 + CirrusAlpha250_g363 ) );
				float4 lerpResult342_g363 = lerp( CloudColor41_g363 , lerpResult338_g363 , ( 1.0 - SimpleRadiance268_g363 ));
				float CloudbreakLightDir426_g363 = saturate( pow( temp_output_49_0_g363 , ( CZY_SunFlareFalloff * 0.5 ) ) );
				float lerpResult316_g363 = lerp( -0.4 , 1.0 , ( saturate( ( ComplexCloudDensity141_g363 - 0.0 ) ) * CloudDetail179_g363 * CloudbreakLightDir426_g363 ));
				float SunThroughClouds399_g363 = saturate( lerpResult316_g363 );
				float3 hsvTorgb2_g366 = RGBToHSV( CZY_AltoCloudColor.rgb );
				float3 hsvTorgb3_g366 = HSVToRGB( float3(hsvTorgb2_g366.x,saturate( ( hsvTorgb2_g366.y + CZY_FilterSaturation ) ),( hsvTorgb2_g366.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g366 = ( float4( hsvTorgb3_g366 , 0.0 ) * CZY_FilterColor );
				float4 CirrusCustomLightColor350_g363 = ( CloudColor41_g363 * ( temp_output_10_0_g366 * CZY_CloudFilterColor ) );
				float temp_output_391_0_g363 = ( AltoCumulusPlacement376_g363 * (0.0 + (tex2D( CZY_AltocumulusTexture, ((Pos33_g363*1.0 + ( CZY_AltocumulusWindSpeed * TIme30_g363 ))*( 1.0 / CZY_AltocumulusScale ) + 0.0) ).r - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) * CZY_AltocumulusMultiplier );
				float AltoCumulusLightTransport393_g363 = temp_output_391_0_g363;
				float ACCustomLightsClipping387_g363 = ( AltoCumulusLightTransport393_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float mulTime193_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D224_g363 = snoise( (Pos33_g363*1.0 + mulTime193_g363)*2.0 );
				float mulTime178_g363 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos138_g363 = cos( ( mulTime178_g363 * 0.01 ) );
				float sin138_g363 = sin( ( mulTime178_g363 * 0.01 ) );
				float2 rotator138_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos138_g363 , -sin138_g363 , sin138_g363 , cos138_g363 )) + float2( 0.5,0.5 );
				float cos198_g363 = cos( ( mulTime178_g363 * -0.02 ) );
				float sin198_g363 = sin( ( mulTime178_g363 * -0.02 ) );
				float2 rotator198_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos198_g363 , -sin198_g363 , sin198_g363 , cos198_g363 )) + float2( 0.5,0.5 );
				float mulTime184_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D216_g363 = snoise( (Pos33_g363*10.0 + mulTime184_g363)*4.0 );
				float4 CirrostratPattern261_g363 = ( ( saturate( simplePerlin2D224_g363 ) * tex2D( CZY_CirrostratusTexture, (rotator138_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator198_g363*1.5 + 0.75) ) * saturate( simplePerlin2D216_g363 ) ) );
				float2 texCoord234_g363 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_243_0_g363 = ( texCoord234_g363 - float2( 0.5,0.5 ) );
				float dotResult238_g363 = dot( temp_output_243_0_g363 , temp_output_243_0_g363 );
				float clampResult264_g363 = clamp( ( CZY_CirrostratusMultiplier * 0.5 ) , 0.0 , 0.98 );
				float CirrostratLightTransport281_g363 = ( ( CirrostratPattern261_g363 * saturate( (0.4 + (dotResult238_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - clampResult264_g363 ) ? 1.0 : 0.0 );
				float CSCustomLightsClipping309_g363 = ( CirrostratLightTransport281_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float CustomRadiance340_g363 = saturate( ( ACCustomLightsClipping387_g363 + CSCustomLightsClipping309_g363 ) );
				float4 lerpResult331_g363 = lerp( ( lerpResult342_g363 + SunThroughClouds399_g363 ) , CirrusCustomLightColor350_g363 , CustomRadiance340_g363);
				float FinalAlpha375_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + AltoCumulusLightTransport393_g363 + ChemtrailsFinal248_g363 + CirrostratLightTransport281_g363 + CirrusAlpha250_g363 + NimbusLightTransport269_g363 ) );
				float4 appendResult420_g363 = (float4((lerpResult331_g363).rgb , FinalAlpha375_g363));
				float4 FinalCloudColor325_g363 = appendResult420_g363;
				bool enabled20_g367 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g367 =(bool)_FullySubmerged;
				float4 screenPos = input.ase_texcoord2;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g367 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g367 = HLSL20_g367( enabled20_g367 , submerged20_g367 , textureSample20_g367 );
				

				surfaceDescription.Alpha = ( ( (FinalCloudColor325_g363).w * ( 1.0 - localHLSL20_g367 ) ) > Clipping208_g363 ? 1.0 : 0.0 );
				surfaceDescription.AlphaClipThreshold = Clipping208_g363;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			

        	#pragma multi_compile _ALPHATEST_ON
        	#pragma multi_compile_instancing
        	#define _SURFACE_TYPE_TRANSPARENT 1
        	#define ASE_VERSION 19801
        	#define ASE_SRP_VERSION 140010


			

        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			
			#if ASE_SRP_VERSION >=140010
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#endif
		

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            #if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
						#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			float4 CZY_CloudColor;
			float CZY_FilterSaturation;
			float CZY_FilterValue;
			float4 CZY_FilterColor;
			float4 CZY_CloudFilterColor;
			float4 CZY_CloudHighlightColor;
			float4 CZY_SunFilterColor;
			float CZY_WindSpeed;
			float CZY_MainCloudScale;
			float CZY_CumulusCoverageMultiplier;
			float3 CZY_SunDirection;
			half CZY_SunFlareFalloff;
			float3 CZY_MoonDirection;
			half CZY_CloudMoonFalloff;
			float4 CZY_CloudMoonColor;
			float CZY_DetailScale;
			float CZY_DetailAmount;
			float CZY_BorderHeight;
			float CZY_BorderVariation;
			float CZY_BorderEffect;
			float3 CZY_StormDirection;
			float CZY_NimbusHeight;
			float CZY_NimbusMultiplier;
			float CZY_NimbusVariation;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_ClippingThreshold;
			float4 CZY_AltoCloudColor;
			sampler2D CZY_AltocumulusTexture;
			float2 CZY_AltocumulusWindSpeed;
			float CZY_AltocumulusScale;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
					float2 voronoihash81_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi81_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash81_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash88_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi88_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash88_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash200_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi200_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash200_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return (F2 + F1) * 0.5;
					}
			
					float2 voronoihash232_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi232_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash232_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash84_g363( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi84_g363( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash84_g363( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						return F1;
					}
			
			float HLSL20_g367( bool enabled, bool submerged, float textureSample )
			{
				if(enabled)
				{
					if(submerged) return 1.0;
					else return textureSample;
				}
				else
				{
					return 0.0;
				}
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = TransformObjectToWorldNormal( input.normalOS );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag(PackedVaryings input
						, out half4 outNormalWS : SV_Target0
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );
				float3 WorldPosition = input.positionWS;
				float3 WorldNormal = input.normalWS;
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float3 hsvTorgb2_g364 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g364 = HSVToRGB( float3(hsvTorgb2_g364.x,saturate( ( hsvTorgb2_g364.y + CZY_FilterSaturation ) ),( hsvTorgb2_g364.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g364 = ( float4( hsvTorgb3_g364 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor41_g363 = ( temp_output_10_0_g364 * CZY_CloudFilterColor );
				float3 hsvTorgb2_g368 = RGBToHSV( CZY_CloudHighlightColor.rgb );
				float3 hsvTorgb3_g368 = HSVToRGB( float3(hsvTorgb2_g368.x,saturate( ( hsvTorgb2_g368.y + CZY_FilterSaturation ) ),( hsvTorgb2_g368.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g368 = ( float4( hsvTorgb3_g368 , 0.0 ) * CZY_FilterColor );
				float4 CloudHighlightColor55_g363 = ( temp_output_10_0_g368 * CZY_SunFilterColor );
				float2 texCoord31_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos33_g363 = texCoord31_g363;
				float mulTime29_g363 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme30_g363 = mulTime29_g363;
				float simplePerlin2D409_g363 = snoise( ( Pos33_g363 + ( TIme30_g363 * float2( 0.2,-0.4 ) ) )*( 100.0 / CZY_MainCloudScale ) );
				simplePerlin2D409_g363 = simplePerlin2D409_g363*0.5 + 0.5;
				float SimpleCloudDensity153_g363 = simplePerlin2D409_g363;
				float time81_g363 = 0.0;
				float2 voronoiSmoothId81_g363 = 0;
				float2 temp_output_94_0_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) );
				float2 coords81_g363 = temp_output_94_0_g363 * ( 140.0 / CZY_MainCloudScale );
				float2 id81_g363 = 0;
				float2 uv81_g363 = 0;
				float voroi81_g363 = voronoi81_g363( coords81_g363, time81_g363, id81_g363, uv81_g363, 0, voronoiSmoothId81_g363 );
				float time88_g363 = 0.0;
				float2 voronoiSmoothId88_g363 = 0;
				float2 coords88_g363 = temp_output_94_0_g363 * ( 500.0 / CZY_MainCloudScale );
				float2 id88_g363 = 0;
				float2 uv88_g363 = 0;
				float voroi88_g363 = voronoi88_g363( coords88_g363, time88_g363, id88_g363, uv88_g363, 0, voronoiSmoothId88_g363 );
				float2 appendResult95_g363 = (float2(voroi81_g363 , voroi88_g363));
				float2 VoroDetails109_g363 = appendResult95_g363;
				float CumulusCoverage34_g363 = CZY_CumulusCoverageMultiplier;
				float ComplexCloudDensity141_g363 = (0.0 + (min( SimpleCloudDensity153_g363 , ( 1.0 - VoroDetails109_g363.x ) ) - ( 1.0 - CumulusCoverage34_g363 )) * (1.0 - 0.0) / (1.0 - ( 1.0 - CumulusCoverage34_g363 )));
				float4 lerpResult315_g363 = lerp( CloudHighlightColor55_g363 , CloudColor41_g363 , saturate( (2.0 + (ComplexCloudDensity141_g363 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
				float3 normalizeResult40_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult42_g363 = dot( normalizeResult40_g363 , CZY_SunDirection );
				float temp_output_49_0_g363 = abs( (dotResult42_g363*0.5 + 0.5) );
				half LightMask56_g363 = saturate( pow( temp_output_49_0_g363 , CZY_SunFlareFalloff ) );
				float time200_g363 = 0.0;
				float2 voronoiSmoothId200_g363 = 0;
				float mulTime163_g363 = _TimeParameters.x * 0.003;
				float2 coords200_g363 = (Pos33_g363*1.0 + ( float2( 1,-2 ) * mulTime163_g363 )) * 10.0;
				float2 id200_g363 = 0;
				float2 uv200_g363 = 0;
				float voroi200_g363 = voronoi200_g363( coords200_g363, time200_g363, id200_g363, uv200_g363, 0, voronoiSmoothId200_g363 );
				float time232_g363 = ( 10.0 * mulTime163_g363 );
				float2 voronoiSmoothId232_g363 = 0;
				float2 coords232_g363 = input.ase_texcoord3.xy * 10.0;
				float2 id232_g363 = 0;
				float2 uv232_g363 = 0;
				float voroi232_g363 = voronoi232_g363( coords232_g363, time232_g363, id232_g363, uv232_g363, 0, voronoiSmoothId232_g363 );
				float temp_output_242_0_g363 = ( ( ( 1.0 - 0.0 ) - (1.0 + (voroi200_g363 - 0.0) * (-0.5 - 1.0) / (1.0 - 0.0)) ) - voroi232_g363 );
				float AltoCumulusPlacement376_g363 = temp_output_242_0_g363;
				float CloudThicknessDetails286_g363 = ( VoroDetails109_g363.y * saturate( ( AltoCumulusPlacement376_g363 - 0.26 ) ) );
				float3 normalizeResult43_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult46_g363 = dot( normalizeResult43_g363 , CZY_MoonDirection );
				half MoonlightMask57_g363 = saturate( pow( abs( (dotResult46_g363*0.5 + 0.5) ) , CZY_CloudMoonFalloff ) );
				float3 hsvTorgb2_g365 = RGBToHSV( CZY_CloudMoonColor.rgb );
				float3 hsvTorgb3_g365 = HSVToRGB( float3(hsvTorgb2_g365.x,saturate( ( hsvTorgb2_g365.y + CZY_FilterSaturation ) ),( hsvTorgb2_g365.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g365 = ( float4( hsvTorgb3_g365 , 0.0 ) * CZY_FilterColor );
				float4 MoonlightColor60_g363 = ( temp_output_10_0_g365 * CZY_CloudFilterColor );
				float4 lerpResult338_g363 = lerp( ( lerpResult315_g363 + ( LightMask56_g363 * CloudHighlightColor55_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) + ( MoonlightMask57_g363 * MoonlightColor60_g363 * ( 1.0 - CloudThicknessDetails286_g363 ) ) ) , ( CloudColor41_g363 * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , CloudThicknessDetails286_g363);
				float time84_g363 = 0.0;
				float2 voronoiSmoothId84_g363 = 0;
				float2 coords84_g363 = ( Pos33_g363 + ( TIme30_g363 * float2( 0.3,0.2 ) ) ) * ( 100.0 / CZY_DetailScale );
				float2 id84_g363 = 0;
				float2 uv84_g363 = 0;
				float fade84_g363 = 0.5;
				float voroi84_g363 = 0;
				float rest84_g363 = 0;
				for( int it84_g363 = 0; it84_g363 <3; it84_g363++ ){
				voroi84_g363 += fade84_g363 * voronoi84_g363( coords84_g363, time84_g363, id84_g363, uv84_g363, 0,voronoiSmoothId84_g363 );
				rest84_g363 += fade84_g363;
				coords84_g363 *= 2;
				fade84_g363 *= 0.5;
				}//Voronoi84_g363
				voroi84_g363 /= rest84_g363;
				float temp_output_173_0_g363 = ( (0.0 + (( 1.0 - voroi84_g363 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * CZY_DetailAmount );
				float DetailedClouds252_g363 = saturate( ( ComplexCloudDensity141_g363 + temp_output_173_0_g363 ) );
				float CloudDetail179_g363 = temp_output_173_0_g363;
				float2 texCoord79_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_161_0_g363 = ( texCoord79_g363 - float2( 0.5,0.5 ) );
				float dotResult212_g363 = dot( temp_output_161_0_g363 , temp_output_161_0_g363 );
				float BorderHeight154_g363 = ( 1.0 - CZY_BorderHeight );
				float temp_output_151_0_g363 = ( -2.0 * ( 1.0 - CZY_BorderVariation ) );
				float clampResult247_g363 = clamp( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( BorderHeight154_g363 * temp_output_151_0_g363 ) + (dotResult212_g363 - 0.0) * (( temp_output_151_0_g363 * -4.0 ) - ( BorderHeight154_g363 * temp_output_151_0_g363 )) / (0.5 - 0.0)) ) ) * 10.0 * CZY_BorderEffect ) , -1.0 , 1.0 );
				float BorderLightTransport278_g363 = clampResult247_g363;
				float3 normalizeResult116_g363 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float3 normalizeResult146_g363 = normalize( CZY_StormDirection );
				float dotResult150_g363 = dot( normalizeResult116_g363 , normalizeResult146_g363 );
				float2 texCoord98_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_124_0_g363 = ( texCoord98_g363 - float2( 0.5,0.5 ) );
				float dotResult125_g363 = dot( temp_output_124_0_g363 , temp_output_124_0_g363 );
				float temp_output_140_0_g363 = ( -2.0 * ( 1.0 - ( CZY_NimbusVariation * 0.9 ) ) );
				float NimbusLightTransport269_g363 = saturate( ( ( ( CloudDetail179_g363 + SimpleCloudDensity153_g363 ) * saturate( (( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 ) + (( dotResult150_g363 + ( CZY_NimbusHeight * 4.0 * dotResult125_g363 ) ) - 0.5) * (( temp_output_140_0_g363 * -4.0 ) - ( ( 1.0 - CZY_NimbusMultiplier ) * temp_output_140_0_g363 )) / (7.0 - 0.5)) ) ) * 10.0 ) );
				float mulTime104_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D143_g363 = snoise( (Pos33_g363*1.0 + mulTime104_g363)*2.0 );
				float mulTime93_g363 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos97_g363 = cos( ( mulTime93_g363 * 0.01 ) );
				float sin97_g363 = sin( ( mulTime93_g363 * 0.01 ) );
				float2 rotator97_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos97_g363 , -sin97_g363 , sin97_g363 , cos97_g363 )) + float2( 0.5,0.5 );
				float cos131_g363 = cos( ( mulTime93_g363 * -0.02 ) );
				float sin131_g363 = sin( ( mulTime93_g363 * -0.02 ) );
				float2 rotator131_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos131_g363 , -sin131_g363 , sin131_g363 , cos131_g363 )) + float2( 0.5,0.5 );
				float mulTime107_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D147_g363 = snoise( (Pos33_g363*1.0 + mulTime107_g363)*4.0 );
				float4 ChemtrailsPattern210_g363 = ( ( saturate( simplePerlin2D143_g363 ) * tex2D( CZY_ChemtrailsTexture, (rotator97_g363*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator131_g363 ) * saturate( simplePerlin2D147_g363 ) ) );
				float2 texCoord139_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_162_0_g363 = ( texCoord139_g363 - float2( 0.5,0.5 ) );
				float dotResult207_g363 = dot( temp_output_162_0_g363 , temp_output_162_0_g363 );
				float ChemtrailsFinal248_g363 = ( ( ChemtrailsPattern210_g363 * saturate( (0.4 + (dotResult207_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - ( CZY_ChemtrailsMultiplier * 0.5 ) ) ? 1.0 : 0.0 );
				float mulTime80_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D126_g363 = snoise( (Pos33_g363*1.0 + mulTime80_g363)*2.0 );
				float mulTime75_g363 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos101_g363 = cos( ( mulTime75_g363 * 0.01 ) );
				float sin101_g363 = sin( ( mulTime75_g363 * 0.01 ) );
				float2 rotator101_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos101_g363 , -sin101_g363 , sin101_g363 , cos101_g363 )) + float2( 0.5,0.5 );
				float cos112_g363 = cos( ( mulTime75_g363 * -0.02 ) );
				float sin112_g363 = sin( ( mulTime75_g363 * -0.02 ) );
				float2 rotator112_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos112_g363 , -sin112_g363 , sin112_g363 , cos112_g363 )) + float2( 0.5,0.5 );
				float mulTime135_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D122_g363 = snoise( (Pos33_g363*1.0 + mulTime135_g363) );
				simplePerlin2D122_g363 = simplePerlin2D122_g363*0.5 + 0.5;
				float4 CirrusPattern137_g363 = ( ( saturate( simplePerlin2D126_g363 ) * tex2D( CZY_CirrusTexture, (rotator101_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator112_g363*1.0 + 0.0) ) * saturate( simplePerlin2D122_g363 ) ) );
				float2 texCoord134_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_164_0_g363 = ( texCoord134_g363 - float2( 0.5,0.5 ) );
				float dotResult157_g363 = dot( temp_output_164_0_g363 , temp_output_164_0_g363 );
				float4 temp_output_217_0_g363 = ( CirrusPattern137_g363 * saturate( (0.0 + (dotResult157_g363 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) );
				float Clipping208_g363 = CZY_ClippingThreshold;
				float CirrusAlpha250_g363 = ( ( temp_output_217_0_g363 * ( CZY_CirrusMultiplier * 10.0 ) ).r > Clipping208_g363 ? 1.0 : 0.0 );
				float SimpleRadiance268_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + NimbusLightTransport269_g363 + ChemtrailsFinal248_g363 + CirrusAlpha250_g363 ) );
				float4 lerpResult342_g363 = lerp( CloudColor41_g363 , lerpResult338_g363 , ( 1.0 - SimpleRadiance268_g363 ));
				float CloudbreakLightDir426_g363 = saturate( pow( temp_output_49_0_g363 , ( CZY_SunFlareFalloff * 0.5 ) ) );
				float lerpResult316_g363 = lerp( -0.4 , 1.0 , ( saturate( ( ComplexCloudDensity141_g363 - 0.0 ) ) * CloudDetail179_g363 * CloudbreakLightDir426_g363 ));
				float SunThroughClouds399_g363 = saturate( lerpResult316_g363 );
				float3 hsvTorgb2_g366 = RGBToHSV( CZY_AltoCloudColor.rgb );
				float3 hsvTorgb3_g366 = HSVToRGB( float3(hsvTorgb2_g366.x,saturate( ( hsvTorgb2_g366.y + CZY_FilterSaturation ) ),( hsvTorgb2_g366.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g366 = ( float4( hsvTorgb3_g366 , 0.0 ) * CZY_FilterColor );
				float4 CirrusCustomLightColor350_g363 = ( CloudColor41_g363 * ( temp_output_10_0_g366 * CZY_CloudFilterColor ) );
				float temp_output_391_0_g363 = ( AltoCumulusPlacement376_g363 * (0.0 + (tex2D( CZY_AltocumulusTexture, ((Pos33_g363*1.0 + ( CZY_AltocumulusWindSpeed * TIme30_g363 ))*( 1.0 / CZY_AltocumulusScale ) + 0.0) ).r - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) * CZY_AltocumulusMultiplier );
				float AltoCumulusLightTransport393_g363 = temp_output_391_0_g363;
				float ACCustomLightsClipping387_g363 = ( AltoCumulusLightTransport393_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float mulTime193_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D224_g363 = snoise( (Pos33_g363*1.0 + mulTime193_g363)*2.0 );
				float mulTime178_g363 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos138_g363 = cos( ( mulTime178_g363 * 0.01 ) );
				float sin138_g363 = sin( ( mulTime178_g363 * 0.01 ) );
				float2 rotator138_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos138_g363 , -sin138_g363 , sin138_g363 , cos138_g363 )) + float2( 0.5,0.5 );
				float cos198_g363 = cos( ( mulTime178_g363 * -0.02 ) );
				float sin198_g363 = sin( ( mulTime178_g363 * -0.02 ) );
				float2 rotator198_g363 = mul( Pos33_g363 - float2( 0.5,0.5 ) , float2x2( cos198_g363 , -sin198_g363 , sin198_g363 , cos198_g363 )) + float2( 0.5,0.5 );
				float mulTime184_g363 = _TimeParameters.x * 0.01;
				float simplePerlin2D216_g363 = snoise( (Pos33_g363*10.0 + mulTime184_g363)*4.0 );
				float4 CirrostratPattern261_g363 = ( ( saturate( simplePerlin2D224_g363 ) * tex2D( CZY_CirrostratusTexture, (rotator138_g363*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator198_g363*1.5 + 0.75) ) * saturate( simplePerlin2D216_g363 ) ) );
				float2 texCoord234_g363 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_243_0_g363 = ( texCoord234_g363 - float2( 0.5,0.5 ) );
				float dotResult238_g363 = dot( temp_output_243_0_g363 , temp_output_243_0_g363 );
				float clampResult264_g363 = clamp( ( CZY_CirrostratusMultiplier * 0.5 ) , 0.0 , 0.98 );
				float CirrostratLightTransport281_g363 = ( ( CirrostratPattern261_g363 * saturate( (0.4 + (dotResult238_g363 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - clampResult264_g363 ) ? 1.0 : 0.0 );
				float CSCustomLightsClipping309_g363 = ( CirrostratLightTransport281_g363 * ( SimpleRadiance268_g363 > Clipping208_g363 ? 0.0 : 1.0 ) );
				float CustomRadiance340_g363 = saturate( ( ACCustomLightsClipping387_g363 + CSCustomLightsClipping309_g363 ) );
				float4 lerpResult331_g363 = lerp( ( lerpResult342_g363 + SunThroughClouds399_g363 ) , CirrusCustomLightColor350_g363 , CustomRadiance340_g363);
				float FinalAlpha375_g363 = saturate( ( DetailedClouds252_g363 + BorderLightTransport278_g363 + AltoCumulusLightTransport393_g363 + ChemtrailsFinal248_g363 + CirrostratLightTransport281_g363 + CirrusAlpha250_g363 + NimbusLightTransport269_g363 ) );
				float4 appendResult420_g363 = (float4((lerpResult331_g363).rgb , FinalAlpha375_g363));
				float4 FinalCloudColor325_g363 = appendResult420_g363;
				bool enabled20_g367 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g367 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g367 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g367 = HLSL20_g367( enabled20_g367 , submerged20_g367 , textureSample20_g367 );
				

				float Alpha = ( ( (FinalCloudColor325_g363).w * ( 1.0 - localHLSL20_g367 ) ) > Clipping208_g363 ? 1.0 : 0.0 );
				float AlphaClipThreshold = Clipping208_g363;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(input.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = input.normalWS;
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}
			ENDHLSL
		}

	
	}
	
	CustomEditor "DistantLands.Cozy.EditorScripts.EmptyShaderGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback "Hidden/InternalErrorShader"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.FunctionNode;873;-4064,-688;Inherit;False;Stylized Clouds (Desktop);0;;363;b8040dba3255391449edffa0921d9c37;0;0;3;FLOAT4;0;FLOAT;414;FLOAT;415
Node;AmplifyShaderEditor.FunctionNode;874;-3744,-688;Inherit;False;AddFogToSkyLayer;-1;;369;36a78fe96c9f6fa4dab85c7793736468;0;3;89;COLOR;0,0,0,0;False;91;FLOAT;0;False;59;FLOAT;0;False;2;COLOR;84;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;876;-4080,-480;Inherit;False;Global;CZY_CloudsFogAmount;CZY_CloudsFogAmount;8;0;Create;True;0;0;0;False;0;False;0;0.509;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;875;-4080,-560;Inherit;False;Global;CZY_CloudsFogLightAmount;CZY_CloudsFogLightAmount;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;803;-678.2959,-671.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;802;-678.2959,-671.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;801;-678.2959,-671.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;804;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;805;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;806;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;807;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;808;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;799;-3024,-688;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;800;-3424,-688;Float;False;True;-1;2;DistantLands.Cozy.EditorScripts.EmptyShaderGUI;0;13;Distant Lands/Cozy/URP/Stylized Clouds (COZY Desktop);2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;9;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;1;False;;False;False;False;False;False;False;False;False;True;True;True;221;False;;255;False;;255;False;;7;False;;2;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=-50;UniversalMaterialType=Unlit;True;2;True;12;all;0;False;True;1;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;25;Surface;1;638295390293297003;  Blend;0;0;Two Sided;2;638295390676621873;Alpha Clipping;1;0;  Use Shadow Threshold;0;0;Forward Only;1;638295390392913430;Cast Shadows;1;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;True;True;False;False;True;True;True;False;False;;False;0
WireConnection;874;89;873;0
WireConnection;874;91;875;0
WireConnection;874;59;876;0
WireConnection;800;2;874;84
WireConnection;800;3;873;414
WireConnection;800;4;873;415
ASEEND*/
//CHKSM=85A81F5357F7356A4DC5770B4113EA3E6565EA5D