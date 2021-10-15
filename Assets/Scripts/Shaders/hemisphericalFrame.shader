Shader "Custom/Hemispherical Frame" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("NormalMap", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//_Metallic ("Metallic", 2D) = "grey" {} //**************

		_Center ("Center", Vector) = (0,0,0,0)
		_Radius ("Radius", float) = 0.25
		_Fade ("Fade Area", float) = .1
		_Alpha ("Max Alpha", Range(0,1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Fade" "Queue"="AlphaTest"}
		LOD 200
		Cull Off

		Pass {
        ZWrite On
        ColorMask 0
		}

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		//sampler2D _Metallic; //************

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			//float2 uv_Metallic; //************
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic; //*********
		fixed4 _Color;

		float4 _Center;
		float _Radius;
		float _Fade;
		float _Alpha;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {

			// Discard pixels farther from radius + fade from center point
			// Also discard pixels lower than center point - fade
			float d = distance(_Center, IN.worldPos);
			if (d > _Radius + _Fade) 
			discard;
			if (IN.worldPos.y < _Center.y - _Fade)
			discard;

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			//o.Metallic = tex2D(_Metallic, IN.uv_Metallic); // **********
			o.Smoothness = _Glossiness;

			o.Alpha = _Alpha;
			// Fade out pixels lower than center point
			if (IN.worldPos.y < _Center.y)
			o.Alpha = _Alpha - (abs(IN.worldPos.y / _Fade));
			// Fade out pixels greater than radius from center point
			if (d > _Radius)
			o.Alpha = o.Alpha - ((d - _Radius)/_Fade);

		}
		ENDCG
	}
	FallBack "Diffuse"
}
