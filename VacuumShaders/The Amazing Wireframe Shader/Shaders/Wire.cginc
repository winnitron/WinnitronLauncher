#ifndef VACUUM_WIRE_CGINC
#define VACUUM_WIRE_CGINC


//////////////////////////////////////////////////////////////////////////////
//                                                                          // 
//Variables                                                                 //                
//                                                                          //               
//////////////////////////////////////////////////////////////////////////////


fixed4 V_WIRE_COLOR;
half V_WIRE_SIZE;	

 
fixed4 _Color;

#ifdef V_WIRE_HAS_TEXTURE
	sampler2D _MainTex;

	#ifndef V_WIRE_SURFACE
		half4 _MainTex_ST;
	#endif
#endif

#ifdef V_WIRE_BUMP
	sampler2D _BumpMap;
#endif

#ifdef V_WIRE_GLOSS
	half _Shininess;
#endif

#if defined(LIGHTMAP_ON) && !defined(V_WIRE_SURFACE)
	half4 unity_LightmapST;
	sampler2D unity_Lightmap;				
#endif

#if defined(PASS_FORWARD_BASE) || defined(PASS_FORWARD_ADD)
	uniform half4 _LightColor0;
#endif

#if defined(V_WIRE_REFLECTION) || defined(V_WIRE_REFLECTION_BUMPSPECULAR)
	samplerCUBE _Cube;

	#ifdef V_WIRE_REFLECTION_COLOR
		fixed4 _ReflectColor;
	#endif
#endif

#if defined(V_WIRE_CUTOUT) && !defined(V_WIRE_SURFACE)
	half _Cutoff;
#endif

#ifdef V_WIRE_GRADIENT
	half V_WIRE_GradientMin;
	half V_WIRE_GradientMax;
	half V_WIRE_GradientOffset;
	fixed4 V_WIRE_GradientColor;
#endif

//////////////////////////////////////////////////////////////////////////////
//                                                                          // 
//Structs                                                                   //                
//                                                                          //               
//////////////////////////////////////////////////////////////////////////////

#ifndef V_WIRE_SURFACE
struct vInput
{
    float4 vertex : POSITION;

	#if defined(PASS_FORWARD_BASE) || defined(V_WIRE_REFLECTION) || defined(V_WIRE_FRESNEL_ON)
		half3 normal : NORMAL;
	#endif

	#ifdef V_WIRE_HAS_TEXTURE
		half4 texcoord : TEXCOORD0;
	#endif

	#ifdef LIGHTMAP_ON
		half4 texcoord1 :TEXCOORD1;
	#endif

	fixed4 color : COLOR;
};

struct vOutput
{
	#if defined(PASS_SHADOW_COLLECTOR) || defined(PASS_SHADOW_CASTER)

		#ifdef PASS_SHADOW_COLLECTOR
			V2F_SHADOW_COLLECTOR;
		#endif

		#ifdef PASS_SHADOW_CASTER
			V2F_SHADOW_CASTER;
		#endif

		#ifdef V_WIRE_HAS_TEXTURE
			half2 uv :TEXCOORD5;
		#endif
				
	#else
		 float4 pos :SV_POSITION;

		#ifdef V_WIRE_HAS_TEXTURE
			half3 uv : TEXCOORD0;

			//if defined V_WIRE_GRADIENT
			//uv.z will contain gradient
		#endif

		#ifdef LIGHTMAP_ON
			half2 lmap : TEXCOORD1;
		#endif

		#ifdef V_WIRE_REFLECTION
			half3 refl : TEXCOORD2;
		#endif

		#ifdef V_WIRE_FRESNEL_ON
			half fresnel : TEXCOORD3;
		#endif

		#if defined(PASS_FORWARD_BASE) || defined(PASS_FORWARD_ADD) || defined(V_WIRE_FRESNEL_ON)	
				half3 normal: TEXCOORD4;
		#endif

		#if defined(PASS_FORWARD_BASE) || defined(PASS_FORWARD_ADD)	
			#ifndef V_WIRE_TRANSPARENT
				LIGHTING_COORDS(5,6)
			#endif
		#endif
	#endif

	fixed4 color : COLOR;   
};
#endif

#ifdef V_WIRE_SURFACE
	struct Input 
	{
		half2 uv_MainTex;

		#ifdef V_WIRE_BUMP
			float2 uv_BumpMap;
		#endif

		#ifdef V_WIRE_REFLECTION
			half3 refl;
		#endif

		#ifdef V_WIRE_FRESNEL_ON
			half fresnel;
		#endif

		#ifdef V_WIRE_GRADIENT
			half grad;
		#endif

		fixed4 color;
	};


