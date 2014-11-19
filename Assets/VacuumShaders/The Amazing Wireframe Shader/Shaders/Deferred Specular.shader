// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/The Amazing Wireframe/Deferred/Specular" 
{
	Properties 
	{
		//Tag
		[Tag]
		V_WIRE_TAG("", float) = 0

		//Default Options
		[DefaultOptions]
		V_WIRE_D_OPTIONS("", float) = 0

		_Color("Main Color (RGB) Gloss (A)", color) = (1, 1, 1, 1)
		_SpecColor ("Specular Color (RGB)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white"{}

		_ReflectColor ("Reflection Color (RGB)", Color) = (1, 1, 1, 0.5)
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }

		//Wire Options
		[WireframeOptions]
		V_WIRE_W_OPTIONS("", float) = 0

		[MaterialToggle(V_WIRE_ANTIALIASING_ON)] AO ("Antialiasing", Float) = 0
		[MaterialToggle(V_WIRE_LIGHT_ON)] LO ("Lights & Lightmaps effect Wire", Float) = 0
		[MaterialToggle(V_WIRE_FRESNEL_ON)] FO ("Fresnel Wire", Float) = 0

		V_WIRE_COLOR("Wire Color (RGB) Trans (A)", color) = (0, 0, 0, 1)	
		V_WIRE_SIZE("Wire Size", Range(0, 0.5)) = 0.05
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" "WireframeTag"="Deferred/Specular"}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert finalcolor:wireColor

		#pragma multi_compile V_WIRE_LIGHT_OFF V_WIRE_LIGHT_ON
		#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON
		#pragma multi_compile V_WIRE_FRESNEL_OFF V_WIRE_FRESNEL_ON

		#ifdef V_WIRE_ANTIALIASING_ON
			#pragma target 3.0
			#pragma glsl
		#endif
		 
	    #define V_WIRE_HAS_TEXTURE
		#define V_WIRE_SURFACE
		#define V_WIRE_GLOSS
		
		
		#include "Wire.cginc"

		ENDCG
	} 

	Fallback "VertexLit"

}
