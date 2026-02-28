Shader "UltraTimeManipulationTargetShader" 
{
    //applies gamma. Used to reverse the game's gamma across the entire screen for enemies and projectiles (targets) (gamma is a reversible process). 
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
        //_UltraTimeManipulationTargetTint ("Tint", Vector) = (1, 1, 1, 1)
		//[HideInInspector] _UltraTimeManipulationColorChangeAmount ("Color Change Amount", Range(0, 1)) = 1
        //[HideInInspector] _UltraTimeManipulationGammaAdjust("Gamma Adjust", Range(0, 100)) = 1
	}
	//DummyShaderTextExporter
	SubShader
	{
		Tags {"RenderType"="Opaque"}
		LOD 200

		Pass
		{
            Blend SrcAlpha OneMinusSrcAlpha // (Traditional transparency)
	        BlendOp Add // (is default anyway)

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			//float4x4 unity_ObjectToWorld;
			//float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			//Texture2D<float4> _MainTex;
			sampler2D _MainTex;
			//float4 _WhiteTint;
			//float4 _BlackTint;
            //float4 _UltraTimeManipulationTargetTint;
			//float _UltraTimeManipulationColorChangeAmount;
            float _UltraTimeManipulationGammaCurrentAlpha;
            float _UltraTimeManipulationGammaAdjust;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				output.color = input.color;
				return output;
			}

			//SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				float4 col = tex2D(_MainTex, input.uv);
                /*float opacity = col.a;
                float colBrightness = (col.r + col.g + col.b) / 3;
                col = (col * (1 - _UltraTimeManipulationColorChangeAmount)) + (_UltraTimeManipulationTargetTint * colBrightness * _UltraTimeManipulationColorChangeAmount);
                col.a = opacity;*/
                col = pow(col.rgba, 1.0 / _UltraTimeManipulationGammaAdjust);
                col.a = col.a * _UltraTimeManipulationGammaCurrentAlpha;
                return col;
			}

			ENDHLSL
		}
	}
}