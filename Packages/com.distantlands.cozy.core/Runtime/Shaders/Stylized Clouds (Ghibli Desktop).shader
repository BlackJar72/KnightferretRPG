// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/URP/Stylized Clouds (Ghibli Desktop)"
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
			Tags { "LightMode"="UniversalForward" }

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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
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

			float4 CZY_AltoCloudColor;
			float CZY_FilterSaturation;
			float CZY_FilterValue;
			float4 CZY_FilterColor;
			float4 CZY_CloudFilterColor;
			float4 CZY_CloudHighlightColor;
			float4 CZY_CloudColor;
			float CZY_Spherize;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			float CZY_CloudCohesion;
			float CZY_CumulusCoverageMultiplier;
			float CZY_ShadowingDistance;
			float4 CZY_LightColor;
			float4 CZY_FogColor5;
			float CZY_LightFlareSquish;
			float3 CZY_SunDirection;
			half CZY_LightIntensity;
			half CZY_LightFalloff;
			float CZY_CloudsFogLightAmount;
			float4 CZY_SunFilterColor;
			float3 CZY_MoonDirection;
			float4 CZY_FogMoonFlareColor;
			float CZY_CloudsFogAmount;
			float CZY_FogSmoothness;
			float CZY_FogOffset;
			float CZY_FogIntensity;
			float CZY_CloudThickness;
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
			
					float2 voronoihash35_g71( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g71( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g71( n + g );
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
			
					float2 voronoihash13_g71( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g71( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g71( n + g );
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
			
					float2 voronoihash11_g71( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g71( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g71( n + g );
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
			
					float2 voronoihash35_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g68( n + g );
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
			
					float2 voronoihash13_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g68( n + g );
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
			
					float2 voronoihash11_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g68( n + g );
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
			
					float2 voronoihash35_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g67( n + g );
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
			
					float2 voronoihash13_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g67( n + g );
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
			
					float2 voronoihash11_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g67( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g70( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g70( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g70( n + g );
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
			
					float2 voronoihash13_g70( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g70( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g70( n + g );
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
			
					float2 voronoihash11_g70( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g70( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g70( n + g );
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
			
					float2 voronoihash35_g69( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g69( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g69( n + g );
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
			
					float2 voronoihash13_g69( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g69( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g69( n + g );
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
			
					float2 voronoihash11_g69( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g69( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g69( n + g );
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
			
			float HLSL20_g76( bool enabled, bool submerged, float textureSample )
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

				float3 hsvTorgb2_g72 = RGBToHSV( CZY_AltoCloudColor.rgb );
				float3 hsvTorgb3_g72 = HSVToRGB( float3(hsvTorgb2_g72.x,saturate( ( hsvTorgb2_g72.y + CZY_FilterSaturation ) ),( hsvTorgb2_g72.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g72 = ( float4( hsvTorgb3_g72 , 0.0 ) * CZY_FilterColor );
				float3 hsvTorgb2_g74 = RGBToHSV( CZY_CloudHighlightColor.rgb );
				float3 hsvTorgb3_g74 = HSVToRGB( float3(hsvTorgb2_g74.x,saturate( ( hsvTorgb2_g74.y + CZY_FilterSaturation ) ),( hsvTorgb2_g74.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g74 = ( float4( hsvTorgb3_g74 , 0.0 ) * CZY_FilterColor );
				float4 CloudHighlightColor142_g66 = ( temp_output_10_0_g74 * CZY_CloudFilterColor );
				float3 hsvTorgb2_g73 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g73 = HSVToRGB( float3(hsvTorgb2_g73.x,saturate( ( hsvTorgb2_g73.y + CZY_FilterSaturation ) ),( hsvTorgb2_g73.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g73 = ( float4( hsvTorgb3_g73 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor129_g66 = ( temp_output_10_0_g73 * CZY_CloudFilterColor );
				float4 color169_g66 = IsGammaSpace() ? float4(0.8396226,0.8396226,0.8396226,0) : float4(0.673178,0.673178,0.673178,0);
				Gradient gradient118_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.5411765 ), float4( 1, 1, 1, 0.6441138 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord10_g66 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_13_0_g66 = ( texCoord10_g66 - float2( 0.5,0.5 ) );
				float dotResult23_g66 = dot( temp_output_13_0_g66 , temp_output_13_0_g66 );
				float Dot49_g66 = saturate( (0.85 + (dotResult23_g66 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g71 = 0.0;
				float2 voronoiSmoothId35_g71 = 0;
				float2 texCoord12_g66 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV27_g66 = ( texCoord12_g66 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g71 = (CentralUV27_g66*1.58 + 0.0);
				float2 break2_g71 = abs( temp_output_21_0_g71 );
				float saferPower4_g71 = abs( break2_g71.x );
				float saferPower3_g71 = abs( break2_g71.y );
				float saferPower6_g71 = abs( ( pow( saferPower4_g71 , 2.0 ) + pow( saferPower3_g71 , 2.0 ) ) );
				float Spherize26_g66 = CZY_Spherize;
				float Flatness32_g66 = ( 20.0 * CZY_Spherize );
				float Scale164_g66 = ( CZY_MainCloudScale * 0.1 );
				float mulTime5_g66 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float Time6_g66 = mulTime5_g66;
				float2 Wind18_g66 = ( Time6_g66 * float2( 0.1,0.2 ) );
				float2 temp_output_10_0_g71 = (( ( temp_output_21_0_g71 * ( pow( saferPower6_g71 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g71 = temp_output_10_0_g71 * 60.0;
				float2 id35_g71 = 0;
				float2 uv35_g71 = 0;
				float fade35_g71 = 0.5;
				float voroi35_g71 = 0;
				float rest35_g71 = 0;
				for( int it35_g71 = 0; it35_g71 <2; it35_g71++ ){
				voroi35_g71 += fade35_g71 * voronoi35_g71( coords35_g71, time35_g71, id35_g71, uv35_g71, 0,voronoiSmoothId35_g71 );
				rest35_g71 += fade35_g71;
				coords35_g71 *= 2;
				fade35_g71 *= 0.5;
				}//Voronoi35_g71
				voroi35_g71 /= rest35_g71;
				float time13_g71 = 0.0;
				float2 voronoiSmoothId13_g71 = 0;
				float2 coords13_g71 = temp_output_10_0_g71 * 25.0;
				float2 id13_g71 = 0;
				float2 uv13_g71 = 0;
				float fade13_g71 = 0.5;
				float voroi13_g71 = 0;
				float rest13_g71 = 0;
				for( int it13_g71 = 0; it13_g71 <2; it13_g71++ ){
				voroi13_g71 += fade13_g71 * voronoi13_g71( coords13_g71, time13_g71, id13_g71, uv13_g71, 0,voronoiSmoothId13_g71 );
				rest13_g71 += fade13_g71;
				coords13_g71 *= 2;
				fade13_g71 *= 0.5;
				}//Voronoi13_g71
				voroi13_g71 /= rest13_g71;
				float time11_g71 = 17.23;
				float2 voronoiSmoothId11_g71 = 0;
				float2 coords11_g71 = temp_output_10_0_g71 * 9.0;
				float2 id11_g71 = 0;
				float2 uv11_g71 = 0;
				float fade11_g71 = 0.5;
				float voroi11_g71 = 0;
				float rest11_g71 = 0;
				for( int it11_g71 = 0; it11_g71 <2; it11_g71++ ){
				voroi11_g71 += fade11_g71 * voronoi11_g71( coords11_g71, time11_g71, id11_g71, uv11_g71, 0,voronoiSmoothId11_g71 );
				rest11_g71 += fade11_g71;
				coords11_g71 *= 2;
				fade11_g71 *= 0.5;
				}//Voronoi11_g71
				voroi11_g71 /= rest11_g71;
				float2 texCoord2_g66 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g66 = ( texCoord2_g66 - float2( 0.5,0.5 ) );
				float dotResult7_g66 = dot( temp_output_4_0_g66 , temp_output_4_0_g66 );
				float ModifiedCohesion17_g66 = ( CZY_CloudCohesion * 1.0 * ( 1.0 - dotResult7_g66 ) );
				float lerpResult15_g71 = lerp( saturate( ( voroi35_g71 + voroi13_g71 ) ) , voroi11_g71 , ModifiedCohesion17_g66);
				float CumulusCoverage29_g66 = CZY_CumulusCoverageMultiplier;
				float lerpResult16_g71 = lerp( lerpResult15_g71 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float time35_g68 = 0.0;
				float2 voronoiSmoothId35_g68 = 0;
				float2 temp_output_21_0_g68 = CentralUV27_g66;
				float2 break2_g68 = abs( temp_output_21_0_g68 );
				float saferPower4_g68 = abs( break2_g68.x );
				float saferPower3_g68 = abs( break2_g68.y );
				float saferPower6_g68 = abs( ( pow( saferPower4_g68 , 2.0 ) + pow( saferPower3_g68 , 2.0 ) ) );
				float2 temp_output_10_0_g68 = (( ( temp_output_21_0_g68 * ( pow( saferPower6_g68 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g68 = temp_output_10_0_g68 * 60.0;
				float2 id35_g68 = 0;
				float2 uv35_g68 = 0;
				float fade35_g68 = 0.5;
				float voroi35_g68 = 0;
				float rest35_g68 = 0;
				for( int it35_g68 = 0; it35_g68 <2; it35_g68++ ){
				voroi35_g68 += fade35_g68 * voronoi35_g68( coords35_g68, time35_g68, id35_g68, uv35_g68, 0,voronoiSmoothId35_g68 );
				rest35_g68 += fade35_g68;
				coords35_g68 *= 2;
				fade35_g68 *= 0.5;
				}//Voronoi35_g68
				voroi35_g68 /= rest35_g68;
				float time13_g68 = 0.0;
				float2 voronoiSmoothId13_g68 = 0;
				float2 coords13_g68 = temp_output_10_0_g68 * 25.0;
				float2 id13_g68 = 0;
				float2 uv13_g68 = 0;
				float fade13_g68 = 0.5;
				float voroi13_g68 = 0;
				float rest13_g68 = 0;
				for( int it13_g68 = 0; it13_g68 <2; it13_g68++ ){
				voroi13_g68 += fade13_g68 * voronoi13_g68( coords13_g68, time13_g68, id13_g68, uv13_g68, 0,voronoiSmoothId13_g68 );
				rest13_g68 += fade13_g68;
				coords13_g68 *= 2;
				fade13_g68 *= 0.5;
				}//Voronoi13_g68
				voroi13_g68 /= rest13_g68;
				float time11_g68 = 17.23;
				float2 voronoiSmoothId11_g68 = 0;
				float2 coords11_g68 = temp_output_10_0_g68 * 9.0;
				float2 id11_g68 = 0;
				float2 uv11_g68 = 0;
				float fade11_g68 = 0.5;
				float voroi11_g68 = 0;
				float rest11_g68 = 0;
				for( int it11_g68 = 0; it11_g68 <2; it11_g68++ ){
				voroi11_g68 += fade11_g68 * voronoi11_g68( coords11_g68, time11_g68, id11_g68, uv11_g68, 0,voronoiSmoothId11_g68 );
				rest11_g68 += fade11_g68;
				coords11_g68 *= 2;
				fade11_g68 *= 0.5;
				}//Voronoi11_g68
				voroi11_g68 /= rest11_g68;
				float lerpResult15_g68 = lerp( saturate( ( voroi35_g68 + voroi13_g68 ) ) , voroi11_g68 , ModifiedCohesion17_g66);
				float lerpResult16_g68 = lerp( lerpResult15_g68 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_60_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g68 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1PreAlpha119_g66 = temp_output_60_0_g66;
				float time35_g67 = 0.0;
				float2 voronoiSmoothId35_g67 = 0;
				float2 temp_output_21_0_g67 = CentralUV27_g66;
				float2 break2_g67 = abs( temp_output_21_0_g67 );
				float saferPower4_g67 = abs( break2_g67.x );
				float saferPower3_g67 = abs( break2_g67.y );
				float saferPower6_g67 = abs( ( pow( saferPower4_g67 , 2.0 ) + pow( saferPower3_g67 , 2.0 ) ) );
				float2 temp_output_10_0_g67 = (( ( temp_output_21_0_g67 * ( pow( saferPower6_g67 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale164_g66 * 1.5 ) ) + ( Wind18_g66 * float2( 0.5,0.5 ) ));
				float2 coords35_g67 = temp_output_10_0_g67 * 60.0;
				float2 id35_g67 = 0;
				float2 uv35_g67 = 0;
				float fade35_g67 = 0.5;
				float voroi35_g67 = 0;
				float rest35_g67 = 0;
				for( int it35_g67 = 0; it35_g67 <2; it35_g67++ ){
				voroi35_g67 += fade35_g67 * voronoi35_g67( coords35_g67, time35_g67, id35_g67, uv35_g67, 0,voronoiSmoothId35_g67 );
				rest35_g67 += fade35_g67;
				coords35_g67 *= 2;
				fade35_g67 *= 0.5;
				}//Voronoi35_g67
				voroi35_g67 /= rest35_g67;
				float time13_g67 = 0.0;
				float2 voronoiSmoothId13_g67 = 0;
				float2 coords13_g67 = temp_output_10_0_g67 * 25.0;
				float2 id13_g67 = 0;
				float2 uv13_g67 = 0;
				float fade13_g67 = 0.5;
				float voroi13_g67 = 0;
				float rest13_g67 = 0;
				for( int it13_g67 = 0; it13_g67 <2; it13_g67++ ){
				voroi13_g67 += fade13_g67 * voronoi13_g67( coords13_g67, time13_g67, id13_g67, uv13_g67, 0,voronoiSmoothId13_g67 );
				rest13_g67 += fade13_g67;
				coords13_g67 *= 2;
				fade13_g67 *= 0.5;
				}//Voronoi13_g67
				voroi13_g67 /= rest13_g67;
				float time11_g67 = 17.23;
				float2 voronoiSmoothId11_g67 = 0;
				float2 coords11_g67 = temp_output_10_0_g67 * 9.0;
				float2 id11_g67 = 0;
				float2 uv11_g67 = 0;
				float fade11_g67 = 0.5;
				float voroi11_g67 = 0;
				float rest11_g67 = 0;
				for( int it11_g67 = 0; it11_g67 <2; it11_g67++ ){
				voroi11_g67 += fade11_g67 * voronoi11_g67( coords11_g67, time11_g67, id11_g67, uv11_g67, 0,voronoiSmoothId11_g67 );
				rest11_g67 += fade11_g67;
				coords11_g67 *= 2;
				fade11_g67 *= 0.5;
				}//Voronoi11_g67
				voroi11_g67 /= rest11_g67;
				float lerpResult15_g67 = lerp( saturate( ( voroi35_g67 + voroi13_g67 ) ) , voroi11_g67 , ( ModifiedCohesion17_g66 * 1.1 ));
				float lerpResult16_g67 = lerp( lerpResult15_g67 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_61_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g67 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2PreAlpha114_g66 = temp_output_61_0_g66;
				float temp_output_151_0_g66 = (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g71 ) ) - 0.6) * (max( IT1PreAlpha119_g66 , IT2PreAlpha114_g66 ) - 0.0) / (1.5 - 0.6));
				float clampResult73_g66 = clamp( temp_output_151_0_g66 , 0.0 , 0.9 );
				float AdditionalLayer138_g66 = SampleGradient( gradient118_g66, clampResult73_g66 ).r;
				float4 lerpResult75_g66 = lerp( CloudColor129_g66 , ( CloudColor129_g66 * color169_g66 ) , AdditionalLayer138_g66);
				float4 ModifiedCloudColor100_g66 = lerpResult75_g66;
				Gradient gradient136_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4411841 ), float4( 1, 1, 1, 0.5794156 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g70 = 0.0;
				float2 voronoiSmoothId35_g70 = 0;
				float2 ShadowUV115_g66 = ( CentralUV27_g66 + ( CentralUV27_g66 * float2( -1,-1 ) * CZY_ShadowingDistance * Dot49_g66 ) );
				float2 temp_output_21_0_g70 = ShadowUV115_g66;
				float2 break2_g70 = abs( temp_output_21_0_g70 );
				float saferPower4_g70 = abs( break2_g70.x );
				float saferPower3_g70 = abs( break2_g70.y );
				float saferPower6_g70 = abs( ( pow( saferPower4_g70 , 2.0 ) + pow( saferPower3_g70 , 2.0 ) ) );
				float2 temp_output_10_0_g70 = (( ( temp_output_21_0_g70 * ( pow( saferPower6_g70 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g70 = temp_output_10_0_g70 * 60.0;
				float2 id35_g70 = 0;
				float2 uv35_g70 = 0;
				float fade35_g70 = 0.5;
				float voroi35_g70 = 0;
				float rest35_g70 = 0;
				for( int it35_g70 = 0; it35_g70 <2; it35_g70++ ){
				voroi35_g70 += fade35_g70 * voronoi35_g70( coords35_g70, time35_g70, id35_g70, uv35_g70, 0,voronoiSmoothId35_g70 );
				rest35_g70 += fade35_g70;
				coords35_g70 *= 2;
				fade35_g70 *= 0.5;
				}//Voronoi35_g70
				voroi35_g70 /= rest35_g70;
				float time13_g70 = 0.0;
				float2 voronoiSmoothId13_g70 = 0;
				float2 coords13_g70 = temp_output_10_0_g70 * 25.0;
				float2 id13_g70 = 0;
				float2 uv13_g70 = 0;
				float fade13_g70 = 0.5;
				float voroi13_g70 = 0;
				float rest13_g70 = 0;
				for( int it13_g70 = 0; it13_g70 <2; it13_g70++ ){
				voroi13_g70 += fade13_g70 * voronoi13_g70( coords13_g70, time13_g70, id13_g70, uv13_g70, 0,voronoiSmoothId13_g70 );
				rest13_g70 += fade13_g70;
				coords13_g70 *= 2;
				fade13_g70 *= 0.5;
				}//Voronoi13_g70
				voroi13_g70 /= rest13_g70;
				float time11_g70 = 17.23;
				float2 voronoiSmoothId11_g70 = 0;
				float2 coords11_g70 = temp_output_10_0_g70 * 9.0;
				float2 id11_g70 = 0;
				float2 uv11_g70 = 0;
				float fade11_g70 = 0.5;
				float voroi11_g70 = 0;
				float rest11_g70 = 0;
				for( int it11_g70 = 0; it11_g70 <2; it11_g70++ ){
				voroi11_g70 += fade11_g70 * voronoi11_g70( coords11_g70, time11_g70, id11_g70, uv11_g70, 0,voronoiSmoothId11_g70 );
				rest11_g70 += fade11_g70;
				coords11_g70 *= 2;
				fade11_g70 *= 0.5;
				}//Voronoi11_g70
				voroi11_g70 /= rest11_g70;
				float lerpResult15_g70 = lerp( saturate( ( voroi35_g70 + voroi13_g70 ) ) , voroi11_g70 , ModifiedCohesion17_g66);
				float lerpResult16_g70 = lerp( lerpResult15_g70 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float4 lerpResult83_g66 = lerp( CloudHighlightColor142_g66 , ModifiedCloudColor100_g66 , saturate( SampleGradient( gradient136_g66, saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g70 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) ) ).r ));
				float4 IT1Color80_g66 = lerpResult83_g66;
				Gradient gradient109_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4411841 ), float4( 1, 1, 1, 0.5794156 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g69 = 0.0;
				float2 voronoiSmoothId35_g69 = 0;
				float2 temp_output_21_0_g69 = ShadowUV115_g66;
				float2 break2_g69 = abs( temp_output_21_0_g69 );
				float saferPower4_g69 = abs( break2_g69.x );
				float saferPower3_g69 = abs( break2_g69.y );
				float saferPower6_g69 = abs( ( pow( saferPower4_g69 , 2.0 ) + pow( saferPower3_g69 , 2.0 ) ) );
				float2 temp_output_10_0_g69 = (( ( temp_output_21_0_g69 * ( pow( saferPower6_g69 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale164_g66 * 1.5 ) ) + ( Wind18_g66 * float2( 0.5,0.5 ) ));
				float2 coords35_g69 = temp_output_10_0_g69 * 60.0;
				float2 id35_g69 = 0;
				float2 uv35_g69 = 0;
				float fade35_g69 = 0.5;
				float voroi35_g69 = 0;
				float rest35_g69 = 0;
				for( int it35_g69 = 0; it35_g69 <2; it35_g69++ ){
				voroi35_g69 += fade35_g69 * voronoi35_g69( coords35_g69, time35_g69, id35_g69, uv35_g69, 0,voronoiSmoothId35_g69 );
				rest35_g69 += fade35_g69;
				coords35_g69 *= 2;
				fade35_g69 *= 0.5;
				}//Voronoi35_g69
				voroi35_g69 /= rest35_g69;
				float time13_g69 = 0.0;
				float2 voronoiSmoothId13_g69 = 0;
				float2 coords13_g69 = temp_output_10_0_g69 * 25.0;
				float2 id13_g69 = 0;
				float2 uv13_g69 = 0;
				float fade13_g69 = 0.5;
				float voroi13_g69 = 0;
				float rest13_g69 = 0;
				for( int it13_g69 = 0; it13_g69 <2; it13_g69++ ){
				voroi13_g69 += fade13_g69 * voronoi13_g69( coords13_g69, time13_g69, id13_g69, uv13_g69, 0,voronoiSmoothId13_g69 );
				rest13_g69 += fade13_g69;
				coords13_g69 *= 2;
				fade13_g69 *= 0.5;
				}//Voronoi13_g69
				voroi13_g69 /= rest13_g69;
				float time11_g69 = 17.23;
				float2 voronoiSmoothId11_g69 = 0;
				float2 coords11_g69 = temp_output_10_0_g69 * 9.0;
				float2 id11_g69 = 0;
				float2 uv11_g69 = 0;
				float fade11_g69 = 0.5;
				float voroi11_g69 = 0;
				float rest11_g69 = 0;
				for( int it11_g69 = 0; it11_g69 <2; it11_g69++ ){
				voroi11_g69 += fade11_g69 * voronoi11_g69( coords11_g69, time11_g69, id11_g69, uv11_g69, 0,voronoiSmoothId11_g69 );
				rest11_g69 += fade11_g69;
				coords11_g69 *= 2;
				fade11_g69 *= 0.5;
				}//Voronoi11_g69
				voroi11_g69 /= rest11_g69;
				float lerpResult15_g69 = lerp( saturate( ( voroi35_g69 + voroi13_g69 ) ) , voroi11_g69 , ( ModifiedCohesion17_g66 * 1.1 ));
				float lerpResult16_g69 = lerp( lerpResult15_g69 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float4 lerpResult88_g66 = lerp( CloudHighlightColor142_g66 , ModifiedCloudColor100_g66 , saturate( SampleGradient( gradient109_g66, saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g69 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) ) ).r ));
				float4 IT2Color106_g66 = lerpResult88_g66;
				Gradient gradient62_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float IT2Alpha65_g66 = SampleGradient( gradient62_g66, temp_output_61_0_g66 ).r;
				float4 lerpResult79_g66 = lerp( ( ( temp_output_10_0_g72 * CZY_CloudFilterColor ) * IT1Color80_g66 ) , IT2Color106_g66 , IT2Alpha65_g66);
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
				float4 lerpResult90_g369 = lerp( lerpResult79_g66 , ( ( temp_output_10_0_g371 * CZY_SunFilterColor ) + temp_output_10_0_g370 ) , temp_output_34_0_g369);
				
				Gradient gradient59_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float IT1Alpha66_g66 = SampleGradient( gradient59_g66, temp_output_60_0_g66 ).r;
				float temp_output_173_0_g66 = max( IT1Alpha66_g66 , IT2Alpha65_g66 );
				bool enabled20_g76 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g76 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g76 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g76 = HLSL20_g76( enabled20_g76 , submerged20_g76 , textureSample20_g76 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = lerpResult90_g369.rgb;
				float Alpha = ( saturate( ( temp_output_173_0_g66 + ( temp_output_173_0_g66 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g76 ) );
				float AlphaClipThreshold = 0.5;
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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
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

			float CZY_Spherize;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			float CZY_CloudCohesion;
			float CZY_CumulusCoverageMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			
					float2 voronoihash35_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g68( n + g );
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
			
					float2 voronoihash13_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g68( n + g );
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
			
					float2 voronoihash11_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g68( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g67( n + g );
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
			
					float2 voronoihash13_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g67( n + g );
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
			
					float2 voronoihash11_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g67( n + g );
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
			
			float HLSL20_g76( bool enabled, bool submerged, float textureSample )
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

				Gradient gradient59_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord10_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_13_0_g66 = ( texCoord10_g66 - float2( 0.5,0.5 ) );
				float dotResult23_g66 = dot( temp_output_13_0_g66 , temp_output_13_0_g66 );
				float Dot49_g66 = saturate( (0.85 + (dotResult23_g66 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g68 = 0.0;
				float2 voronoiSmoothId35_g68 = 0;
				float2 texCoord12_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV27_g66 = ( texCoord12_g66 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g68 = CentralUV27_g66;
				float2 break2_g68 = abs( temp_output_21_0_g68 );
				float saferPower4_g68 = abs( break2_g68.x );
				float saferPower3_g68 = abs( break2_g68.y );
				float saferPower6_g68 = abs( ( pow( saferPower4_g68 , 2.0 ) + pow( saferPower3_g68 , 2.0 ) ) );
				float Spherize26_g66 = CZY_Spherize;
				float Flatness32_g66 = ( 20.0 * CZY_Spherize );
				float Scale164_g66 = ( CZY_MainCloudScale * 0.1 );
				float mulTime5_g66 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float Time6_g66 = mulTime5_g66;
				float2 Wind18_g66 = ( Time6_g66 * float2( 0.1,0.2 ) );
				float2 temp_output_10_0_g68 = (( ( temp_output_21_0_g68 * ( pow( saferPower6_g68 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g68 = temp_output_10_0_g68 * 60.0;
				float2 id35_g68 = 0;
				float2 uv35_g68 = 0;
				float fade35_g68 = 0.5;
				float voroi35_g68 = 0;
				float rest35_g68 = 0;
				for( int it35_g68 = 0; it35_g68 <2; it35_g68++ ){
				voroi35_g68 += fade35_g68 * voronoi35_g68( coords35_g68, time35_g68, id35_g68, uv35_g68, 0,voronoiSmoothId35_g68 );
				rest35_g68 += fade35_g68;
				coords35_g68 *= 2;
				fade35_g68 *= 0.5;
				}//Voronoi35_g68
				voroi35_g68 /= rest35_g68;
				float time13_g68 = 0.0;
				float2 voronoiSmoothId13_g68 = 0;
				float2 coords13_g68 = temp_output_10_0_g68 * 25.0;
				float2 id13_g68 = 0;
				float2 uv13_g68 = 0;
				float fade13_g68 = 0.5;
				float voroi13_g68 = 0;
				float rest13_g68 = 0;
				for( int it13_g68 = 0; it13_g68 <2; it13_g68++ ){
				voroi13_g68 += fade13_g68 * voronoi13_g68( coords13_g68, time13_g68, id13_g68, uv13_g68, 0,voronoiSmoothId13_g68 );
				rest13_g68 += fade13_g68;
				coords13_g68 *= 2;
				fade13_g68 *= 0.5;
				}//Voronoi13_g68
				voroi13_g68 /= rest13_g68;
				float time11_g68 = 17.23;
				float2 voronoiSmoothId11_g68 = 0;
				float2 coords11_g68 = temp_output_10_0_g68 * 9.0;
				float2 id11_g68 = 0;
				float2 uv11_g68 = 0;
				float fade11_g68 = 0.5;
				float voroi11_g68 = 0;
				float rest11_g68 = 0;
				for( int it11_g68 = 0; it11_g68 <2; it11_g68++ ){
				voroi11_g68 += fade11_g68 * voronoi11_g68( coords11_g68, time11_g68, id11_g68, uv11_g68, 0,voronoiSmoothId11_g68 );
				rest11_g68 += fade11_g68;
				coords11_g68 *= 2;
				fade11_g68 *= 0.5;
				}//Voronoi11_g68
				voroi11_g68 /= rest11_g68;
				float2 texCoord2_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g66 = ( texCoord2_g66 - float2( 0.5,0.5 ) );
				float dotResult7_g66 = dot( temp_output_4_0_g66 , temp_output_4_0_g66 );
				float ModifiedCohesion17_g66 = ( CZY_CloudCohesion * 1.0 * ( 1.0 - dotResult7_g66 ) );
				float lerpResult15_g68 = lerp( saturate( ( voroi35_g68 + voroi13_g68 ) ) , voroi11_g68 , ModifiedCohesion17_g66);
				float CumulusCoverage29_g66 = CZY_CumulusCoverageMultiplier;
				float lerpResult16_g68 = lerp( lerpResult15_g68 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_60_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g68 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1Alpha66_g66 = SampleGradient( gradient59_g66, temp_output_60_0_g66 ).r;
				Gradient gradient62_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g67 = 0.0;
				float2 voronoiSmoothId35_g67 = 0;
				float2 temp_output_21_0_g67 = CentralUV27_g66;
				float2 break2_g67 = abs( temp_output_21_0_g67 );
				float saferPower4_g67 = abs( break2_g67.x );
				float saferPower3_g67 = abs( break2_g67.y );
				float saferPower6_g67 = abs( ( pow( saferPower4_g67 , 2.0 ) + pow( saferPower3_g67 , 2.0 ) ) );
				float2 temp_output_10_0_g67 = (( ( temp_output_21_0_g67 * ( pow( saferPower6_g67 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale164_g66 * 1.5 ) ) + ( Wind18_g66 * float2( 0.5,0.5 ) ));
				float2 coords35_g67 = temp_output_10_0_g67 * 60.0;
				float2 id35_g67 = 0;
				float2 uv35_g67 = 0;
				float fade35_g67 = 0.5;
				float voroi35_g67 = 0;
				float rest35_g67 = 0;
				for( int it35_g67 = 0; it35_g67 <2; it35_g67++ ){
				voroi35_g67 += fade35_g67 * voronoi35_g67( coords35_g67, time35_g67, id35_g67, uv35_g67, 0,voronoiSmoothId35_g67 );
				rest35_g67 += fade35_g67;
				coords35_g67 *= 2;
				fade35_g67 *= 0.5;
				}//Voronoi35_g67
				voroi35_g67 /= rest35_g67;
				float time13_g67 = 0.0;
				float2 voronoiSmoothId13_g67 = 0;
				float2 coords13_g67 = temp_output_10_0_g67 * 25.0;
				float2 id13_g67 = 0;
				float2 uv13_g67 = 0;
				float fade13_g67 = 0.5;
				float voroi13_g67 = 0;
				float rest13_g67 = 0;
				for( int it13_g67 = 0; it13_g67 <2; it13_g67++ ){
				voroi13_g67 += fade13_g67 * voronoi13_g67( coords13_g67, time13_g67, id13_g67, uv13_g67, 0,voronoiSmoothId13_g67 );
				rest13_g67 += fade13_g67;
				coords13_g67 *= 2;
				fade13_g67 *= 0.5;
				}//Voronoi13_g67
				voroi13_g67 /= rest13_g67;
				float time11_g67 = 17.23;
				float2 voronoiSmoothId11_g67 = 0;
				float2 coords11_g67 = temp_output_10_0_g67 * 9.0;
				float2 id11_g67 = 0;
				float2 uv11_g67 = 0;
				float fade11_g67 = 0.5;
				float voroi11_g67 = 0;
				float rest11_g67 = 0;
				for( int it11_g67 = 0; it11_g67 <2; it11_g67++ ){
				voroi11_g67 += fade11_g67 * voronoi11_g67( coords11_g67, time11_g67, id11_g67, uv11_g67, 0,voronoiSmoothId11_g67 );
				rest11_g67 += fade11_g67;
				coords11_g67 *= 2;
				fade11_g67 *= 0.5;
				}//Voronoi11_g67
				voroi11_g67 /= rest11_g67;
				float lerpResult15_g67 = lerp( saturate( ( voroi35_g67 + voroi13_g67 ) ) , voroi11_g67 , ( ModifiedCohesion17_g66 * 1.1 ));
				float lerpResult16_g67 = lerp( lerpResult15_g67 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_61_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g67 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2Alpha65_g66 = SampleGradient( gradient62_g66, temp_output_61_0_g66 ).r;
				float temp_output_173_0_g66 = max( IT1Alpha66_g66 , IT2Alpha65_g66 );
				bool enabled20_g76 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g76 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g76 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g76 = HLSL20_g76( enabled20_g76 , submerged20_g76 , textureSample20_g76 );
				

				float Alpha = ( saturate( ( temp_output_173_0_g66 + ( temp_output_173_0_g66 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g76 ) );
				float AlphaClipThreshold = 0.5;
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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
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

			float CZY_Spherize;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			float CZY_CloudCohesion;
			float CZY_CumulusCoverageMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			
					float2 voronoihash35_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g68( n + g );
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
			
					float2 voronoihash13_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g68( n + g );
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
			
					float2 voronoihash11_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g68( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g67( n + g );
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
			
					float2 voronoihash13_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g67( n + g );
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
			
					float2 voronoihash11_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g67( n + g );
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
			
			float HLSL20_g76( bool enabled, bool submerged, float textureSample )
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

				Gradient gradient59_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord10_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_13_0_g66 = ( texCoord10_g66 - float2( 0.5,0.5 ) );
				float dotResult23_g66 = dot( temp_output_13_0_g66 , temp_output_13_0_g66 );
				float Dot49_g66 = saturate( (0.85 + (dotResult23_g66 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g68 = 0.0;
				float2 voronoiSmoothId35_g68 = 0;
				float2 texCoord12_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV27_g66 = ( texCoord12_g66 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g68 = CentralUV27_g66;
				float2 break2_g68 = abs( temp_output_21_0_g68 );
				float saferPower4_g68 = abs( break2_g68.x );
				float saferPower3_g68 = abs( break2_g68.y );
				float saferPower6_g68 = abs( ( pow( saferPower4_g68 , 2.0 ) + pow( saferPower3_g68 , 2.0 ) ) );
				float Spherize26_g66 = CZY_Spherize;
				float Flatness32_g66 = ( 20.0 * CZY_Spherize );
				float Scale164_g66 = ( CZY_MainCloudScale * 0.1 );
				float mulTime5_g66 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float Time6_g66 = mulTime5_g66;
				float2 Wind18_g66 = ( Time6_g66 * float2( 0.1,0.2 ) );
				float2 temp_output_10_0_g68 = (( ( temp_output_21_0_g68 * ( pow( saferPower6_g68 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g68 = temp_output_10_0_g68 * 60.0;
				float2 id35_g68 = 0;
				float2 uv35_g68 = 0;
				float fade35_g68 = 0.5;
				float voroi35_g68 = 0;
				float rest35_g68 = 0;
				for( int it35_g68 = 0; it35_g68 <2; it35_g68++ ){
				voroi35_g68 += fade35_g68 * voronoi35_g68( coords35_g68, time35_g68, id35_g68, uv35_g68, 0,voronoiSmoothId35_g68 );
				rest35_g68 += fade35_g68;
				coords35_g68 *= 2;
				fade35_g68 *= 0.5;
				}//Voronoi35_g68
				voroi35_g68 /= rest35_g68;
				float time13_g68 = 0.0;
				float2 voronoiSmoothId13_g68 = 0;
				float2 coords13_g68 = temp_output_10_0_g68 * 25.0;
				float2 id13_g68 = 0;
				float2 uv13_g68 = 0;
				float fade13_g68 = 0.5;
				float voroi13_g68 = 0;
				float rest13_g68 = 0;
				for( int it13_g68 = 0; it13_g68 <2; it13_g68++ ){
				voroi13_g68 += fade13_g68 * voronoi13_g68( coords13_g68, time13_g68, id13_g68, uv13_g68, 0,voronoiSmoothId13_g68 );
				rest13_g68 += fade13_g68;
				coords13_g68 *= 2;
				fade13_g68 *= 0.5;
				}//Voronoi13_g68
				voroi13_g68 /= rest13_g68;
				float time11_g68 = 17.23;
				float2 voronoiSmoothId11_g68 = 0;
				float2 coords11_g68 = temp_output_10_0_g68 * 9.0;
				float2 id11_g68 = 0;
				float2 uv11_g68 = 0;
				float fade11_g68 = 0.5;
				float voroi11_g68 = 0;
				float rest11_g68 = 0;
				for( int it11_g68 = 0; it11_g68 <2; it11_g68++ ){
				voroi11_g68 += fade11_g68 * voronoi11_g68( coords11_g68, time11_g68, id11_g68, uv11_g68, 0,voronoiSmoothId11_g68 );
				rest11_g68 += fade11_g68;
				coords11_g68 *= 2;
				fade11_g68 *= 0.5;
				}//Voronoi11_g68
				voroi11_g68 /= rest11_g68;
				float2 texCoord2_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g66 = ( texCoord2_g66 - float2( 0.5,0.5 ) );
				float dotResult7_g66 = dot( temp_output_4_0_g66 , temp_output_4_0_g66 );
				float ModifiedCohesion17_g66 = ( CZY_CloudCohesion * 1.0 * ( 1.0 - dotResult7_g66 ) );
				float lerpResult15_g68 = lerp( saturate( ( voroi35_g68 + voroi13_g68 ) ) , voroi11_g68 , ModifiedCohesion17_g66);
				float CumulusCoverage29_g66 = CZY_CumulusCoverageMultiplier;
				float lerpResult16_g68 = lerp( lerpResult15_g68 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_60_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g68 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1Alpha66_g66 = SampleGradient( gradient59_g66, temp_output_60_0_g66 ).r;
				Gradient gradient62_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g67 = 0.0;
				float2 voronoiSmoothId35_g67 = 0;
				float2 temp_output_21_0_g67 = CentralUV27_g66;
				float2 break2_g67 = abs( temp_output_21_0_g67 );
				float saferPower4_g67 = abs( break2_g67.x );
				float saferPower3_g67 = abs( break2_g67.y );
				float saferPower6_g67 = abs( ( pow( saferPower4_g67 , 2.0 ) + pow( saferPower3_g67 , 2.0 ) ) );
				float2 temp_output_10_0_g67 = (( ( temp_output_21_0_g67 * ( pow( saferPower6_g67 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale164_g66 * 1.5 ) ) + ( Wind18_g66 * float2( 0.5,0.5 ) ));
				float2 coords35_g67 = temp_output_10_0_g67 * 60.0;
				float2 id35_g67 = 0;
				float2 uv35_g67 = 0;
				float fade35_g67 = 0.5;
				float voroi35_g67 = 0;
				float rest35_g67 = 0;
				for( int it35_g67 = 0; it35_g67 <2; it35_g67++ ){
				voroi35_g67 += fade35_g67 * voronoi35_g67( coords35_g67, time35_g67, id35_g67, uv35_g67, 0,voronoiSmoothId35_g67 );
				rest35_g67 += fade35_g67;
				coords35_g67 *= 2;
				fade35_g67 *= 0.5;
				}//Voronoi35_g67
				voroi35_g67 /= rest35_g67;
				float time13_g67 = 0.0;
				float2 voronoiSmoothId13_g67 = 0;
				float2 coords13_g67 = temp_output_10_0_g67 * 25.0;
				float2 id13_g67 = 0;
				float2 uv13_g67 = 0;
				float fade13_g67 = 0.5;
				float voroi13_g67 = 0;
				float rest13_g67 = 0;
				for( int it13_g67 = 0; it13_g67 <2; it13_g67++ ){
				voroi13_g67 += fade13_g67 * voronoi13_g67( coords13_g67, time13_g67, id13_g67, uv13_g67, 0,voronoiSmoothId13_g67 );
				rest13_g67 += fade13_g67;
				coords13_g67 *= 2;
				fade13_g67 *= 0.5;
				}//Voronoi13_g67
				voroi13_g67 /= rest13_g67;
				float time11_g67 = 17.23;
				float2 voronoiSmoothId11_g67 = 0;
				float2 coords11_g67 = temp_output_10_0_g67 * 9.0;
				float2 id11_g67 = 0;
				float2 uv11_g67 = 0;
				float fade11_g67 = 0.5;
				float voroi11_g67 = 0;
				float rest11_g67 = 0;
				for( int it11_g67 = 0; it11_g67 <2; it11_g67++ ){
				voroi11_g67 += fade11_g67 * voronoi11_g67( coords11_g67, time11_g67, id11_g67, uv11_g67, 0,voronoiSmoothId11_g67 );
				rest11_g67 += fade11_g67;
				coords11_g67 *= 2;
				fade11_g67 *= 0.5;
				}//Voronoi11_g67
				voroi11_g67 /= rest11_g67;
				float lerpResult15_g67 = lerp( saturate( ( voroi35_g67 + voroi13_g67 ) ) , voroi11_g67 , ( ModifiedCohesion17_g66 * 1.1 ));
				float lerpResult16_g67 = lerp( lerpResult15_g67 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_61_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g67 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2Alpha65_g66 = SampleGradient( gradient62_g66, temp_output_61_0_g66 ).r;
				float temp_output_173_0_g66 = max( IT1Alpha66_g66 , IT2Alpha65_g66 );
				bool enabled20_g76 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g76 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g76 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g76 = HLSL20_g76( enabled20_g76 , submerged20_g76 , textureSample20_g76 );
				

				float Alpha = ( saturate( ( temp_output_173_0_g66 + ( temp_output_173_0_g66 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g76 ) );
				float AlphaClipThreshold = 0.5;

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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"


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

			float CZY_Spherize;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			float CZY_CloudCohesion;
			float CZY_CumulusCoverageMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			
					float2 voronoihash35_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g68( n + g );
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
			
					float2 voronoihash13_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g68( n + g );
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
			
					float2 voronoihash11_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g68( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g67( n + g );
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
			
					float2 voronoihash13_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g67( n + g );
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
			
					float2 voronoihash11_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g67( n + g );
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
			
			float HLSL20_g76( bool enabled, bool submerged, float textureSample )
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

				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord1 = screenPos;
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

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

				Gradient gradient59_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord10_g66 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_13_0_g66 = ( texCoord10_g66 - float2( 0.5,0.5 ) );
				float dotResult23_g66 = dot( temp_output_13_0_g66 , temp_output_13_0_g66 );
				float Dot49_g66 = saturate( (0.85 + (dotResult23_g66 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g68 = 0.0;
				float2 voronoiSmoothId35_g68 = 0;
				float2 texCoord12_g66 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV27_g66 = ( texCoord12_g66 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g68 = CentralUV27_g66;
				float2 break2_g68 = abs( temp_output_21_0_g68 );
				float saferPower4_g68 = abs( break2_g68.x );
				float saferPower3_g68 = abs( break2_g68.y );
				float saferPower6_g68 = abs( ( pow( saferPower4_g68 , 2.0 ) + pow( saferPower3_g68 , 2.0 ) ) );
				float Spherize26_g66 = CZY_Spherize;
				float Flatness32_g66 = ( 20.0 * CZY_Spherize );
				float Scale164_g66 = ( CZY_MainCloudScale * 0.1 );
				float mulTime5_g66 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float Time6_g66 = mulTime5_g66;
				float2 Wind18_g66 = ( Time6_g66 * float2( 0.1,0.2 ) );
				float2 temp_output_10_0_g68 = (( ( temp_output_21_0_g68 * ( pow( saferPower6_g68 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g68 = temp_output_10_0_g68 * 60.0;
				float2 id35_g68 = 0;
				float2 uv35_g68 = 0;
				float fade35_g68 = 0.5;
				float voroi35_g68 = 0;
				float rest35_g68 = 0;
				for( int it35_g68 = 0; it35_g68 <2; it35_g68++ ){
				voroi35_g68 += fade35_g68 * voronoi35_g68( coords35_g68, time35_g68, id35_g68, uv35_g68, 0,voronoiSmoothId35_g68 );
				rest35_g68 += fade35_g68;
				coords35_g68 *= 2;
				fade35_g68 *= 0.5;
				}//Voronoi35_g68
				voroi35_g68 /= rest35_g68;
				float time13_g68 = 0.0;
				float2 voronoiSmoothId13_g68 = 0;
				float2 coords13_g68 = temp_output_10_0_g68 * 25.0;
				float2 id13_g68 = 0;
				float2 uv13_g68 = 0;
				float fade13_g68 = 0.5;
				float voroi13_g68 = 0;
				float rest13_g68 = 0;
				for( int it13_g68 = 0; it13_g68 <2; it13_g68++ ){
				voroi13_g68 += fade13_g68 * voronoi13_g68( coords13_g68, time13_g68, id13_g68, uv13_g68, 0,voronoiSmoothId13_g68 );
				rest13_g68 += fade13_g68;
				coords13_g68 *= 2;
				fade13_g68 *= 0.5;
				}//Voronoi13_g68
				voroi13_g68 /= rest13_g68;
				float time11_g68 = 17.23;
				float2 voronoiSmoothId11_g68 = 0;
				float2 coords11_g68 = temp_output_10_0_g68 * 9.0;
				float2 id11_g68 = 0;
				float2 uv11_g68 = 0;
				float fade11_g68 = 0.5;
				float voroi11_g68 = 0;
				float rest11_g68 = 0;
				for( int it11_g68 = 0; it11_g68 <2; it11_g68++ ){
				voroi11_g68 += fade11_g68 * voronoi11_g68( coords11_g68, time11_g68, id11_g68, uv11_g68, 0,voronoiSmoothId11_g68 );
				rest11_g68 += fade11_g68;
				coords11_g68 *= 2;
				fade11_g68 *= 0.5;
				}//Voronoi11_g68
				voroi11_g68 /= rest11_g68;
				float2 texCoord2_g66 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g66 = ( texCoord2_g66 - float2( 0.5,0.5 ) );
				float dotResult7_g66 = dot( temp_output_4_0_g66 , temp_output_4_0_g66 );
				float ModifiedCohesion17_g66 = ( CZY_CloudCohesion * 1.0 * ( 1.0 - dotResult7_g66 ) );
				float lerpResult15_g68 = lerp( saturate( ( voroi35_g68 + voroi13_g68 ) ) , voroi11_g68 , ModifiedCohesion17_g66);
				float CumulusCoverage29_g66 = CZY_CumulusCoverageMultiplier;
				float lerpResult16_g68 = lerp( lerpResult15_g68 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_60_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g68 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1Alpha66_g66 = SampleGradient( gradient59_g66, temp_output_60_0_g66 ).r;
				Gradient gradient62_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g67 = 0.0;
				float2 voronoiSmoothId35_g67 = 0;
				float2 temp_output_21_0_g67 = CentralUV27_g66;
				float2 break2_g67 = abs( temp_output_21_0_g67 );
				float saferPower4_g67 = abs( break2_g67.x );
				float saferPower3_g67 = abs( break2_g67.y );
				float saferPower6_g67 = abs( ( pow( saferPower4_g67 , 2.0 ) + pow( saferPower3_g67 , 2.0 ) ) );
				float2 temp_output_10_0_g67 = (( ( temp_output_21_0_g67 * ( pow( saferPower6_g67 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale164_g66 * 1.5 ) ) + ( Wind18_g66 * float2( 0.5,0.5 ) ));
				float2 coords35_g67 = temp_output_10_0_g67 * 60.0;
				float2 id35_g67 = 0;
				float2 uv35_g67 = 0;
				float fade35_g67 = 0.5;
				float voroi35_g67 = 0;
				float rest35_g67 = 0;
				for( int it35_g67 = 0; it35_g67 <2; it35_g67++ ){
				voroi35_g67 += fade35_g67 * voronoi35_g67( coords35_g67, time35_g67, id35_g67, uv35_g67, 0,voronoiSmoothId35_g67 );
				rest35_g67 += fade35_g67;
				coords35_g67 *= 2;
				fade35_g67 *= 0.5;
				}//Voronoi35_g67
				voroi35_g67 /= rest35_g67;
				float time13_g67 = 0.0;
				float2 voronoiSmoothId13_g67 = 0;
				float2 coords13_g67 = temp_output_10_0_g67 * 25.0;
				float2 id13_g67 = 0;
				float2 uv13_g67 = 0;
				float fade13_g67 = 0.5;
				float voroi13_g67 = 0;
				float rest13_g67 = 0;
				for( int it13_g67 = 0; it13_g67 <2; it13_g67++ ){
				voroi13_g67 += fade13_g67 * voronoi13_g67( coords13_g67, time13_g67, id13_g67, uv13_g67, 0,voronoiSmoothId13_g67 );
				rest13_g67 += fade13_g67;
				coords13_g67 *= 2;
				fade13_g67 *= 0.5;
				}//Voronoi13_g67
				voroi13_g67 /= rest13_g67;
				float time11_g67 = 17.23;
				float2 voronoiSmoothId11_g67 = 0;
				float2 coords11_g67 = temp_output_10_0_g67 * 9.0;
				float2 id11_g67 = 0;
				float2 uv11_g67 = 0;
				float fade11_g67 = 0.5;
				float voroi11_g67 = 0;
				float rest11_g67 = 0;
				for( int it11_g67 = 0; it11_g67 <2; it11_g67++ ){
				voroi11_g67 += fade11_g67 * voronoi11_g67( coords11_g67, time11_g67, id11_g67, uv11_g67, 0,voronoiSmoothId11_g67 );
				rest11_g67 += fade11_g67;
				coords11_g67 *= 2;
				fade11_g67 *= 0.5;
				}//Voronoi11_g67
				voroi11_g67 /= rest11_g67;
				float lerpResult15_g67 = lerp( saturate( ( voroi35_g67 + voroi13_g67 ) ) , voroi11_g67 , ( ModifiedCohesion17_g66 * 1.1 ));
				float lerpResult16_g67 = lerp( lerpResult15_g67 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_61_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g67 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2Alpha65_g66 = SampleGradient( gradient62_g66, temp_output_61_0_g66 ).r;
				float temp_output_173_0_g66 = max( IT1Alpha66_g66 , IT2Alpha65_g66 );
				bool enabled20_g76 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g76 =(bool)_FullySubmerged;
				float4 screenPos = input.ase_texcoord1;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g76 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g76 = HLSL20_g76( enabled20_g76 , submerged20_g76 , textureSample20_g76 );
				

				surfaceDescription.Alpha = ( saturate( ( temp_output_173_0_g66 + ( temp_output_173_0_g66 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g76 ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"


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

			float CZY_Spherize;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			float CZY_CloudCohesion;
			float CZY_CumulusCoverageMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			
					float2 voronoihash35_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g68( n + g );
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
			
					float2 voronoihash13_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g68( n + g );
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
			
					float2 voronoihash11_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g68( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g67( n + g );
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
			
					float2 voronoihash13_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g67( n + g );
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
			
					float2 voronoihash11_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g67( n + g );
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
			
			float HLSL20_g76( bool enabled, bool submerged, float textureSample )
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

				float4 ase_positionCS = TransformObjectToHClip( ( input.positionOS ).xyz );
				float4 screenPos = ComputeScreenPos( ase_positionCS );
				output.ase_texcoord1 = screenPos;
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

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

				Gradient gradient59_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord10_g66 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_13_0_g66 = ( texCoord10_g66 - float2( 0.5,0.5 ) );
				float dotResult23_g66 = dot( temp_output_13_0_g66 , temp_output_13_0_g66 );
				float Dot49_g66 = saturate( (0.85 + (dotResult23_g66 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g68 = 0.0;
				float2 voronoiSmoothId35_g68 = 0;
				float2 texCoord12_g66 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV27_g66 = ( texCoord12_g66 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g68 = CentralUV27_g66;
				float2 break2_g68 = abs( temp_output_21_0_g68 );
				float saferPower4_g68 = abs( break2_g68.x );
				float saferPower3_g68 = abs( break2_g68.y );
				float saferPower6_g68 = abs( ( pow( saferPower4_g68 , 2.0 ) + pow( saferPower3_g68 , 2.0 ) ) );
				float Spherize26_g66 = CZY_Spherize;
				float Flatness32_g66 = ( 20.0 * CZY_Spherize );
				float Scale164_g66 = ( CZY_MainCloudScale * 0.1 );
				float mulTime5_g66 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float Time6_g66 = mulTime5_g66;
				float2 Wind18_g66 = ( Time6_g66 * float2( 0.1,0.2 ) );
				float2 temp_output_10_0_g68 = (( ( temp_output_21_0_g68 * ( pow( saferPower6_g68 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g68 = temp_output_10_0_g68 * 60.0;
				float2 id35_g68 = 0;
				float2 uv35_g68 = 0;
				float fade35_g68 = 0.5;
				float voroi35_g68 = 0;
				float rest35_g68 = 0;
				for( int it35_g68 = 0; it35_g68 <2; it35_g68++ ){
				voroi35_g68 += fade35_g68 * voronoi35_g68( coords35_g68, time35_g68, id35_g68, uv35_g68, 0,voronoiSmoothId35_g68 );
				rest35_g68 += fade35_g68;
				coords35_g68 *= 2;
				fade35_g68 *= 0.5;
				}//Voronoi35_g68
				voroi35_g68 /= rest35_g68;
				float time13_g68 = 0.0;
				float2 voronoiSmoothId13_g68 = 0;
				float2 coords13_g68 = temp_output_10_0_g68 * 25.0;
				float2 id13_g68 = 0;
				float2 uv13_g68 = 0;
				float fade13_g68 = 0.5;
				float voroi13_g68 = 0;
				float rest13_g68 = 0;
				for( int it13_g68 = 0; it13_g68 <2; it13_g68++ ){
				voroi13_g68 += fade13_g68 * voronoi13_g68( coords13_g68, time13_g68, id13_g68, uv13_g68, 0,voronoiSmoothId13_g68 );
				rest13_g68 += fade13_g68;
				coords13_g68 *= 2;
				fade13_g68 *= 0.5;
				}//Voronoi13_g68
				voroi13_g68 /= rest13_g68;
				float time11_g68 = 17.23;
				float2 voronoiSmoothId11_g68 = 0;
				float2 coords11_g68 = temp_output_10_0_g68 * 9.0;
				float2 id11_g68 = 0;
				float2 uv11_g68 = 0;
				float fade11_g68 = 0.5;
				float voroi11_g68 = 0;
				float rest11_g68 = 0;
				for( int it11_g68 = 0; it11_g68 <2; it11_g68++ ){
				voroi11_g68 += fade11_g68 * voronoi11_g68( coords11_g68, time11_g68, id11_g68, uv11_g68, 0,voronoiSmoothId11_g68 );
				rest11_g68 += fade11_g68;
				coords11_g68 *= 2;
				fade11_g68 *= 0.5;
				}//Voronoi11_g68
				voroi11_g68 /= rest11_g68;
				float2 texCoord2_g66 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g66 = ( texCoord2_g66 - float2( 0.5,0.5 ) );
				float dotResult7_g66 = dot( temp_output_4_0_g66 , temp_output_4_0_g66 );
				float ModifiedCohesion17_g66 = ( CZY_CloudCohesion * 1.0 * ( 1.0 - dotResult7_g66 ) );
				float lerpResult15_g68 = lerp( saturate( ( voroi35_g68 + voroi13_g68 ) ) , voroi11_g68 , ModifiedCohesion17_g66);
				float CumulusCoverage29_g66 = CZY_CumulusCoverageMultiplier;
				float lerpResult16_g68 = lerp( lerpResult15_g68 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_60_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g68 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1Alpha66_g66 = SampleGradient( gradient59_g66, temp_output_60_0_g66 ).r;
				Gradient gradient62_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g67 = 0.0;
				float2 voronoiSmoothId35_g67 = 0;
				float2 temp_output_21_0_g67 = CentralUV27_g66;
				float2 break2_g67 = abs( temp_output_21_0_g67 );
				float saferPower4_g67 = abs( break2_g67.x );
				float saferPower3_g67 = abs( break2_g67.y );
				float saferPower6_g67 = abs( ( pow( saferPower4_g67 , 2.0 ) + pow( saferPower3_g67 , 2.0 ) ) );
				float2 temp_output_10_0_g67 = (( ( temp_output_21_0_g67 * ( pow( saferPower6_g67 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale164_g66 * 1.5 ) ) + ( Wind18_g66 * float2( 0.5,0.5 ) ));
				float2 coords35_g67 = temp_output_10_0_g67 * 60.0;
				float2 id35_g67 = 0;
				float2 uv35_g67 = 0;
				float fade35_g67 = 0.5;
				float voroi35_g67 = 0;
				float rest35_g67 = 0;
				for( int it35_g67 = 0; it35_g67 <2; it35_g67++ ){
				voroi35_g67 += fade35_g67 * voronoi35_g67( coords35_g67, time35_g67, id35_g67, uv35_g67, 0,voronoiSmoothId35_g67 );
				rest35_g67 += fade35_g67;
				coords35_g67 *= 2;
				fade35_g67 *= 0.5;
				}//Voronoi35_g67
				voroi35_g67 /= rest35_g67;
				float time13_g67 = 0.0;
				float2 voronoiSmoothId13_g67 = 0;
				float2 coords13_g67 = temp_output_10_0_g67 * 25.0;
				float2 id13_g67 = 0;
				float2 uv13_g67 = 0;
				float fade13_g67 = 0.5;
				float voroi13_g67 = 0;
				float rest13_g67 = 0;
				for( int it13_g67 = 0; it13_g67 <2; it13_g67++ ){
				voroi13_g67 += fade13_g67 * voronoi13_g67( coords13_g67, time13_g67, id13_g67, uv13_g67, 0,voronoiSmoothId13_g67 );
				rest13_g67 += fade13_g67;
				coords13_g67 *= 2;
				fade13_g67 *= 0.5;
				}//Voronoi13_g67
				voroi13_g67 /= rest13_g67;
				float time11_g67 = 17.23;
				float2 voronoiSmoothId11_g67 = 0;
				float2 coords11_g67 = temp_output_10_0_g67 * 9.0;
				float2 id11_g67 = 0;
				float2 uv11_g67 = 0;
				float fade11_g67 = 0.5;
				float voroi11_g67 = 0;
				float rest11_g67 = 0;
				for( int it11_g67 = 0; it11_g67 <2; it11_g67++ ){
				voroi11_g67 += fade11_g67 * voronoi11_g67( coords11_g67, time11_g67, id11_g67, uv11_g67, 0,voronoiSmoothId11_g67 );
				rest11_g67 += fade11_g67;
				coords11_g67 *= 2;
				fade11_g67 *= 0.5;
				}//Voronoi11_g67
				voroi11_g67 /= rest11_g67;
				float lerpResult15_g67 = lerp( saturate( ( voroi35_g67 + voroi13_g67 ) ) , voroi11_g67 , ( ModifiedCohesion17_g66 * 1.1 ));
				float lerpResult16_g67 = lerp( lerpResult15_g67 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_61_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g67 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2Alpha65_g66 = SampleGradient( gradient62_g66, temp_output_61_0_g66 ).r;
				float temp_output_173_0_g66 = max( IT1Alpha66_g66 , IT2Alpha65_g66 );
				bool enabled20_g76 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g76 =(bool)_FullySubmerged;
				float4 screenPos = input.ase_texcoord1;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g76 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g76 = HLSL20_g76( enabled20_g76 , submerged20_g76 , textureSample20_g76 );
				

				surfaceDescription.Alpha = ( saturate( ( temp_output_173_0_g66 + ( temp_output_173_0_g66 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g76 ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
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

			float CZY_Spherize;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			float CZY_CloudCohesion;
			float CZY_CumulusCoverageMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


			
					float2 voronoihash35_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g68( n + g );
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
			
					float2 voronoihash13_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g68( n + g );
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
			
					float2 voronoihash11_g68( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g68( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g68( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g67( n + g );
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
			
					float2 voronoihash13_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g67( n + g );
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
			
					float2 voronoihash11_g67( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g67( n + g );
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
			
			float HLSL20_g76( bool enabled, bool submerged, float textureSample )
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

				Gradient gradient59_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord10_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_13_0_g66 = ( texCoord10_g66 - float2( 0.5,0.5 ) );
				float dotResult23_g66 = dot( temp_output_13_0_g66 , temp_output_13_0_g66 );
				float Dot49_g66 = saturate( (0.85 + (dotResult23_g66 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g68 = 0.0;
				float2 voronoiSmoothId35_g68 = 0;
				float2 texCoord12_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV27_g66 = ( texCoord12_g66 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g68 = CentralUV27_g66;
				float2 break2_g68 = abs( temp_output_21_0_g68 );
				float saferPower4_g68 = abs( break2_g68.x );
				float saferPower3_g68 = abs( break2_g68.y );
				float saferPower6_g68 = abs( ( pow( saferPower4_g68 , 2.0 ) + pow( saferPower3_g68 , 2.0 ) ) );
				float Spherize26_g66 = CZY_Spherize;
				float Flatness32_g66 = ( 20.0 * CZY_Spherize );
				float Scale164_g66 = ( CZY_MainCloudScale * 0.1 );
				float mulTime5_g66 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float Time6_g66 = mulTime5_g66;
				float2 Wind18_g66 = ( Time6_g66 * float2( 0.1,0.2 ) );
				float2 temp_output_10_0_g68 = (( ( temp_output_21_0_g68 * ( pow( saferPower6_g68 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale164_g66 ) + Wind18_g66);
				float2 coords35_g68 = temp_output_10_0_g68 * 60.0;
				float2 id35_g68 = 0;
				float2 uv35_g68 = 0;
				float fade35_g68 = 0.5;
				float voroi35_g68 = 0;
				float rest35_g68 = 0;
				for( int it35_g68 = 0; it35_g68 <2; it35_g68++ ){
				voroi35_g68 += fade35_g68 * voronoi35_g68( coords35_g68, time35_g68, id35_g68, uv35_g68, 0,voronoiSmoothId35_g68 );
				rest35_g68 += fade35_g68;
				coords35_g68 *= 2;
				fade35_g68 *= 0.5;
				}//Voronoi35_g68
				voroi35_g68 /= rest35_g68;
				float time13_g68 = 0.0;
				float2 voronoiSmoothId13_g68 = 0;
				float2 coords13_g68 = temp_output_10_0_g68 * 25.0;
				float2 id13_g68 = 0;
				float2 uv13_g68 = 0;
				float fade13_g68 = 0.5;
				float voroi13_g68 = 0;
				float rest13_g68 = 0;
				for( int it13_g68 = 0; it13_g68 <2; it13_g68++ ){
				voroi13_g68 += fade13_g68 * voronoi13_g68( coords13_g68, time13_g68, id13_g68, uv13_g68, 0,voronoiSmoothId13_g68 );
				rest13_g68 += fade13_g68;
				coords13_g68 *= 2;
				fade13_g68 *= 0.5;
				}//Voronoi13_g68
				voroi13_g68 /= rest13_g68;
				float time11_g68 = 17.23;
				float2 voronoiSmoothId11_g68 = 0;
				float2 coords11_g68 = temp_output_10_0_g68 * 9.0;
				float2 id11_g68 = 0;
				float2 uv11_g68 = 0;
				float fade11_g68 = 0.5;
				float voroi11_g68 = 0;
				float rest11_g68 = 0;
				for( int it11_g68 = 0; it11_g68 <2; it11_g68++ ){
				voroi11_g68 += fade11_g68 * voronoi11_g68( coords11_g68, time11_g68, id11_g68, uv11_g68, 0,voronoiSmoothId11_g68 );
				rest11_g68 += fade11_g68;
				coords11_g68 *= 2;
				fade11_g68 *= 0.5;
				}//Voronoi11_g68
				voroi11_g68 /= rest11_g68;
				float2 texCoord2_g66 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g66 = ( texCoord2_g66 - float2( 0.5,0.5 ) );
				float dotResult7_g66 = dot( temp_output_4_0_g66 , temp_output_4_0_g66 );
				float ModifiedCohesion17_g66 = ( CZY_CloudCohesion * 1.0 * ( 1.0 - dotResult7_g66 ) );
				float lerpResult15_g68 = lerp( saturate( ( voroi35_g68 + voroi13_g68 ) ) , voroi11_g68 , ModifiedCohesion17_g66);
				float CumulusCoverage29_g66 = CZY_CumulusCoverageMultiplier;
				float lerpResult16_g68 = lerp( lerpResult15_g68 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_60_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g68 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1Alpha66_g66 = SampleGradient( gradient59_g66, temp_output_60_0_g66 ).r;
				Gradient gradient62_g66 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g67 = 0.0;
				float2 voronoiSmoothId35_g67 = 0;
				float2 temp_output_21_0_g67 = CentralUV27_g66;
				float2 break2_g67 = abs( temp_output_21_0_g67 );
				float saferPower4_g67 = abs( break2_g67.x );
				float saferPower3_g67 = abs( break2_g67.y );
				float saferPower6_g67 = abs( ( pow( saferPower4_g67 , 2.0 ) + pow( saferPower3_g67 , 2.0 ) ) );
				float2 temp_output_10_0_g67 = (( ( temp_output_21_0_g67 * ( pow( saferPower6_g67 , Spherize26_g66 ) * Flatness32_g66 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale164_g66 * 1.5 ) ) + ( Wind18_g66 * float2( 0.5,0.5 ) ));
				float2 coords35_g67 = temp_output_10_0_g67 * 60.0;
				float2 id35_g67 = 0;
				float2 uv35_g67 = 0;
				float fade35_g67 = 0.5;
				float voroi35_g67 = 0;
				float rest35_g67 = 0;
				for( int it35_g67 = 0; it35_g67 <2; it35_g67++ ){
				voroi35_g67 += fade35_g67 * voronoi35_g67( coords35_g67, time35_g67, id35_g67, uv35_g67, 0,voronoiSmoothId35_g67 );
				rest35_g67 += fade35_g67;
				coords35_g67 *= 2;
				fade35_g67 *= 0.5;
				}//Voronoi35_g67
				voroi35_g67 /= rest35_g67;
				float time13_g67 = 0.0;
				float2 voronoiSmoothId13_g67 = 0;
				float2 coords13_g67 = temp_output_10_0_g67 * 25.0;
				float2 id13_g67 = 0;
				float2 uv13_g67 = 0;
				float fade13_g67 = 0.5;
				float voroi13_g67 = 0;
				float rest13_g67 = 0;
				for( int it13_g67 = 0; it13_g67 <2; it13_g67++ ){
				voroi13_g67 += fade13_g67 * voronoi13_g67( coords13_g67, time13_g67, id13_g67, uv13_g67, 0,voronoiSmoothId13_g67 );
				rest13_g67 += fade13_g67;
				coords13_g67 *= 2;
				fade13_g67 *= 0.5;
				}//Voronoi13_g67
				voroi13_g67 /= rest13_g67;
				float time11_g67 = 17.23;
				float2 voronoiSmoothId11_g67 = 0;
				float2 coords11_g67 = temp_output_10_0_g67 * 9.0;
				float2 id11_g67 = 0;
				float2 uv11_g67 = 0;
				float fade11_g67 = 0.5;
				float voroi11_g67 = 0;
				float rest11_g67 = 0;
				for( int it11_g67 = 0; it11_g67 <2; it11_g67++ ){
				voroi11_g67 += fade11_g67 * voronoi11_g67( coords11_g67, time11_g67, id11_g67, uv11_g67, 0,voronoiSmoothId11_g67 );
				rest11_g67 += fade11_g67;
				coords11_g67 *= 2;
				fade11_g67 *= 0.5;
				}//Voronoi11_g67
				voroi11_g67 /= rest11_g67;
				float lerpResult15_g67 = lerp( saturate( ( voroi35_g67 + voroi13_g67 ) ) , voroi11_g67 , ( ModifiedCohesion17_g66 * 1.1 ));
				float lerpResult16_g67 = lerp( lerpResult15_g67 , 1.0 , ( ( 1.0 - CumulusCoverage29_g66 ) + -0.7 ));
				float temp_output_61_0_g66 = saturate( (0.0 + (( Dot49_g66 * ( 1.0 - lerpResult16_g67 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2Alpha65_g66 = SampleGradient( gradient62_g66, temp_output_61_0_g66 ).r;
				float temp_output_173_0_g66 = max( IT1Alpha66_g66 , IT2Alpha65_g66 );
				bool enabled20_g76 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g76 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g76 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g76 = HLSL20_g76( enabled20_g76 , submerged20_g76 , textureSample20_g76 );
				

				float Alpha = ( saturate( ( temp_output_173_0_g66 + ( temp_output_173_0_g66 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g76 ) );
				float AlphaClipThreshold = 0.5;

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
Node;AmplifyShaderEditor.FunctionNode;1251;-944,-400;Inherit;False;Stylized Clouds (Ghibli Desktop);0;;66;6b2329401197307438a30ab78c237583;0;0;3;COLOR;0;FLOAT;183;FLOAT;184
Node;AmplifyShaderEditor.RangedFloatNode;1252;-848,-192;Inherit;False;Global;CZY_CloudsFogAmount;CZY_CloudsFogAmount;8;0;Create;True;0;0;0;False;0;False;0;0.509;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1253;-848,-272;Inherit;False;Global;CZY_CloudsFogLightAmount;CZY_CloudsFogLightAmount;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1254;-512,-400;Inherit;False;AddFogToSkyLayer;-1;;369;36a78fe96c9f6fa4dab85c7793736468;0;3;89;COLOR;0,0,0,0;False;91;FLOAT;0;False;59;FLOAT;0;False;2;COLOR;84;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1236;-670.0242,-403.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1234;-670.0242,-403.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1235;-670.0242,-403.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1232;-670.0242,-400;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1237;-670.0242,-353.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1238;-670.0242,-353.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1239;-670.0242,-353.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1240;-670.0242,-353.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1241;-670.0242,-353.7039;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1233;-240,-400;Float;False;True;-1;2;DistantLands.Cozy.EditorScripts.EmptyShaderGUI;0;13;Distant Lands/Cozy/URP/Stylized Clouds (Ghibli Desktop);2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;9;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;1;False;;False;False;False;False;False;False;False;False;True;True;True;221;False;;255;False;;255;False;;7;False;;2;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=-50;UniversalMaterialType=Unlit;True;2;True;12;all;0;False;True;1;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;25;Surface;1;637952267701750089;  Blend;0;0;Two Sided;2;637952267726528307;Alpha Clipping;1;0;  Use Shadow Threshold;0;0;Forward Only;0;0;Cast Shadows;1;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;True;True;False;False;True;True;True;False;False;;False;0
WireConnection;1254;89;1251;0
WireConnection;1254;91;1253;0
WireConnection;1254;59;1252;0
WireConnection;1233;2;1254;84
WireConnection;1233;3;1251;183
WireConnection;1233;4;1251;184
ASEEND*/
//CHKSM=AA13ACB82AE12C9F5FA06766419CA48DFC1CED1C