#endif

//////////////////////////////////////////////////////////////////////////////
//                                                                          // 
//Functions                                                                 //                
//                                                                          //               
//////////////////////////////////////////////////////////////////////////////

#ifdef LIGHTMAP_ON
inline fixed3 WIRE_DecodeLightmap( fixed4 color )
{
	#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
		return 2.0 * color.rgb;
	#else
		return (8.0 * color.a) * color.rgb;
	#endif
}
#endif

inline float3 V_WIRE_WorldSpaceViewDir( in float4 v )
{
	return _WorldSpaceCameraPos.xyz - mul(_Object2World, v).xyz;
}

inline void Wire(inout fixed4 srcColor, fixed4 mass, half gradient)
{

	#ifdef V_WIRE_ANTIALIASING_ON
		half3 width = abs(ddx(mass.xyz)) + abs(ddy(mass.xyz));
		half3 eF = smoothstep(half3(0, 0, 0), width * V_WIRE_SIZE * 20, mass.xyz);		
	
		half value = min(min(eF.x, eF.y), eF.z);		
	#else		
		half value = step(V_WIRE_SIZE, min(min(mass.x, mass.y), mass.z));		
	#endif



	#ifdef V_WIRE_SAME_COLOR
		srcColor.rgb = V_WIRE_COLOR.rgb;
	#endif

	#ifdef V_WIRE_TRANSPARENCY_ON
		V_WIRE_COLOR.a *= srcColor.a;
	#endif
		
	#if !defined(V_WIRE_TRANSPARENT) && !defined(V_WIRE_SAME_COLOR) && !defined(V_WIRE_GRADIENT)
		V_WIRE_COLOR = lerp(srcColor, V_WIRE_COLOR, V_WIRE_COLOR.a);
	#endif 
		


	#ifdef V_WIRE_GRADIENT
		value = lerp(value, 1, gradient);
		srcColor.rgb = lerp(V_WIRE_GradientColor.rgb, srcColor.rgb, gradient);


		#ifdef V_WIRE_GRADIENT_TRANSPARENT			
			srcColor.a = lerp(gradient, 1, V_WIRE_GradientColor.a);			
		#endif

		V_WIRE_COLOR = lerp(srcColor, V_WIRE_COLOR, V_WIRE_COLOR.a);
	#endif

	#ifdef V_WIRE_CUTWIRE_ON
		V_WIRE_COLOR.a = srcColor.a;
	#endif


	srcColor = lerp(V_WIRE_COLOR, srcColor, value);
}

inline half3 V_WIRE_ObjViewDir ( half3 vertexPos )
{				
	half3 objSpaceCameraPos = mul(_World2Object, half4(_WorldSpaceCameraPos.xyz, 1)).xyz * unity_Scale.w;
				
	return normalize(objSpaceCameraPos - vertexPos);
}

