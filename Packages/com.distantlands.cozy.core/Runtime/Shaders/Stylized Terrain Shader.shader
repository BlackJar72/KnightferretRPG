// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/URP/Demo/Stylized Terrain First Pass"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector]_TerrainHolesTexture("_TerrainHolesTexture", 2D) = "white" {}
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Normal0("Normal0", 2D) = "white" {}
		[HideInInspector]_Normal1("Normal1", 2D) = "white" {}
		[HideInInspector]_Normal2("Normal2", 2D) = "white" {}
		[HideInInspector]_Normal3("Normal3", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector][Gamma]_Metallic0("Metallic0", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic2("Metallic2", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic3("Metallic3", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic1("Metallic1", Range( 0 , 1)) = 0
		[ASEBegin][Toggle(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)] _EnablePerpixelNormals("Enable Per-pixel Normals", Float) = 0
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		_Splat3Scale("Splat3Scale", Float) = 0
		_Splat0Scale("Splat0Scale", Float) = 0
		_Splat1Scale("Splat1Scale", Float) = 0
		_Splat2Scale("Splat2Scale", Float) = 0
		[Normal]_RockNormals("Rock Normals", 2D) = "bump" {}
		_RockNormals1("Rock Normals", 2D) = "white" {}
		_RockThreshold("Rock Threshold", Float) = 0
		_RockNormalScale("Rock Normal Scale", Float) = 0
		_RockHeight("Rock Height", Float) = 0
		[ASEEnd]_RockColor("Rock Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector][ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[HideInInspector][ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Lit" }

		Cull Back
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 3.5
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

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _LIGHT_COOKIES
			#pragma multi_compile _ _CLUSTERED_RENDERING
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					float4 screenPos : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			float4 CZY_SnowColor;
			TEXTURE2D(CZY_SnowTexture);
			SAMPLER(samplerCZY_SnowTexture);
			TEXTURE2D(_Splat0);
			SAMPLER(sampler_Splat0);
			float _LayerHasMask0;
			float _LayerHasMask1;
			float _LayerHasMask2;
			float _LayerHasMask3;
			SAMPLER(sampler_Linear_Repeat);
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			float4 _DiffuseRemapScale0;
			TEXTURE2D(_Splat1);
			SAMPLER(sampler_Splat1);
			float4 _DiffuseRemapScale1;
			TEXTURE2D(_Splat2);
			SAMPLER(sampler_Splat2);
			float4 _DiffuseRemapScale2;
			TEXTURE2D(_Splat3);
			SAMPLER(sampler_Splat3);
			float4 _DiffuseRemapScale3;
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);
			float CZY_SnowScale;
			float CZY_SnowAmount;
			TEXTURE2D(_RockNormals1);
			TEXTURE2D(_RockNormals);
			TEXTURE2D(_Normal0);
			TEXTURE2D(_Normal1);
			TEXTURE2D(_Normal2);
			TEXTURE2D(_Normal3);
			float CZY_PuddleScale;
			float CZY_WetnessAmount;


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

			//#ifdef HAVE_VFX_MODIFICATION
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			//#endif

			real4 ASESafeNormalize(float4 inVec)
			{
				real dp3 = max(FLT_MIN, dot(inVec, inVec));
				return inVec* rsqrt( dp3);
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
			
					float2 voronoihash5_g112( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi5_g112( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash5_g112( n + g );
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
			
					float2 voronoihash1_g111( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi1_g111( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash1_g111( n + g );
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
			
					float2 voronoihash8_g111( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi8_g111( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash8_g111( n + g );
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
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord8.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				#endif

				#if !defined(LIGHTMAP_ON)
					OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					o.dynamicLightmapUV.xy = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );

				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif

				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					o.screenPos = ComputeScreenPos(positionCS);
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					float4 ScreenPos = IN.screenPos;
				#endif

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uvCZY_SnowTexture = IN.ase_texcoord8.xy * CZY_SnowTexture_ST.xy + CZY_SnowTexture_ST.zw;
				float4 appendResult535_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
				float2 temp_cast_0 = (_Splat0Scale).xx;
				float2 texCoord687_g124 = IN.ase_texcoord8.xy * temp_cast_0 + float2( 0,0 );
				float2 SplatUV0689_g124 = texCoord687_g124;
				float4 tex2DNode572_g124 = SAMPLE_TEXTURE2D( _Splat0, sampler_Splat0, SplatUV0689_g124 );
				float localComputeMasks683_g124 = ( 0.0 );
				float4 masks0683_g124 = float4( 0,0,0,0 );
				float4 masks1683_g124 = float4( 0,0,0,0 );
				float4 masks2683_g124 = float4( 0,0,0,0 );
				float4 masks3683_g124 = float4( 0,0,0,0 );
				float4 appendResult625_g124 = (float4(_LayerHasMask0 , _LayerHasMask1 , _LayerHasMask2 , _LayerHasMask3));
				float4 layerHasMask678_g124 = appendResult625_g124;
				float4 hasMask683_g124 = layerHasMask678_g124;
				float2 uvMask0683_g124 = SplatUV0689_g124;
				float2 temp_cast_2 = (_Splat1Scale).xx;
				float2 texCoord690_g124 = IN.ase_texcoord8.xy * temp_cast_2 + float2( 0,0 );
				float2 SplatUV1692_g124 = texCoord690_g124;
				float2 uvMask1683_g124 = SplatUV1692_g124;
				float2 temp_cast_3 = (_Splat2Scale).xx;
				float2 texCoord693_g124 = IN.ase_texcoord8.xy * temp_cast_3 + float2( 0,0 );
				float2 SplatUV2695_g124 = texCoord693_g124;
				float2 uvMask2683_g124 = SplatUV2695_g124;
				float2 temp_cast_4 = (_Splat3Scale).xx;
				float2 texCoord696_g124 = IN.ase_texcoord8.xy * temp_cast_4 + float2( 0,0 );
				float2 SplatUV3698_g124 = texCoord696_g124;
				float2 uvMask3683_g124 = SplatUV3698_g124;
				SamplerState sampler_Mask0683_g124 = sampler_Linear_Repeat;
				{
				masks0683_g124 = 0.5h;
				masks1683_g124 = 0.5h;
				masks2683_g124 = 0.5h;
				masks3683_g124 = 0.5h;
				#ifdef _MASKMAP
				masks0683_g124 = lerp(masks0683_g124, SAMPLE_TEXTURE2D(_Mask0, sampler_Mask0683_g124, uvMask0683_g124), hasMask683_g124.x);
				masks1683_g124 = lerp(masks1683_g124, SAMPLE_TEXTURE2D(_Mask1, sampler_Mask0683_g124, uvMask1683_g124), hasMask683_g124.y);
				masks2683_g124 = lerp(masks2683_g124, SAMPLE_TEXTURE2D(_Mask2, sampler_Mask0683_g124, uvMask2683_g124), hasMask683_g124.z);
				masks3683_g124 = lerp(masks3683_g124, SAMPLE_TEXTURE2D(_Mask3, sampler_Mask0683_g124, uvMask3683_g124), hasMask683_g124.w);
				#endif
				masks0683_g124 *= _MaskMapRemapScale0.rgba;
				masks0683_g124 += _MaskMapRemapOffset0.rgba;
				masks1683_g124 *= _MaskMapRemapScale1.rgba;
				masks1683_g124 += _MaskMapRemapOffset1.rgba;
				masks2683_g124 *= _MaskMapRemapScale2.rgba;
				masks2683_g124 += _MaskMapRemapOffset2.rgba;
				masks3683_g124 *= _MaskMapRemapScale3.rgba;
				masks3683_g124 += _MaskMapRemapOffset3.rgba;
				}
				float4 mask0633_g124 = masks0683_g124;
				float4 mask1626_g124 = masks1683_g124;
				float4 mask2627_g124 = masks2683_g124;
				float4 mask3624_g124 = masks3683_g124;
				float4 appendResult718_g124 = (float4((mask0633_g124).z , (mask1626_g124).z , (mask2627_g124).z , (mask3624_g124).z));
				float4 maskHeight716_g124 = appendResult718_g124;
				float4 temp_output_803_0_g124 = ( maskHeight716_g124 + float4( 1,1,1,1 ) );
				float2 uv_Control = IN.ase_texcoord8.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				float localSplatClip486_g124 = ( SplatWeight552_g124 );
				float SplatWeight486_g124 = SplatWeight552_g124;
				{
				#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight486_g124 == 0.0f ? -1 : 1);
				#endif
				}
				float4 SplatControl553_g124 = ( tex2DNode598_g124 / ( localSplatClip486_g124 + 0.001 ) );
				float4 break754_g124 = temp_output_803_0_g124;
				float4 break733_g124 = SplatControl553_g124;
				float4 temp_output_802_0_g124 = ( ( break754_g124.x * break733_g124.r ) >= ( break754_g124.y * break733_g124.g ) ? float4( 1,0,0,0 ) : float4( 0,1,0,0 ) );
				float4 temp_output_806_0_g124 = ( ( break754_g124.z * break733_g124.b ) >= ( break754_g124.w * break733_g124.a ) ? float4( 0,0,1,0 ) : float4( 0,0,0,1 ) );
				float4 normalizeResult814_g124 = ASESafeNormalize( ( length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_802_0_g124 ) ) >= length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_806_0_g124 ) ) ? temp_output_802_0_g124 : temp_output_806_0_g124 ) );
				float4 HeightbasedSplat739_g124 = saturate( normalizeResult814_g124 );
				float4 appendResult574_g124 = (float4(( (HeightbasedSplat739_g124).xxx * (_DiffuseRemapScale0).rgb ) , 1.0));
				float4 tintLayer0526_g124 = appendResult574_g124;
				float4 appendResult540_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
				float4 tex2DNode546_g124 = SAMPLE_TEXTURE2D( _Splat1, sampler_Splat1, SplatUV1692_g124 );
				float4 appendResult591_g124 = (float4(( (HeightbasedSplat739_g124).yyy * (_DiffuseRemapScale1).rgb ) , 1.0));
				float4 tintLayer1589_g124 = appendResult591_g124;
				float4 appendResult508_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
				float4 tex2DNode509_g124 = SAMPLE_TEXTURE2D( _Splat2, sampler_Splat2, SplatUV2695_g124 );
				float4 appendResult594_g124 = (float4(( (HeightbasedSplat739_g124).zzz * (_DiffuseRemapScale2).rgb ) , 1.0));
				float4 tintLayer2592_g124 = appendResult594_g124;
				float4 appendResult520_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
				float4 tex2DNode582_g124 = SAMPLE_TEXTURE2D( _Splat3, sampler_Splat3, SplatUV3698_g124 );
				float4 appendResult542_g124 = (float4(( (HeightbasedSplat739_g124).www * (_DiffuseRemapScale3).rgb ) , 1.0));
				float4 tintLayer3595_g124 = appendResult542_g124;
				float4 weightedBlendVar684_g124 = float4(1,1,1,1);
				float4 weightedBlend684_g124 = ( weightedBlendVar684_g124.x*( appendResult535_g124 * tex2DNode572_g124 * tintLayer0526_g124 ) + weightedBlendVar684_g124.y*( appendResult540_g124 * tex2DNode546_g124 * tintLayer1589_g124 ) + weightedBlendVar684_g124.z*( appendResult508_g124 * tex2DNode509_g124 * tintLayer2592_g124 ) + weightedBlendVar684_g124.w*( appendResult520_g124 * tex2DNode582_g124 * tintLayer3595_g124 ) );
				float4 MixDiffuse491_g124 = weightedBlend684_g124;
				float4 temp_output_499_0_g124 = MixDiffuse491_g124;
				float2 uv_TerrainHolesTexture = IN.ase_texcoord8.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue586_g124 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				clip( holeClipValue586_g124 - 0.5);
				float2 appendResult3_g112 = (float2(WorldPosition.x , WorldPosition.z));
				float temp_output_6_0_g112 = ( 1.0 / CZY_SnowScale );
				float simplePerlin2D7_g112 = snoise( appendResult3_g112*temp_output_6_0_g112 );
				simplePerlin2D7_g112 = simplePerlin2D7_g112*0.5 + 0.5;
				float time5_g112 = 0.0;
				float2 voronoiSmoothId5_g112 = 0;
				float2 coords5_g112 = appendResult3_g112 * ( temp_output_6_0_g112 / 0.1 );
				float2 id5_g112 = 0;
				float2 uv5_g112 = 0;
				float voroi5_g112 = voronoi5_g112( coords5_g112, time5_g112, id5_g112, uv5_g112, 0, voronoiSmoothId5_g112 );
				float4 appendResult564_g124 = (float4(_Metallic0 , _Metallic1 , _Metallic2 , _Metallic3));
				float dotResult571_g124 = dot( SplatControl553_g124 , appendResult564_g124 );
				float4 lerpResult19_g112 = lerp( ( CZY_SnowColor * SAMPLE_TEXTURE2D( CZY_SnowTexture, samplerCZY_SnowTexture, uvCZY_SnowTexture ) ) , temp_output_499_0_g124 , ( ( pow( ( pow( WorldNormal.y , 7.0 ) * ( simplePerlin2D7_g112 * ( 1.0 - voroi5_g112 ) ) ) , 0.5 ) * dotResult571_g124 ) > ( 1.0 - CZY_SnowAmount ) ? 0.0 : 1.0 ));
				float2 uv_RockNormals1 = IN.ase_texcoord8.xy * _RockNormals1_ST.xy + _RockNormals1_ST.zw;
				float temp_output_66_0 = ( ( ( 1.0 - WorldNormal.y ) + ( SAMPLE_TEXTURE2D( _RockNormals1, sampler_Linear_Repeat, uv_RockNormals1 ).r * _RockHeight ) ) > _RockThreshold ? 0.0 : 1.0 );
				float4 lerpResult67 = lerp( _RockColor , lerpResult19_g112 , temp_output_66_0);
				
				float2 uv_RockNormals = IN.ase_texcoord8.xy * _RockNormals_ST.xy + _RockNormals_ST.zw;
				float3 unpack186 = UnpackNormalScale( SAMPLE_TEXTURE2D( _RockNormals, sampler_Linear_Repeat, uv_RockNormals ), _RockNormalScale );
				unpack186.z = lerp( 1, unpack186.z, saturate(_RockNormalScale) );
				float4 temp_output_579_0_g124 = HeightbasedSplat739_g124;
				float4 weightedBlendVar558_g124 = temp_output_579_0_g124;
				float4 weightedBlend558_g124 = ( weightedBlendVar558_g124.x*SAMPLE_TEXTURE2D( _Normal0, sampler_Linear_Repeat, SplatUV0689_g124 ) + weightedBlendVar558_g124.y*SAMPLE_TEXTURE2D( _Normal1, sampler_Linear_Repeat, SplatUV1692_g124 ) + weightedBlendVar558_g124.z*SAMPLE_TEXTURE2D( _Normal2, sampler_Linear_Repeat, SplatUV2695_g124 ) + weightedBlendVar558_g124.w*SAMPLE_TEXTURE2D( _Normal3, sampler_Linear_Repeat, SplatUV3698_g124 ) );
				float3 unpack556_g124 = UnpackNormalScale( weightedBlend558_g124, 0.5 );
				unpack556_g124.z = lerp( 1, unpack556_g124.z, saturate(0.5) );
				float3 temp_output_570_0_g124 = unpack556_g124;
				#ifdef _TERRAIN_INSTANCED_PERPIXEL_NORMAL
				float3 staticSwitch605_g124 = temp_output_570_0_g124;
				#else
				float3 staticSwitch605_g124 = temp_output_570_0_g124;
				#endif
				float3 lerpResult187 = lerp( unpack186 , staticSwitch605_g124 , temp_output_66_0);
				
				float temp_output_5_0_g111 = ( 1.0 / CZY_PuddleScale );
				float time1_g111 = 0.0;
				float2 voronoiSmoothId1_g111 = 0;
				float2 appendResult3_g111 = (float2(WorldPosition.x , WorldPosition.z));
				float2 coords1_g111 = appendResult3_g111 * temp_output_5_0_g111;
				float2 id1_g111 = 0;
				float2 uv1_g111 = 0;
				float voroi1_g111 = voronoi1_g111( coords1_g111, time1_g111, id1_g111, uv1_g111, 0, voronoiSmoothId1_g111 );
				float time8_g111 = 2.16;
				float2 voronoiSmoothId8_g111 = 0;
				float2 coords8_g111 = IN.ase_texcoord8.xy * ( temp_output_5_0_g111 * 3.0 );
				float2 id8_g111 = 0;
				float2 uv8_g111 = 0;
				float voroi8_g111 = voronoi8_g111( coords8_g111, time8_g111, id8_g111, uv8_g111, 0, voronoiSmoothId8_g111 );
				float4 appendResult492_g124 = (float4(_Smoothness0 , _Smoothness1 , _Smoothness2 , _Smoothness3));
				float4 appendResult487_g124 = (float4(tex2DNode572_g124.a , tex2DNode546_g124.a , tex2DNode509_g124.a , tex2DNode582_g124.a));
				float4 defaultSmoothness480_g124 = ( appendResult492_g124 * appendResult487_g124 );
				float4 appendResult615_g124 = (float4((mask0633_g124).w , (mask1626_g124).w , (mask2627_g124).w , (mask3624_g124).w));
				float4 maskSmoothness675_g124 = appendResult615_g124;
				float4 lerpResult577_g124 = lerp( defaultSmoothness480_g124 , maskSmoothness675_g124 , layerHasMask678_g124);
				float dotResult500_g124 = dot( lerpResult577_g124 , SplatControl553_g124 );
				

				float3 BaseColor = lerpResult67.rgb;
				float3 Normal = lerpResult187;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = ( ( WorldNormal.y * 2.0 * ( (1.0 + (voroi1_g111 - 0.0) * (0.0 - 1.0) / (0.4 - 0.0)) + (0.1 + (voroi8_g111 - 0.0) * (-0.3 - 0.1) / (0.21 - 0.0)) ) * (0.3 + (CZY_WetnessAmount - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) ) > ( 1.0 - ( CZY_WetnessAmount * dotResult500_g124 ) ) ? 1.0 : 0.0 );
				float Occlusion = 1;
				float Alpha = SplatWeight552_g124;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _CLEARCOAT
					float CoatMask = 0;
					float CoatSmoothness = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;

				#ifdef _NORMALMAP
						#if _NORMAL_DROPOFF_TS
							inputData.normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent, WorldBiTangent, WorldNormal));
						#elif _NORMAL_DROPOFF_OS
							inputData.normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							inputData.normalWS = Normal;
						#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = ShadowCoords;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				#else
					inputData.shadowCoord = float4(0, 0, 0, 0);
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif
					inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(IN.lightmapUVOrVertexSH.xy, IN.dynamicLightmapUV.xy, SH, inputData.normalWS);
				#else
					inputData.bakedGI = SAMPLE_GI(IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
					#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = IN.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				SurfaceData surfaceData;
				surfaceData.albedo              = BaseColor;
				surfaceData.metallic            = saturate(Metallic);
				surfaceData.specular            = Specular;
				surfaceData.smoothness          = saturate(Smoothness),
				surfaceData.occlusion           = Occlusion,
				surfaceData.emission            = Emission,
				surfaceData.alpha               = saturate(Alpha);
				surfaceData.normalTS            = Normal;
				surfaceData.clearCoatMask       = 0;
				surfaceData.clearCoatSmoothness = 1;

				#ifdef _CLEARCOAT
					surfaceData.clearCoatMask       = saturate(CoatMask);
					surfaceData.clearCoatSmoothness = saturate(CoatSmoothness);
				#endif

				#ifdef _DBUFFER
					ApplyDecalToSurfaceData(IN.clipPos, surfaceData, inputData);
				#endif

				half4 color = UniversalFragmentPBR( inputData, surfaceData);

				#ifdef ASE_TRANSMISSION
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += BaseColor * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += BaseColor * transmission;
						}
					#endif
				}
				#endif

				#ifdef ASE_TRANSLUCENCY
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += BaseColor * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += BaseColor * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef ASE_REFRACTION
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
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

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

			//#ifdef HAVE_VFX_MODIFICATION
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			//#endif

			
			float3 _LightDirection;
			float3 _LightPosition;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = clipPos;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Control = IN.ase_texcoord2.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				

				float Alpha = SplatWeight552_g124;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
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

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

			//#ifdef HAVE_VFX_MODIFICATION
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			//#endif

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Control = IN.ase_texcoord2.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				

				float Alpha = SplatWeight552_g124;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
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
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			float4 CZY_SnowColor;
			TEXTURE2D(CZY_SnowTexture);
			SAMPLER(samplerCZY_SnowTexture);
			TEXTURE2D(_Splat0);
			SAMPLER(sampler_Splat0);
			float _LayerHasMask0;
			float _LayerHasMask1;
			float _LayerHasMask2;
			float _LayerHasMask3;
			SAMPLER(sampler_Linear_Repeat);
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			float4 _DiffuseRemapScale0;
			TEXTURE2D(_Splat1);
			SAMPLER(sampler_Splat1);
			float4 _DiffuseRemapScale1;
			TEXTURE2D(_Splat2);
			SAMPLER(sampler_Splat2);
			float4 _DiffuseRemapScale2;
			TEXTURE2D(_Splat3);
			SAMPLER(sampler_Splat3);
			float4 _DiffuseRemapScale3;
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);
			float CZY_SnowScale;
			float CZY_SnowAmount;
			TEXTURE2D(_RockNormals1);


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

			//#ifdef HAVE_VFX_MODIFICATION
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			//#endif

			real4 ASESafeNormalize(float4 inVec)
			{
				real dp3 = max(FLT_MIN, dot(inVec, inVec));
				return inVec* rsqrt( dp3);
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
			
					float2 voronoihash5_g112( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi5_g112( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash5_g112( n + g );
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
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				o.ase_texcoord3.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uvCZY_SnowTexture = IN.ase_texcoord2.xy * CZY_SnowTexture_ST.xy + CZY_SnowTexture_ST.zw;
				float4 appendResult535_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
				float2 temp_cast_0 = (_Splat0Scale).xx;
				float2 texCoord687_g124 = IN.ase_texcoord2.xy * temp_cast_0 + float2( 0,0 );
				float2 SplatUV0689_g124 = texCoord687_g124;
				float4 tex2DNode572_g124 = SAMPLE_TEXTURE2D( _Splat0, sampler_Splat0, SplatUV0689_g124 );
				float localComputeMasks683_g124 = ( 0.0 );
				float4 masks0683_g124 = float4( 0,0,0,0 );
				float4 masks1683_g124 = float4( 0,0,0,0 );
				float4 masks2683_g124 = float4( 0,0,0,0 );
				float4 masks3683_g124 = float4( 0,0,0,0 );
				float4 appendResult625_g124 = (float4(_LayerHasMask0 , _LayerHasMask1 , _LayerHasMask2 , _LayerHasMask3));
				float4 layerHasMask678_g124 = appendResult625_g124;
				float4 hasMask683_g124 = layerHasMask678_g124;
				float2 uvMask0683_g124 = SplatUV0689_g124;
				float2 temp_cast_2 = (_Splat1Scale).xx;
				float2 texCoord690_g124 = IN.ase_texcoord2.xy * temp_cast_2 + float2( 0,0 );
				float2 SplatUV1692_g124 = texCoord690_g124;
				float2 uvMask1683_g124 = SplatUV1692_g124;
				float2 temp_cast_3 = (_Splat2Scale).xx;
				float2 texCoord693_g124 = IN.ase_texcoord2.xy * temp_cast_3 + float2( 0,0 );
				float2 SplatUV2695_g124 = texCoord693_g124;
				float2 uvMask2683_g124 = SplatUV2695_g124;
				float2 temp_cast_4 = (_Splat3Scale).xx;
				float2 texCoord696_g124 = IN.ase_texcoord2.xy * temp_cast_4 + float2( 0,0 );
				float2 SplatUV3698_g124 = texCoord696_g124;
				float2 uvMask3683_g124 = SplatUV3698_g124;
				SamplerState sampler_Mask0683_g124 = sampler_Linear_Repeat;
				{
				masks0683_g124 = 0.5h;
				masks1683_g124 = 0.5h;
				masks2683_g124 = 0.5h;
				masks3683_g124 = 0.5h;
				#ifdef _MASKMAP
				masks0683_g124 = lerp(masks0683_g124, SAMPLE_TEXTURE2D(_Mask0, sampler_Mask0683_g124, uvMask0683_g124), hasMask683_g124.x);
				masks1683_g124 = lerp(masks1683_g124, SAMPLE_TEXTURE2D(_Mask1, sampler_Mask0683_g124, uvMask1683_g124), hasMask683_g124.y);
				masks2683_g124 = lerp(masks2683_g124, SAMPLE_TEXTURE2D(_Mask2, sampler_Mask0683_g124, uvMask2683_g124), hasMask683_g124.z);
				masks3683_g124 = lerp(masks3683_g124, SAMPLE_TEXTURE2D(_Mask3, sampler_Mask0683_g124, uvMask3683_g124), hasMask683_g124.w);
				#endif
				masks0683_g124 *= _MaskMapRemapScale0.rgba;
				masks0683_g124 += _MaskMapRemapOffset0.rgba;
				masks1683_g124 *= _MaskMapRemapScale1.rgba;
				masks1683_g124 += _MaskMapRemapOffset1.rgba;
				masks2683_g124 *= _MaskMapRemapScale2.rgba;
				masks2683_g124 += _MaskMapRemapOffset2.rgba;
				masks3683_g124 *= _MaskMapRemapScale3.rgba;
				masks3683_g124 += _MaskMapRemapOffset3.rgba;
				}
				float4 mask0633_g124 = masks0683_g124;
				float4 mask1626_g124 = masks1683_g124;
				float4 mask2627_g124 = masks2683_g124;
				float4 mask3624_g124 = masks3683_g124;
				float4 appendResult718_g124 = (float4((mask0633_g124).z , (mask1626_g124).z , (mask2627_g124).z , (mask3624_g124).z));
				float4 maskHeight716_g124 = appendResult718_g124;
				float4 temp_output_803_0_g124 = ( maskHeight716_g124 + float4( 1,1,1,1 ) );
				float2 uv_Control = IN.ase_texcoord2.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				float localSplatClip486_g124 = ( SplatWeight552_g124 );
				float SplatWeight486_g124 = SplatWeight552_g124;
				{
				#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight486_g124 == 0.0f ? -1 : 1);
				#endif
				}
				float4 SplatControl553_g124 = ( tex2DNode598_g124 / ( localSplatClip486_g124 + 0.001 ) );
				float4 break754_g124 = temp_output_803_0_g124;
				float4 break733_g124 = SplatControl553_g124;
				float4 temp_output_802_0_g124 = ( ( break754_g124.x * break733_g124.r ) >= ( break754_g124.y * break733_g124.g ) ? float4( 1,0,0,0 ) : float4( 0,1,0,0 ) );
				float4 temp_output_806_0_g124 = ( ( break754_g124.z * break733_g124.b ) >= ( break754_g124.w * break733_g124.a ) ? float4( 0,0,1,0 ) : float4( 0,0,0,1 ) );
				float4 normalizeResult814_g124 = ASESafeNormalize( ( length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_802_0_g124 ) ) >= length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_806_0_g124 ) ) ? temp_output_802_0_g124 : temp_output_806_0_g124 ) );
				float4 HeightbasedSplat739_g124 = saturate( normalizeResult814_g124 );
				float4 appendResult574_g124 = (float4(( (HeightbasedSplat739_g124).xxx * (_DiffuseRemapScale0).rgb ) , 1.0));
				float4 tintLayer0526_g124 = appendResult574_g124;
				float4 appendResult540_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
				float4 tex2DNode546_g124 = SAMPLE_TEXTURE2D( _Splat1, sampler_Splat1, SplatUV1692_g124 );
				float4 appendResult591_g124 = (float4(( (HeightbasedSplat739_g124).yyy * (_DiffuseRemapScale1).rgb ) , 1.0));
				float4 tintLayer1589_g124 = appendResult591_g124;
				float4 appendResult508_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
				float4 tex2DNode509_g124 = SAMPLE_TEXTURE2D( _Splat2, sampler_Splat2, SplatUV2695_g124 );
				float4 appendResult594_g124 = (float4(( (HeightbasedSplat739_g124).zzz * (_DiffuseRemapScale2).rgb ) , 1.0));
				float4 tintLayer2592_g124 = appendResult594_g124;
				float4 appendResult520_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
				float4 tex2DNode582_g124 = SAMPLE_TEXTURE2D( _Splat3, sampler_Splat3, SplatUV3698_g124 );
				float4 appendResult542_g124 = (float4(( (HeightbasedSplat739_g124).www * (_DiffuseRemapScale3).rgb ) , 1.0));
				float4 tintLayer3595_g124 = appendResult542_g124;
				float4 weightedBlendVar684_g124 = float4(1,1,1,1);
				float4 weightedBlend684_g124 = ( weightedBlendVar684_g124.x*( appendResult535_g124 * tex2DNode572_g124 * tintLayer0526_g124 ) + weightedBlendVar684_g124.y*( appendResult540_g124 * tex2DNode546_g124 * tintLayer1589_g124 ) + weightedBlendVar684_g124.z*( appendResult508_g124 * tex2DNode509_g124 * tintLayer2592_g124 ) + weightedBlendVar684_g124.w*( appendResult520_g124 * tex2DNode582_g124 * tintLayer3595_g124 ) );
				float4 MixDiffuse491_g124 = weightedBlend684_g124;
				float4 temp_output_499_0_g124 = MixDiffuse491_g124;
				float2 uv_TerrainHolesTexture = IN.ase_texcoord2.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue586_g124 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				clip( holeClipValue586_g124 - 0.5);
				float3 ase_worldNormal = IN.ase_texcoord3.xyz;
				float2 appendResult3_g112 = (float2(WorldPosition.x , WorldPosition.z));
				float temp_output_6_0_g112 = ( 1.0 / CZY_SnowScale );
				float simplePerlin2D7_g112 = snoise( appendResult3_g112*temp_output_6_0_g112 );
				simplePerlin2D7_g112 = simplePerlin2D7_g112*0.5 + 0.5;
				float time5_g112 = 0.0;
				float2 voronoiSmoothId5_g112 = 0;
				float2 coords5_g112 = appendResult3_g112 * ( temp_output_6_0_g112 / 0.1 );
				float2 id5_g112 = 0;
				float2 uv5_g112 = 0;
				float voroi5_g112 = voronoi5_g112( coords5_g112, time5_g112, id5_g112, uv5_g112, 0, voronoiSmoothId5_g112 );
				float4 appendResult564_g124 = (float4(_Metallic0 , _Metallic1 , _Metallic2 , _Metallic3));
				float dotResult571_g124 = dot( SplatControl553_g124 , appendResult564_g124 );
				float4 lerpResult19_g112 = lerp( ( CZY_SnowColor * SAMPLE_TEXTURE2D( CZY_SnowTexture, samplerCZY_SnowTexture, uvCZY_SnowTexture ) ) , temp_output_499_0_g124 , ( ( pow( ( pow( ase_worldNormal.y , 7.0 ) * ( simplePerlin2D7_g112 * ( 1.0 - voroi5_g112 ) ) ) , 0.5 ) * dotResult571_g124 ) > ( 1.0 - CZY_SnowAmount ) ? 0.0 : 1.0 ));
				float2 uv_RockNormals1 = IN.ase_texcoord2.xy * _RockNormals1_ST.xy + _RockNormals1_ST.zw;
				float temp_output_66_0 = ( ( ( 1.0 - ase_worldNormal.y ) + ( SAMPLE_TEXTURE2D( _RockNormals1, sampler_Linear_Repeat, uv_RockNormals1 ).r * _RockHeight ) ) > _RockThreshold ? 0.0 : 1.0 );
				float4 lerpResult67 = lerp( _RockColor , lerpResult19_g112 , temp_output_66_0);
				

				float3 BaseColor = lerpResult67.rgb;
				float Alpha = SplatWeight552_g124;
				float AlphaClipThreshold = 0.5;

				half4 color = half4(BaseColor, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				float4 worldTangent : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			TEXTURE2D(_RockNormals);
			SAMPLER(sampler_Linear_Repeat);
			float _LayerHasMask0;
			float _LayerHasMask1;
			float _LayerHasMask2;
			float _LayerHasMask3;
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Normal0);
			TEXTURE2D(_Normal1);
			TEXTURE2D(_Normal2);
			TEXTURE2D(_Normal3);
			TEXTURE2D(_RockNormals1);


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

			//#ifdef HAVE_VFX_MODIFICATION
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			//#endif

			real4 ASESafeNormalize(float4 inVec)
			{
				real dp3 = max(FLT_MIN, dot(inVec, inVec));
				return inVec* rsqrt( dp3);
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord4.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 tangentWS = float4(TransformObjectToWorldDir( v.ase_tangent.xyz), v.ase_tangent.w);
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;
				o.worldTangent = tangentWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float3 WorldNormal = IN.worldNormal;
				float4 WorldTangent = IN.worldTangent;

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_RockNormals = IN.ase_texcoord4.xy * _RockNormals_ST.xy + _RockNormals_ST.zw;
				float3 unpack186 = UnpackNormalScale( SAMPLE_TEXTURE2D( _RockNormals, sampler_Linear_Repeat, uv_RockNormals ), _RockNormalScale );
				unpack186.z = lerp( 1, unpack186.z, saturate(_RockNormalScale) );
				float localComputeMasks683_g124 = ( 0.0 );
				float4 masks0683_g124 = float4( 0,0,0,0 );
				float4 masks1683_g124 = float4( 0,0,0,0 );
				float4 masks2683_g124 = float4( 0,0,0,0 );
				float4 masks3683_g124 = float4( 0,0,0,0 );
				float4 appendResult625_g124 = (float4(_LayerHasMask0 , _LayerHasMask1 , _LayerHasMask2 , _LayerHasMask3));
				float4 layerHasMask678_g124 = appendResult625_g124;
				float4 hasMask683_g124 = layerHasMask678_g124;
				float2 temp_cast_0 = (_Splat0Scale).xx;
				float2 texCoord687_g124 = IN.ase_texcoord4.xy * temp_cast_0 + float2( 0,0 );
				float2 SplatUV0689_g124 = texCoord687_g124;
				float2 uvMask0683_g124 = SplatUV0689_g124;
				float2 temp_cast_1 = (_Splat1Scale).xx;
				float2 texCoord690_g124 = IN.ase_texcoord4.xy * temp_cast_1 + float2( 0,0 );
				float2 SplatUV1692_g124 = texCoord690_g124;
				float2 uvMask1683_g124 = SplatUV1692_g124;
				float2 temp_cast_2 = (_Splat2Scale).xx;
				float2 texCoord693_g124 = IN.ase_texcoord4.xy * temp_cast_2 + float2( 0,0 );
				float2 SplatUV2695_g124 = texCoord693_g124;
				float2 uvMask2683_g124 = SplatUV2695_g124;
				float2 temp_cast_3 = (_Splat3Scale).xx;
				float2 texCoord696_g124 = IN.ase_texcoord4.xy * temp_cast_3 + float2( 0,0 );
				float2 SplatUV3698_g124 = texCoord696_g124;
				float2 uvMask3683_g124 = SplatUV3698_g124;
				SamplerState sampler_Mask0683_g124 = sampler_Linear_Repeat;
				{
				masks0683_g124 = 0.5h;
				masks1683_g124 = 0.5h;
				masks2683_g124 = 0.5h;
				masks3683_g124 = 0.5h;
				#ifdef _MASKMAP
				masks0683_g124 = lerp(masks0683_g124, SAMPLE_TEXTURE2D(_Mask0, sampler_Mask0683_g124, uvMask0683_g124), hasMask683_g124.x);
				masks1683_g124 = lerp(masks1683_g124, SAMPLE_TEXTURE2D(_Mask1, sampler_Mask0683_g124, uvMask1683_g124), hasMask683_g124.y);
				masks2683_g124 = lerp(masks2683_g124, SAMPLE_TEXTURE2D(_Mask2, sampler_Mask0683_g124, uvMask2683_g124), hasMask683_g124.z);
				masks3683_g124 = lerp(masks3683_g124, SAMPLE_TEXTURE2D(_Mask3, sampler_Mask0683_g124, uvMask3683_g124), hasMask683_g124.w);
				#endif
				masks0683_g124 *= _MaskMapRemapScale0.rgba;
				masks0683_g124 += _MaskMapRemapOffset0.rgba;
				masks1683_g124 *= _MaskMapRemapScale1.rgba;
				masks1683_g124 += _MaskMapRemapOffset1.rgba;
				masks2683_g124 *= _MaskMapRemapScale2.rgba;
				masks2683_g124 += _MaskMapRemapOffset2.rgba;
				masks3683_g124 *= _MaskMapRemapScale3.rgba;
				masks3683_g124 += _MaskMapRemapOffset3.rgba;
				}
				float4 mask0633_g124 = masks0683_g124;
				float4 mask1626_g124 = masks1683_g124;
				float4 mask2627_g124 = masks2683_g124;
				float4 mask3624_g124 = masks3683_g124;
				float4 appendResult718_g124 = (float4((mask0633_g124).z , (mask1626_g124).z , (mask2627_g124).z , (mask3624_g124).z));
				float4 maskHeight716_g124 = appendResult718_g124;
				float4 temp_output_803_0_g124 = ( maskHeight716_g124 + float4( 1,1,1,1 ) );
				float2 uv_Control = IN.ase_texcoord4.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				float localSplatClip486_g124 = ( SplatWeight552_g124 );
				float SplatWeight486_g124 = SplatWeight552_g124;
				{
				#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight486_g124 == 0.0f ? -1 : 1);
				#endif
				}
				float4 SplatControl553_g124 = ( tex2DNode598_g124 / ( localSplatClip486_g124 + 0.001 ) );
				float4 break754_g124 = temp_output_803_0_g124;
				float4 break733_g124 = SplatControl553_g124;
				float4 temp_output_802_0_g124 = ( ( break754_g124.x * break733_g124.r ) >= ( break754_g124.y * break733_g124.g ) ? float4( 1,0,0,0 ) : float4( 0,1,0,0 ) );
				float4 temp_output_806_0_g124 = ( ( break754_g124.z * break733_g124.b ) >= ( break754_g124.w * break733_g124.a ) ? float4( 0,0,1,0 ) : float4( 0,0,0,1 ) );
				float4 normalizeResult814_g124 = ASESafeNormalize( ( length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_802_0_g124 ) ) >= length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_806_0_g124 ) ) ? temp_output_802_0_g124 : temp_output_806_0_g124 ) );
				float4 HeightbasedSplat739_g124 = saturate( normalizeResult814_g124 );
				float4 temp_output_579_0_g124 = HeightbasedSplat739_g124;
				float4 weightedBlendVar558_g124 = temp_output_579_0_g124;
				float4 weightedBlend558_g124 = ( weightedBlendVar558_g124.x*SAMPLE_TEXTURE2D( _Normal0, sampler_Linear_Repeat, SplatUV0689_g124 ) + weightedBlendVar558_g124.y*SAMPLE_TEXTURE2D( _Normal1, sampler_Linear_Repeat, SplatUV1692_g124 ) + weightedBlendVar558_g124.z*SAMPLE_TEXTURE2D( _Normal2, sampler_Linear_Repeat, SplatUV2695_g124 ) + weightedBlendVar558_g124.w*SAMPLE_TEXTURE2D( _Normal3, sampler_Linear_Repeat, SplatUV3698_g124 ) );
				float3 unpack556_g124 = UnpackNormalScale( weightedBlend558_g124, 0.5 );
				unpack556_g124.z = lerp( 1, unpack556_g124.z, saturate(0.5) );
				float3 temp_output_570_0_g124 = unpack556_g124;
				#ifdef _TERRAIN_INSTANCED_PERPIXEL_NORMAL
				float3 staticSwitch605_g124 = temp_output_570_0_g124;
				#else
				float3 staticSwitch605_g124 = temp_output_570_0_g124;
				#endif
				float2 uv_RockNormals1 = IN.ase_texcoord4.xy * _RockNormals1_ST.xy + _RockNormals1_ST.zw;
				float temp_output_66_0 = ( ( ( 1.0 - WorldNormal.y ) + ( SAMPLE_TEXTURE2D( _RockNormals1, sampler_Linear_Repeat, uv_RockNormals1 ).r * _RockHeight ) ) > _RockThreshold ? 0.0 : 1.0 );
				float3 lerpResult187 = lerp( unpack186 , staticSwitch605_g124 , temp_output_66_0);
				

				float3 Normal = lerpResult187;
				float Alpha = SplatWeight552_g124;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(WorldNormal);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					return half4(packedNormalWS, 0.0);
				#else
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float crossSign = (WorldTangent.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
							float3 bitangent = crossSign * cross(WorldNormal.xyz, WorldTangent.xyz);
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent.xyz, bitangent, WorldNormal.xyz));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = WorldNormal;
					#endif
					return half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
				float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			float4 CZY_SnowColor;
			TEXTURE2D(CZY_SnowTexture);
			SAMPLER(samplerCZY_SnowTexture);
			TEXTURE2D(_Splat0);
			SAMPLER(sampler_Splat0);
			float _LayerHasMask0;
			float _LayerHasMask1;
			float _LayerHasMask2;
			float _LayerHasMask3;
			SAMPLER(sampler_Linear_Repeat);
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			float4 _DiffuseRemapScale0;
			TEXTURE2D(_Splat1);
			SAMPLER(sampler_Splat1);
			float4 _DiffuseRemapScale1;
			TEXTURE2D(_Splat2);
			SAMPLER(sampler_Splat2);
			float4 _DiffuseRemapScale2;
			TEXTURE2D(_Splat3);
			SAMPLER(sampler_Splat3);
			float4 _DiffuseRemapScale3;
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);
			float CZY_SnowScale;
			float CZY_SnowAmount;
			TEXTURE2D(_RockNormals1);
			TEXTURE2D(_RockNormals);
			TEXTURE2D(_Normal0);
			TEXTURE2D(_Normal1);
			TEXTURE2D(_Normal2);
			TEXTURE2D(_Normal3);
			float CZY_PuddleScale;
			float CZY_WetnessAmount;


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"

			real4 ASESafeNormalize(float4 inVec)
			{
				real dp3 = max(FLT_MIN, dot(inVec, inVec));
				return inVec* rsqrt( dp3);
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
			
					float2 voronoihash5_g112( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi5_g112( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash5_g112( n + g );
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
			
					float2 voronoihash1_g111( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi1_g111( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash1_g111( n + g );
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
			
					float2 voronoihash8_g111( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi8_g111( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash8_g111( n + g );
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
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord8.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV(v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy);
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					o.dynamicLightmapUV.xy = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if !defined(LIGHTMAP_ON)
					OUTPUT_SH(normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz);
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );

				o.fogFactorAndVertexLight = half4(0, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

					o.clipPos = positionCS;

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					o.screenPos = ComputeScreenPos(positionCS);
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			FragmentOutput frag ( VertexOutput IN
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
					float4 ScreenPos = IN.screenPos;
				#endif

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#else
					ShadowCoords = float4(0, 0, 0, 0);
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uvCZY_SnowTexture = IN.ase_texcoord8.xy * CZY_SnowTexture_ST.xy + CZY_SnowTexture_ST.zw;
				float4 appendResult535_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
				float2 temp_cast_0 = (_Splat0Scale).xx;
				float2 texCoord687_g124 = IN.ase_texcoord8.xy * temp_cast_0 + float2( 0,0 );
				float2 SplatUV0689_g124 = texCoord687_g124;
				float4 tex2DNode572_g124 = SAMPLE_TEXTURE2D( _Splat0, sampler_Splat0, SplatUV0689_g124 );
				float localComputeMasks683_g124 = ( 0.0 );
				float4 masks0683_g124 = float4( 0,0,0,0 );
				float4 masks1683_g124 = float4( 0,0,0,0 );
				float4 masks2683_g124 = float4( 0,0,0,0 );
				float4 masks3683_g124 = float4( 0,0,0,0 );
				float4 appendResult625_g124 = (float4(_LayerHasMask0 , _LayerHasMask1 , _LayerHasMask2 , _LayerHasMask3));
				float4 layerHasMask678_g124 = appendResult625_g124;
				float4 hasMask683_g124 = layerHasMask678_g124;
				float2 uvMask0683_g124 = SplatUV0689_g124;
				float2 temp_cast_2 = (_Splat1Scale).xx;
				float2 texCoord690_g124 = IN.ase_texcoord8.xy * temp_cast_2 + float2( 0,0 );
				float2 SplatUV1692_g124 = texCoord690_g124;
				float2 uvMask1683_g124 = SplatUV1692_g124;
				float2 temp_cast_3 = (_Splat2Scale).xx;
				float2 texCoord693_g124 = IN.ase_texcoord8.xy * temp_cast_3 + float2( 0,0 );
				float2 SplatUV2695_g124 = texCoord693_g124;
				float2 uvMask2683_g124 = SplatUV2695_g124;
				float2 temp_cast_4 = (_Splat3Scale).xx;
				float2 texCoord696_g124 = IN.ase_texcoord8.xy * temp_cast_4 + float2( 0,0 );
				float2 SplatUV3698_g124 = texCoord696_g124;
				float2 uvMask3683_g124 = SplatUV3698_g124;
				SamplerState sampler_Mask0683_g124 = sampler_Linear_Repeat;
				{
				masks0683_g124 = 0.5h;
				masks1683_g124 = 0.5h;
				masks2683_g124 = 0.5h;
				masks3683_g124 = 0.5h;
				#ifdef _MASKMAP
				masks0683_g124 = lerp(masks0683_g124, SAMPLE_TEXTURE2D(_Mask0, sampler_Mask0683_g124, uvMask0683_g124), hasMask683_g124.x);
				masks1683_g124 = lerp(masks1683_g124, SAMPLE_TEXTURE2D(_Mask1, sampler_Mask0683_g124, uvMask1683_g124), hasMask683_g124.y);
				masks2683_g124 = lerp(masks2683_g124, SAMPLE_TEXTURE2D(_Mask2, sampler_Mask0683_g124, uvMask2683_g124), hasMask683_g124.z);
				masks3683_g124 = lerp(masks3683_g124, SAMPLE_TEXTURE2D(_Mask3, sampler_Mask0683_g124, uvMask3683_g124), hasMask683_g124.w);
				#endif
				masks0683_g124 *= _MaskMapRemapScale0.rgba;
				masks0683_g124 += _MaskMapRemapOffset0.rgba;
				masks1683_g124 *= _MaskMapRemapScale1.rgba;
				masks1683_g124 += _MaskMapRemapOffset1.rgba;
				masks2683_g124 *= _MaskMapRemapScale2.rgba;
				masks2683_g124 += _MaskMapRemapOffset2.rgba;
				masks3683_g124 *= _MaskMapRemapScale3.rgba;
				masks3683_g124 += _MaskMapRemapOffset3.rgba;
				}
				float4 mask0633_g124 = masks0683_g124;
				float4 mask1626_g124 = masks1683_g124;
				float4 mask2627_g124 = masks2683_g124;
				float4 mask3624_g124 = masks3683_g124;
				float4 appendResult718_g124 = (float4((mask0633_g124).z , (mask1626_g124).z , (mask2627_g124).z , (mask3624_g124).z));
				float4 maskHeight716_g124 = appendResult718_g124;
				float4 temp_output_803_0_g124 = ( maskHeight716_g124 + float4( 1,1,1,1 ) );
				float2 uv_Control = IN.ase_texcoord8.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				float localSplatClip486_g124 = ( SplatWeight552_g124 );
				float SplatWeight486_g124 = SplatWeight552_g124;
				{
				#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight486_g124 == 0.0f ? -1 : 1);
				#endif
				}
				float4 SplatControl553_g124 = ( tex2DNode598_g124 / ( localSplatClip486_g124 + 0.001 ) );
				float4 break754_g124 = temp_output_803_0_g124;
				float4 break733_g124 = SplatControl553_g124;
				float4 temp_output_802_0_g124 = ( ( break754_g124.x * break733_g124.r ) >= ( break754_g124.y * break733_g124.g ) ? float4( 1,0,0,0 ) : float4( 0,1,0,0 ) );
				float4 temp_output_806_0_g124 = ( ( break754_g124.z * break733_g124.b ) >= ( break754_g124.w * break733_g124.a ) ? float4( 0,0,1,0 ) : float4( 0,0,0,1 ) );
				float4 normalizeResult814_g124 = ASESafeNormalize( ( length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_802_0_g124 ) ) >= length( ( temp_output_803_0_g124 * SplatControl553_g124 * temp_output_806_0_g124 ) ) ? temp_output_802_0_g124 : temp_output_806_0_g124 ) );
				float4 HeightbasedSplat739_g124 = saturate( normalizeResult814_g124 );
				float4 appendResult574_g124 = (float4(( (HeightbasedSplat739_g124).xxx * (_DiffuseRemapScale0).rgb ) , 1.0));
				float4 tintLayer0526_g124 = appendResult574_g124;
				float4 appendResult540_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
				float4 tex2DNode546_g124 = SAMPLE_TEXTURE2D( _Splat1, sampler_Splat1, SplatUV1692_g124 );
				float4 appendResult591_g124 = (float4(( (HeightbasedSplat739_g124).yyy * (_DiffuseRemapScale1).rgb ) , 1.0));
				float4 tintLayer1589_g124 = appendResult591_g124;
				float4 appendResult508_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
				float4 tex2DNode509_g124 = SAMPLE_TEXTURE2D( _Splat2, sampler_Splat2, SplatUV2695_g124 );
				float4 appendResult594_g124 = (float4(( (HeightbasedSplat739_g124).zzz * (_DiffuseRemapScale2).rgb ) , 1.0));
				float4 tintLayer2592_g124 = appendResult594_g124;
				float4 appendResult520_g124 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
				float4 tex2DNode582_g124 = SAMPLE_TEXTURE2D( _Splat3, sampler_Splat3, SplatUV3698_g124 );
				float4 appendResult542_g124 = (float4(( (HeightbasedSplat739_g124).www * (_DiffuseRemapScale3).rgb ) , 1.0));
				float4 tintLayer3595_g124 = appendResult542_g124;
				float4 weightedBlendVar684_g124 = float4(1,1,1,1);
				float4 weightedBlend684_g124 = ( weightedBlendVar684_g124.x*( appendResult535_g124 * tex2DNode572_g124 * tintLayer0526_g124 ) + weightedBlendVar684_g124.y*( appendResult540_g124 * tex2DNode546_g124 * tintLayer1589_g124 ) + weightedBlendVar684_g124.z*( appendResult508_g124 * tex2DNode509_g124 * tintLayer2592_g124 ) + weightedBlendVar684_g124.w*( appendResult520_g124 * tex2DNode582_g124 * tintLayer3595_g124 ) );
				float4 MixDiffuse491_g124 = weightedBlend684_g124;
				float4 temp_output_499_0_g124 = MixDiffuse491_g124;
				float2 uv_TerrainHolesTexture = IN.ase_texcoord8.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue586_g124 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				clip( holeClipValue586_g124 - 0.5);
				float2 appendResult3_g112 = (float2(WorldPosition.x , WorldPosition.z));
				float temp_output_6_0_g112 = ( 1.0 / CZY_SnowScale );
				float simplePerlin2D7_g112 = snoise( appendResult3_g112*temp_output_6_0_g112 );
				simplePerlin2D7_g112 = simplePerlin2D7_g112*0.5 + 0.5;
				float time5_g112 = 0.0;
				float2 voronoiSmoothId5_g112 = 0;
				float2 coords5_g112 = appendResult3_g112 * ( temp_output_6_0_g112 / 0.1 );
				float2 id5_g112 = 0;
				float2 uv5_g112 = 0;
				float voroi5_g112 = voronoi5_g112( coords5_g112, time5_g112, id5_g112, uv5_g112, 0, voronoiSmoothId5_g112 );
				float4 appendResult564_g124 = (float4(_Metallic0 , _Metallic1 , _Metallic2 , _Metallic3));
				float dotResult571_g124 = dot( SplatControl553_g124 , appendResult564_g124 );
				float4 lerpResult19_g112 = lerp( ( CZY_SnowColor * SAMPLE_TEXTURE2D( CZY_SnowTexture, samplerCZY_SnowTexture, uvCZY_SnowTexture ) ) , temp_output_499_0_g124 , ( ( pow( ( pow( WorldNormal.y , 7.0 ) * ( simplePerlin2D7_g112 * ( 1.0 - voroi5_g112 ) ) ) , 0.5 ) * dotResult571_g124 ) > ( 1.0 - CZY_SnowAmount ) ? 0.0 : 1.0 ));
				float2 uv_RockNormals1 = IN.ase_texcoord8.xy * _RockNormals1_ST.xy + _RockNormals1_ST.zw;
				float temp_output_66_0 = ( ( ( 1.0 - WorldNormal.y ) + ( SAMPLE_TEXTURE2D( _RockNormals1, sampler_Linear_Repeat, uv_RockNormals1 ).r * _RockHeight ) ) > _RockThreshold ? 0.0 : 1.0 );
				float4 lerpResult67 = lerp( _RockColor , lerpResult19_g112 , temp_output_66_0);
				
				float2 uv_RockNormals = IN.ase_texcoord8.xy * _RockNormals_ST.xy + _RockNormals_ST.zw;
				float3 unpack186 = UnpackNormalScale( SAMPLE_TEXTURE2D( _RockNormals, sampler_Linear_Repeat, uv_RockNormals ), _RockNormalScale );
				unpack186.z = lerp( 1, unpack186.z, saturate(_RockNormalScale) );
				float4 temp_output_579_0_g124 = HeightbasedSplat739_g124;
				float4 weightedBlendVar558_g124 = temp_output_579_0_g124;
				float4 weightedBlend558_g124 = ( weightedBlendVar558_g124.x*SAMPLE_TEXTURE2D( _Normal0, sampler_Linear_Repeat, SplatUV0689_g124 ) + weightedBlendVar558_g124.y*SAMPLE_TEXTURE2D( _Normal1, sampler_Linear_Repeat, SplatUV1692_g124 ) + weightedBlendVar558_g124.z*SAMPLE_TEXTURE2D( _Normal2, sampler_Linear_Repeat, SplatUV2695_g124 ) + weightedBlendVar558_g124.w*SAMPLE_TEXTURE2D( _Normal3, sampler_Linear_Repeat, SplatUV3698_g124 ) );
				float3 unpack556_g124 = UnpackNormalScale( weightedBlend558_g124, 0.5 );
				unpack556_g124.z = lerp( 1, unpack556_g124.z, saturate(0.5) );
				float3 temp_output_570_0_g124 = unpack556_g124;
				#ifdef _TERRAIN_INSTANCED_PERPIXEL_NORMAL
				float3 staticSwitch605_g124 = temp_output_570_0_g124;
				#else
				float3 staticSwitch605_g124 = temp_output_570_0_g124;
				#endif
				float3 lerpResult187 = lerp( unpack186 , staticSwitch605_g124 , temp_output_66_0);
				
				float temp_output_5_0_g111 = ( 1.0 / CZY_PuddleScale );
				float time1_g111 = 0.0;
				float2 voronoiSmoothId1_g111 = 0;
				float2 appendResult3_g111 = (float2(WorldPosition.x , WorldPosition.z));
				float2 coords1_g111 = appendResult3_g111 * temp_output_5_0_g111;
				float2 id1_g111 = 0;
				float2 uv1_g111 = 0;
				float voroi1_g111 = voronoi1_g111( coords1_g111, time1_g111, id1_g111, uv1_g111, 0, voronoiSmoothId1_g111 );
				float time8_g111 = 2.16;
				float2 voronoiSmoothId8_g111 = 0;
				float2 coords8_g111 = IN.ase_texcoord8.xy * ( temp_output_5_0_g111 * 3.0 );
				float2 id8_g111 = 0;
				float2 uv8_g111 = 0;
				float voroi8_g111 = voronoi8_g111( coords8_g111, time8_g111, id8_g111, uv8_g111, 0, voronoiSmoothId8_g111 );
				float4 appendResult492_g124 = (float4(_Smoothness0 , _Smoothness1 , _Smoothness2 , _Smoothness3));
				float4 appendResult487_g124 = (float4(tex2DNode572_g124.a , tex2DNode546_g124.a , tex2DNode509_g124.a , tex2DNode582_g124.a));
				float4 defaultSmoothness480_g124 = ( appendResult492_g124 * appendResult487_g124 );
				float4 appendResult615_g124 = (float4((mask0633_g124).w , (mask1626_g124).w , (mask2627_g124).w , (mask3624_g124).w));
				float4 maskSmoothness675_g124 = appendResult615_g124;
				float4 lerpResult577_g124 = lerp( defaultSmoothness480_g124 , maskSmoothness675_g124 , layerHasMask678_g124);
				float dotResult500_g124 = dot( lerpResult577_g124 , SplatControl553_g124 );
				

				float3 BaseColor = lerpResult67.rgb;
				float3 Normal = lerpResult187;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = ( ( WorldNormal.y * 2.0 * ( (1.0 + (voroi1_g111 - 0.0) * (0.0 - 1.0) / (0.4 - 0.0)) + (0.1 + (voroi8_g111 - 0.0) * (-0.3 - 0.1) / (0.21 - 0.0)) ) * (0.3 + (CZY_WetnessAmount - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) ) > ( 1.0 - ( CZY_WetnessAmount * dotResult500_g124 ) ) ? 1.0 : 0.0 );
				float Occlusion = 1;
				float Alpha = SplatWeight552_g124;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.positionCS = IN.clipPos;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
						inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
						inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
						inputData.normalWS = Normal;
					#endif
				#else
					inputData.normalWS = WorldNormal;
				#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = SafeNormalize( WorldViewDirection );

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#else
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.dynamicLightmapUV.xy, SH, inputData.normalWS);
					#else
						inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
					#endif
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
						#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = IN.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				#ifdef _DBUFFER
					ApplyDecal(IN.clipPos,
						BaseColor,
						Specular,
						inputData.normalWS,
						Metallic,
						Occlusion,
						Smoothness);
				#endif

				BRDFData brdfData;
				InitializeBRDFData
				(BaseColor, Metallic, Specular, Smoothness, Alpha, brdfData);

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
				half4 color;
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
				color.rgb = GlobalIllumination(brdfData, inputData.bakedGI, Occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb, Occlusion);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#define SCENESELECTIONPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

			//#ifdef HAVE_VFX_MODIFICATION
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			//#endif

			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 uv_Control = IN.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				

				surfaceDescription.Alpha = SplatWeight552_g124;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 120108
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

		    #define SCENEPICKINGPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#pragma multi_compile_local __ _ALPHATEST_ON
			#pragma shader_feature_local _MASKMAP


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _RockColor;
			float4 CZY_SnowTexture_ST;
			float4 _RockNormals1_ST;
			float4 _Control_ST;
			float4 _RockNormals_ST;
			float4 _TerrainHolesTexture_ST;
			float _RockThreshold;
			float _RockHeight;
			float _Metallic3;
			float _Metallic2;
			float _Metallic1;
			float _Metallic0;
			float _Smoothness3;
			float _Smoothness1;
			float _Splat3Scale;
			float _Splat2Scale;
			float _Splat1Scale;
			float _Splat0Scale;
			float _Smoothness0;
			float _Smoothness2;
			float _RockNormalScale;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			// Property used by ScenePickingPass
			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			// Properties used by SceneSelectionPass
			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Mask1);
			SAMPLER(sampler_Mask1);
			TEXTURE2D(_Mask3);
			SAMPLER(sampler_Mask3);
			float4 _MaskMapRemapScale0;
			float4 _MaskMapRemapOffset2;
			float4 _MaskMapRemapScale2;
			float4 _MaskMapRemapScale1;
			float4 _MaskMapRemapOffset1;
			float4 _MaskMapRemapScale3;
			float4 _MaskMapRemapOffset3;
			float4 _MaskMapRemapOffset0;
			TEXTURE2D(_Mask2);
			SAMPLER(sampler_Mask2);
			TEXTURE2D(_Mask0);
			SAMPLER(sampler_Mask0);
			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);


			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

			//#ifdef HAVE_VFX_MODIFICATION
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
			//#endif

			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
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
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 uv_Control = IN.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode598_g124 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				float dotResult576_g124 = dot( tex2DNode598_g124 , float4(1,1,1,1) );
				float SplatWeight552_g124 = dotResult576_g124;
				

				surfaceDescription.Alpha = SplatWeight552_g124;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
						clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraphLitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;144;657.0565,40.11808;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;143;657.0565,40.11808;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;145;657.0565,40.11808;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;150;632.0565,282.1181;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;142;657.0565,40.11808;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;140;657.0565,40.11808;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;149;632.0565,282.1181;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;163;801.0565,302.1181;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;164;801.0565,302.1181;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;141;801.0565,222.1181;Float;False;True;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;Distant Lands/Cozy/URP/Demo/Stylized Terrain First Pass;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;19;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;41;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Forward Only;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,;0;Translucency;0;0;  Translucency Strength;1,False,;0;  Normal Distortion;0.5,False,;0;  Scattering;2,False,;0;  Direct;0.9,False,;0;  Ambient;0.1,False,;0;  Shadow;0.5,False,;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;0;638398551225475800;Override Baked GI;0;0;Extra Pre Pass;0;0;DOTS Instancing;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;Debug Display;0;638380786108109506;Clear Coat;0;0;0;10;False;True;True;True;False;True;True;True;True;True;False;;True;0
Node;AmplifyShaderEditor.LerpOp;187;-352,194.4445;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;65;-960,-509.5555;Inherit;False;Property;_RockColor;Rock Color;35;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.3490559,0.3490559,0.3490559,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;62;-1616,-253.5555;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;64;-1392,-253.5555;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-1600,82.44448;Inherit;False;Property;_RockHeight;Rock Height;34;0;Create;True;0;0;0;False;0;False;0;0.84;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;191;-1616,-109.5555;Inherit;True;Property;_RockNormals1;Rock Normals;29;0;Create;True;0;0;0;False;0;False;-1;None;9b2407256c42f5d4fbb566bd926538a1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;197;-976,-109.5555;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-1200,-45.55552;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;66;-688,-45.55552;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;67;-352,64;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;186;-960,-333.5555;Inherit;True;Property;_RockNormals;Rock Normals;28;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;6a40a13a0056f6649bcce95734364b25;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;-1312,130.4445;Inherit;False;Property;_RockThreshold;Rock Threshold;32;0;Create;True;0;0;0;False;0;False;0;0.29;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-1200,-320;Inherit;False;Property;_RockNormalScale;Rock Normal Scale;33;0;Create;True;0;0;0;False;0;False;0;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;275;560,432;Inherit;False;Constant;_Clip;Clip;8;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;159;-933.9283,338.4445;Inherit;False;StylizedRainPuddles;-1;;111;ff913d501aadc8b4cb4ec6889f2354ca;0;1;14;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-924.4044,221.8838;Inherit;False;Stylized Snow Blend;30;;112;359b695eb7172584f9df5a0d55bd52e9;0;2;34;FLOAT;1;False;22;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;285;-1836.932,240.3765;Inherit;False;Stylized Terrain Blend;0;;124;9d4ad2a2d80f7ca4d90e641385d406e3;2,501,1,568,1;7;579;FLOAT4;0,0,0,0;False;499;FLOAT4;0,0,0,0;False;570;FLOAT3;0,0,0;False;497;FLOAT;0;False;565;FLOAT;0;False;650;FLOAT;0;False;580;FLOAT;0;False;9;FLOAT;817;SAMPLERSTATE;686;FLOAT4;611;FLOAT3;612;FLOAT;607;FLOAT;609;FLOAT;677;FLOAT;610;FLOAT3;608
WireConnection;141;0;67;0
WireConnection;141;1;187;0
WireConnection;141;4;159;0
WireConnection;141;6;285;610
WireConnection;141;7;275;0
WireConnection;187;0;186;0
WireConnection;187;1;285;612
WireConnection;187;2;66;0
WireConnection;64;0;62;2
WireConnection;191;7;285;686
WireConnection;197;0;64;0
WireConnection;197;1;199;0
WireConnection;199;0;191;1
WireConnection;199;1;190;0
WireConnection;66;0;197;0
WireConnection;66;1;63;0
WireConnection;67;0;65;0
WireConnection;67;1;157;0
WireConnection;67;2;66;0
WireConnection;186;5;216;0
WireConnection;186;7;285;686
WireConnection;159;14;285;609
WireConnection;157;34;285;607
WireConnection;157;22;285;611
ASEEND*/
//CHKSM=621C83A3F5E4B14B0E221AA514AA8A38DE5CF147