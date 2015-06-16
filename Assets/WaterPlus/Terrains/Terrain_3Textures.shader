Shader "Custom/Terrain_3Textures" {
	Properties {
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}		
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Control ("Control (RGBA)", 2D) = "white" {}
		
		_MainTex ("Never Used", 2D) = "white" {}
	}
	
	SubShader {
		Tags {
	   "SplatCount" = "3"
	   "RenderType" = "Opaque"
		}
		
		CGPROGRAM
		#pragma surface surf T4M exclude_path:prepass approxview halfasview
		#pragma exclude_renderers xbox360 ps3 flash
		//#pragma multi_compile NOT_IN_EDITOR_MODE IN_EDITOR_MODE
		
		inline fixed4 LightingT4M (SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed diff = max (0, dot (s.Normal, lightDir));
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
			c.a = 0.0;
			return c;
		}
		
		struct Input {
			float2 uv_Control : TEXCOORD0;
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
		};
		 
		sampler2D _Control;
		sampler2D _Splat0,_Splat1,_Splat2;

		 
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 splat_control = tex2D (_Control, IN.uv_Control).rgba;
			
			fixed3 lay1 = tex2D (_Splat0, IN.uv_Splat0).rgb;
			fixed3 lay2 = tex2D (_Splat1, IN.uv_Splat1).rgb;
			fixed3 lay3 = tex2D (_Splat2, IN.uv_Splat2).rgb;
			o.Alpha = 0.0;
			o.Albedo.rgb = lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b;
		}
		ENDCG 
	}
	// Fallback to VertexLit
	Fallback "VertexLit"
}