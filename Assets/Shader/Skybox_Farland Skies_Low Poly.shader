Shader "Skybox/Farland Skies/Low Poly" {
	Properties {
		_TopColor ("Color Top", Vector) = (0.5,0.5,0.5,1)
		_MiddleColor ("Color Middle", Vector) = (0.5,0.5,0.5,1)
		_BottomColor ("Color Bottom", Vector) = (0.5,0.5,0.5,1)
		_TopExponent ("Exponent Top", Range(0.01, 5)) = 1
		_BottomExponent ("Exponent Bottom", Range(0.01, 5)) = 1
		_StarsTint ("Stars Tint", Vector) = (0.5,0.5,0.5,1)
		_StarsExtinction ("Stars Extinction", Range(0, 10)) = 2
		_StarsTwinklingSpeed ("Stars Twinkling Speed", Range(0, 25)) = 4
		[NoScaleOffset] _StarsTex ("Stars Cubemap", Cube) = "grey" {}
		_SunSize ("Sun Size", Range(0.1, 3)) = 1
		_SunTint ("Sun Tint", Vector) = (0.5,0.5,0.5,1)
		_SunHalo ("Sun Halo", Range(0, 2)) = 1
		[NoScaleOffset] _SunTex ("Sun Texture", 2D) = "grey" {}
		_MoonSize ("Moon Size", Range(0.1, 3)) = 1
		_MoonTint ("Moon Tint", Vector) = (0.5,0.5,0.5,1)
		_MoonHalo ("Moon Halo", Range(0, 2)) = 1
		[NoScaleOffset] _MoonTex ("Moon Texture", 2D) = "grey" {}
		_CloudsTint ("Clouds Tint", Vector) = (0.5,0.5,0.5,1)
		_CloudsRotation ("Clouds Rotation", Range(0, 360)) = 0
		_CloudsHeight ("Clouds Height", Range(-0.75, 0.75)) = 0
		[NoScaleOffset] _CloudsTex ("Clouds Cubemap", Cube) = "grey" {}
		[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
}