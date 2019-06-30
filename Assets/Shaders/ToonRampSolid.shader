Shader "ShadersPlayground/ToonRampSolid" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_RampTex ("Ramp Texture", 2D) = "white" {}
	}
	SubShader {

		Cull Off
		CGPROGRAM
		#pragma surface surf ToonRamp

		sampler2D _RampTex;
		sampler2D _MainTex;

		struct Input {
			float2 uv_RampTex;
			float2 uv_MainTex;
			float3 viewDir; INTERNAL_DATA
		};

		fixed4 _Color;

		float4 LightingToonRamp(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			float diff = dot(s.Normal, lightDir);
			float h = diff * 0.5 + 0.5;
			float2 rh = h;
			float3 ramp = tex2D(_RampTex, rh).rgb;

			float4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (ramp);
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half NdotVD = 1 - dot(o.Normal, IN.viewDir);
			half h = NdotVD * 0.5 + 0.5;
			half2 rh = h;
			float3 toonRamp = tex2D(_RampTex, rh);
			//float3 mainTex = tex2D(_MainTex, IN.uv_MainTex).rgb;

			o.Albedo = toonRamp.rgb * _Color.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