//////////////////////////////////////////////////////////////////////////////
//                                                                          // 
//Vertex Shader                                                             //                
//                                                                          //               
//////////////////////////////////////////////////////////////////////////////
#ifdef V_WIRE_SURFACE 
void vert (inout appdata_full v, out Input o)
#else
vOutput vert(vInput v)
#endif
{ 
	#ifdef V_WIRE_SURFACE
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#else
		vOutput o;
	#endif

	
	#ifndef V_WIRE_SURFACE
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	#endif
	o.color = v.color;

	#ifdef V_WIRE_REFLECTION
		half3 viewDir = _WorldSpaceCameraPos.xyz - mul(_Object2World, v.vertex).xyz;
		half3 worldN = mul((half3x3)_Object2World, v.normal * unity_Scale.w);

		o.refl = reflect( -viewDir, worldN );
	#endif

	#ifdef V_WIRE_FRESNEL_ON
		o.fresnel = 1.0 - dot ( v.normal, V_WIRE_ObjViewDir(v.vertex.xyz) );
		//o.fresnel *= o.fresnel;
	#endif

	#ifdef PASS_FORWARD_BASE
		o.normal = mul((half3x3)_Object2World, v.normal * unity_Scale.w);
	#endif


	#if defined(V_WIRE_HAS_TEXTURE) && !defined(V_WIRE_SURFACE)
		o.uv = half3(v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw, 0);

		#ifdef V_WIRE_GRADIENT
			V_WIRE_GradientMax += V_WIRE_GradientOffset;
			V_WIRE_GradientMin += V_WIRE_GradientOffset;

			

			#ifdef V_WIRE_GRADIENT_SPACE_LOCAL
				half3 vPos = v.vertex.xyz;
			#else
				half3 vPos = mul(_Object2World, half4(v.vertex.xyz, 1)).xyz;
			#endif 

			#ifdef V_WIRE_GRADIENT_AXIS_Y
				o.uv.z = (vPos.y - V_WIRE_GradientMin) / (V_WIRE_GradientMax - V_WIRE_GradientMin);
			#else
				#ifdef V_WIRE_GRADIENT_AXIS_Z
					o.uv.z = (vPos.z - V_WIRE_GradientMin) / (V_WIRE_GradientMax - V_WIRE_GradientMin);
				#else
					o.uv.z = (vPos.x - V_WIRE_GradientMin) / (V_WIRE_GradientMax - V_WIRE_GradientMin);
				#endif
			#endif

			o.uv.z = saturate(o.uv.z);
			//o.grad = (pow(saturate(o.grad), _Pow));
		#endif
	#endif

	#if defined(V_WIRE_SURFACE) && defined(V_WIRE_GRADIENT)
		V_WIRE_GradientMax += V_WIRE_GradientOffset;
		V_WIRE_GradientMin += V_WIRE_GradientOffset;

		#ifdef V_WIRE_GRADIENT_SPACE_LOCAL
			half3 vPos = v.vertex.xyz;
		#else
			half3 vPos = mul(_Object2World, half4(v.vertex.xyz, 1)).xyz;
		#endif 

		#ifdef V_WIRE_GRADIENT_AXIS_Y
				o.grad = (vPos.y - V_WIRE_GradientMin) / (V_WIRE_GradientMax - V_WIRE_GradientMin);
			#else
				#ifdef V_WIRE_GRADIENT_AXIS_Z
					o.grad = (vPos.z - V_WIRE_GradientMin) / (V_WIRE_GradientMax - V_WIRE_GradientMin);
				#else
					o.grad = (vPos.x - V_WIRE_GradientMin) / (V_WIRE_GradientMax - V_WIRE_GradientMin);
				#endif
			#endif

			o.grad = saturate(o.grad);
	#endif


	//Lightmap
	#ifndef  V_WIRE_SURFACE
		#ifdef LIGHTMAP_ON
			o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif
	 

		//Shadows
		#if !defined(PASS_SHADOW_CASTER) && !defined(PASS_SHADOW_COLLECTOR) 
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			#if defined(PASS_FORWARD_BASE) || defined(PASS_FORWARD_ADD)
				TRANSFER_VERTEX_TO_FRAGMENT(o);
			#endif
		#endif

		#ifdef PASS_SHADOW_CASTER
			TRANSFER_SHADOW_CASTER(o)
		#endif

		#ifdef PASS_SHADOW_COLLECTOR
			TRANSFER_SHADOW_COLLECTOR(o)
		#endif

		return o;
	#endif	
				
}

//////////////////////////////////////////////////////////////////////////////
//                                                                          // 
//Pixel Shaders                                                             //                
//                                                                          //               
//////////////////////////////////////////////////////////////////////////////
#if !defined(PASS_SHADOW_COLLECTOR) && !defined(PASS_SHADOW_CASTER) && !defined(V_WIRE_SURFACE)
fixed4 frag(vOutput i) : COLOR 
{			

	#if defined(V_WIRE_GRADIENT) && defined(V_WIRE_GRADIENT_ON)
		return half4(i.uv.zzz, 1);
	#endif
	
	fixed4 retColor = _Color;

	#ifdef V_WIRE_HAS_TEXTURE	
		half4 mainTex = tex2D(_MainTex, i.uv.xy);
		retColor *= mainTex;
	#endif


	#if defined(LIGHTMAP_ON) && !defined(PASS_FORWARD_ADD)
		fixed4 lmtex = tex2D(unity_Lightmap, i.lmap.xy);
		fixed3 lm = WIRE_DecodeLightmap (lmtex);

		retColor.rgb *= lm;

		#ifdef V_WIRE_LIGHT_ON 
			V_WIRE_COLOR.rgb *= lm;		
			
			#ifdef V_WIRE_GRADIENT
				V_WIRE_GradientColor.rgb *= lm;
			#endif			
		#endif
	#endif

	#ifndef LIGHTMAP_ON
		#ifdef PASS_FORWARD_BASE
			fixed3 diff = (_LightColor0.rgb * (max(0, dot (normalize(i.normal), _WorldSpaceLightPos0.xyz)) * LIGHT_ATTENUATION(i)) + UNITY_LIGHTMODEL_AMBIENT.xyz) * 2;
				 
			#ifdef V_WIRE_LIGHT_ON 
				V_WIRE_COLOR.rgb *= diff;
	
				#ifdef V_WIRE_GRADIENT
					V_WIRE_GradientColor.rgb *= diff;
				#endif
			#endif
			
			retColor.rgb *= diff;
		#endif	
	#endif

	//Reflection
	#ifdef V_WIRE_REFLECTION
		fixed4 reflTex = texCUBE( _Cube, i.refl );

		#ifdef V_WIRE_REFLECTION_COLOR
			reflTex.rgb *= _ReflectColor.rgb;
		#endif

		retColor.rgb += reflTex.rgb * mainTex.a;	
	#endif


	#ifdef V_WIRE_FRESNEL_ON
		V_WIRE_COLOR.a *= i.fresnel;
	#endif
		
	#ifdef V_WIRE_GRADIENT
		Wire(retColor, i.color, i.uv.z);
	#else
		Wire(retColor, i.color, 1);
	#endif

	#ifdef V_WIRE_CUTOUT
		clip( retColor.a - _Cutoff );
	#endif
	
	return retColor;
} 
#endif //frag


