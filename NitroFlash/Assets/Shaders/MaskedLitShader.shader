Shader "Custom/MaskedLitShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_PrimaryMaskReplace("Primary Mask", 2D) = "white" {}
		_PrimaryTex("Primary Albedo (RGB)", 2D) = "white" {}
		_PrimaryColor ("Primary Color", Color) = (1,1,1,1)
		_PrimaryGloss  ("Primary Smoothness", Range(0,1)) = 0.5
		_PrimaryMetallic  ("Primary Metallic", Range(0,1)) = 0.5

		_SecondaryMaskReplace("Secondary Mask", 2D) = "white" {}
		_SecondaryTex("Secondary Albedo (RGB)", 2D) = "white" {}
		_SecondaryColor ("Secondary Color", Color) = (1,1,1,1)
		_SecondaryGloss  ("Secondary Smoothness", Range(0,1)) = 0.5
		_SecondaryMetallic  ("Secondary Metallic", Range(0,1)) = 0.5

		_TertiaryMaskReplace("Tertiary Mask", 2D) = "white" {}
		_TertiaryTex("Tertiary Albedo (RGB)", 2D) = "white" {}
		_TertiaryColor ("Tertiary Color", Color) = (1,1,1,1)
		_TertiaryGloss  ("Tertiary Smoothness", Range(0,1)) = 0.5
		_TertiaryMetallic  ("Tertiary Metallic", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _PrimaryTex;
		sampler2D _PrimaryMaskReplace;

		sampler2D _SecondaryTex;
		sampler2D _SecondaryMaskReplace;

		sampler2D _TertiaryTex;
		sampler2D _TertiaryMaskReplace;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _PrimaryColor;
		fixed4 _SecondaryColor;
		fixed4 _TertiaryColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 mc = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 pc = tex2D (_PrimaryTex, IN.uv_MainTex) * _PrimaryColor;
			fixed4 sc = tex2D (_SecondaryTex, IN.uv_MainTex) * _SecondaryColor;
			fixed4 tc = tex2D (_TertiaryTex, IN.uv_MainTex) * _TertiaryColor;

			fixed4 pcMask = tex2D (_PrimaryMaskReplace, IN.uv_MainTex);
			fixed4 scMask = tex2D (_SecondaryMaskReplace, IN.uv_MainTex);
			fixed4 tcMask = tex2D (_TertiaryMaskReplace, IN.uv_MainTex);

			fixed rOutput = lerp(lerp(lerp(mc.r,pc.r,pcMask.r),sc.r,scMask.r),tc.r,tcMask.r);
			fixed gOutput = lerp(lerp(lerp(mc.g,pc.g,pcMask.g),sc.g,scMask.g),tc.g,tcMask.g);
			fixed bOutput = lerp(lerp(lerp(mc.b,pc.b,pcMask.b),sc.b,scMask.b),tc.b,tcMask.b);

			fixed3 output = {rOutput, gOutput, bOutput};

			o.Albedo = output;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = mc.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
