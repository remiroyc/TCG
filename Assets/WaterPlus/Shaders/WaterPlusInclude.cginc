#ifndef WATERPLUS_CG_INCLUDED
#define WATERPLUS_CG_INCLUDED
	
	#ifdef PRO_FEATURES
		#define PRO_REFRACTIONS
		#define WATER_EDGEBLEND_ON
	#else
		#undef PRO_REFRACTIONS
		#undef WATER_EDGEBLEND_ON
	#endif
	
	#if !defined(WATERMAPS_ON)
		#undef BAKED_ANISOTROPY_DIR
		//#undef BAKED_ANISOTROPIC_LIGHTING
	#endif
	
	//#define LIGHT_MODEL_ANISOTROPIC
	//#define BAKED_ANISOTROPY_DIR		//Suitable only for flat horizontal surfaces
	//#define BAKED_ANISOTROPIC_LIGHTING
	
//	#if defined (QUALITY_FASTEST)
//		#undef WATER_EDGEBLEND_ON
//	#elif defined(QUALITY_FAST)
//		#undef WATER_EDGEBLEND_ON
//		#define REFLECTIONS_ON
//	#elif defined(QUALITY_SIMPLE)
//		#undef WATER_EDGEBLEND_ON
//		#define REFLECTIONS_ON
//		#define WATERMAPS_ON
//	#elif defined(QUALITY_GOOD) 
//		#define REFLECTIONS_ON
//		#define WATERMAPS_ON
//		//#define CALCULATE_NORMALS_ON
//		#define PERPIXEL_SPECULARITY_ON
//		//#define MULTIPLE_SCALE_WAVES_ON
//		#define LIGHTING_ON
//	#elif defined(QUALITY_BEAUTIFUL)
//		#define REFLECTIONS_ON
//		#define WATERMAPS_ON
//		//#define CALCULATE_NORMALS_ON
//		#define PERPIXEL_SPECULARITY_ON
//		#define MULTIPLE_SCALE_WAVES_ON
//		#define LIGHTING_ON
//		#define REFRACTIONS_ON
//	#elif defined(QUALITY_FANTASTIC)
//		#define LIGHTING_ON
//		#define PERPIXEL_SPECULARITY_ON
//		#define CAUSTICS_ON
//		#define CAUSTICS_ALL
//		#define FOAM_ON
//		#define REFLECTIONS_ON
//		#define WATERMAPS_ON
//		#define CALCULATE_NORMALS_ON
//		
//		#define REFRACTIONS_ON
//		#define USE_SECONDARY_REFRACTION
//	#endif
	
	//#undef FLOWMAP_ANIMATION_ON
	
	#ifdef FLOWMAP_ANIMATION_ON
		#define FLOWMAP_ALL_ON
	#endif
	
	#define FLAT_HORIZONTAL_SURFACE

	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "AutoLight.cginc"
	
	sampler2D _MainTex;
	float4 _MainTex_ST;
	
	#ifdef FOAM_ON
	//sampler2D _FoamTex;
	half _EdgeFoamStrength;
	#endif
	
	#ifdef USE_REFR_ADJUSTMENT
	half _RefrAdj;
	#endif
	
	sampler2D _HeightGlossMap;
	sampler2D _DUDVFoamMap;
	
	sampler2D _WaterMap;
	float4 _WaterMap_ST;
	
	#ifdef REFRACTIONS_ON
		#ifdef PRO_REFRACTIONS
			sampler2D _GrabTexture;
		#else
			#ifdef BAKED_REFRACTIONS
			sampler2D _RefractionMap;
			half4 _RefractionMap_ST;
			#endif
			
			#ifdef USE_SECONDARY_REFRACTION
			sampler2D _SecondaryRefractionTex;
			float4 _SecondaryRefractionTex_ST;
			half _refractionsWetness;
			#endif
		#endif
	
	half _Refractivity;
	#endif
	
	#ifdef FLOWMAP_ANIMATION_ON
		sampler2D _FlowMap;
		half flowMapOffset0, flowMapOffset1, halfCycle;
		//float flowSpeed;
		#ifdef FLOWMAP_TIDE
		//float flowTide;
		#endif
	#endif
	
	
	half _normalStrength;
	#ifdef CALCULATE_NORMALS_ON
	sampler2D _NormalMap;
	#endif
	
	#if defined(WATER_EDGEBLEND_ON) || defined(PRO_REFRACTIONS)
	sampler2D _CameraDepthTexture;
	#endif
	
	samplerCUBE _Cube;
	
	#ifdef BLEND_CUBEMAPS
	samplerCUBE _Cube2;
	half _CubemapBlend;
	#endif
	
	half _Reflectivity;
	half _WaterAttenuation;
	
	fixed3 _DeepWaterTint;
	fixed3 _ShallowWaterTint;
	
	//float _DeepWaterCoefficient;
	
	half _Shininess;
	half _Gloss;
	
	//float _yOffset;
	half _Fresnel0;
	
	//half _EdgeBlendStrength;
	
	#ifdef LIGHT_MODEL_ANISOTROPIC
	//half3 anisoLightPos;
	//half anisoAnimationOffset;
	sampler2D _AnisoMap;
	//half2 anisoDirAnimationOffset;
	#endif
	
	#ifdef CAUSTICS_ON
	half _CausticsStrength;
	half _CausticsScale;
	
	sampler2D _CausticsAnimationTexture;
	half3 causticsOffsetAndScale;
	half4 causticsAnimationColorChannel;
	#endif
	
	struct v2f {
    	float4  pos : SV_POSITION;
    	float2	uv_MainTex : TEXCOORD0;
    	
    	half2	uv_WaterMap : TEXCOORD1;
    	
    	fixed3	viewDir	: COLOR;
    	
    	#if defined(PERPIXEL_SPECULARITY_ON) || defined(LIGHTING_ON)
    	fixed3	lightDir : TEXCOORD2;
    	#else
    	//float3 specColor : COLOR;
    	#endif
    	
    	#ifdef ENABLE_SHADOWS
    	LIGHTING_COORDS(4,3)
    	#endif
    	
    	#ifdef REFRACTIONS_ON
	    	#ifdef PRO_REFRACTIONS
	    	half4 grabPassPos : TEXCOORD5;
	    	#else
		    	#ifdef BAKED_REFRACTIONS
		    	//half2 uv_RefrMap : TEXCOORD7;
		    	#endif
	    	float2 uv_SecondaryRefrTex : TEXCOORD5;
	    	#endif
    	#endif
    	
    	#ifdef LIGHT_MODEL_ANISOTROPIC
    	//float3 anisoDir : TEXCOORD5;
    	//float2 anisoParameters : TEXCOORD6;
	    	#ifdef BAKED_ANISOTROPY_DIR
	    	//float2 anisoDirUV : TEXCOORD4;
	    	#else
		    	#ifndef PERPIXEL_SPECULARITY_ON
		    	fixed3 anisoDir : TEXCOORD6;
		    	#endif
	    	#endif
    	#endif
    	
    	#ifndef FLAT_HORIZONTAL_SURFACE
    	fixed3	normal;		//In world space
    	fixed3  tangent;		//In world space
		fixed3  binormal : TEXCOORD7;
    	#endif
    	
    	
    	
    	
    	
    	#if defined(WATER_EDGEBLEND_ON) || defined(PRO_REFRACTIONS)
    	half4 screenPos : TEXCOORD9;
    	#endif
    	
    	
	};
	
	
	v2f vert (appdata_tan v)
	{
	    v2f o;
	    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	    o.uv_MainTex = TRANSFORM_TEX (v.texcoord, _MainTex);
	    
	    o.uv_WaterMap = TRANSFORM_TEX (v.texcoord, _WaterMap);
	    
	    //TANGENT_SPACE_ROTATION;
	    
		//o.viewDir = mul (rotation, ObjSpaceViewDir(v.vertex)); 
		//o.viewDir = mul ( rotation, normalize( float3(1, 0, 0) ) );
		//top: (-x; -z; y)
		//bottom: (0; 1; 0) (0;0;-1) (-1;0;0) (-x, z, -y)
		
	    o.viewDir = WorldSpaceViewDir(v.vertex);
	    //o.distToCamera = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex) ) / 10000.0;//length(o.pos - _WorldSpaceCameraPos);

		#ifndef FLAT_HORIZONTAL_SURFACE

		//Normalize here???
//				    o.tangent = normalize( mul( _Object2World, float4(v.tangent, 0.0) ).xyz );
//				    o.normal = normalize( mul( float4(v.normal, 0.0), _World2Object ).xyz );
//				    o.binormal = normalize( cross(o.normal, o.tangent) );// * v.tangent.w;
	    o.tangent = mul( _Object2World, half4(v.tangent.xyz, 0.0) );
	    o.normal = mul( half4(v.normal.xyz, 0.0), _World2Object );
	    o.binormal = cross(o.normal, o.tangent) * v.tangent.w;
	    
	    o.tangent = normalize(o.tangent);
	    o.normal = normalize(o.normal);
	    o.binormal = normalize(o.binormal);
		#endif

	    #if defined(PERPIXEL_SPECULARITY_ON) || defined(LIGHTING_ON)
	    //o.lightDir = normalize( mul(rotation, ObjSpaceViewDir(v.vertex) ) );//
	    o.lightDir = normalize(WorldSpaceLightDir( v.vertex ));
	    #else	//Per-vertex specularity
//				    float3 lightDir = normalize( mul(rotation, ObjSpaceViewDir(v.vertex) ) );//
//				    //float3 lightDir = normalize(WorldSpaceLightDir( v.vertex ));
//				    float nh = max (0, dot (float3(.0, 1.0, .0), normalize ( lightDir + normalize( o.viewDir ) ) ) );
//				    float spec = pow (nh, _Shininess * 32.0) * .5;
//				    spec = min(spec, 1.0);
//				    o.specColor = float3(spec, spec, spec);
	    #endif
	    
	    #ifdef LIGHT_MODEL_ANISOTROPIC
	   		#ifdef BAKED_ANISOTROPY_DIR
	    	//o.anisoDirUV = o.uv_WaterMap;// + anisoDirAnimationOffset;
	    	#else
	    		#ifndef PERPIXEL_SPECULARITY_ON
			    	#ifndef FLAT_HORIZONTAL_SURFACE
			    	o.anisoDir = normalize( cross(o.lightDir, o.normal) );
			    	#else
			    	o.anisoDir = normalize( cross(o.lightDir, fixed3(0.0, 1.0, 0.0) ) );
			    	#endif
			    #endif
	    	#endif
	    	
	    	#ifdef FLAT_HORIZONTAL_SURFACE
	    	//o.anisoDir = normalize( cross(o.lightDir, float3(0.0, 1.0, 0.0)) );
	    	
	    	//float3 viewProjected = o.viewDir - dot( o.viewDir, float3(0.0, 1.0, 0.0) ) * float3(0.0, 1.0, 0.0);
	    	//float3 lightProjected = dot( o.lightDir, float3(0.0, 1.0, 0.0) ) * float3(0.0, 1.0, 0.0) - o.lightDir; //lightProjected = -lightProjected
	    	#else
	    	//o.anisoDir = normalize( cross(o.lightDir, o.normal) );
	    	
	    	//float3 viewProjected = o.viewDir - dot( o.viewDir, o.normal ) * o.normal;
	    	//float3 lightProjected = dot( o.lightDir, o.normal ) * o.normal - o.lightDir;	//lightProjected = -lightProjected
	    	#endif
	    	
	    	//Prevent leakage of the highlights to the unlit side
		    //o.anisoParameters.x = dot( normalize(viewProjected), lightProjected);
		    
		    //Widen the speck shape as we get closer to the viewer
		    //o.anisoParameters.y = saturate(o.pos.w * .01);
		    //o.anisoParameters.y = length(o.viewDir) * .001;
    	#endif
	    
	    #if defined(WATER_EDGEBLEND_ON) || defined(PRO_REFRACTIONS)
		o.screenPos = ComputeScreenPos(o.pos);
	    #endif
	    
	    #ifdef REFRACTIONS_ON
	    	#ifdef PRO_REFRACTIONS
			    #if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0f;
				#endif
			
		    o.grabPassPos.xy = ( float2( o.pos.x, o.pos.y*scale ) + o.pos.w ) * 0.5;
			o.grabPassPos.zw = o.pos.zw;
			#else
				#ifdef BAKED_REFRACTIONS
				o.uv_RefrMap = TRANSFORM_TEX (v.texcoord, _RefractionMap);
				#endif
			
				#ifdef USE_SECONDARY_REFRACTION
				o.uv_SecondaryRefrTex = TRANSFORM_TEX (v.texcoord, _SecondaryRefractionTex);
				#endif
			#endif
	    #endif
	    
	    #ifdef ENABLE_SHADOWS
	    TRANSFER_VERTEX_TO_FRAGMENT(o);
	    #endif
	    
	    return o;
	}
	
	inline float GetLodLevel(float2 _uv) {
		float2 dx = ddx( _uv * 512.0 );	//Multiply by texture size
		float2 dy = ddy( _uv * 512.0 );
		float d = max( dot( dx, dx ), dot( dy, dy ) ) * 10.0;
		return d;//log2(d);//pow(d, 0.5);
	}
	
	#ifdef CAUSTICS_ON
	inline half CalculateCaustics(float2 uv_WaterMap, half waterAttenuationValue) {
		float4 causticsFrame = tex2D(_CausticsAnimationTexture, frac(uv_WaterMap * _CausticsScale) * causticsOffsetAndScale.zz + causticsOffsetAndScale.xy );
		
		return (causticsAnimationColorChannel.x * causticsFrame.r
				+ causticsAnimationColorChannel.y * causticsFrame.g
				+ causticsAnimationColorChannel.z * causticsFrame.b) * _CausticsStrength * (1.0 - waterAttenuationValue);
	}
	#endif
	
	inline half CalculateAttenuation(half sinAlpha, fixed3 normViewDir, half4 waterMapValue) {
		//return waterMapValue.r;
		//Apply parallax to get a more precise depth value
	    //Alpha = angle between viewDir and viewDir[projected]
	    
	    //sinAlpha = abs(sinAlpha);

//		#ifdef IS_MOBILE_ON
//		sinAlpha = min(sinAlpha, .2);	//Again, iPhone has problems with alpha blending
//		#else
//		sinAlpha = min(sinAlpha, 1.0);
//		#endif
		
			#ifdef BINARY_SEARCH_PARALLAX_ATTEN

			#else
				#ifdef IS_MOBILE_ON
			    //iPhone has an issue with low-value alphas
			    //float heightValue = max(waterMapValue.r + _yOffset, .03);	//0..1m deep
			    #else
			    //float heightValue = max(waterMapValue.r + _yOffset, .0);	//0..1m deep
			    float heightValue = waterMapValue.r;
			    #endif
			    
			    //return ( sinAlpha / (_WaterAttenuation * heightValue) );
			    return heightValue;//( sinAlpha / heightValue );
			    
		    #endif
	}
	
	inline fixed3 CalculateNormalInTangentSpace(half2 uv_MainTex, out half2 _displacedUV, fixed3 normViewDir,
												//half sinAlpha,
												half4 waterMapValue
												#ifdef FLOWMAP_ANIMATION_ON
												,half2 flowmapValue, half flowLerp, half flowSpeed
													#ifdef FLOWMAP_ADD_NOISE_ON
													,half flowmapCycleOffset
													#endif
												#endif
												)
	{
		//return float3(0.0, 0.0, 1.0);
		#ifdef CALCULATE_NORMALS_ON
//			#undef BINARY_SEARCH_PARALLAX_NORMALMAP_ON
//			#ifdef BINARY_SEARCH_PARALLAX_NORMALMAP_ON
//				float2 currentUV;	//Parallaxed UVs
//				float2 normalmapUV;	//Final UVs
//				
//				//lodLevel = 1.0;
//				//if (lodLevel < 1.0) {
//				//if (true) {
//					//float noiseValue = tex2D (_WaterMap, uv_MainTex * 5.0).g;
//					
//					float cosAlpha = sqrt( 1.0 - sinAlpha * sinAlpha);
//					float tanAlpha = sinAlpha / cosAlpha;
//					
//					float2 maxDeltaUV = normalize( float2(normViewDir.x, normViewDir.z) ) * .1 / tanAlpha;	//Max height = 1.0
//					float2 startUV = uv_MainTex;
//					float mapHeight;
//					float step = 0.5;
//					float currentOffset = 0.5;
//					
//					for (float i = 0.0; i < 1.0; i+= 0.1 ) {
//						currentUV = startUV + maxDeltaUV * i;
//						//if (currentUV.x > 1.0 || currentUV.x < 0.0 || currentUV.y < 0.0 || currentUV.y > 1.0)
//						//	break;
//							
//						mapHeight = 1.0 - tex2Dlod  (_HeightGlossMap, float4(currentUV, .0, .0) ).r;
//						
//						if (mapHeight >= 1.0 - i) {	//searchHeight = 1.0 - currentOffset
//							break;
//						}
//					}
//					
//					//maxDeltaUV = currentUV - startUV;
//					
////								for (int i = 0; i < 10; i++ ) {
////									//float cosAlpha = sqrt( 1.0 - sinAlpha * sinAlpha);
////									currentUV = startUV + maxDeltaUV * currentOffset;// * .3s;
////									//if (currentUV.x > 1.0 || currentUV.x < 0.0 || currentUV.y < 0.0 || currentUV.y > 1.0)
////									//	break;
////										
////									//mapHeight = tex2Dlod  (_WaterMap, float4(currentUV, .0, .0) ).r;
////									mapHeight = 1.0 - tex2Dlod  (_HeightGlossMap, float4(currentUV, .0, .0) ).r;
////									//mapHeight = 1.0 - tex2D  (_HeightGlossMap, currentUV).r * .1;// - noiseValue;
////									//mapHeight = 1.0 - lerp( tex2D(_HeightGlossMap, currentUV * .5).r, tex2D(_HeightGlossMap, currentUV).r, noiseValue);
////									//mapHeight = 1.0 - lerp( tex2D(_HeightGlossMap, currentUV * .5).r, tex2D(_HeightGlossMap, currentUV).r, tex2D (_WaterMap, currentUV).g);
////														
////									step *= 0.5;
////									//if (mapHeight < 1.0 - currentOffset) {	//searchHeight = 1.0 - currentOffset
////									if (mapHeight < 1.0 - currentOffset) {	//searchHeight = 1.0 - currentOffset
////										currentOffset += step;
////									}     else {
////										currentOffset -= step;
////									}
////								}
//					
//					//currentUV = startUV + maxDeltaUV * currentOffset * .1;
//
//					//i.uv_MainTex = currentUV;
//					
//					//Blend
//					normalmapUV = currentUV;//lerp(currentUV, uv_MainTex, clamp(lodLevel, 0.0, 1.0) );
//					
////							}     else {	//LOD >= 1.0 (no parallax)
////								normalmapUV = uv_MainTex;
////							}
//			#else
				float2 normalmapUV = uv_MainTex;
//			#endif
			
			_displacedUV = normalmapUV;
			
			//normalmapUV = uv_MainTex;
			//float2 normalmapUV = uv_MainTex;
			
			#ifdef MULTIPLE_SCALE_WAVES_ON
				#ifdef THREE_WAVE_SCALES_ON
				//float3 pNormal = lerp( tex2D (_HeightGlossDUDVMap, i.uv_MainTex * .37), tex2D (_HeightGlossDUDVMap, i.uv_MainTex * .5), waterMapValue.g);
				//pNormal = lerp( texcol, tex2D (_HeightGlossDUDVMap, i.uv_MainTex), waterMapValue.b);
				
				//float3 pNormal = lerp( UnpackNormal( tex2D(_NormalMap, normalmapUV * .37) ),
				//					UnpackNormal( tex2D(_NormalMap, normalmapUV * .5) ), waterMapValue.g * 5.0);
									
				
				//pNormal = lerp( pNormal,
				//					UnpackNormal( tex2D(_NormalMap, normalmapUV) ), waterMapValue.b);
				#else
				//float3 pNormal = lerp( UnpackNormal( tex2D(_NormalMap, normalmapUV * .37) ),
				//					UnpackNormal( tex2D(_NormalMap, normalmapUV) ), waterMapValue.b * .5);
				#endif
			//pNormal.y *= .1;
			pNormal = normalize( pNormal );
			#else
				#ifdef FLOWMAP_ANIMATION_ON	
					#ifdef FLOWMAP_ADD_NOISE_ON
					flowmapCycleOffset *= .2;

					fixed3 normalT0 = UnpackNormal( tex2D(_NormalMap, normalmapUV + flowmapValue * (flowmapCycleOffset * .5 + flowMapOffset0) ) );
					fixed3 normalT1 = UnpackNormal( tex2D(_NormalMap, normalmapUV + flowmapValue * (flowmapCycleOffset * .5 + flowMapOffset1) ) );
					#else
					fixed3 normalT0 = UnpackNormal( tex2D(_NormalMap, normalmapUV + flowmapValue * flowMapOffset0 ) );
					fixed3 normalT1 = UnpackNormal( tex2D(_NormalMap, normalmapUV + flowmapValue * flowMapOffset1 ) );
					#endif
					
				fixed3 pNormal = lerp( normalT0, normalT1, flowLerp );
				
				//Account for speed
				pNormal.z /= max(flowSpeed, .1);	//Account for flow map average velocity
				//pNormal.z = tex2D( _WaterMap, normalmapUV * .1 ).g;
				#else
				fixed3 pNormal = UnpackNormal( tex2D(_NormalMap, normalmapUV) );
				#endif
			#endif
			
			pNormal.z /= _normalStrength;
			//pNormal.z /= _normalStrength * .05;
			pNormal = normalize(pNormal);	//Very very important to normalize!!!
			
			return pNormal;
		#else
		
			return fixed3(0.0, 0.0, 1.0);
		#endif
	}
	
	#ifdef FLOWMAP_ANIMATION_ON
	inline fixed4 SampleTextureWithRespectToFlow(sampler2D _tex, float2 _uv, half2 flowmapValue, half flowLerp
													#ifdef FLOWMAP_ADD_NOISE_ON
													, flowmapCycleOffset
													#endif
													) {
		#ifdef FLOWMAP_ADD_NOISE_ON
			fixed4 texT0 = tex2D(_tex, _uv + flowmapValue * (flowmapCycleOffset * .5 + flowMapOffset0) );
			fixed4 texT1 = tex2D(_tex, _uv + flowmapValue * (flowmapCycleOffset * .5 + flowMapOffset1) );
		#else
		  	fixed4 texT0 = tex2D(_tex, _uv + flowmapValue * flowMapOffset0 );
			fixed4 texT1 = tex2D(_tex, _uv + flowmapValue * flowMapOffset1 );
		#endif
		
		return lerp( texT0, texT1, flowLerp );
	}
	#endif
	
	#ifdef REFRACTIONS_ON
		#ifdef PRO_REFRACTIONS
		inline fixed3 CalculateRefraction(half2 uv_WaterMap, fixed3 normViewDir, fixed3 pNormal, half4 grabPassPos) {
			fixed3 bump = pNormal.xxy * fixed3(1,0,1) * .1;	//* refraction strength
			float4 distortOffset = float4(bump.xz * 10.0, 0, 0);
			float4 grabWithOffset = grabPassPos + distortOffset;
			
			//!!!!!!!!!!!!!!!!
			//If depth < w then discard refraction (refract only objects behind water)
			//!!!!!!!!!!!!!!!!
			return tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(grabWithOffset) ).rgb;
		}
		#else
			inline fixed3 CalculateRefraction(
												#ifdef BAKED_REFRACTIONS
												half2 uv_RefrMap,
												#endif
												float2 uv_WaterMap,
												#ifdef USE_SECONDARY_REFRACTION
												half refrStrength,
												float2 uv_SecondaryRefrTex,
												#endif
												
												#ifdef CAUSTICS_ON
												half waterAttenuationValue,
												#endif
												fixed3 normViewDir,
												// half sinAlpha,
												float2 _dudvValue)
			{
				//Unpack and scale
				float2 dudvValue = _dudvValue * _Refractivity / 100000.0;
				
				//float2 mainTexRefractedUV = uv_MainTex + dudvValue * _MainTex_ST.xy * .05;
				
				fixed3 refractionColor;
				
				//return tex2D(_SecondaryRefractionTex, uv_SecondaryRefrTex + dudvValue).rgb;
				
				//Flat
				//return tex2D(_RefractionMap, uv_WaterMap).rgb;
				#ifdef BAKED_REFRACTIONS
					#ifdef USE_SECONDARY_REFRACTION
						fixed3 mainRefr = tex2D(_RefractionMap, uv_RefrMap + dudvValue * _RefractionMap_ST.x).rgb;
						
						fixed3 secondaryRefr = tex2D(_SecondaryRefractionTex, uv_SecondaryRefrTex + dudvValue * _SecondaryRefractionTex_ST.x).rgb * _refractionsWetness;
						
						refractionColor = lerp(mainRefr, secondaryRefr, refrStrength);
					#else
						refractionColor = tex2D(_RefractionMap, uv_RefrMap + dudvValue * _RefractionMap_ST.x).rgb;
					#endif
				#else
					#ifdef USE_SECONDARY_REFRACTION
						refractionColor = tex2D(_SecondaryRefractionTex, uv_SecondaryRefrTex + dudvValue * _SecondaryRefractionTex_ST.x).rgb * _refractionsWetness;
						//refractionColor = secondaryRefr;
					#endif
				#endif
				
				//refractionColor *= _ShallowWaterTint.rgb;
				
				#ifdef CAUSTICS_ON
					refractionColor += CalculateCaustics(uv_WaterMap + dudvValue, waterAttenuationValue);
					//refractionColor += CalculateCaustics(uv_MainTex + dudvValue, 0.0);
				#endif
				
				//refractionColor = lerp(refractionColor, _DeepWaterTint, refrStrength);
				
				return refractionColor;
				
//				//float slope
//				//x = (c-b)/(m-k)
//				//(0;0) at UV center
//				
//				float cosAlpha = sqrt( 1.0 - sinAlpha * sinAlpha);
//				float tanAlpha = sinAlpha / cosAlpha;
////					
////					//return float3 (tanAlpha, tanAlpha, tanAlpha);
////					
////					float totalX = length(uv_WaterMap - float2 (0.5, 0.5) );
////					//return float3(totalX, totalX, totalX) * .1;
////					
////					float shoreSlope = -1.0;
////					float bottomDepth = 1.0;
////					float islandHeight = 10.1;
////					
////					//0,1..0,3*(-1) + 1	
////					float dirB = (-totalX * tanAlpha + islandHeight);
////					float shoreB = 0.0;
////					
////					float x = (shoreB - dirB) / (tanAlpha - shoreSlope);
////					float dx = totalX - x;
////					
////					float2 viewDir2D = normalize( float2(normViewDir.x, normViewDir.z) ) * .01;
////					
////					return tex2D(_RefractionMap, uv_WaterMap + viewDir2D * dx).rgb;
//				
//				float3 refractionColor;
//				//return refractionColor;
//
//			
//				float2 currentUV;	//Parallaxed UVs
//				float2 normalmapUV;	//Final UVs
//				
//				//uv_WaterMap.x = 1.0 - uv_WaterMap.x;
//				//uv_WaterMap.y = 1.0 - uv_WaterMap.y;
//				
//				//return tex2Dlod(_RefractionMap, float4(uv_WaterMap.x, uv_WaterMap.y, .0, .0) ).rgb;
//				//lodLevel = 0.0;
//						
//				//if (lodLevel < 1.0) {	//Apply refraction
//				//if (true) {
//					float3 refractionVector = normViewDir;
//					//float3 refractionVector = refract(normViewDir, pNormal, 1.01);	//Normalize??????
//					
//					//float3 refractionVector = refract(normViewDir, pNormal, 0.75);
//					//float3 refractionVector = refract(normViewDir, float3(0.0, 1.0, 0.0) , 1.333);
//					
//					//return texCUBE( _Cube , refractionVector ).rgb;
//					
//					//float sinAlpha = dot( float3(.0, 1.0, .0), refractionVector);
//					//sinAlpha = max(0.0, sinAlpha);
//
//					
//					float2 startUV;// = uv_WaterMap + dudvValue;
//					
//					float2 maxDeltaUV = normalize( float2(refractionVector.x, refractionVector.z) ) * .02 / tanAlpha;	//Max height = 1.0
//					
//					float mapHeight;
//					float currentOffset = 0.0;
//					float previousOffset;
//					
//					float deltaOffset = .01;
//					
//					//Linear search first
//					for (int i = 0; i < 1/deltaOffset; i++ ) {
//						previousOffset = currentOffset;
//						currentUV = startUV + maxDeltaUV * currentOffset;
//						mapHeight = tex2Dlod  (_WaterMap, float4(currentUV, .0, .0) ).r;
//
//						//depth = 1; h = 0;
//						//offset = 0.1
//						
//						if (mapHeight > currentOffset) {	//searchHeight = 1.0 - currentOffset
//							currentOffset += deltaOffset;
//						}     else {
//							break;
//						}
//					}
//					
//					float step = 0.5;
//					//Then binary search
//					for (int i = 0; i < 10; i++ ) {
//						currentUV = startUV + maxDeltaUV * currentOffset;
//						
//						mapHeight = tex2Dlod  (_WaterMap, float4(currentUV, .0, .0) ).r;
//						if (mapHeight == currentOffset) {
//							break;
//						}     else if (mapHeight < currentOffset) {	//searchHeight = 1.0 - currentOffset
//							//best_height = currentOffset;
//							currentOffset -= step;
//						}     else {
//							currentOffset += step;
//						}
//						
//						step *= 0.5;
//					}
//					
//					//currentUV = startUV + maxDeltaUV * (mapHeight - _yOffset * 1.1 * 0 - .05);
//					
//					//float3 resultColor = tex2Dlod(_RefractionMap, float4(currentUV, .0, .0) ).rgb;
//					//float3 resultColor = tex2Dlod(_RefractionMap, float4(currentUV * 1.0 + dudvValue, .0, .0) ).rgb;
//					
//					//currentUV += dudvValue;
//					
//					//return float3(1.0, 0.0, 0.0);
//					float3 resultColor = tex2D(_RefractionMap, currentUV).rgb;
//					
//					return resultColor;
//					//Blend
//					//refractionColor = lerp(resultColor, refractionColor, clamp(lodLevel, 0.0, 1.0) );
//				//}
//				
//				//return refractionColor;
			}
		#endif
	#endif
	
	inline fixed3 CombineEffectsWithLighting(
							#ifdef REFRACTIONS_ON
								fixed3 refraction, half refrStrength,
							#endif
							#ifdef REFLECTIONS_ON
								fixed3 reflection,
							#endif	
								fixed3 pNormal,
									
							fixed3 normViewDir,
							#ifdef LIGHTING_ON
							fixed3 normLightDir,
							#endif
							half2 uv_MainTex, half waterAttenuationValue
							#ifdef FOAM_ON
							,inout half foamAmount,
								fixed foamValue
							#endif
							#ifdef LIGHTING_ON
								#ifdef LIGHT_MODEL_ANISOTROPIC
									#ifndef PERPIXEL_SPECULARITY_ON
										#ifdef BAKED_ANISOTROPY_DIR
										,half2 anisoDirUV
										#else
										,fixed3 anisoDir
										#endif
									#else
										,fixed3 lightDir
									#endif
								#endif
							#endif
							//#ifdef FLOWMAP_ANIMATION_ON
							//,float2 flowmapValue, float flowLerp, float flowmapCycleOffset
							//#endif
							)
	{
	half nDotView = dot(pNormal, normViewDir);		//Masking
	#ifdef LIGHTING_ON
			//return refraction;
		//float nDotView = dot(pNormal, normViewDir);
		//#ifdef PERPIXEL_SPECULARITY_ON
			#ifndef LIGHT_MODEL_ANISOTROPIC
		    fixed3 halfView = normalize ( normLightDir + normViewDir );	//No need in anisotropic!!!!!!!!!
		    half nDotHalf = saturate( dot (pNormal, halfView) );
		    half spec = pow (nDotHalf, _Shininess * 128.0) * _Gloss * 10;
		    #endif
		    
		    
		    #ifndef LIGHT_MODEL_FASTEST
		    //half nDotView = dot(pNormal, normViewDir);		//Masking
		    half nDotLight = dot(pNormal, normLightDir);	//Shadows (diffuse)
		    #endif
		    
//		    #if defined(LIGHT_MODEL_FASTEST)
//		    float spec = pow (nDotHalf, _Shininess * 128.0) * _Gloss;
//		    #elif defined(LIGHT_MODEL_FAST)
//		    //Blinn-Phong with microfacets masking
//		    float spec = pow (nDotHalf, _Shininess * 128.0) * _Gloss;
//		    spec *= saturate(nDotView) * saturate(nDotLight);
//		   	//spec /= 4.0 * min(nDotView, nDotLight);	//Geometry term
//		   	#elif defined(LIGHT_MODEL_ANISOTROPIC)
			#ifdef LIGHT_MODEL_ANISOTROPIC
		   	//float3 anisoDir = anisoDir;
		   	//anisoDir = normalize( float3(1.0, 0.0, .0) );
		   	//o.anisoDir - dot(o.anisoDir, o.normal) * o.normal;
		   	//float3 strongestNormal = normLightDir - anisoDir * dot(normLightDir, anisoDir);		//Calculate in vert
		   	
		   	//float anisoDotfloat = dot(anisoDir, halfView);
		   	
		   	//float aniso = sqrt(1.0 - anisoDotfloat * anisoDotfloat);
		   	
		   	//float spec = pow ( dot(strongestNormal, halfView), _Shininess * 128.0) * _Gloss;
		    //spec *= saturate(nDotView) * saturate(nDotLight);
		    //return float3( sin(anisoDirUV.x),  sin(anisoDirUV.y), 1.0);
		    
		    #ifndef PERPIXEL_SPECULARITY_ON
			    #ifdef BAKED_ANISOTROPY_DIR
			    fixed3 anisoDir = tex2D(_AnisoMap, anisoDirUV).rgb * 2.0 - 1.0;
			    #else
			    //half3 anisoDir = half3(1.0);
			    #endif
		    #else
		    fixed3 anisoDir = normalize( cross(pNormal, lightDir) );
		    #endif
		    
		    half lightDotT = dot(normLightDir, anisoDir);
		    half viewDotT = dot(normViewDir, anisoDir);
		    
			    #ifdef BAKED_ANISOTROPIC_LIGHTING
			    //float spec = tex2Dlod(_AnisoMap, float4( ( float2(lightDotT, viewDotT) + 1.0 ) * .5, 0.0, 0.0) ).a;
			    half spec = tex2D(_AnisoMap, ( float2(lightDotT, viewDotT) + 1.0 ) * .5).a;
			    //float spec = tex2Dlod(_AnisoMap, ( float2(lightDotT, viewDotT) + 1.0 ) * .5, 0.0, 0.0).a;
			    #else
			    half spec = sqrt(1.0 - lightDotT * lightDotT) * sqrt(1.0 - viewDotT * viewDotT) - lightDotT * viewDotT;
			    spec = pow(spec, _Shininess * 128.0);
			    #endif
			    
			    spec *= _Gloss;
		    
		    //Masking & self-shadowing
		    spec *= max(.0, nDotView) * max(.0, nDotLight);
		    //spec *= saturate(nDotView + .2) * saturate(nDotLight + .2);		//Should account for water translucency
		    
		    //spec *= saturate(nDotView * nDotLight + .2);
		    
		    //Prevent highlights from leaking to the wrong side of the light
		    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		    spec *= max( sign( dot(normViewDir, -normLightDir) ), 0.0 );

		    		    
		    //return float3(1.0) * max( sign(anisoParameters.x), 0.0 );
		    
		    //return float3(1.0) * anisoParameters.y;
		    
		    //spec = max(spec, 0.0);
			
			//spec = anisoParameters.x;
		    //spec *= max( sign(nDotView), 0.0);
		    //if (nDotLight <= 0.0)
		   	//	spec = 0.0;
		   	#endif
		    
//		    #elif defined(LIGHT_MODEL_GOOD)
//		    //Blinn-phong distribution term
//		    float distrTerm = pow (nDotHalf, _Shininess * 128.0) * _Gloss;
//		    
//		    //Cook-Torrance geometry term
//		    float geometryTerm = (2.0 * ( nDotHalf/dot(normViewDir, halfView) ) *  min(nDotView, nDotLight) );
//		    geometryTerm = min(1.0, geometryTerm);
//			
//			float spec = geometryTerm * distrTerm / (4.0 * nDotView * nDotLight);
//			#elif defined(LIGHT_MODEL_FANTASTIC)
//		    //Blinn-phong distribution term
//		    float distrTerm = pow (nDotHalf, _Shininess * 128.0) * _Gloss;
//		    
//		    //Cook-Torrance geometry term
//		    float geometryTerm = (2.0 * ( nDotHalf/dot(normViewDir, halfView) ) *  min(nDotView, nDotLight) );
//		    geometryTerm = min(1.0, geometryTerm);
//			
//			float spec = geometryTerm * distrTerm / (4.0 * nDotView * nDotLight);
//			#else
//			float spec = .0;					
//		    #endif
		    
		    //Saving the extra instruction
		    #ifndef ENABLE_SHADOWS
		    //Changed to fixed
		    fixed specularComponent = spec * _LightColor0.r;
		    #else
		    fixed specularComponent = spec;
		    #endif
		    
		    //nDotView = nDotView * .5 + .5;
		    //nDotLight = nDotLight * .5 + .5;
		    //specularComponent = float3(nDotLight, nDotLight, 1.0);
	   		//return specularComponent;
	   	//#else
	   	//	float3 specularComponent = spec * _LightColor0.rgb;
	    //#endif
	    #endif
	    
	    //return 1.0 - float3(nDotView, nDotView, nDotView);
	    
	    //float fresnel = _Fresnel0 + (1.0 - _Fresnel0) * pow( (1.0 - dot(halfView, normLightDir) ), 5.0);
	    //float fresnel = _Fresnel0 + (1.0 - _Fresnel0) * pow( (1.0 - dot(halfView, normViewDir) ), 5.0);
	    #ifdef FRESNEL_ON
		    #ifdef SIMPLIFIED_FRESNEL
		    half fresnel = .5 - nDotView;
		    	#ifdef USE_FRESNEL0_IN_SIMPLIFIED
		    	fresnel += _Fresnel0;
		    	#endif
		    fresnel = max(0.0, fresnel);
		    #else
		  	half fresnel = _Fresnel0 + (1.0 - _Fresnel0) * pow( (1.0 - nDotView ), 5.0);
		  	fresnel = max(0.0, fresnel - .1);
		  	#endif
		  	//float fresnel = _Fresnel0 + (1.0 - _Fresnel0) * pow( (1.0 - dot( float3(0.0, 1.0, 0.0), normViewDir) ), 5.0);
		  	
		  	//fresnel = clamp(fresnel, 0.0, _Reflectivity);
		  	
	//				  	#if defined(FLOWMAP_ANIMATION_ON) && defined(FLOWMAP_ALL_ON)
	//					  	#ifdef FLOWMAP_ADD_NOISE_ON
	//						float glossT0 = tex2D(_HeightGlossDUDVMap, uv_MainTex + flowmapValue * (flowmapCycleOffset * .5 + flowMapOffset0) ).g;
	//						float glossT1 = tex2D(_HeightGlossDUDVMap, uv_MainTex + flowmapValue * (flowmapCycleOffset * .5 + flowMapOffset1) ).g;
	//						#else
	//					  	float glossT0 = tex2D(_HeightGlossDUDVMap, uv_MainTex + flowmapValue * flowMapOffset0 ).g;
	//						float glossT1 = tex2D(_HeightGlossDUDVMap, uv_MainTex + flowmapValue * flowMapOffset1 ).g;
	//						#endif
	//						float glossValue = lerp( glossT0, glossT1, flowLerp );
	//				  	#else
	//				    float glossValue = tex2D(_HeightGlossDUDVMap, uv_MainTex).g;
	//				    #endif
	//				    glossValue = min(0.8, glossValue);
		    //glossValue = .5;
		    //specularComponent *= glossValue * fresnel;
		   	#ifdef LIGHTING_ON
		    specularComponent *= fresnel;
		    #endif
		#endif
	    
	    #ifdef LIGHTING_ON
	    specularComponent = specularComponent * specularComponent * 10.0;
	    #endif
	    
	    //specularComponent = 1.0 - specularComponent;
	    //specularComponent = 1.0 - specularComponent * specularComponent;
	    
	    //return specularComponent;
	    
	    //return float3(fresnel, fresnel, fresnel);
	    
		fixed3 finalColor;
	    //finalColor = lerp(_DeepWaterTint, _ShallowWaterTint, saturate(waterAttenuationValue * 1.0) );
	    
	    //#ifdef LINEAR_COLOR_SPACE
	    //_ShallowWaterTint.rgb *= .45;
	    //_DeepWaterTint.rgb *= .45;
	    //#endif
	    
	    //#ifdef WATERMAPS_ON
	    finalColor = lerp(_ShallowWaterTint, _DeepWaterTint, waterAttenuationValue );
	   
	    #ifdef REFRACTIONS_ON
	    	#ifdef USE_SECONDARY_REFRACTION
	    	
		    	//#ifndef USE_REFR_ADJUSTMENT
		    	//#define USE_REFR_ADJUSTMENT
		    	//half _RefrAdj = .7;
	    		//#endif
		    	
		    	//!!!!!!!!!!!!!!!!!!!!
		    	//!Magic! Don't touch!
		    	//!!!!!!!!!!!!!!!!!!!!
		    	
		    	refraction = lerp(refraction, _ShallowWaterTint, refrStrength * .5);
		    	#ifndef USE_REFR_ADJUSTMENT
			    	finalColor = lerp(refraction, finalColor, saturate( max(waterAttenuationValue, refrStrength * .5) * .8 ) );
			    #else
			    	//finalColor = lerp(refraction, finalColor, saturate( max(waterAttenuationValue, refrStrength * .5) * .8 * _RefrAdj ) );
			    	half refrAmount = saturate( max(waterAttenuationValue, refrStrength * .5) * .8 );
			    	refrAmount = lerp(refrAmount, 1.0, _RefrAdj);
			    	finalColor = lerp(refraction, finalColor, refrAmount);
		    	#endif
		    	//finalColor = lerp(refraction, finalColor, refrStrength);
		    	//return finalColor;
		    	//finalColor = lerp(refraction, finalColor, waterAttenuationValue);
		    	//finalColor = lerp(refraction, finalColor, refrStrength);
		    	//finalColor = lerp(refraction, finalColor, saturate( refrStrength + waterAttenuationValue ) );
		    #else
		    	finalColor = lerp(refraction, finalColor, refrStrength); 
	    	#endif
	    #else
	    	#ifndef WATERMAPS_ON
	   		finalColor = tex2D(_MainTex, uv_MainTex).rgb;
	   		#endif
	    #endif
	    
	    #ifdef CAUSTICS_ALL
	    //finalColor += CalculateCaustics(uv_MainTex, 1.0 - refrStrength);
	    #endif
	    
	    
	    //float specIntensity = tex2D( _WaterMap, (uv_MainTex + nDotHalf * 100.0) * .01 ).b * tex2D( _WaterMap, (uv_MainTex + nDotHalf * 100.0) * .1 ).b;// *
	   	//					 tex2D( _WaterMap, (uv_MainTex + nDotHalf) *.3 ).b;
	   	//float specIntensity = dot( float3(uv_MainTex.x, 0.0, uv_MainTex.y), normViewDir);
	    //finalColor += min( .7, pow(specIntensity * 1.0, 2.0) );
	    
	    //
	    //We want soft edges - near the edge color=refraction
	    //float deepWaterAmount = lerp(0.0, 1.0, (1.0 - waterAttenuationValue) * .5 );
	    //float waterBorderColor = lerp(0.0, 1.0, .45 );
	    //float shallowWaterAmount = lerp(0.0, .45, waterAttenuationValue * 10.0 );
	    
	    //
	    //finalColor = waterDeepColor;
	    //finalColor = lerp(waterShallowColor, waterDeepColor, saturate( (waterAttenuationValue - .1) * 100000.0) );
	    //float deepWaterAmount = .5 * (1.0 + waterAttenuationValue);
	    //float shallowWaterAmount = .55 * ( 1.0 - waterAttenuationValue);
	    
	    
	    
	    
	    
//	    float deepWaterAmount = (1.0 - waterAttenuationValue) * .5;
//	    float shallowWaterAmount = 4.5 * waterAttenuationValue;
//	    float combinedDepthAmount = lerp(shallowWaterAmount, deepWaterAmount, saturate( (waterAttenuationValue - .1) * 100000.0) );
//
//		finalColor = lerp(finalColor, _ShallowWaterTint, combinedDepthAmount);


		//finalColor = finalColor + (_ShallowWaterTint - finalColor) * waterAttenuationValue / .2;
	    
//	    if (waterAttenuationValue > .1)
//		    finalColor = waterDeepColor;
//	    else {
//	   		//finalColor = lerp(finalColor, _ShallowWaterTint, .45 );
//	    	//float3 water1 = lerp(finalColor, _ShallowWaterTint, .45 );
//	    	finalColor = lerp(finalColor, waterBorderColor, waterAttenuationValue * 10.0 );
//	    }
	    
	    //Add reflection
	    #ifdef REFLECTIONS_ON
		    #ifdef FRESNEL_ON
		    //finalColor = lerp(finalColor, reflection, clamp(fresnel, 0.0, _Reflectivity) );
		    finalColor = lerp(finalColor, reflection, clamp(fresnel, 0.0, _Reflectivity) );
		    #else
		    finalColor = lerp(finalColor, reflection, _Reflectivity);
		    #endif
		#endif
	    
	    //Foam isn't reflective, it goes on top of everything
	    #ifdef FOAM_ON
		    //if (foamAmount > 0.0) {
	    	//float4 foamColor = tex2D(_FoamTex, uv_MainTex);
	    	foamAmount = saturate(foamAmount * foamValue);
	    	//foamAmount = foamAmount * foamColor.r;
	    	//foamAmount = foamAmount * foamAmount;
			finalColor.rgb = lerp(finalColor, fixed3(foamValue, foamValue, foamValue), foamAmount);
	    	
	    	//finalColor = lerp(finalColor, float3(1.0), foamAmount);
	    	//finalColor = lerp(finalColor, tex2D(_FoamTex, uv_MainTex), foamAmount);
		    //}
	    #endif
	    
	    //return refraction;
	    //float3 finalColor = lerp(_ShallowWaterTint, _DeepWaterTint, waterAttenuationValue + .2);
	    
	    //float3 finalColor = reflection;
	    
	    //return float3(waterAttenuationValue, waterAttenuationValue, waterAttenuationValue);
	    
	    //return finalColor;
	    //#define ADD_DIFFUSE_LIGHT_COMPONENT
	    
	    #ifdef LIGHTING_ON
		    #if defined(ADD_DIFFUSE_LIGHT_COMPONENT)
		    //float silhouetteness = max(.8, 1.0 - abs( nDotView ) );
			//fixed3 diffuseComponent = (nDotLight * .2 + 1.0) * _LightColor0.rgb;// * silhouetteness;
		    float3 diffuseComponent = nDotLight * _LightColor0.rgb;// * silhouetteness;
		    
		    return (finalColor * diffuseComponent + specularComponent) * 2.0 + UNITY_LIGHTMODEL_AMBIENT.rgb;
		    //return (finalColor * _LightColor0.rgb + specularComponent) * 2.0 + UNITY_LIGHTMODEL_AMBIENT.rgb;
		    #else
		    //return finalColor + specularComponent * 2.0 + UNITY_LIGHTMODEL_AMBIENT;
		    //return specularComponent * 2.0;//(finalColor * _LightColor0.rgb + specularComponent) * 2.0 + UNITY_LIGHTMODEL_AMBIENT.rgb;
		    return (finalColor * _LightColor0.rgb + specularComponent) * 2.0 + UNITY_LIGHTMODEL_AMBIENT.rgb;
		    #endif
		#else
			return finalColor;
		#endif
	}

	fixed4 frag (v2f i) : COLOR
	{
		fixed4 outColor;
		
		//return fixed4(tex2D (_MainTex, i.uv_MainTex).rgb, 0.3);
		
		//return float4( (i.anisoDir + float3(1.0, 1.0, 1.0) ) * .5, 1.0);
		
		//return float4( GetLodLevel(i.uv_WaterMap), GetLodLevel(i.uv_WaterMap), GetLodLevel(i.uv_WaterMap), 1.0);

		#ifdef WATERMAPS_ON
		fixed4 waterMapValue = tex2D (_WaterMap, i.uv_WaterMap);
		#else
		fixed4 waterMapValue = fixed4(0.5, 0.0, 0.0, 0.0);
		#endif
		
		//#ifndef IS_MOBILE
		fixed3 normViewDir = normalize(i.viewDir);
		//return float4(normViewDir.x, normViewDir.y, normViewDir.z, alpha) * 0.5 + 0.5 * float4(1.0, 1.0, 1.0, alpha);
		//#endif
		
//		#if defined(WATERMAPS_ON) || defined(CALCULATE_NORMALS_ON)
//			#ifndef FLAT_HORIZONTAL_SURFACE
//		    half sinAlpha = dot(i.normal, normViewDir);	//cos (beta) = n * b / (|n| * |b|)
//		    #else
//		    half sinAlpha = dot( half3(0.0, 1.0, 0.0), normViewDir);
//		    #endif
//	    sinAlpha = max(0.0, sinAlpha);
//	    #endif
	    
	    #ifdef FLOWMAP_ANIMATION_ON
//	   		#ifdef FLOWMAP_TIDE
//	   		float2 flowmapValue;
//	   		float flowSpeed;
//	   		
//	   		//if (waterMapValue.r < .5) {
//			    float2 flowmapValue0 = tex2D (_FlowMap, i.uv_WaterMap).rg * 2.0 - 1.0;
//			    float2 flowmapValue1 = 1.0 - tex2D (_FlowMap, i.uv_WaterMap).rg * 2.0;
//			    
//			    flowmapValue = lerp(flowmapValue0, flowmapValue1, flowTide);
//		    
//		    	flowSpeed = length(flowmapValue0);
////					    } else {
////					    	flowmapValue = tex2D (_FlowMap, i.uv_WaterMap).rg * 2.0 - 1.0;
////					    	flowSpeed = length(flowmapValue);
////					    }
//			#else
	    	half2 flowmapValue = tex2D (_FlowMap, i.uv_WaterMap).rg * 2.0 - 1.0;
	    	half flowSpeed = length(flowmapValue);
//	    	#endif
	    	
	    	half flowLerp = ( abs( halfCycle - flowMapOffset0 ) / halfCycle );
	    	
	    	#ifdef FLOWMAP_ADD_NOISE_ON
	    	half flowmapCycleOffset = 0.0;// * tex2D( _WaterMap, i.uv_MainTex * .3).g;	//Noise
	    	#endif
	    	
	    	//return float4(flowmapCycleOffset, flowmapCycleOffset, flowmapCycleOffset, 1.0);
	    #endif
	    
	    #ifdef CALCULATE_NORMALS_ON
	 	   	//float3 pNormal = UnpackNormal( tex2D(_NormalMap, normalmapUV) );
	    	half2 displacedUV;
	   		//#ifdef BINARY_SEARCH_PARALLAX_NORMALMAP_ON
			//float lodLevel = GetLodLevel( i.uv_WaterMap );
			fixed3 pNormal = CalculateNormalInTangentSpace(i.uv_MainTex, displacedUV, normViewDir,
														//sinAlpha,
														waterMapValue
														#ifdef FLOWMAP_ANIMATION_ON
														,flowmapValue, flowLerp, flowSpeed
															#ifdef FLOWMAP_ADD_NOISE_ON
															,flowmapCycleOffset
															#endif
														#endif
														);
	    	//#else
	    	//float3 pNormal = CalculateNormalInTangentSpace(i.uv_MainTex, displacedUV, normViewDir, sinAlpha, waterMapValue);
	    	//#endif
	    	
	    	#ifndef FLAT_HORIZONTAL_SURFACE
	    	//pNormal = float3(0.0, 0.0, 1.0);
	    	//Convert to world space from tangent space
	    	pNormal = (i.tangent * pNormal.x) + (i.binormal * pNormal.y) + (i.normal * pNormal.z);
	    	//pNormal = normalize(pNormal);
	    	#else
	    		pNormal = fixed3(-pNormal.x, pNormal.z, -pNormal.y);
	    	#endif						
		#else
			#ifndef FLAT_HORIZONTAL_SURFACE
				fixed3 pNormal = i.normal;
			#else
				fixed3 pNormal = fixed3(0.0, 1.0, 0.0);
			#endif
	    #endif
	    
	    
	   //return float4(pNormal * 2 - float3(1,1,1), 1.0);
	    
	    //
	    //Attenuation calculations
	    //#ifdef WATERMAPS_ON
		//float waterAttenuationValue = saturate( CalculateAttenuation(sinAlpha, normViewDir, waterMapValue ) * _WaterAttenuation);
		half waterAttenuationValue = saturate( waterMapValue.r * _WaterAttenuation );
		//float deepwaterCorrection = min(waterAttenuationValue * _DeepWaterCoefficient, 1.0);
	    //#else
	    //half waterAttenuationValue = _WaterAttenuation;
	    //#endif
	    
	    //
	    //Sample dudv/foam texture
	    #if defined(REFRACTIONS_ON) || defined(FOAM_ON)
		    #if defined(FLOWMAP_ANIMATION_ON) && defined(FLOWMAP_ALL_ON)
				fixed3 dudvFoamValue = SampleTextureWithRespectToFlow(_DUDVFoamMap, i.uv_MainTex, flowmapValue, flowLerp
																		#ifdef FLOWMAP_ADD_NOISE_ON
																		, flowmapCycleOffset
																		#endif
																		).rgb;
				
				//dudvValue *= max(flowSpeed, 0.2);	//Account for flow speed
				
			#else
				fixed3 dudvFoamValue = tex2D(_DUDVFoamMap, i.uv_MainTex).rgb;
			#endif
		#endif
	     
     	#ifdef REFRACTIONS_ON
	     	float2 dudvValue = dudvFoamValue.rg;
			dudvValue = dudvValue * 2.0 - float2(1.0, 1.0);
			//dudvValue = dudvValue * _Refractivity / 100000.0;
		
//					inline float3 CalculateRefraction(
//												#ifdef BAKED_REFRACTIONS
//												float2 uv_RefrMap,
//												#endif
//												float2 uv_MainTex,
//												#ifdef USE_SECONDARY_REFRACTION
//												float refrStrength,
//												float2 uv_SecondaryRefrTex,
//												#endif
//												
//												#ifdef CAUSTICS_ON
//												float waterAttenuationValue,
//												#endif
//												float3 normViewDir, float sinAlpha, float2 _dudvValue)
		
    		#ifdef PRO_REFRACTIONS
				fixed3 refrColor = CalculateRefraction(i.uv_WaterMap, normViewDir, pNormal, i.grabPassPos);
			#else
				fixed3 refrColor = CalculateRefraction(
														#ifdef BAKED_REFRACTIONS
														i.uv_RefrMap
														#endif
														i.uv_WaterMap,
														#ifdef USE_SECONDARY_REFRACTION
														waterMapValue.a,
														i.uv_SecondaryRefrTex,
														#endif
														#ifdef CAUSTICS_ON
														waterAttenuationValue,
														#endif
														normViewDir,// sinAlpha,
														dudvValue);
				//return fixed4(refrColor.rgb, 1.0);
			#endif
			
			//return float4(refrColor.rgb * refrColor.a, 1.0);
			
			
			
			//return float4(refrColor.rgb * refrColor.a, 1.0);
//						#ifdef COLOR_EXTINCTION
//						float3 colorExt = float3(4.5, 75.0, 300.0);
//						//outColor.rgb = lerp(refrColor, _ShallowWaterTint, sinAlpha);
//						
//						outColor.rgb = lerp(refrColor, _DeepWaterTint, waterMapValue.r * 5.0 / colorExt);
//						#else
//						float3 waterTintColor = lerp(_DeepWaterTint, _ShallowWaterTint, deepwaterCorrection);
//						outColor.rgb = lerp(waterTintColor, refrColor, .5);
//						#endif
		#else
			#ifdef CAUSTICS_ON
				fixed3 refrColor = CalculateCaustics(i.uv_WaterMap, waterAttenuationValue);
			#else
				fixed3 refrColor = tex2D(_MainTex, i.uv_MainTex).rgb;
			#endif
		#endif
	    
	    #if !defined(CALCULATE_NORMALS_ON) && defined(REFRACTIONS_ON)
	    float2 _dudvValue = dudvValue * _normalStrength / 50.0;
	    //float2 _dudvValue = dudvValue * _normalStrength / 10000.0;
	    
	    pNormal.xz += _dudvValue.xy;// * 10.0;
	    #endif
	    
	    //
	    //Reflectivity
	    #if defined(REFLECTIONS_ON)
	    	//Normals illusion
	   		//#if !defined(CALCULATE_NORMALS_ON) && defined(REFRACTIONS_ON)
	    	//pNormal.xz += _dudvValue.xy;
	    	//#endif
	    
			fixed3 refl = reflect( -normViewDir, pNormal);
			//fixed3 refl = reflect( -normViewDir, float3(0.0, 1.0, 0.0));
			//refl.xz *= -1.0;
			//refl.xz *= 3.0;
			//refl = normalize(refl);
			
			//Prevent the reflection from going underneath the water
			//refl.y = max(0.01, refl.y);
			#ifndef BLEND_CUBEMAPS
				fixed3 reflectCol = texCUBE( _Cube , refl ).rgb;
			#else
				fixed3 reflectCol = lerp( texCUBE( _Cube , refl ).rgb, texCUBE( _Cube2 , refl ).rgb, _CubemapBlend);
			#endif
			
			//reflectCol = fixed3(0,1,0);
			
		    //texcol.rgb = lerp(texcol.rgb, reflectCol, _Reflectivity * fresnel);	//Add reflections with fresnel
		    
		    //return fixed4( lerp(refrColor, reflectCol, 0.5), waterMapValue.b);
		    
		    //return fixed4(reflectCol, 1.0);
		    //texcol.rgb = lerp(texcol.rgb, refrColor, .5);
		#endif
		
		#ifdef FOAM_ON
			fixed foamValue = dudvFoamValue.b;
			half foamAmount = waterMapValue.g * _EdgeFoamStrength;
		
			#ifdef FLOWMAP_ANIMATION_ON
			//Have foam in the undefined areas
			foamAmount = max(foamAmount, flowSpeed * foamValue * .5);// / _EdgeFoamStrength;
			#endif
		//foamAmount = saturate(1.0 - foamAmount);
		
		//If there's foam then the refractions should be darker
		//refrColor = refrColor * (1.0 - foamAmount * .1);
		#endif
		
		#ifdef ENABLE_SHADOWS
	    refrColor *= SHADOW_ATTENUATION(i);
	    #endif
		
		outColor.rgb = CombineEffectsWithLighting(
									#ifdef REFRACTIONS_ON
									refrColor, waterMapValue.a,
									#endif
									#ifdef REFLECTIONS_ON
									reflectCol,
									#endif
									pNormal,
									normViewDir,
									#ifdef LIGHTING_ON
									i.lightDir,
									#endif
									i.uv_MainTex, waterAttenuationValue
									#ifdef FOAM_ON
									,foamAmount,
									foamValue
									#endif
									#ifdef LIGHTING_ON
										#ifdef LIGHT_MODEL_ANISOTROPIC
										//float3(tex2Dlod(_HeightGlossDUDVMap, float4(i.uv_MainTex, 0, 0) ).b, 0.0, tex2Dlod(_HeightGlossDUDVMap, float4(i.uv_MainTex, 0, 0)).a),
										#ifdef LIGHT_MODEL_ANISOTROPIC
											#ifndef PERPIXEL_SPECULARITY_ON
												#ifdef BAKED_ANISOTROPY_DIR
												//,i.anisoDirUV
												,i.uv_WaterMap
												#else
													#if !defined(CALCULATE_NORMALS_ON) && defined(REFRACTIONS_ON)
													//,fixed3(i.anisoDir.xy + _dudvValue.xy * .01, i.anisoDir.z)
													,fixed3(i.anisoDir.xy, i.anisoDir.z)
													#else
													,i.anisoDir
													#endif
												#endif
											#else
												,i.lightDir
											#endif
										#endif
										//i.anisoDir,
										//i.anisoParameters,
										#endif
									#endif
									//#ifdef FLOWMAP_ANIMATION_ON
									//,flowmapValue, flowLerp, flowmapCycleOffset
									//#endif
									);

		//#ifdef ENABLE_SHADOWS
	    //outColor.rgb = LIGHT_ATTENUATION(i);
	    //#endif

		//return half4(outColor.rgb, 1.0);
		//
		//Alpha
		#ifndef OPAQUE_SURFACE
			#ifdef WATER_EDGEBLEND_ON
			float depth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
			depth = LinearEyeDepth(depth);
			//depth = Linear01Depth(depth);					
			//x is alpha
			//y used for foam amount
			float4 fadeParameters = float4(0.15, 0.15, 0.5, 1.0);
			float4 edgeBlendFactors = saturate( fadeParameters * (depth-i.screenPos.w) );		
			//edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
			//depth = edgeBlendFactors.x;
			//texcol = float4( (depth-i.screenPos.w) * 0.05, 0.0, 0.0, 1.0);
			
			outColor.a = edgeBlendFactors.x;
	//					if ( edgeBlendFactors.x >= 1.0 )
	//						texcol.a = 1.0 - attenuationValue;
	//					else
	//						texcol.a = edgeBlendFactors.x;
	//					//texcol = float4(depth, depth, depth, 1.0);
	
	
			//texcol = float4(i.screenPos.w * 0.01, 0.0, 0.0, 1.0);
			#else
				#ifdef WATERMAPS_ON
					//outColor.a = 1.0;
					outColor.a = waterMapValue.b;
	//				#ifdef FOAM_ON
	//				//outColor.a = (1.0 - waterMapValue.g);// * _EdgeBlendStrength;// / saturate(1.0 - foamAmount);
	//				outColor.a = waterMapValue.b;
	//				//outColor.a = max(outColor.a, foamAmount);	//Foam isn't affected by transparency
	//				// - waterMapValue.g * waterMapValue.g * .5
	//				//outColor.a = 1.0;
	//				#else
	//				outColor.a = waterMapValue.b;
	//				#endif
					//outColor.a = waterMapValue.r * _EdgeBlendStrength * saturate(foamAmount;
					//outColor.a += (1.0 - sinAlpha) * .1;	//Simplified parallax
					//outColor.a = pow(outColor.a, .5);
					//outColor.rgb = sinAlpha;
					//outColor.a = 1.0;
	//				#ifdef REFRACTIONS_ON
	//				outColor.a = waterMapValue.r * _EdgeBlendStrength;
	//				//outColor.a = 1.0;
	//				//outColor.a = pow(waterMapValue.r, .2);
	//				//outColor.a = max(outColor.a, foamAmount);
	//				//outColor.a = pow(waterMapValue.r, .2);
	//				#else
	//				    #ifdef IS_MOBILE_ON
	//				    outColor.a = max(1.0 - attenuationValue, .2);
	//				    #else
	//				    outColor.a = 1.0 - attenuationValue;
	//				    #endif
	//				#endif
			    #else
			    	outColor.a = 1.0;//_WaterAttenuation / 2.0;
			    #endif
			    
			     //texcol.a = 1.0;
			#endif
			
		#else
		outColor.a = 1;
		#endif
		
		//return float4(waterMapValue.b, waterMapValue.b, waterMapValue.b, 1.0);
		
	    return outColor;
	}
				
				
#endif