#ifdef PASS_SHADOW_CASTER
half4 frag_ShadowCaster(vOutput i) : COLOR 
{	
	#if defined(V_WIRE_CUTOUT)

		#if defined(V_WIRE_CUTWIRE_ON)
			clip(tex2D(_MainTex, i.uv.xy).a * _Color.a - _Cutoff);			
		#else			
			fixed4 retColor = tex2D(_MainTex, i.uv.xy) * _Color;
			Wire(retColor, i.color, 1);

			clip( retColor.a - _Cutoff );
		#endif
		
	#endif

	SHADOW_CASTER_FRAGMENT(i)
}
#endif

#ifdef PASS_SHADOW_COLLECTOR
half4 frag_ShadowCollector(vOutput i) : COLOR 
{
	#if defined(V_WIRE_CUTOUT)
		clip(tex2D(_MainTex, i.uv.xy).a * _Color.a - _Cutoff);
	#endif
	SHADOW_COLLECTOR_FRAGMENT(i)
}
#endif

#ifdef V_WIRE_SURFACE
void surf (Input i, inout SurfaceOutput o) 
{
	half4 retColor = tex2D (_MainTex, i.uv_MainTex);

	#ifdef V_WIRE_REFLECTION
		fixed4 reflcol = texCUBE (_Cube, i.refl);

		#ifdef V_WIRE_REFLECTION_COLOR
			reflcol.rgb *= _ReflectColor.rgb;
		#endif

		o.Emission = reflcol.rgb * retColor.a;
	#endif	

	retColor *= _Color;


	#ifdef V_WIRE_GLOSS
		o.Gloss = retColor.a;
		o.Specular = _Shininess;
	#endif

	#ifdef V_WIRE_FRESNEL_ON
		V_WIRE_COLOR.a *= i.fresnel;
	#endif

	#ifdef V_WIRE_LIGHT_ON 
		#ifdef V_WIRE_GRADIENT
			Wire(retColor, i.color, i.grad);
		#else
			Wire(retColor, i.color, 1);
		#endif
	#endif


	o.Albedo = retColor.rgb;
	o.Alpha = retColor.a; 


	//Point light fix for unlit gradient wire
	#ifndef V_WIRE_LIGHT_ON 
		#ifdef V_WIRE_GRADIENT
			o.Alpha *= i.grad;

			#ifndef V_WIRE_GRADIENT_TRANSPARENT
				o.Albedo *= i.grad;
			#endif
		#endif
	#endif

	#ifdef V_WIRE_BUMP
		o.Normal = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap));
	#endif

	
}
void wireColor (Input i, SurfaceOutput o, inout fixed4 color)
{
	
		#if defined(V_WIRE_GRADIENT) && defined(V_WIRE_GRADIENT_ON)
			color = fixed4(i.grad, i.grad, i.grad, 1);
		#else
			#ifdef UNITY_PASS_FORWARDBASE
				#ifndef V_WIRE_LIGHT_ON
					#ifdef V_WIRE_GRADIENT
						Wire(color, i.color, i.grad);
					#else
						Wire(color, i.color, 1);
					#endif
				#endif
			#endif
		#endif  
	
} 
#endif

#endif	//cginc