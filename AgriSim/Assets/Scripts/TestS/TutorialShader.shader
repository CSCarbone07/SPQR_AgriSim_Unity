// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "zeroTutorial"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTexture ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_DissolveTexture("Cheese", 2D) = "white" {}
		_DissolveAmount("CheeseCutOutAmount", Float) = 1

		_ExtrudeAmount("Extrude Amount", float) = 1

    }
    SubShader
	{

		Pass
		{
			CGPROGRAM

			#pragma vertex vertexFunction
			#pragma fragment fragmentFunction

			#include "UnityCG.cginc"

			
			//Vertices
			//Normal
			//Color
			//UV
			struct appdata
			{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//importing variables into the CG part
			float4 _Color;
			sampler2D _MainTexture;

			sampler2D _DissolveTexture;
			float _DissolveAmount;

			float _ExtrudeAmount;

			//Build our object
			v2f vertexFunction(appdata IN)
			{
				v2f OUT;

				IN.vertex.xyz += IN.normal.xyz * _ExtrudeAmount;

				OUT.position = UnityObjectToClipPos(IN.vertex); //mul(UNITY_MATRIX_MVP, IN.vertex); //
				OUT.uv = IN.uv;

				return OUT;


			};

			//Color it in, pixel to the screen
			fixed4 fragmentFunction(v2f IN) : SV_Target
			{
				float4 textureColor = tex2D(_MainTexture, IN.uv);
				float4 DissolveTexture = tex2D(_DissolveTexture, IN.uv);
				
				//clip(dissolveColor.rgb - _DissolveAmount);

				return textureColor * _Color;
			}



			ENDCG
		}
		

	}


}
