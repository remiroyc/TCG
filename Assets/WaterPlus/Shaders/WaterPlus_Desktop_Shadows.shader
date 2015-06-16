//All: 8.3 ms
//No flowmap: 5.6 ms
//Pervertex aniso: 6 ms
//Pervertex baked aniso: 7.8 ms

//From now on: all - flowmap:
//Caustics off: 5.1 ms
//Foam off: 4.9 ms
//Calc normals off: 3.6 ms
//Refractions off: 3.0 ms


Shader "Water+/Desktop with Shadows" {

Properties {
	_MainTex("Main texture", 2D) = "bump" {}
	_NormalMap("Normalmap", 2D) = "bump" {}
	//_FoamTex("Seafoam texture", 2D) = "white" {}
	//_HeightGlossMap ("Height(R), Gloss(G)", 2D) = "white" { }
	_DUDVFoamMap ("DUDV(RG) Foam(B)", 2D) = "white" { }	//All the stuff affected by flowmaps goes in here
	_Cube ("Cubemap", CUBE) = "" {}
	_WaterMap ("Depth (R), Foam (G), Transparency(B) Refr strength(A)", 2D) = "white" {}
	//_RefractionMap ("Refraction map", 2D) = "white" {}
	_SecondaryRefractionTex("Refraction texture", 2D) = "bump" {}
	//_RefractionTileTexture ("Refraction tile texture", 2D) = "white" {}
	_FlowMap ("Flowmap (RG)", 2D) = "white" {}
	_AnisoMap ("AnisoDir(RGB), AnisoLookup(A)", 2D) = "bump" {}
	_CausticsAnimationTexture ("Caustics animation", 2D) = "white" {}
	_Reflectivity("Reflectivity", Range (.0, 1.0)) = .3
	_Refractivity("Refractivity", Range (1.0, 5.0)) = 1.0
	_WaterAttenuation("Water attenuation", Range (0.0, 2.0)) = 1.0
	_ShallowWaterTint("Shallow water wint", Color) = (.0, .26, .39, 1.0)
	_DeepWaterTint("Deep water tint", Color) = (.0, .26, .39, 1.0)
	//_DeepWaterCoefficient("DeepWaterCoefficient", Range(1.0, 10.0) ) = 5.0
	_Shininess ("Shininess", Range (.05, 20.0)) = 1.0
	_Gloss("Gloss", Range(0.0, 20.0)) = 10.0
	_Fresnel0 ("fresnel0", Float) = 0.1
	_EdgeFoamStrength ("Edge foam strength", Range (.0, 3.0) ) = 1.0
	//_EdgeBlendStrength ("Edge blend strength", Range (10.0, 100.0) ) = 50.0
	////////////////////////
	
	_CausticsStrength ("Caustics strength", Range (0.05, 0.3) ) = 0.1
	_CausticsScale ("Caustics scale", Range (10.0, 2000.0) ) = 500.0
	
	_normalStrength("Normal strength",  Range (.01, 5.0)) = .5
	_refractionsWetness("Refractions wetness", Range (.0, 1.0)) = .8
	
	//All of the following is provided by script:
	//_yOffset ("internal yOffset", Float) = .0									//Provided by script
	flowMapOffset0 ("internal FlowMapOffset0", Float) = 0.0						//Provided by script
	flowMapOffset1 ("internal FlowMapOffset1", Float) = 0.5						//Provided by script
	halfCycle ("internal HalfCycle", Float) = 0.25								//Provided by script
	//flowTide ("internal FlowTide", Float) = 0.0									//Provided by script
	//anisoDirAnimationOffset("internal Aniso dir animation offset", Vector) = (.0, .0, .0, .0)
	causticsOffsetAndScale("internal caustics animation offset and scale", Vector) = (.0, .0, .25, .0)
	causticsAnimationColorChannel("internal caustics animation color channel", Vector) = (1.0, .0, .0, .0)
	//anisoAnimationOffset("Anisotropic animation offset", Float) = 0.0	//Provided by script
}

	Category {
		//Lighting on
	    
	
		 SubShader {
		 	Tags {"Queue"="Geometry" "RenderType"="Opaque" }
		 //GrabPass { }
		 
		 	Pass {
				Name "FORWARD"
				Tags { "LightMode" = "ForwardBase" }
			    
				CGPROGRAM
				#pragma exclude_renderers xbox360 flash
				#pragma multi_compile FLOWMAP_ANIMATION_ON FLOWMAP_ANIMATION_OFF
				
				//#pragma multi_compile_fwdbase
				//#define UNITY_PASS_FORWARDBASE
				
				#pragma target 3.0
				#pragma glsl
				//#pragma glsl_no_auto_normalization	 //Important for mobile
				
				#pragma vertex vert
				#pragma fragment frag
				
				//#define LINEAR_COLOR_SPACE
				
				#define LIGHT_MODEL_ANISOTROPIC
				//#undef BAKED_ANISOTROPY_DIR
				
				#define LIGHTING_ON
				#define PERPIXEL_SPECULARITY_ON
				#define CAUSTICS_ON
				#define CAUSTICS_ALL
				#define FOAM_ON
				#define REFLECTIONS_ON
				#define WATERMAPS_ON
				#define CALCULATE_NORMALS_ON
				
				#define REFRACTIONS_ON
				#define USE_SECONDARY_REFRACTION
				
				#undef FLOWMAP_ADD_NOISE_ON
				
				#define FRESNEL_ON
				
				#define ENABLE_SHADOWS
				
				#ifdef ENABLE_SHADOWS
					#define OPAQUE_SURFACE
					#pragma multi_compile_fwdbase
					#define UNITY_PASS_FORWARDBASE
				#endif
				
				#include "WaterPlusInclude.cginc"
				
				
				ENDCG
		
			}
			
//			Pass {
//				Name "ShadowCaster"
//				Tags { "LightMode" = "ShadowCaster" }
//				
//				Fog {Mode Off}
//				ZWrite On ZTest Less Cull Off
//				Offset 1, 1
//		
//				CGPROGRAM
//				#pragma vertex vert
//				#pragma fragment frag
//				#pragma multi_compile_shadowcaster
//				#pragma fragmentoption ARB_precision_hint_fastest
//				#include "UnityCG.cginc"
//				
//				struct v2f { 
//					V2F_SHADOW_CASTER;
//				};
//				
//				v2f vert( appdata_base v )
//				{
//					v2f o;
//					TRANSFER_SHADOW_CASTER(o)
//					return o;
//				}
//				
//				float4 frag( v2f i ) : COLOR
//				{
//					SHADOW_CASTER_FRAGMENT(i)
//				}
//				ENDCG
//		
//			}
			
			// Pass to render object as a shadow collector
			Pass {
				Name "ShadowCollector"
				Tags { "LightMode" = "ShadowCollector" }
				
				Fog {Mode Off}
				ZWrite On ZTest Less
		
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_shadowcollector
				
				#define SHADOW_COLLECTOR_PASS
				#include "UnityCG.cginc"
				
				struct appdata {
					float4 vertex : POSITION;
				};
				
				struct v2f {
					V2F_SHADOW_COLLECTOR;
				};
				
				v2f vert (appdata v)
				{
					v2f o;
					TRANSFER_SHADOW_COLLECTOR(o)
					return o;
				}
				
				half4 frag (v2f i) : COLOR
				{
					//return 0.5;
					SHADOW_COLLECTOR_FRAGMENT(i)
				}
				ENDCG
		
			}
		 }
		 
	}
	
	//Fallback "Water+/Desktop"
 }