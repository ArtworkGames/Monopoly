// Toony Colors Pro+Mobile 2
// (c) 2014,2015 Jean Moreno


Shader "Toony Colors Pro 2/User/TCP2TransparentTwoSidedShader"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5

		//DIFFUSE
		_MainTex ("Main Texture (RGBA)", 2D) = "white" {}
		
		//TOONY COLORS RAMP
		_RampThreshold ("#RAMPF# Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("#RAMPF# Ramp Smoothing", Range(0.001,1)) = 0.1
		
		//SPECULAR
		_SpecColor ("#SPEC# Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("#SPEC# Shininess", Range(0.0,2)) = 0.1
		
	}
	
	SubShader
	{
		Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		Cull Off
		//Pass {
		//	ZWrite On
		//	ColorMask 0
		//}

		CGPROGRAM
		
		#include "../../Shaders 2.0/Include/TCP2_Include.cginc"
		#pragma surface surf ToonyColorsSpec alphatest:_Cutoff
		#pragma target 3.0
		#pragma glsl
		
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		
		fixed _Shininess;
		
		struct Input
		{
			half2 uv_MainTex;
		};
		
		//================================================================
		// SURFACE FUNCTION
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;
			
			//Specular
			//o.Gloss = 1;
			o.Gloss = mainTex.a;
			o.Specular = _Shininess;
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector"
}
