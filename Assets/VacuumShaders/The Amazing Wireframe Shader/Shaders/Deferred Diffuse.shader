// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/The Amazing Wireframe/Deferred/Diffuse" 
{
	Properties 
	{		
		//Tag
		[Tag]
		V_WIRE_TAG("", float) = 0

		//Default Options
		[DefaultOptions]
		V_WIRE_D_OPTIONS("", float) = 0

		_Color("Main Color (RGB)", color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white"{}


		//Wire Options
		[WireframeOptions]
		V_WIRE_W_OPTIONS("", float) = 0

		[MaterialToggle(V_WIRE_ANTIALIASING_ON)] AO ("Antialiasing", Float) = 0
		[MaterialToggle(V_WIRE_LIGHT_ON)] LO ("Lights & Lightmaps effect Wire", Float) = 0
		[MaterialToggle(V_WIRE_FRESNEL_ON)] FO ("Fresnel Wire", Float) = 0

		V_WIRE_COLOR("Wire Color (RGB) Trans (A)", color) = (0, 0, 0, 1)	
		V_WIRE_SIZE("Wire Size", Range(0, 0.5)) = 0.042
	}
	 
	SubShader 
	{ 
		Tags { "RenderType"="Opaque" "WireframeTag"="Deferred/Diffuse"}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert finalcolor:wireColor

		 
		#pragma multi_compile V_WIRE_LIGHT_OFF V_WIRE_LIGHT_ON
		#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON
		#pragma multi_compile V_WIRE_FRESNEL_OFF V_WIRE_FRESNEL_ON

		#ifdef V_WIRE_ANTIALIASING_ON
			#pragma target 3.0
			#pragma glsl
		#endif
		 
	    #define V_WIRE_HAS_TEXTURE
		#define V_WIRE_SURFACE
		
		
		#include "Wire.cginc"

		ENDCG
	} 

	FallBack "VertexLit"

}
