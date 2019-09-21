Shader "Custom/CustomStandard"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		[MaterialToggle] _EnableHDR("Enable HDR", Float) = 0
		[HDR]_HDRColor("HDR Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Occlusion("Occlusion Map", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

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


		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Occlusion;
		fixed4 _Color;
		fixed4 _HDRColor;
		float _Metallic;
		float _Glossiness;
		float _EnableHDR;

        struct Input
        {
			float2 uv_MainTex;
			float2 uv_Occlusion;
			float2 uv_BumpMap;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 Occ = tex2D(_Occlusion, IN.uv_Occlusion);
			fixed4 hdr;
			float4 clear = (1, 1, 1, 1);
			hdr = lerp(clear, _HDRColor, _EnableHDR);
            o.Albedo = c.rgb * Occ.rgb * hdr.rgb * _Color.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
