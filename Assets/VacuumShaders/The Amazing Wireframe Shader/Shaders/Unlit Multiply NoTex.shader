// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/The Amazing Wireframe/Unlit/Multiply/NoTex"
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

		//Wire Options
		[WireframeOptions]
		V_WIRE_W_OPTIONS("", float) = 0

		[MaterialToggle(V_WIRE_ANTIALIASING_ON)] AO ("Antialiasing", Float) = 0
		[MaterialToggle(V_WIRE_FRESNEL_ON)] FO ("Fresnel Wire", Float) = 0

		V_WIRE_COLOR("Wire Color (RGB)", color) = (0, 0, 0, 1)	
		V_WIRE_SIZE("Wire Size", Range(0, 0.5)) = 0.05	
    }

    SubShader  
    {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "WireframeTag"="Unlit/Multiply/NoTex"}

		
		Blend Zero SrcColor
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		 

		Pass 
	    {			

            CGPROGRAM
		    #pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest		 
			
			#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON
			#pragma multi_compile V_WIRE_FRESNEL_OFF V_WIRE_FRESNEL_ON

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
