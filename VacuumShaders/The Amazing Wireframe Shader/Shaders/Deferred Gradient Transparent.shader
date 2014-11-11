// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/The Amazing Wireframe/Deferred/Gradient/Transparent" 
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
		

		V_WIRE_COLOR("Wire Color (RGB) Trans (A)", color) = (0, 0, 0, 1)	
		V_WIRE_SIZE("Wire Size", Range(0, 0.5)) = 0.042
		
		[MaterialToggle(V_WIRE_GRADIENT_ON)] GO ("Preview Gradient", Float) = 0
		[KeywordEnum(Local, World)] V_WIRE_GRADIENT_SPACE ("Gradient Space", Float) = 0
		[KeywordEnum(X, Y, Z)] V_WIRE_GRADIENT_AXIS ("Gradient Axis", Float) = 0
		V_WIRE_GradientMin("Gradient Min", float) = -1
		V_WIRE_GradientMax("Gradient Max", float) = 1
		V_WIRE_GradientOffset("Gradient Offset", float) = 0
		V_WIRE_GradientColor("Gradient Color (RGB) Trans (A)", color) = (1, 1, 1, 1)
		
	}
	 
	SubShader 
	{ 
		Tags {"Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent" "WireframeTag"="Deferred/Gradient/Transparent"}
		LOD 200 


		Pass 
		{
			ZWrite On
			ColorMask 0
		}
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert finalcolor:wireColor alpha

		 
		#pragma multi_compile V_WIRE_LIGHT_OFF V_WIRE_LIGHT_ON
		#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON
		#pragma multi_compile V_WIRE_GRADIENT_OFF V_WIRE_GRADIENT_ON
		#pragma multi_compile V_WIRE_GRADIENT_AXIS_X V_WIRE_GRADIENT_AXIS_Y V_WIRE_GRADIENT_AXIS_Z
		#pragma multi_compile V_WIRE_GRADIENT_SPACE_LOCAL V_WIRE_GRADIENT_SPACE_WORLD


		#ifdef V_WIRE_ANTIALIASING_ON
			#pragma target 3.0
			#pragma glsl 
		#endif
		 

	    #define V_WIRE_HAS_TEXTURE
		#define V_WIRE_GRADIENT
		#define V_WIRE_GRADIENT_TRANSPARENT

		#define V_WIRE_SURFACE
		  
				
		#include "Wire.cginc"

		ENDCG
	} 
	
	Fallback "Transparent/VertexLit"

}
