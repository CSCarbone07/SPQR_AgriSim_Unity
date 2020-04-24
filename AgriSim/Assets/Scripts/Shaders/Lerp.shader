Shader "Plant/Lerp"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        _Tex1 ("Texture 1 (RGB)", 2D) = "white" {}
		_Tex1N ("Texture 1 Normal", 2D) = "white" {}

		_Tex2 ("Texture 2 (RGB)", 2D) = "white" {}
		_Tex2N ("Texture 2 Normal", 2D) = "white" {}

		_Nstr ("Normals Strenght", float) = 1

		_TexN ("Noise", 2D) = "white" {}
		_NoiseOffset("NoiseOffset", Range(0,1)) = 0


        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Tex1;
		sampler2D _Tex1N;

		sampler2D _Tex2;
		sampler2D _Tex2N;

		float _Nstr;

		sampler2D _TexN;
		float _NoiseOffset;

        struct Input
        {
            float2 uv_Tex1;
			float2 uv_Tex1N;

			float2 uv_Tex2;
			float2 uv_Tex2N;

			float2 uv_TexN;

        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			IN.uv_TexN = IN.uv_TexN + _NoiseOffset;
			fixed4 c = tex2D(_Tex1, IN.uv_Tex1) * tex2D(_TexN, IN.uv_TexN) + tex2D(_Tex2, IN.uv_Tex2) * ((1,1)-tex2D(_TexN, IN.uv_TexN));
            o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_Tex1N, IN.uv_Tex1N)*tex2D(_TexN, IN.uv_TexN) + tex2D(_Tex2N, IN.uv_Tex2N) * ((1, 1) - tex2D(_TexN, IN.uv_TexN)));
			o.Normal *= float3(_Nstr, _Nstr, 1);
			// Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
