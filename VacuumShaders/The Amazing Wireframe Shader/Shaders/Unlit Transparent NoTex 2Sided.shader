// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/The Amazing Wireframe/Unlit/Transparent/NoTex 2 Sided"
{
    Properties 
    {
		//Tag
		[Tag]
		V_WIRE_TAG("", float) = 0

		//Wire Options
		[WireframeOptions]
		V_WIRE_W_OPTIONS("", float) = 0

		[MaterialToggle(V_WIRE_ANTIALIASING_ON)] AO ("Antialiasing", Float) = 0
		[MaterialToggle(V_WIRE_LIGHT_ON)] LO ("Lightmaps effect Wire", Float) = 0
		[MaterialToggle(V_WIRE_FRESNEL_ON)] FO ("Fresnel Wire", Float) = 0

		V_WIRE_COLOR("Wire Color (RGB) Trans (A)", color) = (0, 0, 0, 1)	
		V_WIRE_SIZE("Wire Size", Range(0, 0.5)) = 0.05
    }

    SubShader  
    {
		Tags {"Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent" "WireframeTag"="Unlit/Transparent/NoTex 2 Sided"}

		
		Blend SrcAlpha OneMinusSrcAlpha 
		 

		Pass 
	    {			
			ZWrite Off
			Cull Front

            CGPROGRAM
		    #pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest		 
			
            #pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile V_WIRE_LIGHT_OFF V_WIRE_LIGHT_ON
			#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON
			#pragma multi_compile V_WIRE_FRESNEL_OFF V_WIRE_FRESNEL_ON

			#define V_WIRE_SAME_COLOR
			#define V_WIRE_TRANSPARENT
			
			#ifdef V_WIRE_ANTIALIASING_ON
				#pragma target 3.0
				#pragma glsl
			#endif
			
			#include "Wire.cginc"

	    	ENDCG

    	} //Pass

		Pass 
	    {			

			ZWrite Off
			Cull Back

            CGPROGRAM
		    #pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest		 
			
            #pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON

			#define V_WIRE_SAME_COLOR
			#define V_WIRE_TRANSPARENT
			
			#ifdef V_WIRE_ANTIALIASING_ON
				#pragma target 3.0
				#pragma glsl
			#endif
			
			#include "Wire.cginc"

	    	ENDCG

    	} //Pass
        
    } //SubShader
	
} //Shader
