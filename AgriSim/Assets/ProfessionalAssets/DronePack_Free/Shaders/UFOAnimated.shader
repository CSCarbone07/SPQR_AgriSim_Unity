// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PA/UFOAnimated"
{
	Properties
	{
		[HDR][NoScaleOffset]_EmissionMap("EmissionMap", 2D) = "white" {}
		[HDR]_Tint("Tint", Color) = (1,1,1,0)
		_EmissionValue("EmissionValue", Float) = 1
		_LEDspeed("LED speed", Range( 0 , 1.5)) = 0.3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" }
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
			uniform sampler2D _EmissionMap;
			uniform float4 _Tint;
			uniform float _LEDspeed;
			uniform float _EmissionValue;
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
				float4 screenColor7 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
				float2 uv_EmissionMap2 = i.ase_texcoord1.xy;
				float2 uv_EmissionMap92 = i.ase_texcoord1.xy;
				float2 uv26 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult29 = lerp( -0.9 , 0.0 , frac( ( _Time.y * _LEDspeed ) ));
				
				
				finalColor = ( screenColor7 + ( ( ( (0.7 + (sin( ( _Time.y * 2.0 ) ) - 0.0) * (1.0 - 0.7) / (1.0 - 0.0)) * tex2D( _EmissionMap, uv_EmissionMap2 ) ) + ( _Tint * tex2D( _EmissionMap, uv_EmissionMap92 ).a * ceil( ( uv26.y + lerpResult29 ) ) ) ) * _EmissionValue * 3.0 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=15401
170;305;1369;660;2119.605;283.5193;1.603232;True;False
Node;AmplifyShaderEditor.RangedFloatNode;22;-1732.506,497.4738;Float;False;Property;_LEDspeed;LED speed;3;0;Create;True;0;0;False;0;0.3;0.3;0;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;40;-1754.805,-47.86496;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1468.515,494.0096;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;45;-1339.678,492.7332;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;29;-1225.662,491.5568;Float;False;3;0;FLOAT;-0.9;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-1555.469,-49.77081;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-1355.084,377.7539;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;98;-1425.638,-48.67851;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-1081.018,488.5213;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;99;-1312.142,-48.21461;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.7;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;92;-1135.393,303.238;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;53;-1050.92,140.4338;Float;False;Property;_Tint;Tint;1;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0.6666667,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CeilOpNode;46;-968.676,490.4141;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1138.551,-47.2402;Float;True;Property;_EmissionMap;EmissionMap;0;2;[HDR];[NoScaleOffset];Create;True;0;0;False;0;9f18a7c145667d744ab2551f85cefc2c;9f18a7c145667d744ab2551f85cefc2c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-857.1002,-45.48456;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-856.4092,44.2155;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-720.7576,-14.313;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-854.9672,159.4551;Float;False;Property;_EmissionValue;EmissionValue;2;0;Create;True;0;0;False;0;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-822.3164,230.8747;Float;False;Constant;_HDRvalue;HDR value;1;0;Create;True;0;0;False;0;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-606.7246,-12.67842;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;7;-576.0554,-181.1174;Float;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-385.4427,-90.73676;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;107;-253.3141,-90.03957;Float;False;True;2;Float;ASEMaterialInspector;0;1;PA/UFOAnimated;0770190933193b94aaa3065e307002fa;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;-1;False;-1;-1;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Transparent;True;2;0;False;False;False;False;False;False;False;False;False;False;0;;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;23;0;40;2
WireConnection;23;1;22;0
WireConnection;45;0;23;0
WireConnection;29;2;45;0
WireConnection;97;0;40;2
WireConnection;98;0;97;0
WireConnection;30;0;26;2
WireConnection;30;1;29;0
WireConnection;99;0;98;0
WireConnection;46;0;30;0
WireConnection;95;0;99;0
WireConnection;95;1;2;0
WireConnection;47;0;53;0
WireConnection;47;1;92;4
WireConnection;47;2;46;0
WireConnection;101;0;95;0
WireConnection;101;1;47;0
WireConnection;3;0;101;0
WireConnection;3;1;4;0
WireConnection;3;2;6;0
WireConnection;14;0;7;0
WireConnection;14;1;3;0
WireConnection;107;0;14;0
ASEEND*/
//CHKSM=D8A9FAB72C8225FC837A4B14EE548389A2874A55