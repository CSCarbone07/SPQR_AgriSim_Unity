// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PA/SpaceProbeAnimated"
{
	Properties
	{
		[NoScaleOffset]_EmissionMap("EmissionMap", 2D) = "white" {}
		[HDR]_EmmissionColor("Emmission Color", Color) = (1,0.757,0,1)
		[NoScaleOffset]_FaceMask("FaceMask", 2D) = "white" {}
		_FaceLEDRandomization("FaceLEDRandomization", Range( 0 , 0.5)) = 0.3
		_FaceLEDSpeed("FaceLEDSpeed", Float) = 6
		_ArmsLEDSpeed("ArmsLEDSpeed", Float) = 6
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		GrabPass{ }


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
			};

			uniform sampler2D _GrabTexture;
			uniform float _FaceLEDRandomization;
			uniform float _FaceLEDSpeed;
			uniform sampler2D _FaceMask;
			uniform sampler2D _EmissionMap;
			uniform float _ArmsLEDSpeed;
			uniform float4 _EmmissionColor;
			inline float4 ASE_ComputeGrabScreenPos( float4 pos )
			{
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				float4 o = pos;
				o.y = pos.w * 0.5f;
				o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
				return o;
			}
			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				float4 screenPos = i.ase_texcoord;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 screenColor413 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
				float temp_output_239_0 = ( _FaceLEDRandomization * _Time.x );
				float2 panner233 = ( temp_output_239_0 * float2( 1,1 ) + float2( 0.5,0 ));
				float2 uv_FaceMask155 = i.ase_texcoord1.xy;
				float4 tex2DNode155 = tex2D( _FaceMask, uv_FaceMask155 );
				float2 panner231 = ( temp_output_239_0 * float2( 1,1 ) + float2( 0,0.5 ));
				float2 panner228 = ( temp_output_239_0 * float2( 1,1 ) + float2( 0,0 ));
				float2 uv_EmissionMap4 = i.ase_texcoord1.xy;
				float4 tex2DNode4 = tex2D( _EmissionMap, uv_EmissionMap4 );
				float2 uv379 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_387_0 = sin( ( _Time.y * _ArmsLEDSpeed ) );
				float lerpResult383 = lerp( -0.495 , -0.45 , temp_output_387_0);
				float lerpResult394 = lerp( -0.13 , -0.08 , -temp_output_387_0);
				float2 uv392 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				
				
				finalColor = ( screenColor413 + ( max( ( ( round( ( sin( ( ( tex2D( _FaceMask, panner233 ).a + _Time.z ) * _FaceLEDSpeed ) ) + 0.8 ) ) * tex2DNode155.r ) + ( tex2DNode155.g * round( ( sin( ( _FaceLEDSpeed * ( tex2D( _FaceMask, panner231 ).a + _Time.z ) ) ) + 0.8 ) ) ) + ( tex2DNode155.b * round( ( sin( ( _FaceLEDSpeed * ( tex2D( _FaceMask, panner228 ).a + _Time.z ) ) ) + 0.8 ) ) ) ) , max( ( tex2DNode4.a * (0.0 + (abs( ( uv379.y + lerpResult383 ) ) - 0.03) * (1.0 - 0.0) / (0.01 - 0.03)) ) , ( tex2DNode4.a * (0.0 + (abs( ( lerpResult394 + uv392.y ) ) - 0.03) * (1.0 - 0.0) / (0.01 - 0.03)) ) ) ) * 1.5 * _EmmissionColor * 3.0 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=15401
62;218;1069;660;2478.414;-1064.825;1.740449;True;False
Node;AmplifyShaderEditor.TimeNode;179;-1502.24,1138.115;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;240;-1569.608,1051.563;Float;False;Property;_FaceLEDRandomization;FaceLEDRandomization;3;0;Create;True;0;0;False;0;0.3;0.3;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-1299.523,1056.304;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;384;-1327.448,1668.816;Float;False;Property;_ArmsLEDSpeed;ArmsLEDSpeed;5;0;Create;True;0;0;False;0;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;233;-1125.131,884.0399;Float;False;3;0;FLOAT2;0.5,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;386;-1139.649,1650.935;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;228;-1122.987,1134.715;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;231;-1125.623,1010.217;Float;False;3;0;FLOAT2;0,0.5;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;227;-933.4722,1179.898;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;155;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;387;-1011.198,1652.802;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;234;-932.7626,781.0701;Float;True;Property;_TextureSample2;Texture Sample 2;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;155;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;232;-932.4017,981.9694;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;155;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-604.4517,878.6761;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;400;-897.7,1651.398;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;-833.6393,704.0293;Float;False;Property;_FaceLEDSpeed;FaceLEDSpeed;4;0;Create;True;0;0;False;0;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-605.2745,991.3508;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-603.1528,1105.212;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-474.744,1103.208;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-480.8648,992.8262;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;383;-768.8347,1491.748;Float;False;3;0;FLOAT;-0.495;False;1;FLOAT;-0.45;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-482.9263,877.5057;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;379;-845.44,1378.621;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;394;-769.5969,1606.101;Float;False;3;0;FLOAT;-0.13;False;1;FLOAT;-0.08;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;392;-845.812,1720.753;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;188;-346.744,1103.208;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;182;-346.5108,878.5528;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;180;-343.1849,992.3503;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;393;-609.817,1606.531;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;380;-614.4502,1467.916;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;260;-216.7186,992.4456;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;396;-492.1318,1606.198;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;259;-212.2243,1106.588;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;381;-494.6811,1468.385;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;261;-220.7549,879.0308;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;183;-93.64156,878.7215;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;397;-376.515,1634.149;Float;False;5;0;FLOAT;0;False;1;FLOAT;0.03;False;2;FLOAT;0.01;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;172;-90.16024,992.2916;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;390;-376.7739,1469.17;Float;False;5;0;FLOAT;0;False;1;FLOAT;0.03;False;2;FLOAT;0.01;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;189;-90.52534,1107.271;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;155;-600.4744,1202.937;Float;True;Property;_FaceMask;FaceMask;2;1;[NoScaleOffset];Create;True;0;0;False;0;017a54f884a981348a183f64b04e496d;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1125.381,1376.783;Float;True;Property;_EmissionMap;EmissionMap;0;1;[NoScaleOffset];Create;True;0;0;False;0;101a6f6a9bf2ad74bbd861c8d6aa9885;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;32.13008,989.9047;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;398;-194.7206,1568.265;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;32.63275,1103.007;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;391;-196.2127,1471.927;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;32.03059,878.4313;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;401;-48.01715,1486.57;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;177;191.5014,965.824;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;408;286.0728,1316.003;Float;False;Constant;_HDRStrength;HDRStrength;6;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;259.2023,1152.699;Float;False;Property;_EmmissionColor;Emmission Color;1;1;[HDR];Create;True;0;0;False;0;1,0.757,0,1;1,0.8078431,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;191.3492,1079.058;Float;False;Constant;_EmmissionValue;Emmission Value;2;0;Create;True;0;0;False;0;1.5;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;416;338.6237,965.3248;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;409;497.5061,1067.795;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;413;530.2278,900.3984;Float;False;Global;_GrabScreen0;Grab Screen 0;6;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;411;725.2629,994.0613;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;417;851.7404,993.3809;Float;False;True;2;Float;ASEMaterialInspector;0;1;PA/SpaceProbeAnimated;0770190933193b94aaa3065e307002fa;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;-1;False;-1;-1;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque;True;2;0;False;False;False;False;False;False;False;False;False;False;0;;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;239;0;240;0
WireConnection;239;1;179;1
WireConnection;233;1;239;0
WireConnection;386;0;179;2
WireConnection;386;1;384;0
WireConnection;228;1;239;0
WireConnection;231;1;239;0
WireConnection;227;1;228;0
WireConnection;387;0;386;0
WireConnection;234;1;233;0
WireConnection;232;1;231;0
WireConnection;207;0;234;4
WireConnection;207;1;179;3
WireConnection;400;0;387;0
WireConnection;185;0;232;4
WireConnection;185;1;179;3
WireConnection;191;0;227;4
WireConnection;191;1;179;3
WireConnection;187;0;184;0
WireConnection;187;1;191;0
WireConnection;175;0;184;0
WireConnection;175;1;185;0
WireConnection;383;2;387;0
WireConnection;208;0;207;0
WireConnection;208;1;184;0
WireConnection;394;2;400;0
WireConnection;188;0;187;0
WireConnection;182;0;208;0
WireConnection;180;0;175;0
WireConnection;393;0;394;0
WireConnection;393;1;392;2
WireConnection;380;0;379;2
WireConnection;380;1;383;0
WireConnection;260;0;180;0
WireConnection;396;0;393;0
WireConnection;259;0;188;0
WireConnection;381;0;380;0
WireConnection;261;0;182;0
WireConnection;183;0;261;0
WireConnection;397;0;396;0
WireConnection;172;0;260;0
WireConnection;390;0;381;0
WireConnection;189;0;259;0
WireConnection;176;0;155;2
WireConnection;176;1;172;0
WireConnection;398;0;4;4
WireConnection;398;1;397;0
WireConnection;192;0;155;3
WireConnection;192;1;189;0
WireConnection;391;0;4;4
WireConnection;391;1;390;0
WireConnection;165;0;183;0
WireConnection;165;1;155;1
WireConnection;401;0;391;0
WireConnection;401;1;398;0
WireConnection;177;0;165;0
WireConnection;177;1;176;0
WireConnection;177;2;192;0
WireConnection;416;0;177;0
WireConnection;416;1;401;0
WireConnection;409;0;416;0
WireConnection;409;1;5;0
WireConnection;409;2;7;0
WireConnection;409;3;408;0
WireConnection;411;0;413;0
WireConnection;411;1;409;0
WireConnection;417;0;411;0
ASEEND*/
//CHKSM=29EC0875F6F5C8F98856F68B823B520D41556830