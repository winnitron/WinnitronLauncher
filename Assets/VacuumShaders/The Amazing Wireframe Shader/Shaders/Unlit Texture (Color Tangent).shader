// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/The Amazing Wireframe/(Preview) Color and Tangent"
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

		[KeywordEnum(ColorBuffer, TangentBuffer)] V_WIRE_IN ("Wire data is inside", Float) = 0
		[MaterialToggle(V_WIRE_ANTIALIASING_ON)] AO ("Antialiasing", Float) = 0

		V_WIRE_COLOR("Wire Color (RGB) Trans (A)", color) = (0, 0, 0, 1)	
		V_WIRE_SIZE("Wire Size", Range(0, 0.5)) = 0.05
		
    }

    SubShader 
    {
		Tags { "RenderType"="Opaque" "WireframeTag"="(Preview) Color and Tangent"}

		Pass
	    {			  
		 
            CGPROGRAM 
		    #pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest		 
			 
			#pragma multi_compile V_WIRE_ANTIALIASING_OFF V_WIRE_ANTIALIASING_ON
			#pragma multi_compile V_WIRE_IN_COLORBUFFER V_WIRE_IN_TANGENTBUFFER
					
			
			#ifdef V_WIRE_ANTIALIASING_ON
				#pragma target 3.0
				#pragma glsl
			#endif

			fixed4 V_WIRE_COLOR;
			half V_WIRE_SIZE;	

			fixed4 _Color;
			sampler2D _MainTex;
			half4 _MainTex_ST;


			struct vInput
			{
				float4 vertex : POSITION;
				half4 texcoord : TEXCOORD0;
				
				#ifdef V_WIRE_IN_COLORBUFFER
					fixed4 color : COLOR;
				#else
					float4 tangent : TANGENT;
				#endif
			};

			struct vOutput
			{
				float4 pos :SV_POSITION;
				half3 uv : TEXCOORD0;

				fixed4 mass : TEXCOORD1;				
			};

			vOutput vert(vInput v)
			{
				vOutput o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = half3(v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw, 0);

				#ifdef V_WIRE_IN_COLORBUFFER
					o.mass = v.color;
				#else
					o.mass = v.tangent;
				#endif

				return o;
			}

			fixed4 frag(vOutput i) : SV_Target 
			{	
				fixed4 retColor = tex2D(_MainTex, i.uv.xy) * _Color;

				
				#ifdef V_WIRE_ANTIALIASING_ON
					half3 width = abs(ddx(i.mass.xyz)) + abs(ddy(i.mass.xyz));
					half3 eF = smoothstep(half3(0, 0, 0), width * V_WIRE_SIZE * 20, i.mass.xyz);		
	
					half value = min(min(eF.x, eF.y), eF.z);	
				#else
					half value = step(V_WIRE_SIZE, min(min(i.mass.x, i.mass.y), i.mass.z));
				#endif
				

				return lerp(V_WIRE_COLOR, retColor, value);
			}


			ENDCG

    	} //Pass
			
        
    } //SubShader


} //Shader
