// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/URP/Stylized Clouds (Luxury)"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		CZY_LuxuryVariationTexture("CZY_LuxuryVariationTexture", 2D) = "white" {}


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
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			sampler2D CZY_LuxuryVariationTexture;
			float CZY_CirrostratusMultiplier;
			sampler2D CZY_AltocumulusTexture;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_PartlyCloudyTexture;
			float CZY_CumulusCoverageMultiplier;
			sampler2D CZY_MostlyCloudyTexture;
			sampler2D CZY_OvercastTexture;
			sampler2D CZY_LowNimbusTexture;
			float CZY_NimbusMultiplier;
			sampler2D CZY_MidNimbusTexture;
			sampler2D CZY_HighNimbusTexture;
			sampler2D CZY_LowBorderTexture;
			float CZY_BorderHeight;
			sampler2D CZY_HighBorderTexture;
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
			
			float HLSL20_g118( bool enabled, bool submerged, float textureSample )
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

				float3 hsvTorgb2_g116 = RGBToHSV( CZY_CloudColor.rgb );
				float3 hsvTorgb3_g116 = HSVToRGB( float3(hsvTorgb2_g116.x,saturate( ( hsvTorgb2_g116.y + CZY_FilterSaturation ) ),( hsvTorgb2_g116.z + CZY_FilterValue )) );
				float4 temp_output_10_0_g116 = ( float4( hsvTorgb3_g116 , 0.0 ) * CZY_FilterColor );
				float4 CloudColor13_g115 = ( temp_output_10_0_g116 * CZY_CloudFilterColor );
				float2 texCoord5_g115 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos6_g115 = texCoord5_g115;
				float mulTime104_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D116_g115 = snoise( (Pos6_g115*1.0 + mulTime104_g115)*2.0 );
				float mulTime102_g115 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos110_g115 = cos( ( mulTime102_g115 * 0.01 ) );
				float sin110_g115 = sin( ( mulTime102_g115 * 0.01 ) );
				float2 rotator110_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos110_g115 , -sin110_g115 , sin110_g115 , cos110_g115 )) + float2( 0.5,0.5 );
				float cos112_g115 = cos( ( mulTime102_g115 * -0.02 ) );
				float sin112_g115 = sin( ( mulTime102_g115 * -0.02 ) );
				float2 rotator112_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos112_g115 , -sin112_g115 , sin112_g115 , cos112_g115 )) + float2( 0.5,0.5 );
				float mulTime118_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D115_g115 = snoise( (Pos6_g115*1.0 + mulTime118_g115) );
				simplePerlin2D115_g115 = simplePerlin2D115_g115*0.5 + 0.5;
				float4 CirrostratusPattern134_g115 = ( ( saturate( simplePerlin2D116_g115 ) * tex2D( CZY_CirrostratusTexture, (rotator110_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator112_g115*1.0 + 0.0) ) * saturate( simplePerlin2D115_g115 ) ) );
				float2 texCoord117_g115 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_123_0_g115 = ( texCoord117_g115 - float2( 0.5,0.5 ) );
				float dotResult120_g115 = dot( temp_output_123_0_g115 , temp_output_123_0_g115 );
				float2 texCoord23_g121 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g121 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g121 = ( texCoord2_g121 - float2( 0.5,0.5 ) );
				float dotResult3_g121 = dot( temp_output_4_0_g121 , temp_output_4_0_g121 );
				float temp_output_14_0_g121 = ( CZY_CirrostratusMultiplier * 0.5 );
				float3 appendResult131_g115 = (float3(( CirrostratusPattern134_g115 * saturate( (0.0 + (dotResult120_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g121*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g121*48.2 + (-15.0 + (temp_output_14_0_g121 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g121 ) ) ).rgb));
				float temp_output_130_0_g115 = length( appendResult131_g115 );
				float CirrostratusColoring135_g115 = temp_output_130_0_g115;
				float CirrostratusAlpha136_g115 = temp_output_130_0_g115;
				float lerpResult260_g115 = lerp( 1.0 , CirrostratusColoring135_g115 , CirrostratusAlpha136_g115);
				float mulTime212_g115 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme213_g115 = mulTime212_g115;
				float2 cloudPosition211_g115 = (Pos6_g115*( 18.0 / CZY_MainCloudScale ) + ( TIme213_g115 * float2( 0.2,-0.4 ) ));
				float4 tex2DNode315_g115 = tex2D( CZY_AltocumulusTexture, cloudPosition211_g115 );
				float altocumulusColor216_g115 = tex2DNode315_g115.r;
				float2 texCoord23_g127 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g127 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g127 = ( texCoord2_g127 - float2( 0.5,0.5 ) );
				float dotResult3_g127 = dot( temp_output_4_0_g127 , temp_output_4_0_g127 );
				float temp_output_14_0_g127 = CZY_AltocumulusMultiplier;
				float altocumulusAlpha217_g115 = tex2DNode315_g115.a;
				float temp_output_253_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g127*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g127*48.2 + (-15.0 + (temp_output_14_0_g127 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g127 ) ) * altocumulusAlpha217_g115 );
				float lerpResult252_g115 = lerp( 1.0 , altocumulusColor216_g115 , temp_output_253_0_g115);
				float finalAcColor288_g115 = lerpResult252_g115;
				float finalAcAlpha286_g115 = temp_output_253_0_g115;
				float lerpResult284_g115 = lerp( lerpResult260_g115 , finalAcColor288_g115 , finalAcAlpha286_g115);
				float mulTime64_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D76_g115 = snoise( (Pos6_g115*1.0 + mulTime64_g115)*2.0 );
				float mulTime62_g115 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos70_g115 = cos( ( mulTime62_g115 * 0.01 ) );
				float sin70_g115 = sin( ( mulTime62_g115 * 0.01 ) );
				float2 rotator70_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos70_g115 , -sin70_g115 , sin70_g115 , cos70_g115 )) + float2( 0.5,0.5 );
				float cos72_g115 = cos( ( mulTime62_g115 * -0.02 ) );
				float sin72_g115 = sin( ( mulTime62_g115 * -0.02 ) );
				float2 rotator72_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos72_g115 , -sin72_g115 , sin72_g115 , cos72_g115 )) + float2( 0.5,0.5 );
				float mulTime78_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D75_g115 = snoise( (Pos6_g115*1.0 + mulTime78_g115) );
				simplePerlin2D75_g115 = simplePerlin2D75_g115*0.5 + 0.5;
				float4 CirrusPattern79_g115 = ( ( saturate( simplePerlin2D76_g115 ) * tex2D( CZY_CirrusTexture, (rotator70_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator72_g115*1.0 + 0.0) ) * saturate( simplePerlin2D75_g115 ) ) );
				float2 texCoord77_g115 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_84_0_g115 = ( texCoord77_g115 - float2( 0.5,0.5 ) );
				float dotResult81_g115 = dot( temp_output_84_0_g115 , temp_output_84_0_g115 );
				float2 texCoord23_g120 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g120 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g120 = ( texCoord2_g120 - float2( 0.5,0.5 ) );
				float dotResult3_g120 = dot( temp_output_4_0_g120 , temp_output_4_0_g120 );
				float temp_output_14_0_g120 = ( CZY_CirrusMultiplier * 0.5 );
				float3 appendResult95_g115 = (float3(( CirrusPattern79_g115 * saturate( (0.0 + (dotResult81_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g120*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g120*48.2 + (-15.0 + (temp_output_14_0_g120 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g120 ) ) ).rgb));
				float temp_output_94_0_g115 = length( appendResult95_g115 );
				float CirrusColoring96_g115 = temp_output_94_0_g115;
				float CirrusAlpha97_g115 = temp_output_94_0_g115;
				float lerpResult283_g115 = lerp( lerpResult284_g115 , CirrusColoring96_g115 , CirrusAlpha97_g115);
				float mulTime33_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D40_g115 = snoise( (Pos6_g115*1.0 + mulTime33_g115)*2.0 );
				float mulTime31_g115 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos32_g115 = cos( ( mulTime31_g115 * 0.01 ) );
				float sin32_g115 = sin( ( mulTime31_g115 * 0.01 ) );
				float2 rotator32_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos32_g115 , -sin32_g115 , sin32_g115 , cos32_g115 )) + float2( 0.5,0.5 );
				float cos39_g115 = cos( ( mulTime31_g115 * -0.02 ) );
				float sin39_g115 = sin( ( mulTime31_g115 * -0.02 ) );
				float2 rotator39_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos39_g115 , -sin39_g115 , sin39_g115 , cos39_g115 )) + float2( 0.5,0.5 );
				float mulTime29_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D41_g115 = snoise( (Pos6_g115*1.0 + mulTime29_g115)*4.0 );
				float4 ChemtrailsPattern46_g115 = ( ( saturate( simplePerlin2D40_g115 ) * tex2D( CZY_ChemtrailsTexture, (rotator32_g115*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator39_g115 ) * saturate( simplePerlin2D41_g115 ) ) );
				float2 texCoord23_g115 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_24_0_g115 = ( texCoord23_g115 - float2( 0.5,0.5 ) );
				float dotResult26_g115 = dot( temp_output_24_0_g115 , temp_output_24_0_g115 );
				float2 texCoord23_g119 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g119 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g119 = ( texCoord2_g119 - float2( 0.5,0.5 ) );
				float dotResult3_g119 = dot( temp_output_4_0_g119 , temp_output_4_0_g119 );
				float temp_output_14_0_g119 = ( CZY_ChemtrailsMultiplier * 0.5 );
				float3 appendResult58_g115 = (float3(( ChemtrailsPattern46_g115 * saturate( (0.4 + (dotResult26_g115 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g119*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g119*48.2 + (-15.0 + (temp_output_14_0_g119 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g119 ) ) ).rgb));
				float temp_output_59_0_g115 = length( appendResult58_g115 );
				float ChemtrailsColoring60_g115 = temp_output_59_0_g115;
				float ChemtrailsAlpha61_g115 = temp_output_59_0_g115;
				float lerpResult265_g115 = lerp( lerpResult283_g115 , ChemtrailsColoring60_g115 , ChemtrailsAlpha61_g115);
				float4 tex2DNode319_g115 = tex2D( CZY_PartlyCloudyTexture, cloudPosition211_g115 );
				float PartlyCloudyColor220_g115 = tex2DNode319_g115.r;
				float2 texCoord23_g126 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g126 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g126 = ( texCoord2_g126 - float2( 0.5,0.5 ) );
				float dotResult3_g126 = dot( temp_output_4_0_g126 , temp_output_4_0_g126 );
				float temp_output_294_0_g115 = ( CZY_CumulusCoverageMultiplier * 1.0 );
				float temp_output_14_0_g126 = saturate( (0.0 + (min( temp_output_294_0_g115 , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float PartlyCloudyAlpha219_g115 = tex2DNode319_g115.a;
				float temp_output_249_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g126*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g126*48.2 + (-15.0 + (temp_output_14_0_g126 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g126 ) ) * PartlyCloudyAlpha219_g115 );
				float lerpResult242_g115 = lerp( 1.0 , PartlyCloudyColor220_g115 , temp_output_249_0_g115);
				float4 tex2DNode308_g115 = tex2D( CZY_MostlyCloudyTexture, cloudPosition211_g115 );
				float MostlyCloudyColor222_g115 = tex2DNode308_g115.r;
				float2 texCoord23_g125 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g125 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g125 = ( texCoord2_g125 - float2( 0.5,0.5 ) );
				float dotResult3_g125 = dot( temp_output_4_0_g125 , temp_output_4_0_g125 );
				float temp_output_14_0_g125 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.3 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float MostlyCloudyAlpha221_g115 = tex2DNode308_g115.a;
				float temp_output_226_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g125*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g125*48.2 + (-15.0 + (temp_output_14_0_g125 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g125 ) ) * MostlyCloudyAlpha221_g115 );
				float lerpResult243_g115 = lerp( lerpResult242_g115 , MostlyCloudyColor222_g115 , temp_output_226_0_g115);
				float4 tex2DNode309_g115 = tex2D( CZY_OvercastTexture, cloudPosition211_g115 );
				float OvercastCloudyColoring223_g115 = tex2DNode309_g115.r;
				float2 texCoord23_g129 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g129 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g129 = ( texCoord2_g129 - float2( 0.5,0.5 ) );
				float dotResult3_g129 = dot( temp_output_4_0_g129 , temp_output_4_0_g129 );
				float temp_output_14_0_g129 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.7 ) , 0.35 ) - 0.0) * (1.0 - 0.0) / (0.35 - 0.0)) );
				float OvercastCloudyAlpha224_g115 = tex2DNode309_g115.a;
				float temp_output_231_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g129*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g129*48.2 + (-15.0 + (temp_output_14_0_g129 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g129 ) ) * OvercastCloudyAlpha224_g115 );
				float lerpResult239_g115 = lerp( lerpResult243_g115 , OvercastCloudyColoring223_g115 , temp_output_231_0_g115);
				float cumulusCloudColor237_g115 = saturate( lerpResult239_g115 );
				float cumulusAlpha232_g115 = saturate( ( temp_output_249_0_g115 + temp_output_226_0_g115 + temp_output_231_0_g115 ) );
				float lerpResult269_g115 = lerp( lerpResult265_g115 , cumulusCloudColor237_g115 , cumulusAlpha232_g115);
				float mulTime202_g115 = _TimeParameters.x * 0.005;
				float cos201_g115 = cos( mulTime202_g115 );
				float sin201_g115 = sin( mulTime202_g115 );
				float2 rotator201_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos201_g115 , -sin201_g115 , sin201_g115 , cos201_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode312_g115 = tex2D( CZY_LowNimbusTexture, rotator201_g115 );
				float lowNimbusColor196_g115 = tex2DNode312_g115.r;
				float2 texCoord23_g124 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g124 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g124 = ( texCoord2_g124 - float2( 0.5,0.5 ) );
				float dotResult3_g124 = dot( temp_output_4_0_g124 , temp_output_4_0_g124 );
				float temp_output_188_0_g115 = ( CZY_NimbusMultiplier * 0.5 );
				float temp_output_14_0_g124 = saturate( (0.0 + (min( temp_output_188_0_g115 , 2.0 ) - 0.0) * (1.0 - 0.0) / (2.0 - 0.0)) );
				float lowNimbusAlpha197_g115 = tex2DNode312_g115.a;
				float temp_output_166_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g124*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g124*48.2 + (-15.0 + (temp_output_14_0_g124 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g124 ) ) * lowNimbusAlpha197_g115 );
				float lerpResult173_g115 = lerp( 1.0 , lowNimbusColor196_g115 , temp_output_166_0_g115);
				float4 tex2DNode313_g115 = tex2D( CZY_MidNimbusTexture, rotator201_g115 );
				float mediumNimbusColor198_g115 = tex2DNode313_g115.r;
				float2 texCoord23_g130 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g130 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g130 = ( texCoord2_g130 - float2( 0.5,0.5 ) );
				float dotResult3_g130 = dot( temp_output_4_0_g130 , temp_output_4_0_g130 );
				float temp_output_14_0_g130 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.2 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float mediumNimbusAlpha199_g115 = tex2DNode313_g115.a;
				float temp_output_164_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g130*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g130*48.2 + (-15.0 + (temp_output_14_0_g130 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g130 ) ) * mediumNimbusAlpha199_g115 );
				float lerpResult175_g115 = lerp( lerpResult173_g115 , mediumNimbusColor198_g115 , temp_output_164_0_g115);
				float4 tex2DNode314_g115 = tex2D( CZY_HighNimbusTexture, rotator201_g115 );
				float highNimbusColor204_g115 = tex2DNode314_g115.r;
				float2 texCoord23_g123 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g123 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g123 = ( texCoord2_g123 - float2( 0.5,0.5 ) );
				float dotResult3_g123 = dot( temp_output_4_0_g123 , temp_output_4_0_g123 );
				float temp_output_14_0_g123 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.7 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float HighNimbusAlpha205_g115 = tex2DNode314_g115.a;
				float temp_output_162_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g123*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g123*48.2 + (-15.0 + (temp_output_14_0_g123 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g123 ) ) * HighNimbusAlpha205_g115 );
				float lerpResult177_g115 = lerp( lerpResult175_g115 , highNimbusColor204_g115 , temp_output_162_0_g115);
				float nimbusColoring180_g115 = saturate( lerpResult177_g115 );
				float nimbusAlpha172_g115 = saturate( ( temp_output_166_0_g115 + temp_output_164_0_g115 + temp_output_162_0_g115 ) );
				float lerpResult274_g115 = lerp( lerpResult269_g115 , nimbusColoring180_g115 , nimbusAlpha172_g115);
				float mulTime195_g115 = _TimeParameters.x * 0.005;
				float cos194_g115 = cos( mulTime195_g115 );
				float sin194_g115 = sin( mulTime195_g115 );
				float2 rotator194_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos194_g115 , -sin194_g115 , sin194_g115 , cos194_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode311_g115 = tex2D( CZY_LowBorderTexture, rotator194_g115 );
				float MediumBorderColor189_g115 = tex2DNode311_g115.r;
				float2 texCoord23_g122 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g122 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g122 = ( texCoord2_g122 - float2( 0.5,0.5 ) );
				float dotResult3_g122 = dot( temp_output_4_0_g122 , temp_output_4_0_g122 );
				float temp_output_14_0_g122 = saturate( (0.0 + (min( CZY_BorderHeight , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float MediumBorderAlpha190_g115 = tex2DNode311_g115.a;
				float temp_output_140_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g122*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g122*48.2 + (-15.0 + (temp_output_14_0_g122 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g122 ) ) * MediumBorderAlpha190_g115 );
				float lerpResult142_g115 = lerp( 1.0 , MediumBorderColor189_g115 , temp_output_140_0_g115);
				float4 tex2DNode310_g115 = tex2D( CZY_HighBorderTexture, rotator194_g115 );
				float HighBorderColoring191_g115 = tex2DNode310_g115.r;
				float2 texCoord23_g128 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g128 = input.ase_texcoord6.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g128 = ( texCoord2_g128 - float2( 0.5,0.5 ) );
				float dotResult3_g128 = dot( temp_output_4_0_g128 , temp_output_4_0_g128 );
				float temp_output_14_0_g128 = saturate( (0.0 + (min( ( CZY_BorderHeight - 0.5 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float HighBorderAlpha192_g115 = tex2DNode310_g115.a;
				float temp_output_159_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g128*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g128*48.2 + (-15.0 + (temp_output_14_0_g128 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g128 ) ) * HighBorderAlpha192_g115 );
				float lerpResult141_g115 = lerp( lerpResult142_g115 , HighBorderColoring191_g115 , temp_output_159_0_g115);
				float borderCloudsColor257_g115 = saturate( lerpResult141_g115 );
				float borderAlpha169_g115 = saturate( ( temp_output_140_0_g115 + temp_output_159_0_g115 ) );
				float lerpResult272_g115 = lerp( lerpResult274_g115 , borderCloudsColor257_g115 , borderAlpha169_g115);
				float cloudColoring270_g115 = lerpResult272_g115;
				float4 lerpResult12_g115 = lerp( float4( 0,0,0,0 ) , CloudColor13_g115 , cloudColoring270_g115);
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
				float4 lerpResult90_g369 = lerp( lerpResult12_g115 , ( ( temp_output_10_0_g371 * CZY_SunFilterColor ) + temp_output_10_0_g370 ) , temp_output_34_0_g369);
				
				float cloudAlpha278_g115 = ( borderAlpha169_g115 + nimbusAlpha172_g115 + cumulusAlpha232_g115 + ChemtrailsAlpha61_g115 + CirrusAlpha97_g115 + finalAcAlpha286_g115 + CirrostratusAlpha136_g115 );
				bool enabled20_g118 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g118 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g118 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g118 = HLSL20_g118( enabled20_g118 , submerged20_g118 , textureSample20_g118 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = lerpResult90_g369.rgb;
				float Alpha = ( saturate( ( cloudAlpha278_g115 + ( 0.0 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g118 ) );
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

			sampler2D CZY_LuxuryVariationTexture;
			float CZY_BorderHeight;
			sampler2D CZY_LowBorderTexture;
			sampler2D CZY_HighBorderTexture;
			float CZY_NimbusMultiplier;
			sampler2D CZY_LowNimbusTexture;
			sampler2D CZY_MidNimbusTexture;
			sampler2D CZY_HighNimbusTexture;
			float CZY_CumulusCoverageMultiplier;
			sampler2D CZY_PartlyCloudyTexture;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			sampler2D CZY_MostlyCloudyTexture;
			sampler2D CZY_OvercastTexture;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_AltocumulusTexture;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


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
			
			float HLSL20_g118( bool enabled, bool submerged, float textureSample )
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

				float2 texCoord23_g122 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g122 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g122 = ( texCoord2_g122 - float2( 0.5,0.5 ) );
				float dotResult3_g122 = dot( temp_output_4_0_g122 , temp_output_4_0_g122 );
				float temp_output_14_0_g122 = saturate( (0.0 + (min( CZY_BorderHeight , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float2 texCoord5_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos6_g115 = texCoord5_g115;
				float mulTime195_g115 = _TimeParameters.x * 0.005;
				float cos194_g115 = cos( mulTime195_g115 );
				float sin194_g115 = sin( mulTime195_g115 );
				float2 rotator194_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos194_g115 , -sin194_g115 , sin194_g115 , cos194_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode311_g115 = tex2D( CZY_LowBorderTexture, rotator194_g115 );
				float MediumBorderAlpha190_g115 = tex2DNode311_g115.a;
				float temp_output_140_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g122*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g122*48.2 + (-15.0 + (temp_output_14_0_g122 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g122 ) ) * MediumBorderAlpha190_g115 );
				float2 texCoord23_g128 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g128 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g128 = ( texCoord2_g128 - float2( 0.5,0.5 ) );
				float dotResult3_g128 = dot( temp_output_4_0_g128 , temp_output_4_0_g128 );
				float temp_output_14_0_g128 = saturate( (0.0 + (min( ( CZY_BorderHeight - 0.5 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode310_g115 = tex2D( CZY_HighBorderTexture, rotator194_g115 );
				float HighBorderAlpha192_g115 = tex2DNode310_g115.a;
				float temp_output_159_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g128*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g128*48.2 + (-15.0 + (temp_output_14_0_g128 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g128 ) ) * HighBorderAlpha192_g115 );
				float borderAlpha169_g115 = saturate( ( temp_output_140_0_g115 + temp_output_159_0_g115 ) );
				float2 texCoord23_g124 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g124 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g124 = ( texCoord2_g124 - float2( 0.5,0.5 ) );
				float dotResult3_g124 = dot( temp_output_4_0_g124 , temp_output_4_0_g124 );
				float temp_output_188_0_g115 = ( CZY_NimbusMultiplier * 0.5 );
				float temp_output_14_0_g124 = saturate( (0.0 + (min( temp_output_188_0_g115 , 2.0 ) - 0.0) * (1.0 - 0.0) / (2.0 - 0.0)) );
				float mulTime202_g115 = _TimeParameters.x * 0.005;
				float cos201_g115 = cos( mulTime202_g115 );
				float sin201_g115 = sin( mulTime202_g115 );
				float2 rotator201_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos201_g115 , -sin201_g115 , sin201_g115 , cos201_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode312_g115 = tex2D( CZY_LowNimbusTexture, rotator201_g115 );
				float lowNimbusAlpha197_g115 = tex2DNode312_g115.a;
				float temp_output_166_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g124*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g124*48.2 + (-15.0 + (temp_output_14_0_g124 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g124 ) ) * lowNimbusAlpha197_g115 );
				float2 texCoord23_g130 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g130 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g130 = ( texCoord2_g130 - float2( 0.5,0.5 ) );
				float dotResult3_g130 = dot( temp_output_4_0_g130 , temp_output_4_0_g130 );
				float temp_output_14_0_g130 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.2 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode313_g115 = tex2D( CZY_MidNimbusTexture, rotator201_g115 );
				float mediumNimbusAlpha199_g115 = tex2DNode313_g115.a;
				float temp_output_164_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g130*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g130*48.2 + (-15.0 + (temp_output_14_0_g130 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g130 ) ) * mediumNimbusAlpha199_g115 );
				float2 texCoord23_g123 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g123 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g123 = ( texCoord2_g123 - float2( 0.5,0.5 ) );
				float dotResult3_g123 = dot( temp_output_4_0_g123 , temp_output_4_0_g123 );
				float temp_output_14_0_g123 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.7 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode314_g115 = tex2D( CZY_HighNimbusTexture, rotator201_g115 );
				float HighNimbusAlpha205_g115 = tex2DNode314_g115.a;
				float temp_output_162_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g123*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g123*48.2 + (-15.0 + (temp_output_14_0_g123 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g123 ) ) * HighNimbusAlpha205_g115 );
				float nimbusAlpha172_g115 = saturate( ( temp_output_166_0_g115 + temp_output_164_0_g115 + temp_output_162_0_g115 ) );
				float2 texCoord23_g126 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g126 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g126 = ( texCoord2_g126 - float2( 0.5,0.5 ) );
				float dotResult3_g126 = dot( temp_output_4_0_g126 , temp_output_4_0_g126 );
				float temp_output_294_0_g115 = ( CZY_CumulusCoverageMultiplier * 1.0 );
				float temp_output_14_0_g126 = saturate( (0.0 + (min( temp_output_294_0_g115 , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float mulTime212_g115 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme213_g115 = mulTime212_g115;
				float2 cloudPosition211_g115 = (Pos6_g115*( 18.0 / CZY_MainCloudScale ) + ( TIme213_g115 * float2( 0.2,-0.4 ) ));
				float4 tex2DNode319_g115 = tex2D( CZY_PartlyCloudyTexture, cloudPosition211_g115 );
				float PartlyCloudyAlpha219_g115 = tex2DNode319_g115.a;
				float temp_output_249_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g126*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g126*48.2 + (-15.0 + (temp_output_14_0_g126 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g126 ) ) * PartlyCloudyAlpha219_g115 );
				float2 texCoord23_g125 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g125 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g125 = ( texCoord2_g125 - float2( 0.5,0.5 ) );
				float dotResult3_g125 = dot( temp_output_4_0_g125 , temp_output_4_0_g125 );
				float temp_output_14_0_g125 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.3 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode308_g115 = tex2D( CZY_MostlyCloudyTexture, cloudPosition211_g115 );
				float MostlyCloudyAlpha221_g115 = tex2DNode308_g115.a;
				float temp_output_226_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g125*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g125*48.2 + (-15.0 + (temp_output_14_0_g125 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g125 ) ) * MostlyCloudyAlpha221_g115 );
				float2 texCoord23_g129 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g129 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g129 = ( texCoord2_g129 - float2( 0.5,0.5 ) );
				float dotResult3_g129 = dot( temp_output_4_0_g129 , temp_output_4_0_g129 );
				float temp_output_14_0_g129 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.7 ) , 0.35 ) - 0.0) * (1.0 - 0.0) / (0.35 - 0.0)) );
				float4 tex2DNode309_g115 = tex2D( CZY_OvercastTexture, cloudPosition211_g115 );
				float OvercastCloudyAlpha224_g115 = tex2DNode309_g115.a;
				float temp_output_231_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g129*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g129*48.2 + (-15.0 + (temp_output_14_0_g129 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g129 ) ) * OvercastCloudyAlpha224_g115 );
				float cumulusAlpha232_g115 = saturate( ( temp_output_249_0_g115 + temp_output_226_0_g115 + temp_output_231_0_g115 ) );
				float mulTime33_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D40_g115 = snoise( (Pos6_g115*1.0 + mulTime33_g115)*2.0 );
				float mulTime31_g115 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos32_g115 = cos( ( mulTime31_g115 * 0.01 ) );
				float sin32_g115 = sin( ( mulTime31_g115 * 0.01 ) );
				float2 rotator32_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos32_g115 , -sin32_g115 , sin32_g115 , cos32_g115 )) + float2( 0.5,0.5 );
				float cos39_g115 = cos( ( mulTime31_g115 * -0.02 ) );
				float sin39_g115 = sin( ( mulTime31_g115 * -0.02 ) );
				float2 rotator39_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos39_g115 , -sin39_g115 , sin39_g115 , cos39_g115 )) + float2( 0.5,0.5 );
				float mulTime29_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D41_g115 = snoise( (Pos6_g115*1.0 + mulTime29_g115)*4.0 );
				float4 ChemtrailsPattern46_g115 = ( ( saturate( simplePerlin2D40_g115 ) * tex2D( CZY_ChemtrailsTexture, (rotator32_g115*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator39_g115 ) * saturate( simplePerlin2D41_g115 ) ) );
				float2 texCoord23_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_24_0_g115 = ( texCoord23_g115 - float2( 0.5,0.5 ) );
				float dotResult26_g115 = dot( temp_output_24_0_g115 , temp_output_24_0_g115 );
				float2 texCoord23_g119 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g119 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g119 = ( texCoord2_g119 - float2( 0.5,0.5 ) );
				float dotResult3_g119 = dot( temp_output_4_0_g119 , temp_output_4_0_g119 );
				float temp_output_14_0_g119 = ( CZY_ChemtrailsMultiplier * 0.5 );
				float3 appendResult58_g115 = (float3(( ChemtrailsPattern46_g115 * saturate( (0.4 + (dotResult26_g115 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g119*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g119*48.2 + (-15.0 + (temp_output_14_0_g119 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g119 ) ) ).rgb));
				float temp_output_59_0_g115 = length( appendResult58_g115 );
				float ChemtrailsAlpha61_g115 = temp_output_59_0_g115;
				float mulTime64_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D76_g115 = snoise( (Pos6_g115*1.0 + mulTime64_g115)*2.0 );
				float mulTime62_g115 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos70_g115 = cos( ( mulTime62_g115 * 0.01 ) );
				float sin70_g115 = sin( ( mulTime62_g115 * 0.01 ) );
				float2 rotator70_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos70_g115 , -sin70_g115 , sin70_g115 , cos70_g115 )) + float2( 0.5,0.5 );
				float cos72_g115 = cos( ( mulTime62_g115 * -0.02 ) );
				float sin72_g115 = sin( ( mulTime62_g115 * -0.02 ) );
				float2 rotator72_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos72_g115 , -sin72_g115 , sin72_g115 , cos72_g115 )) + float2( 0.5,0.5 );
				float mulTime78_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D75_g115 = snoise( (Pos6_g115*1.0 + mulTime78_g115) );
				simplePerlin2D75_g115 = simplePerlin2D75_g115*0.5 + 0.5;
				float4 CirrusPattern79_g115 = ( ( saturate( simplePerlin2D76_g115 ) * tex2D( CZY_CirrusTexture, (rotator70_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator72_g115*1.0 + 0.0) ) * saturate( simplePerlin2D75_g115 ) ) );
				float2 texCoord77_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_84_0_g115 = ( texCoord77_g115 - float2( 0.5,0.5 ) );
				float dotResult81_g115 = dot( temp_output_84_0_g115 , temp_output_84_0_g115 );
				float2 texCoord23_g120 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g120 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g120 = ( texCoord2_g120 - float2( 0.5,0.5 ) );
				float dotResult3_g120 = dot( temp_output_4_0_g120 , temp_output_4_0_g120 );
				float temp_output_14_0_g120 = ( CZY_CirrusMultiplier * 0.5 );
				float3 appendResult95_g115 = (float3(( CirrusPattern79_g115 * saturate( (0.0 + (dotResult81_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g120*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g120*48.2 + (-15.0 + (temp_output_14_0_g120 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g120 ) ) ).rgb));
				float temp_output_94_0_g115 = length( appendResult95_g115 );
				float CirrusAlpha97_g115 = temp_output_94_0_g115;
				float2 texCoord23_g127 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g127 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g127 = ( texCoord2_g127 - float2( 0.5,0.5 ) );
				float dotResult3_g127 = dot( temp_output_4_0_g127 , temp_output_4_0_g127 );
				float temp_output_14_0_g127 = CZY_AltocumulusMultiplier;
				float4 tex2DNode315_g115 = tex2D( CZY_AltocumulusTexture, cloudPosition211_g115 );
				float altocumulusAlpha217_g115 = tex2DNode315_g115.a;
				float temp_output_253_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g127*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g127*48.2 + (-15.0 + (temp_output_14_0_g127 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g127 ) ) * altocumulusAlpha217_g115 );
				float finalAcAlpha286_g115 = temp_output_253_0_g115;
				float mulTime104_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D116_g115 = snoise( (Pos6_g115*1.0 + mulTime104_g115)*2.0 );
				float mulTime102_g115 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos110_g115 = cos( ( mulTime102_g115 * 0.01 ) );
				float sin110_g115 = sin( ( mulTime102_g115 * 0.01 ) );
				float2 rotator110_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos110_g115 , -sin110_g115 , sin110_g115 , cos110_g115 )) + float2( 0.5,0.5 );
				float cos112_g115 = cos( ( mulTime102_g115 * -0.02 ) );
				float sin112_g115 = sin( ( mulTime102_g115 * -0.02 ) );
				float2 rotator112_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos112_g115 , -sin112_g115 , sin112_g115 , cos112_g115 )) + float2( 0.5,0.5 );
				float mulTime118_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D115_g115 = snoise( (Pos6_g115*1.0 + mulTime118_g115) );
				simplePerlin2D115_g115 = simplePerlin2D115_g115*0.5 + 0.5;
				float4 CirrostratusPattern134_g115 = ( ( saturate( simplePerlin2D116_g115 ) * tex2D( CZY_CirrostratusTexture, (rotator110_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator112_g115*1.0 + 0.0) ) * saturate( simplePerlin2D115_g115 ) ) );
				float2 texCoord117_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_123_0_g115 = ( texCoord117_g115 - float2( 0.5,0.5 ) );
				float dotResult120_g115 = dot( temp_output_123_0_g115 , temp_output_123_0_g115 );
				float2 texCoord23_g121 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g121 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g121 = ( texCoord2_g121 - float2( 0.5,0.5 ) );
				float dotResult3_g121 = dot( temp_output_4_0_g121 , temp_output_4_0_g121 );
				float temp_output_14_0_g121 = ( CZY_CirrostratusMultiplier * 0.5 );
				float3 appendResult131_g115 = (float3(( CirrostratusPattern134_g115 * saturate( (0.0 + (dotResult120_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g121*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g121*48.2 + (-15.0 + (temp_output_14_0_g121 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g121 ) ) ).rgb));
				float temp_output_130_0_g115 = length( appendResult131_g115 );
				float CirrostratusAlpha136_g115 = temp_output_130_0_g115;
				float cloudAlpha278_g115 = ( borderAlpha169_g115 + nimbusAlpha172_g115 + cumulusAlpha232_g115 + ChemtrailsAlpha61_g115 + CirrusAlpha97_g115 + finalAcAlpha286_g115 + CirrostratusAlpha136_g115 );
				bool enabled20_g118 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g118 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g118 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g118 = HLSL20_g118( enabled20_g118 , submerged20_g118 , textureSample20_g118 );
				

				float Alpha = ( saturate( ( cloudAlpha278_g115 + ( 0.0 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g118 ) );
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

			sampler2D CZY_LuxuryVariationTexture;
			float CZY_BorderHeight;
			sampler2D CZY_LowBorderTexture;
			sampler2D CZY_HighBorderTexture;
			float CZY_NimbusMultiplier;
			sampler2D CZY_LowNimbusTexture;
			sampler2D CZY_MidNimbusTexture;
			sampler2D CZY_HighNimbusTexture;
			float CZY_CumulusCoverageMultiplier;
			sampler2D CZY_PartlyCloudyTexture;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			sampler2D CZY_MostlyCloudyTexture;
			sampler2D CZY_OvercastTexture;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_AltocumulusTexture;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


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
			
			float HLSL20_g118( bool enabled, bool submerged, float textureSample )
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

				float2 texCoord23_g122 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g122 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g122 = ( texCoord2_g122 - float2( 0.5,0.5 ) );
				float dotResult3_g122 = dot( temp_output_4_0_g122 , temp_output_4_0_g122 );
				float temp_output_14_0_g122 = saturate( (0.0 + (min( CZY_BorderHeight , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float2 texCoord5_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos6_g115 = texCoord5_g115;
				float mulTime195_g115 = _TimeParameters.x * 0.005;
				float cos194_g115 = cos( mulTime195_g115 );
				float sin194_g115 = sin( mulTime195_g115 );
				float2 rotator194_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos194_g115 , -sin194_g115 , sin194_g115 , cos194_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode311_g115 = tex2D( CZY_LowBorderTexture, rotator194_g115 );
				float MediumBorderAlpha190_g115 = tex2DNode311_g115.a;
				float temp_output_140_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g122*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g122*48.2 + (-15.0 + (temp_output_14_0_g122 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g122 ) ) * MediumBorderAlpha190_g115 );
				float2 texCoord23_g128 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g128 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g128 = ( texCoord2_g128 - float2( 0.5,0.5 ) );
				float dotResult3_g128 = dot( temp_output_4_0_g128 , temp_output_4_0_g128 );
				float temp_output_14_0_g128 = saturate( (0.0 + (min( ( CZY_BorderHeight - 0.5 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode310_g115 = tex2D( CZY_HighBorderTexture, rotator194_g115 );
				float HighBorderAlpha192_g115 = tex2DNode310_g115.a;
				float temp_output_159_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g128*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g128*48.2 + (-15.0 + (temp_output_14_0_g128 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g128 ) ) * HighBorderAlpha192_g115 );
				float borderAlpha169_g115 = saturate( ( temp_output_140_0_g115 + temp_output_159_0_g115 ) );
				float2 texCoord23_g124 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g124 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g124 = ( texCoord2_g124 - float2( 0.5,0.5 ) );
				float dotResult3_g124 = dot( temp_output_4_0_g124 , temp_output_4_0_g124 );
				float temp_output_188_0_g115 = ( CZY_NimbusMultiplier * 0.5 );
				float temp_output_14_0_g124 = saturate( (0.0 + (min( temp_output_188_0_g115 , 2.0 ) - 0.0) * (1.0 - 0.0) / (2.0 - 0.0)) );
				float mulTime202_g115 = _TimeParameters.x * 0.005;
				float cos201_g115 = cos( mulTime202_g115 );
				float sin201_g115 = sin( mulTime202_g115 );
				float2 rotator201_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos201_g115 , -sin201_g115 , sin201_g115 , cos201_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode312_g115 = tex2D( CZY_LowNimbusTexture, rotator201_g115 );
				float lowNimbusAlpha197_g115 = tex2DNode312_g115.a;
				float temp_output_166_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g124*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g124*48.2 + (-15.0 + (temp_output_14_0_g124 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g124 ) ) * lowNimbusAlpha197_g115 );
				float2 texCoord23_g130 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g130 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g130 = ( texCoord2_g130 - float2( 0.5,0.5 ) );
				float dotResult3_g130 = dot( temp_output_4_0_g130 , temp_output_4_0_g130 );
				float temp_output_14_0_g130 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.2 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode313_g115 = tex2D( CZY_MidNimbusTexture, rotator201_g115 );
				float mediumNimbusAlpha199_g115 = tex2DNode313_g115.a;
				float temp_output_164_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g130*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g130*48.2 + (-15.0 + (temp_output_14_0_g130 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g130 ) ) * mediumNimbusAlpha199_g115 );
				float2 texCoord23_g123 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g123 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g123 = ( texCoord2_g123 - float2( 0.5,0.5 ) );
				float dotResult3_g123 = dot( temp_output_4_0_g123 , temp_output_4_0_g123 );
				float temp_output_14_0_g123 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.7 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode314_g115 = tex2D( CZY_HighNimbusTexture, rotator201_g115 );
				float HighNimbusAlpha205_g115 = tex2DNode314_g115.a;
				float temp_output_162_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g123*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g123*48.2 + (-15.0 + (temp_output_14_0_g123 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g123 ) ) * HighNimbusAlpha205_g115 );
				float nimbusAlpha172_g115 = saturate( ( temp_output_166_0_g115 + temp_output_164_0_g115 + temp_output_162_0_g115 ) );
				float2 texCoord23_g126 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g126 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g126 = ( texCoord2_g126 - float2( 0.5,0.5 ) );
				float dotResult3_g126 = dot( temp_output_4_0_g126 , temp_output_4_0_g126 );
				float temp_output_294_0_g115 = ( CZY_CumulusCoverageMultiplier * 1.0 );
				float temp_output_14_0_g126 = saturate( (0.0 + (min( temp_output_294_0_g115 , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float mulTime212_g115 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme213_g115 = mulTime212_g115;
				float2 cloudPosition211_g115 = (Pos6_g115*( 18.0 / CZY_MainCloudScale ) + ( TIme213_g115 * float2( 0.2,-0.4 ) ));
				float4 tex2DNode319_g115 = tex2D( CZY_PartlyCloudyTexture, cloudPosition211_g115 );
				float PartlyCloudyAlpha219_g115 = tex2DNode319_g115.a;
				float temp_output_249_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g126*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g126*48.2 + (-15.0 + (temp_output_14_0_g126 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g126 ) ) * PartlyCloudyAlpha219_g115 );
				float2 texCoord23_g125 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g125 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g125 = ( texCoord2_g125 - float2( 0.5,0.5 ) );
				float dotResult3_g125 = dot( temp_output_4_0_g125 , temp_output_4_0_g125 );
				float temp_output_14_0_g125 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.3 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode308_g115 = tex2D( CZY_MostlyCloudyTexture, cloudPosition211_g115 );
				float MostlyCloudyAlpha221_g115 = tex2DNode308_g115.a;
				float temp_output_226_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g125*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g125*48.2 + (-15.0 + (temp_output_14_0_g125 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g125 ) ) * MostlyCloudyAlpha221_g115 );
				float2 texCoord23_g129 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g129 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g129 = ( texCoord2_g129 - float2( 0.5,0.5 ) );
				float dotResult3_g129 = dot( temp_output_4_0_g129 , temp_output_4_0_g129 );
				float temp_output_14_0_g129 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.7 ) , 0.35 ) - 0.0) * (1.0 - 0.0) / (0.35 - 0.0)) );
				float4 tex2DNode309_g115 = tex2D( CZY_OvercastTexture, cloudPosition211_g115 );
				float OvercastCloudyAlpha224_g115 = tex2DNode309_g115.a;
				float temp_output_231_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g129*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g129*48.2 + (-15.0 + (temp_output_14_0_g129 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g129 ) ) * OvercastCloudyAlpha224_g115 );
				float cumulusAlpha232_g115 = saturate( ( temp_output_249_0_g115 + temp_output_226_0_g115 + temp_output_231_0_g115 ) );
				float mulTime33_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D40_g115 = snoise( (Pos6_g115*1.0 + mulTime33_g115)*2.0 );
				float mulTime31_g115 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos32_g115 = cos( ( mulTime31_g115 * 0.01 ) );
				float sin32_g115 = sin( ( mulTime31_g115 * 0.01 ) );
				float2 rotator32_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos32_g115 , -sin32_g115 , sin32_g115 , cos32_g115 )) + float2( 0.5,0.5 );
				float cos39_g115 = cos( ( mulTime31_g115 * -0.02 ) );
				float sin39_g115 = sin( ( mulTime31_g115 * -0.02 ) );
				float2 rotator39_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos39_g115 , -sin39_g115 , sin39_g115 , cos39_g115 )) + float2( 0.5,0.5 );
				float mulTime29_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D41_g115 = snoise( (Pos6_g115*1.0 + mulTime29_g115)*4.0 );
				float4 ChemtrailsPattern46_g115 = ( ( saturate( simplePerlin2D40_g115 ) * tex2D( CZY_ChemtrailsTexture, (rotator32_g115*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator39_g115 ) * saturate( simplePerlin2D41_g115 ) ) );
				float2 texCoord23_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_24_0_g115 = ( texCoord23_g115 - float2( 0.5,0.5 ) );
				float dotResult26_g115 = dot( temp_output_24_0_g115 , temp_output_24_0_g115 );
				float2 texCoord23_g119 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g119 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g119 = ( texCoord2_g119 - float2( 0.5,0.5 ) );
				float dotResult3_g119 = dot( temp_output_4_0_g119 , temp_output_4_0_g119 );
				float temp_output_14_0_g119 = ( CZY_ChemtrailsMultiplier * 0.5 );
				float3 appendResult58_g115 = (float3(( ChemtrailsPattern46_g115 * saturate( (0.4 + (dotResult26_g115 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g119*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g119*48.2 + (-15.0 + (temp_output_14_0_g119 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g119 ) ) ).rgb));
				float temp_output_59_0_g115 = length( appendResult58_g115 );
				float ChemtrailsAlpha61_g115 = temp_output_59_0_g115;
				float mulTime64_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D76_g115 = snoise( (Pos6_g115*1.0 + mulTime64_g115)*2.0 );
				float mulTime62_g115 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos70_g115 = cos( ( mulTime62_g115 * 0.01 ) );
				float sin70_g115 = sin( ( mulTime62_g115 * 0.01 ) );
				float2 rotator70_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos70_g115 , -sin70_g115 , sin70_g115 , cos70_g115 )) + float2( 0.5,0.5 );
				float cos72_g115 = cos( ( mulTime62_g115 * -0.02 ) );
				float sin72_g115 = sin( ( mulTime62_g115 * -0.02 ) );
				float2 rotator72_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos72_g115 , -sin72_g115 , sin72_g115 , cos72_g115 )) + float2( 0.5,0.5 );
				float mulTime78_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D75_g115 = snoise( (Pos6_g115*1.0 + mulTime78_g115) );
				simplePerlin2D75_g115 = simplePerlin2D75_g115*0.5 + 0.5;
				float4 CirrusPattern79_g115 = ( ( saturate( simplePerlin2D76_g115 ) * tex2D( CZY_CirrusTexture, (rotator70_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator72_g115*1.0 + 0.0) ) * saturate( simplePerlin2D75_g115 ) ) );
				float2 texCoord77_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_84_0_g115 = ( texCoord77_g115 - float2( 0.5,0.5 ) );
				float dotResult81_g115 = dot( temp_output_84_0_g115 , temp_output_84_0_g115 );
				float2 texCoord23_g120 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g120 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g120 = ( texCoord2_g120 - float2( 0.5,0.5 ) );
				float dotResult3_g120 = dot( temp_output_4_0_g120 , temp_output_4_0_g120 );
				float temp_output_14_0_g120 = ( CZY_CirrusMultiplier * 0.5 );
				float3 appendResult95_g115 = (float3(( CirrusPattern79_g115 * saturate( (0.0 + (dotResult81_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g120*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g120*48.2 + (-15.0 + (temp_output_14_0_g120 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g120 ) ) ).rgb));
				float temp_output_94_0_g115 = length( appendResult95_g115 );
				float CirrusAlpha97_g115 = temp_output_94_0_g115;
				float2 texCoord23_g127 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g127 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g127 = ( texCoord2_g127 - float2( 0.5,0.5 ) );
				float dotResult3_g127 = dot( temp_output_4_0_g127 , temp_output_4_0_g127 );
				float temp_output_14_0_g127 = CZY_AltocumulusMultiplier;
				float4 tex2DNode315_g115 = tex2D( CZY_AltocumulusTexture, cloudPosition211_g115 );
				float altocumulusAlpha217_g115 = tex2DNode315_g115.a;
				float temp_output_253_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g127*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g127*48.2 + (-15.0 + (temp_output_14_0_g127 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g127 ) ) * altocumulusAlpha217_g115 );
				float finalAcAlpha286_g115 = temp_output_253_0_g115;
				float mulTime104_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D116_g115 = snoise( (Pos6_g115*1.0 + mulTime104_g115)*2.0 );
				float mulTime102_g115 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos110_g115 = cos( ( mulTime102_g115 * 0.01 ) );
				float sin110_g115 = sin( ( mulTime102_g115 * 0.01 ) );
				float2 rotator110_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos110_g115 , -sin110_g115 , sin110_g115 , cos110_g115 )) + float2( 0.5,0.5 );
				float cos112_g115 = cos( ( mulTime102_g115 * -0.02 ) );
				float sin112_g115 = sin( ( mulTime102_g115 * -0.02 ) );
				float2 rotator112_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos112_g115 , -sin112_g115 , sin112_g115 , cos112_g115 )) + float2( 0.5,0.5 );
				float mulTime118_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D115_g115 = snoise( (Pos6_g115*1.0 + mulTime118_g115) );
				simplePerlin2D115_g115 = simplePerlin2D115_g115*0.5 + 0.5;
				float4 CirrostratusPattern134_g115 = ( ( saturate( simplePerlin2D116_g115 ) * tex2D( CZY_CirrostratusTexture, (rotator110_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator112_g115*1.0 + 0.0) ) * saturate( simplePerlin2D115_g115 ) ) );
				float2 texCoord117_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_123_0_g115 = ( texCoord117_g115 - float2( 0.5,0.5 ) );
				float dotResult120_g115 = dot( temp_output_123_0_g115 , temp_output_123_0_g115 );
				float2 texCoord23_g121 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g121 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g121 = ( texCoord2_g121 - float2( 0.5,0.5 ) );
				float dotResult3_g121 = dot( temp_output_4_0_g121 , temp_output_4_0_g121 );
				float temp_output_14_0_g121 = ( CZY_CirrostratusMultiplier * 0.5 );
				float3 appendResult131_g115 = (float3(( CirrostratusPattern134_g115 * saturate( (0.0 + (dotResult120_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g121*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g121*48.2 + (-15.0 + (temp_output_14_0_g121 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g121 ) ) ).rgb));
				float temp_output_130_0_g115 = length( appendResult131_g115 );
				float CirrostratusAlpha136_g115 = temp_output_130_0_g115;
				float cloudAlpha278_g115 = ( borderAlpha169_g115 + nimbusAlpha172_g115 + cumulusAlpha232_g115 + ChemtrailsAlpha61_g115 + CirrusAlpha97_g115 + finalAcAlpha286_g115 + CirrostratusAlpha136_g115 );
				bool enabled20_g118 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g118 =(bool)_FullySubmerged;
				float4 screenPos = input.ase_texcoord1;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g118 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g118 = HLSL20_g118( enabled20_g118 , submerged20_g118 , textureSample20_g118 );
				

				surfaceDescription.Alpha = ( saturate( ( cloudAlpha278_g115 + ( 0.0 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g118 ) );
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

			sampler2D CZY_LuxuryVariationTexture;
			float CZY_BorderHeight;
			sampler2D CZY_LowBorderTexture;
			sampler2D CZY_HighBorderTexture;
			float CZY_NimbusMultiplier;
			sampler2D CZY_LowNimbusTexture;
			sampler2D CZY_MidNimbusTexture;
			sampler2D CZY_HighNimbusTexture;
			float CZY_CumulusCoverageMultiplier;
			sampler2D CZY_PartlyCloudyTexture;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			sampler2D CZY_MostlyCloudyTexture;
			sampler2D CZY_OvercastTexture;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_AltocumulusTexture;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


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
			
			float HLSL20_g118( bool enabled, bool submerged, float textureSample )
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

				float2 texCoord23_g122 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g122 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g122 = ( texCoord2_g122 - float2( 0.5,0.5 ) );
				float dotResult3_g122 = dot( temp_output_4_0_g122 , temp_output_4_0_g122 );
				float temp_output_14_0_g122 = saturate( (0.0 + (min( CZY_BorderHeight , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float2 texCoord5_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos6_g115 = texCoord5_g115;
				float mulTime195_g115 = _TimeParameters.x * 0.005;
				float cos194_g115 = cos( mulTime195_g115 );
				float sin194_g115 = sin( mulTime195_g115 );
				float2 rotator194_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos194_g115 , -sin194_g115 , sin194_g115 , cos194_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode311_g115 = tex2D( CZY_LowBorderTexture, rotator194_g115 );
				float MediumBorderAlpha190_g115 = tex2DNode311_g115.a;
				float temp_output_140_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g122*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g122*48.2 + (-15.0 + (temp_output_14_0_g122 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g122 ) ) * MediumBorderAlpha190_g115 );
				float2 texCoord23_g128 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g128 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g128 = ( texCoord2_g128 - float2( 0.5,0.5 ) );
				float dotResult3_g128 = dot( temp_output_4_0_g128 , temp_output_4_0_g128 );
				float temp_output_14_0_g128 = saturate( (0.0 + (min( ( CZY_BorderHeight - 0.5 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode310_g115 = tex2D( CZY_HighBorderTexture, rotator194_g115 );
				float HighBorderAlpha192_g115 = tex2DNode310_g115.a;
				float temp_output_159_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g128*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g128*48.2 + (-15.0 + (temp_output_14_0_g128 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g128 ) ) * HighBorderAlpha192_g115 );
				float borderAlpha169_g115 = saturate( ( temp_output_140_0_g115 + temp_output_159_0_g115 ) );
				float2 texCoord23_g124 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g124 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g124 = ( texCoord2_g124 - float2( 0.5,0.5 ) );
				float dotResult3_g124 = dot( temp_output_4_0_g124 , temp_output_4_0_g124 );
				float temp_output_188_0_g115 = ( CZY_NimbusMultiplier * 0.5 );
				float temp_output_14_0_g124 = saturate( (0.0 + (min( temp_output_188_0_g115 , 2.0 ) - 0.0) * (1.0 - 0.0) / (2.0 - 0.0)) );
				float mulTime202_g115 = _TimeParameters.x * 0.005;
				float cos201_g115 = cos( mulTime202_g115 );
				float sin201_g115 = sin( mulTime202_g115 );
				float2 rotator201_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos201_g115 , -sin201_g115 , sin201_g115 , cos201_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode312_g115 = tex2D( CZY_LowNimbusTexture, rotator201_g115 );
				float lowNimbusAlpha197_g115 = tex2DNode312_g115.a;
				float temp_output_166_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g124*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g124*48.2 + (-15.0 + (temp_output_14_0_g124 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g124 ) ) * lowNimbusAlpha197_g115 );
				float2 texCoord23_g130 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g130 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g130 = ( texCoord2_g130 - float2( 0.5,0.5 ) );
				float dotResult3_g130 = dot( temp_output_4_0_g130 , temp_output_4_0_g130 );
				float temp_output_14_0_g130 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.2 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode313_g115 = tex2D( CZY_MidNimbusTexture, rotator201_g115 );
				float mediumNimbusAlpha199_g115 = tex2DNode313_g115.a;
				float temp_output_164_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g130*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g130*48.2 + (-15.0 + (temp_output_14_0_g130 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g130 ) ) * mediumNimbusAlpha199_g115 );
				float2 texCoord23_g123 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g123 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g123 = ( texCoord2_g123 - float2( 0.5,0.5 ) );
				float dotResult3_g123 = dot( temp_output_4_0_g123 , temp_output_4_0_g123 );
				float temp_output_14_0_g123 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.7 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode314_g115 = tex2D( CZY_HighNimbusTexture, rotator201_g115 );
				float HighNimbusAlpha205_g115 = tex2DNode314_g115.a;
				float temp_output_162_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g123*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g123*48.2 + (-15.0 + (temp_output_14_0_g123 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g123 ) ) * HighNimbusAlpha205_g115 );
				float nimbusAlpha172_g115 = saturate( ( temp_output_166_0_g115 + temp_output_164_0_g115 + temp_output_162_0_g115 ) );
				float2 texCoord23_g126 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g126 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g126 = ( texCoord2_g126 - float2( 0.5,0.5 ) );
				float dotResult3_g126 = dot( temp_output_4_0_g126 , temp_output_4_0_g126 );
				float temp_output_294_0_g115 = ( CZY_CumulusCoverageMultiplier * 1.0 );
				float temp_output_14_0_g126 = saturate( (0.0 + (min( temp_output_294_0_g115 , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float mulTime212_g115 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme213_g115 = mulTime212_g115;
				float2 cloudPosition211_g115 = (Pos6_g115*( 18.0 / CZY_MainCloudScale ) + ( TIme213_g115 * float2( 0.2,-0.4 ) ));
				float4 tex2DNode319_g115 = tex2D( CZY_PartlyCloudyTexture, cloudPosition211_g115 );
				float PartlyCloudyAlpha219_g115 = tex2DNode319_g115.a;
				float temp_output_249_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g126*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g126*48.2 + (-15.0 + (temp_output_14_0_g126 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g126 ) ) * PartlyCloudyAlpha219_g115 );
				float2 texCoord23_g125 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g125 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g125 = ( texCoord2_g125 - float2( 0.5,0.5 ) );
				float dotResult3_g125 = dot( temp_output_4_0_g125 , temp_output_4_0_g125 );
				float temp_output_14_0_g125 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.3 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode308_g115 = tex2D( CZY_MostlyCloudyTexture, cloudPosition211_g115 );
				float MostlyCloudyAlpha221_g115 = tex2DNode308_g115.a;
				float temp_output_226_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g125*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g125*48.2 + (-15.0 + (temp_output_14_0_g125 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g125 ) ) * MostlyCloudyAlpha221_g115 );
				float2 texCoord23_g129 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g129 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g129 = ( texCoord2_g129 - float2( 0.5,0.5 ) );
				float dotResult3_g129 = dot( temp_output_4_0_g129 , temp_output_4_0_g129 );
				float temp_output_14_0_g129 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.7 ) , 0.35 ) - 0.0) * (1.0 - 0.0) / (0.35 - 0.0)) );
				float4 tex2DNode309_g115 = tex2D( CZY_OvercastTexture, cloudPosition211_g115 );
				float OvercastCloudyAlpha224_g115 = tex2DNode309_g115.a;
				float temp_output_231_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g129*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g129*48.2 + (-15.0 + (temp_output_14_0_g129 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g129 ) ) * OvercastCloudyAlpha224_g115 );
				float cumulusAlpha232_g115 = saturate( ( temp_output_249_0_g115 + temp_output_226_0_g115 + temp_output_231_0_g115 ) );
				float mulTime33_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D40_g115 = snoise( (Pos6_g115*1.0 + mulTime33_g115)*2.0 );
				float mulTime31_g115 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos32_g115 = cos( ( mulTime31_g115 * 0.01 ) );
				float sin32_g115 = sin( ( mulTime31_g115 * 0.01 ) );
				float2 rotator32_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos32_g115 , -sin32_g115 , sin32_g115 , cos32_g115 )) + float2( 0.5,0.5 );
				float cos39_g115 = cos( ( mulTime31_g115 * -0.02 ) );
				float sin39_g115 = sin( ( mulTime31_g115 * -0.02 ) );
				float2 rotator39_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos39_g115 , -sin39_g115 , sin39_g115 , cos39_g115 )) + float2( 0.5,0.5 );
				float mulTime29_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D41_g115 = snoise( (Pos6_g115*1.0 + mulTime29_g115)*4.0 );
				float4 ChemtrailsPattern46_g115 = ( ( saturate( simplePerlin2D40_g115 ) * tex2D( CZY_ChemtrailsTexture, (rotator32_g115*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator39_g115 ) * saturate( simplePerlin2D41_g115 ) ) );
				float2 texCoord23_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_24_0_g115 = ( texCoord23_g115 - float2( 0.5,0.5 ) );
				float dotResult26_g115 = dot( temp_output_24_0_g115 , temp_output_24_0_g115 );
				float2 texCoord23_g119 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g119 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g119 = ( texCoord2_g119 - float2( 0.5,0.5 ) );
				float dotResult3_g119 = dot( temp_output_4_0_g119 , temp_output_4_0_g119 );
				float temp_output_14_0_g119 = ( CZY_ChemtrailsMultiplier * 0.5 );
				float3 appendResult58_g115 = (float3(( ChemtrailsPattern46_g115 * saturate( (0.4 + (dotResult26_g115 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g119*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g119*48.2 + (-15.0 + (temp_output_14_0_g119 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g119 ) ) ).rgb));
				float temp_output_59_0_g115 = length( appendResult58_g115 );
				float ChemtrailsAlpha61_g115 = temp_output_59_0_g115;
				float mulTime64_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D76_g115 = snoise( (Pos6_g115*1.0 + mulTime64_g115)*2.0 );
				float mulTime62_g115 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos70_g115 = cos( ( mulTime62_g115 * 0.01 ) );
				float sin70_g115 = sin( ( mulTime62_g115 * 0.01 ) );
				float2 rotator70_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos70_g115 , -sin70_g115 , sin70_g115 , cos70_g115 )) + float2( 0.5,0.5 );
				float cos72_g115 = cos( ( mulTime62_g115 * -0.02 ) );
				float sin72_g115 = sin( ( mulTime62_g115 * -0.02 ) );
				float2 rotator72_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos72_g115 , -sin72_g115 , sin72_g115 , cos72_g115 )) + float2( 0.5,0.5 );
				float mulTime78_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D75_g115 = snoise( (Pos6_g115*1.0 + mulTime78_g115) );
				simplePerlin2D75_g115 = simplePerlin2D75_g115*0.5 + 0.5;
				float4 CirrusPattern79_g115 = ( ( saturate( simplePerlin2D76_g115 ) * tex2D( CZY_CirrusTexture, (rotator70_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator72_g115*1.0 + 0.0) ) * saturate( simplePerlin2D75_g115 ) ) );
				float2 texCoord77_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_84_0_g115 = ( texCoord77_g115 - float2( 0.5,0.5 ) );
				float dotResult81_g115 = dot( temp_output_84_0_g115 , temp_output_84_0_g115 );
				float2 texCoord23_g120 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g120 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g120 = ( texCoord2_g120 - float2( 0.5,0.5 ) );
				float dotResult3_g120 = dot( temp_output_4_0_g120 , temp_output_4_0_g120 );
				float temp_output_14_0_g120 = ( CZY_CirrusMultiplier * 0.5 );
				float3 appendResult95_g115 = (float3(( CirrusPattern79_g115 * saturate( (0.0 + (dotResult81_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g120*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g120*48.2 + (-15.0 + (temp_output_14_0_g120 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g120 ) ) ).rgb));
				float temp_output_94_0_g115 = length( appendResult95_g115 );
				float CirrusAlpha97_g115 = temp_output_94_0_g115;
				float2 texCoord23_g127 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g127 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g127 = ( texCoord2_g127 - float2( 0.5,0.5 ) );
				float dotResult3_g127 = dot( temp_output_4_0_g127 , temp_output_4_0_g127 );
				float temp_output_14_0_g127 = CZY_AltocumulusMultiplier;
				float4 tex2DNode315_g115 = tex2D( CZY_AltocumulusTexture, cloudPosition211_g115 );
				float altocumulusAlpha217_g115 = tex2DNode315_g115.a;
				float temp_output_253_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g127*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g127*48.2 + (-15.0 + (temp_output_14_0_g127 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g127 ) ) * altocumulusAlpha217_g115 );
				float finalAcAlpha286_g115 = temp_output_253_0_g115;
				float mulTime104_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D116_g115 = snoise( (Pos6_g115*1.0 + mulTime104_g115)*2.0 );
				float mulTime102_g115 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos110_g115 = cos( ( mulTime102_g115 * 0.01 ) );
				float sin110_g115 = sin( ( mulTime102_g115 * 0.01 ) );
				float2 rotator110_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos110_g115 , -sin110_g115 , sin110_g115 , cos110_g115 )) + float2( 0.5,0.5 );
				float cos112_g115 = cos( ( mulTime102_g115 * -0.02 ) );
				float sin112_g115 = sin( ( mulTime102_g115 * -0.02 ) );
				float2 rotator112_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos112_g115 , -sin112_g115 , sin112_g115 , cos112_g115 )) + float2( 0.5,0.5 );
				float mulTime118_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D115_g115 = snoise( (Pos6_g115*1.0 + mulTime118_g115) );
				simplePerlin2D115_g115 = simplePerlin2D115_g115*0.5 + 0.5;
				float4 CirrostratusPattern134_g115 = ( ( saturate( simplePerlin2D116_g115 ) * tex2D( CZY_CirrostratusTexture, (rotator110_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator112_g115*1.0 + 0.0) ) * saturate( simplePerlin2D115_g115 ) ) );
				float2 texCoord117_g115 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_123_0_g115 = ( texCoord117_g115 - float2( 0.5,0.5 ) );
				float dotResult120_g115 = dot( temp_output_123_0_g115 , temp_output_123_0_g115 );
				float2 texCoord23_g121 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g121 = input.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g121 = ( texCoord2_g121 - float2( 0.5,0.5 ) );
				float dotResult3_g121 = dot( temp_output_4_0_g121 , temp_output_4_0_g121 );
				float temp_output_14_0_g121 = ( CZY_CirrostratusMultiplier * 0.5 );
				float3 appendResult131_g115 = (float3(( CirrostratusPattern134_g115 * saturate( (0.0 + (dotResult120_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g121*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g121*48.2 + (-15.0 + (temp_output_14_0_g121 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g121 ) ) ).rgb));
				float temp_output_130_0_g115 = length( appendResult131_g115 );
				float CirrostratusAlpha136_g115 = temp_output_130_0_g115;
				float cloudAlpha278_g115 = ( borderAlpha169_g115 + nimbusAlpha172_g115 + cumulusAlpha232_g115 + ChemtrailsAlpha61_g115 + CirrusAlpha97_g115 + finalAcAlpha286_g115 + CirrostratusAlpha136_g115 );
				bool enabled20_g118 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g118 =(bool)_FullySubmerged;
				float4 screenPos = input.ase_texcoord1;
				float4 ase_positionSSNorm = screenPos / screenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g118 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g118 = HLSL20_g118( enabled20_g118 , submerged20_g118 , textureSample20_g118 );
				

				surfaceDescription.Alpha = ( saturate( ( cloudAlpha278_g115 + ( 0.0 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g118 ) );
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

			sampler2D CZY_LuxuryVariationTexture;
			float CZY_BorderHeight;
			sampler2D CZY_LowBorderTexture;
			sampler2D CZY_HighBorderTexture;
			float CZY_NimbusMultiplier;
			sampler2D CZY_LowNimbusTexture;
			sampler2D CZY_MidNimbusTexture;
			sampler2D CZY_HighNimbusTexture;
			float CZY_CumulusCoverageMultiplier;
			sampler2D CZY_PartlyCloudyTexture;
			float CZY_MainCloudScale;
			float CZY_WindSpeed;
			sampler2D CZY_MostlyCloudyTexture;
			sampler2D CZY_OvercastTexture;
			sampler2D CZY_ChemtrailsTexture;
			float CZY_ChemtrailsMoveSpeed;
			float CZY_ChemtrailsMultiplier;
			sampler2D CZY_CirrusTexture;
			float CZY_CirrusMoveSpeed;
			float CZY_CirrusMultiplier;
			float CZY_AltocumulusMultiplier;
			sampler2D CZY_AltocumulusTexture;
			sampler2D CZY_CirrostratusTexture;
			float CZY_CirrostratusMoveSpeed;
			float CZY_CirrostratusMultiplier;
			float CZY_CloudThickness;
			float _UnderwaterRenderingEnabled;
			float _FullySubmerged;
			sampler2D _UnderwaterMask;


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
			
			float HLSL20_g118( bool enabled, bool submerged, float textureSample )
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

				float2 texCoord23_g122 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g122 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g122 = ( texCoord2_g122 - float2( 0.5,0.5 ) );
				float dotResult3_g122 = dot( temp_output_4_0_g122 , temp_output_4_0_g122 );
				float temp_output_14_0_g122 = saturate( (0.0 + (min( CZY_BorderHeight , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float2 texCoord5_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos6_g115 = texCoord5_g115;
				float mulTime195_g115 = _TimeParameters.x * 0.005;
				float cos194_g115 = cos( mulTime195_g115 );
				float sin194_g115 = sin( mulTime195_g115 );
				float2 rotator194_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos194_g115 , -sin194_g115 , sin194_g115 , cos194_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode311_g115 = tex2D( CZY_LowBorderTexture, rotator194_g115 );
				float MediumBorderAlpha190_g115 = tex2DNode311_g115.a;
				float temp_output_140_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g122*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g122*48.2 + (-15.0 + (temp_output_14_0_g122 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g122 ) ) * MediumBorderAlpha190_g115 );
				float2 texCoord23_g128 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g128 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g128 = ( texCoord2_g128 - float2( 0.5,0.5 ) );
				float dotResult3_g128 = dot( temp_output_4_0_g128 , temp_output_4_0_g128 );
				float temp_output_14_0_g128 = saturate( (0.0 + (min( ( CZY_BorderHeight - 0.5 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode310_g115 = tex2D( CZY_HighBorderTexture, rotator194_g115 );
				float HighBorderAlpha192_g115 = tex2DNode310_g115.a;
				float temp_output_159_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g128*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g128*48.2 + (-15.0 + (temp_output_14_0_g128 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g128 ) ) * HighBorderAlpha192_g115 );
				float borderAlpha169_g115 = saturate( ( temp_output_140_0_g115 + temp_output_159_0_g115 ) );
				float2 texCoord23_g124 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g124 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g124 = ( texCoord2_g124 - float2( 0.5,0.5 ) );
				float dotResult3_g124 = dot( temp_output_4_0_g124 , temp_output_4_0_g124 );
				float temp_output_188_0_g115 = ( CZY_NimbusMultiplier * 0.5 );
				float temp_output_14_0_g124 = saturate( (0.0 + (min( temp_output_188_0_g115 , 2.0 ) - 0.0) * (1.0 - 0.0) / (2.0 - 0.0)) );
				float mulTime202_g115 = _TimeParameters.x * 0.005;
				float cos201_g115 = cos( mulTime202_g115 );
				float sin201_g115 = sin( mulTime202_g115 );
				float2 rotator201_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos201_g115 , -sin201_g115 , sin201_g115 , cos201_g115 )) + float2( 0.5,0.5 );
				float4 tex2DNode312_g115 = tex2D( CZY_LowNimbusTexture, rotator201_g115 );
				float lowNimbusAlpha197_g115 = tex2DNode312_g115.a;
				float temp_output_166_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g124*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g124*48.2 + (-15.0 + (temp_output_14_0_g124 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g124 ) ) * lowNimbusAlpha197_g115 );
				float2 texCoord23_g130 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g130 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g130 = ( texCoord2_g130 - float2( 0.5,0.5 ) );
				float dotResult3_g130 = dot( temp_output_4_0_g130 , temp_output_4_0_g130 );
				float temp_output_14_0_g130 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.2 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode313_g115 = tex2D( CZY_MidNimbusTexture, rotator201_g115 );
				float mediumNimbusAlpha199_g115 = tex2DNode313_g115.a;
				float temp_output_164_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g130*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g130*48.2 + (-15.0 + (temp_output_14_0_g130 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g130 ) ) * mediumNimbusAlpha199_g115 );
				float2 texCoord23_g123 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g123 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g123 = ( texCoord2_g123 - float2( 0.5,0.5 ) );
				float dotResult3_g123 = dot( temp_output_4_0_g123 , temp_output_4_0_g123 );
				float temp_output_14_0_g123 = saturate( (0.0 + (min( ( temp_output_188_0_g115 - 0.7 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
				float4 tex2DNode314_g115 = tex2D( CZY_HighNimbusTexture, rotator201_g115 );
				float HighNimbusAlpha205_g115 = tex2DNode314_g115.a;
				float temp_output_162_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g123*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g123*48.2 + (-15.0 + (temp_output_14_0_g123 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g123 ) ) * HighNimbusAlpha205_g115 );
				float nimbusAlpha172_g115 = saturate( ( temp_output_166_0_g115 + temp_output_164_0_g115 + temp_output_162_0_g115 ) );
				float2 texCoord23_g126 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g126 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g126 = ( texCoord2_g126 - float2( 0.5,0.5 ) );
				float dotResult3_g126 = dot( temp_output_4_0_g126 , temp_output_4_0_g126 );
				float temp_output_294_0_g115 = ( CZY_CumulusCoverageMultiplier * 1.0 );
				float temp_output_14_0_g126 = saturate( (0.0 + (min( temp_output_294_0_g115 , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float mulTime212_g115 = _TimeParameters.x * ( 0.001 * CZY_WindSpeed );
				float TIme213_g115 = mulTime212_g115;
				float2 cloudPosition211_g115 = (Pos6_g115*( 18.0 / CZY_MainCloudScale ) + ( TIme213_g115 * float2( 0.2,-0.4 ) ));
				float4 tex2DNode319_g115 = tex2D( CZY_PartlyCloudyTexture, cloudPosition211_g115 );
				float PartlyCloudyAlpha219_g115 = tex2DNode319_g115.a;
				float temp_output_249_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g126*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g126*48.2 + (-15.0 + (temp_output_14_0_g126 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g126 ) ) * PartlyCloudyAlpha219_g115 );
				float2 texCoord23_g125 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g125 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g125 = ( texCoord2_g125 - float2( 0.5,0.5 ) );
				float dotResult3_g125 = dot( temp_output_4_0_g125 , temp_output_4_0_g125 );
				float temp_output_14_0_g125 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.3 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
				float4 tex2DNode308_g115 = tex2D( CZY_MostlyCloudyTexture, cloudPosition211_g115 );
				float MostlyCloudyAlpha221_g115 = tex2DNode308_g115.a;
				float temp_output_226_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g125*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g125*48.2 + (-15.0 + (temp_output_14_0_g125 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g125 ) ) * MostlyCloudyAlpha221_g115 );
				float2 texCoord23_g129 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g129 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g129 = ( texCoord2_g129 - float2( 0.5,0.5 ) );
				float dotResult3_g129 = dot( temp_output_4_0_g129 , temp_output_4_0_g129 );
				float temp_output_14_0_g129 = saturate( (0.0 + (min( ( temp_output_294_0_g115 - 0.7 ) , 0.35 ) - 0.0) * (1.0 - 0.0) / (0.35 - 0.0)) );
				float4 tex2DNode309_g115 = tex2D( CZY_OvercastTexture, cloudPosition211_g115 );
				float OvercastCloudyAlpha224_g115 = tex2DNode309_g115.a;
				float temp_output_231_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g129*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g129*48.2 + (-15.0 + (temp_output_14_0_g129 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g129 ) ) * OvercastCloudyAlpha224_g115 );
				float cumulusAlpha232_g115 = saturate( ( temp_output_249_0_g115 + temp_output_226_0_g115 + temp_output_231_0_g115 ) );
				float mulTime33_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D40_g115 = snoise( (Pos6_g115*1.0 + mulTime33_g115)*2.0 );
				float mulTime31_g115 = _TimeParameters.x * CZY_ChemtrailsMoveSpeed;
				float cos32_g115 = cos( ( mulTime31_g115 * 0.01 ) );
				float sin32_g115 = sin( ( mulTime31_g115 * 0.01 ) );
				float2 rotator32_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos32_g115 , -sin32_g115 , sin32_g115 , cos32_g115 )) + float2( 0.5,0.5 );
				float cos39_g115 = cos( ( mulTime31_g115 * -0.02 ) );
				float sin39_g115 = sin( ( mulTime31_g115 * -0.02 ) );
				float2 rotator39_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos39_g115 , -sin39_g115 , sin39_g115 , cos39_g115 )) + float2( 0.5,0.5 );
				float mulTime29_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D41_g115 = snoise( (Pos6_g115*1.0 + mulTime29_g115)*4.0 );
				float4 ChemtrailsPattern46_g115 = ( ( saturate( simplePerlin2D40_g115 ) * tex2D( CZY_ChemtrailsTexture, (rotator32_g115*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator39_g115 ) * saturate( simplePerlin2D41_g115 ) ) );
				float2 texCoord23_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_24_0_g115 = ( texCoord23_g115 - float2( 0.5,0.5 ) );
				float dotResult26_g115 = dot( temp_output_24_0_g115 , temp_output_24_0_g115 );
				float2 texCoord23_g119 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g119 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g119 = ( texCoord2_g119 - float2( 0.5,0.5 ) );
				float dotResult3_g119 = dot( temp_output_4_0_g119 , temp_output_4_0_g119 );
				float temp_output_14_0_g119 = ( CZY_ChemtrailsMultiplier * 0.5 );
				float3 appendResult58_g115 = (float3(( ChemtrailsPattern46_g115 * saturate( (0.4 + (dotResult26_g115 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g119*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g119*48.2 + (-15.0 + (temp_output_14_0_g119 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g119 ) ) ).rgb));
				float temp_output_59_0_g115 = length( appendResult58_g115 );
				float ChemtrailsAlpha61_g115 = temp_output_59_0_g115;
				float mulTime64_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D76_g115 = snoise( (Pos6_g115*1.0 + mulTime64_g115)*2.0 );
				float mulTime62_g115 = _TimeParameters.x * CZY_CirrusMoveSpeed;
				float cos70_g115 = cos( ( mulTime62_g115 * 0.01 ) );
				float sin70_g115 = sin( ( mulTime62_g115 * 0.01 ) );
				float2 rotator70_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos70_g115 , -sin70_g115 , sin70_g115 , cos70_g115 )) + float2( 0.5,0.5 );
				float cos72_g115 = cos( ( mulTime62_g115 * -0.02 ) );
				float sin72_g115 = sin( ( mulTime62_g115 * -0.02 ) );
				float2 rotator72_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos72_g115 , -sin72_g115 , sin72_g115 , cos72_g115 )) + float2( 0.5,0.5 );
				float mulTime78_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D75_g115 = snoise( (Pos6_g115*1.0 + mulTime78_g115) );
				simplePerlin2D75_g115 = simplePerlin2D75_g115*0.5 + 0.5;
				float4 CirrusPattern79_g115 = ( ( saturate( simplePerlin2D76_g115 ) * tex2D( CZY_CirrusTexture, (rotator70_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator72_g115*1.0 + 0.0) ) * saturate( simplePerlin2D75_g115 ) ) );
				float2 texCoord77_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_84_0_g115 = ( texCoord77_g115 - float2( 0.5,0.5 ) );
				float dotResult81_g115 = dot( temp_output_84_0_g115 , temp_output_84_0_g115 );
				float2 texCoord23_g120 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g120 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g120 = ( texCoord2_g120 - float2( 0.5,0.5 ) );
				float dotResult3_g120 = dot( temp_output_4_0_g120 , temp_output_4_0_g120 );
				float temp_output_14_0_g120 = ( CZY_CirrusMultiplier * 0.5 );
				float3 appendResult95_g115 = (float3(( CirrusPattern79_g115 * saturate( (0.0 + (dotResult81_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g120*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g120*48.2 + (-15.0 + (temp_output_14_0_g120 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g120 ) ) ).rgb));
				float temp_output_94_0_g115 = length( appendResult95_g115 );
				float CirrusAlpha97_g115 = temp_output_94_0_g115;
				float2 texCoord23_g127 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g127 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g127 = ( texCoord2_g127 - float2( 0.5,0.5 ) );
				float dotResult3_g127 = dot( temp_output_4_0_g127 , temp_output_4_0_g127 );
				float temp_output_14_0_g127 = CZY_AltocumulusMultiplier;
				float4 tex2DNode315_g115 = tex2D( CZY_AltocumulusTexture, cloudPosition211_g115 );
				float altocumulusAlpha217_g115 = tex2DNode315_g115.a;
				float temp_output_253_0_g115 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g127*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g127*48.2 + (-15.0 + (temp_output_14_0_g127 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g127 ) ) * altocumulusAlpha217_g115 );
				float finalAcAlpha286_g115 = temp_output_253_0_g115;
				float mulTime104_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D116_g115 = snoise( (Pos6_g115*1.0 + mulTime104_g115)*2.0 );
				float mulTime102_g115 = _TimeParameters.x * CZY_CirrostratusMoveSpeed;
				float cos110_g115 = cos( ( mulTime102_g115 * 0.01 ) );
				float sin110_g115 = sin( ( mulTime102_g115 * 0.01 ) );
				float2 rotator110_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos110_g115 , -sin110_g115 , sin110_g115 , cos110_g115 )) + float2( 0.5,0.5 );
				float cos112_g115 = cos( ( mulTime102_g115 * -0.02 ) );
				float sin112_g115 = sin( ( mulTime102_g115 * -0.02 ) );
				float2 rotator112_g115 = mul( Pos6_g115 - float2( 0.5,0.5 ) , float2x2( cos112_g115 , -sin112_g115 , sin112_g115 , cos112_g115 )) + float2( 0.5,0.5 );
				float mulTime118_g115 = _TimeParameters.x * 0.01;
				float simplePerlin2D115_g115 = snoise( (Pos6_g115*1.0 + mulTime118_g115) );
				simplePerlin2D115_g115 = simplePerlin2D115_g115*0.5 + 0.5;
				float4 CirrostratusPattern134_g115 = ( ( saturate( simplePerlin2D116_g115 ) * tex2D( CZY_CirrostratusTexture, (rotator110_g115*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator112_g115*1.0 + 0.0) ) * saturate( simplePerlin2D115_g115 ) ) );
				float2 texCoord117_g115 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_123_0_g115 = ( texCoord117_g115 - float2( 0.5,0.5 ) );
				float dotResult120_g115 = dot( temp_output_123_0_g115 , temp_output_123_0_g115 );
				float2 texCoord23_g121 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord2_g121 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_4_0_g121 = ( texCoord2_g121 - float2( 0.5,0.5 ) );
				float dotResult3_g121 = dot( temp_output_4_0_g121 , temp_output_4_0_g121 );
				float temp_output_14_0_g121 = ( CZY_CirrostratusMultiplier * 0.5 );
				float3 appendResult131_g115 = (float3(( CirrostratusPattern134_g115 * saturate( (0.0 + (dotResult120_g115 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (texCoord23_g121*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g121*48.2 + (-15.0 + (temp_output_14_0_g121 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g121 ) ) ).rgb));
				float temp_output_130_0_g115 = length( appendResult131_g115 );
				float CirrostratusAlpha136_g115 = temp_output_130_0_g115;
				float cloudAlpha278_g115 = ( borderAlpha169_g115 + nimbusAlpha172_g115 + cumulusAlpha232_g115 + ChemtrailsAlpha61_g115 + CirrusAlpha97_g115 + finalAcAlpha286_g115 + CirrostratusAlpha136_g115 );
				bool enabled20_g118 =(bool)_UnderwaterRenderingEnabled;
				bool submerged20_g118 =(bool)_FullySubmerged;
				float4 ase_positionSSNorm = ScreenPos / ScreenPos.w;
				ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
				float textureSample20_g118 = tex2Dlod( _UnderwaterMask, float4( ase_positionSSNorm.xy, 0, 0.0) ).r;
				float localHLSL20_g118 = HLSL20_g118( enabled20_g118 , submerged20_g118 , textureSample20_g118 );
				

				float Alpha = ( saturate( ( cloudAlpha278_g115 + ( 0.0 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g118 ) );
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
Node;AmplifyShaderEditor.FunctionNode;1659;2512,-4336;Inherit;False;Stylized Clouds (Luxury);0;;115;20d558f0b20b5f34db2c08faef3f114d;0;0;3;COLOR;0;FLOAT;334;FLOAT;333
Node;AmplifyShaderEditor.RangedFloatNode;1660;2576,-4128;Inherit;False;Global;CZY_CloudsFogAmount;CZY_CloudsFogAmount;8;0;Create;True;0;0;0;False;0;False;0;0.509;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1661;2576,-4208;Inherit;False;Global;CZY_CloudsFogLightAmount;CZY_CloudsFogLightAmount;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1662;2912,-4336;Inherit;False;AddFogToSkyLayer;-1;;369;36a78fe96c9f6fa4dab85c7793736468;0;3;89;COLOR;0,0,0,0;False;91;FLOAT;0;False;59;FLOAT;0;False;2;COLOR;84;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;799;-1216,-4336;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;803;-678.2959,-671.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;802;-678.2959,-671.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;801;-678.2959,-671.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;804;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;805;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;806;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;807;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;808;-677.2959,-599.1561;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;800;3184,-4336;Float;False;True;-1;2;DistantLands.Cozy.EditorScripts.EmptyShaderGUI;0;13;Distant Lands/Cozy/URP/Stylized Clouds (Luxury);2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;9;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;1;False;;False;False;False;False;False;False;False;False;True;True;True;221;False;;255;False;;255;False;;7;False;;2;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=-50;UniversalMaterialType=Unlit;True;2;True;12;all;0;False;True;1;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;25;Surface;1;638295390293297003;  Blend;0;0;Two Sided;2;638295390676621873;Alpha Clipping;1;0;  Use Shadow Threshold;0;0;Forward Only;1;638295390392913430;Cast Shadows;0;638335145705378424;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;True;False;False;True;True;True;False;False;;False;0
WireConnection;1662;89;1659;0
WireConnection;1662;91;1661;0
WireConnection;1662;59;1660;0
WireConnection;800;2;1662;84
WireConnection;800;3;1659;334
ASEEND*/
//CHKSM=BD6856D5D9A67E1AFC8C578D75E308D795BD007C