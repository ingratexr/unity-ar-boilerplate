// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Stencil Group/Standard Stencil Filter (Fade)"
{
	Properties 
	{
			_Color ("Color", Color) = (1,1,1,1)
			_MainTex ("Albedo (RGB)", 2D) = "white" {}
			_BumpMap ("NormalMap", 2D) = "bump" {}
			_Glossiness ("Smoothness", Range(0,1)) = 0.5
			_Metallic ("Metallic", Range(0,1)) = 0.0
			//_Metallic ("Metallic", 2D) = "grey" {} //**************

			[Enum(Equal,3,NotEqual,6)] _StencilTest ("Stencil Test", int) = 3

			_Alpha ("Alpha", Range(0,1)) = 1
	}

	SubShader 
	{
		Tags { "RenderType"="Fade" "Queue"="Transparent"}
		LOD 200
		Cull Off

		// Ref 3 = both stencil and unstencil is true
		Stencil{
			Ref 3
			Comp[_StencilTest]
		}

		// Ref 1 = just stencil (not unstencil) is true
		Stencil{
			Ref 1
			Comp[_StencilTest]
		}

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
		};

		half _Glossiness;
		half _Metallic; //*********
		fixed4 _Color;
		float _Alpha;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			//o.Metallic = tex2D(_Metallic, IN.uv_Metallic); // **********
			o.Smoothness = _Glossiness;
			o.Alpha = _Alpha;

		}
		ENDCG
	}
	FallBack "Diffuse"
}