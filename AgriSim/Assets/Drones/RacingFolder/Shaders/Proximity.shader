// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "DroneController/Proximity" {
      Properties {
		_PlayerPosition ("Player Position", vector) = (0,0,0,0) // The location of the player - will be set by script
		_VisibleDistance ("Visibility Distance", float) = 10.0 // How close does the player have to be to make object visible
		_OutlineColour ("Outline Colour", color) = (1.0,1.0,0.0,1.0) // Colour of the outline

		_FadeDistance("Fade Distance", Range(-5,5)) = -0.32
		_FadeStrength("FadeSheit", Range(0,0.2)) = 0.048
		_FadePower("Fade Power", float) = 0.24
	  }
      SubShader {
          Tags { "RenderType"="Transparent" "Queue"="Transparent"}
          Pass {
          Blend SrcAlpha OneMinusSrcAlpha
          LOD 200
      
          CGPROGRAM
          #pragma vertex vert
          #pragma fragment frag
  
          // Access the shaderlab properties
          uniform float4 _PlayerPosition;
          uniform float _VisibleDistance;
          uniform fixed4 _OutlineColour;

		  float _FadeStrength;
		  half _FadeDistance;
		  float _FadePower;

          // Input to vertex shader
          struct vertexInput {
              float4 vertex : POSITION;
           };
          // Input to fragment shader
           struct vertexOutput {
              float4 pos : SV_POSITION;
              float4 position_in_world_space : TEXCOORD0;
           };
           
           // VERTEX SHADER
           vertexOutput vert(vertexInput input) 
           {
              vertexOutput output; 
              output.pos =  UnityObjectToClipPos(input.vertex);
              output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
			
              return output;
           }
   
           // FRAGMENT SHADER
          float4 frag(vertexOutput input) : COLOR 
          {
              float dist = distance(input.position_in_world_space, _PlayerPosition);
           
                 _OutlineColour.a = _FadeDistance - (_VisibleDistance -  dist);
				 _OutlineColour.a *= pow(_FadeStrength, _FadePower);
				 _OutlineColour.a *= pow(_FadeStrength, _FadePower);
				 _OutlineColour.a *= pow(_FadeStrength, _FadePower);

                 return _OutlineColour; 
          }

          ENDCG
          }
      }
  }