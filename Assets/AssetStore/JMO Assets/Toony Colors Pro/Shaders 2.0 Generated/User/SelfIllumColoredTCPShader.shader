// Toony Colors Pro+Mobile 2
// (c) 2014,2015 Jean Moreno


Shader "Toony Colors Pro 2/User/SelfIllumColoredTCPShader"
{
	Properties
	{
		//TOONY COLORS
		_Color("Color", Color) = (0.5,0.5,0.5,1.0)
		_HColor("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor("Shadow Color", Color) = (0.3,0.3,0.3,1.0)

		//DIFFUSE
		_MainTex("Main Texture (RGB) Spec Mask (A)", 2D) = "white" {}
		_Mask1("Mask 1 (Self-Illumination)", 2D) = "black" {}

		_SIColor("Self-Illum color", Color) = (1.0,1.0,1.0,1.0)
		_SILevel("Self-Illum level", Range(0.0,1)) = 0.0

		//TOONY COLORS RAMP
		_RampThreshold("#RAMPF# Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth("#RAMPF# Ramp Smoothing", Range(0.001,1)) = 0.1

		//SPECULAR
		_SpecColor("#SPEC# Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("#SPEC# Shininess", Range(0.0,2)) = 0.1

		//RIM LIGHT
		_RimColor("#RIM# Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimMin("#RIM# Rim Min", Range(0,1)) = 0.5
		_RimMax("#RIM# Rim Max", Range(0,1)) = 1.0
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM

#include "../../Shaders 2.0/Include/TCP2_Include.cginc"
		//#pragma surface surf ToonyColorsSpec noforwardadd
#pragma surface surf ToonyColorsSpec vertex:vert noforwardadd interpolateview halfasview nofog
#pragma target 2.0
#pragma glsl


		//================================================================
		// VARIABLES

		fixed4 _Color;
	sampler2D _MainTex;
	sampler2D _Mask1;

	fixed4 _SIColor;
	fixed _SILevel;

	fixed _Shininess;
	fixed4 _RimColor;
	fixed _RimMin;
	fixed _RimMax;
	float4 _RimDir;

	struct Input
	{
		half2 uv_MainTex;
		half2 uv_Mask1;
		fixed rim;
	};

	//================================================================
	// VERTEX FUNCTION

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);
#if TCP2_RIMDIR
		_RimDir.x += UNITY_MATRIX_MV[0][3] * (1 / UNITY_MATRIX_MV[2][3]) * (1 - UNITY_MATRIX_P[3][3]);
		_RimDir.y += UNITY_MATRIX_MV[1][3] * (1 / UNITY_MATRIX_MV[2][3]) * (1 - UNITY_MATRIX_P[3][3]);
		float3 viewDir = normalize(UNITY_MATRIX_V[0].xyz * _RimDir.x + UNITY_MATRIX_V[1].xyz * _RimDir.y + UNITY_MATRIX_V[2].xyz * _RimDir.z);
#else
		float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
#endif
		half rim = 1.0f - saturate(dot(viewDir, v.normal));
		o.rim = smoothstep(_RimMin, _RimMax, rim) * _RimColor.a;
	}

	//================================================================
	// SURFACE FUNCTION

	void surf(Input IN, inout SurfaceOutput o)
	{
		half4 main = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 mask1 = tex2D(_Mask1, IN.uv_Mask1);
		o.Albedo = main.rgb * _Color.rgb;
		o.Alpha = main.a * _Color.a;

		//Specular
		o.Gloss = main.a;
		o.Specular = _Shininess;
#if TCP2_BUMP
		//Normal map
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
#endif
		o.Emission += IN.rim * _RimColor.rgb;
		o.Emission = o.Emission * (1.0f - mask1.a) + mask1.rgb * mask1.a;

		o.Albedo = lerp(o.Albedo, _SIColor, _SILevel);
		o.Gloss = lerp(o.Gloss, 0.0f, _SILevel);
		o.Emission = lerp(o.Emission, _SIColor, _SILevel);
	}

	ENDCG
	}

		Fallback "Diffuse"
		CustomEditor "TCP2_MaterialInspector"
}
