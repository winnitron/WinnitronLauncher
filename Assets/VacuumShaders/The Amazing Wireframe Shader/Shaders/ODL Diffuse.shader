// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/The Amazing Wireframe/One Directional Light/Diffuse"
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
		V_WIRE_SIZE("Wire Size", Range(0, 0.5)) = 0.05
		
    }
	 
    SubShader   
    {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" "WireframeTag"="One Directional Light/Diffuse"}

		Pass
	    {			
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" } 

            CGPROGRAM
		    #pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest		 
			
			#define UNITY_PASS_FORWARDBASE  
            #include "UnityCG.cginc"
            #include "AutoLight.cginc" 
            #pragma multi_compile_fwdbase_fullshadows

			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma multi_compile V_WIRE_LIGHT_OFF V_WIRE_LIGHT_ON
			#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON
			#pragma multi_compile V_WIRE_FRESNEL_OFF V_WIRE_FRESNEL_ON

			#ifdef V_WIRE_ANTIALIASING_ON
				#pragma target 3.0
				#pragma glsl
			#endif

			#define V_WIRE_HAS_TEXTURE
			#define PASS_FORWARD_BASE

			#include "Wire.cginc"

	    	ENDCG

    	} //Pass
		

		Pass  
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
	 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_ShadowCaster   
			 
			#pragma multi_compile_shadowcaster 
			#define UNITY_PASS_SHADOWCASTER
			#include "UnityCG.cginc"  
			  
					 
			#define PASS_SHADOW_CASTER

		    #include "Wire.cginc"
 
			       
			ENDCG 
		}	//ShadowCaster   


		Pass 
		{  
			Name "ShadowCollector"
			Tags { "LightMode" = "ShadowCollector" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual

			    
			CGPROGRAM   
			#pragma vertex vert
			#pragma fragment frag_ShadowCollector
			 
			#pragma multi_compile_shadowcollector
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_SHADOWCOLLECTOR
			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"
			#include "Lighting.cginc"


			#define PASS_SHADOW_COLLECTOR 
		 
 		    #include "Wire.cginc"
			 
		 
 			ENDCG
		}

        
    } //SubShader


} //Shader
