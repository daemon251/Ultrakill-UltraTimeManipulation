Shader "UltraTimeManipulationHandShader" 
{
	//leaves whiteish colors more white. 
    //effect lowers as cooldown remaining is low (done in C#)
    //colors everything bright blue-green, slightly darker in areas originally more dark. 
    //vibrant colors aren't fully converted - yellow -> green, red -> blue

    //implement this by giving a maximum hue change budget- hue can only be changed by 120deg to match the desired color
    //for some reason, weapon variation colors are handled in a FUCKING shader (what the fuck?), 
    //and we won't be respecting that cause this will overwrite that shader, unless if I can find a clever trick around that. So that'll be a problem for some specific 
    //weapon color schemas, and it would be a lot of work to figure that out until we can actually get the code for ultrakill's shaders (decomp dont work)

    //at the end, undo gamma changes same as with targetShader
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
		Tags {"RenderType"="Transparent"}
		LOD 200

		Pass
		{
            Blend SrcAlpha OneMinusSrcAlpha // (Traditional transparency)
	        BlendOp Add // (is default anyway)
            Cull Off 
            //ZWrite Off 
            //ZTest Always

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
            float4 _UltraTimeManipulationTargetTint;
			float _UltraTimeManipulationColorChangeAmount;
            float _UltraTimeManipulationGammaAdjust;

            float _UltraTimeManipulationNoiseIntensity; //0.05 is a good value
            float _UltraTimeManipulationNoiseFrequency; //5000 is a good value
            float _UltraTimeManipulationGrainIntensity; //0.05 is a good value
            float _UltraTimeManipulationColorGrainIntensity; //0.05 is a good value

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

            float getRandom(float2 xy, float seed) //"seed", I doubt its super effective
            {
                float output = 0;
		
                //amalgamation of a bunch of shitty algorithms to produce a slightly less shitty one.
                float input = sin(xy.x * 10000 + seed); //dont trivially relate x and y here, or you will produce seams
                input = sin(input * 10000 + xy.y * 6.28f); 
                
                //this also makes the number positive.
                input = (input * input * 4) % 1;
                //the fact that this is not evenly distributed does not matter
                
                //this step only works if the input float is ALREADY noisy.
                //the idea is the last digit is already pretty "random"
                for(uint i = 0; i < 8; i++)
                {
                    uint valueN = (uint)((pow(input, i) * (uint)pow(10, 9))) % 10; //should be decently random
                    //System.out.println(valueN);
                    output += valueN / pow(10, i);
                }
                return output;
            }

            float4 RGBtoHSV(float4 RGB)
            {
                float4 HSV = 0;
                float M = min(RGB.r, min(RGB.g, RGB.b));
                HSV.z = max(RGB.r, max(RGB.g, RGB.b));
                float C = HSV.z - M;
                if (C != 0)
                {
                    HSV.y = C / HSV.z;
                    float3 D = (((HSV.z - RGB) / 6) + (C / 2)) / C;
                    if (RGB.r == HSV.z)
                        HSV.x = D.b - D.g;
                    else if (RGB.g == HSV.z)
                        HSV.x = (1.0/3.0) + D.r - D.b;
                    else if (RGB.b == HSV.z)
                        HSV.x = (2.0/3.0) + D.g - D.r;
                    if (HSV.x < 0.0) {HSV.x += 1.0;}
                    if (HSV.x > 1.0) {HSV.x -= 1.0;}
                }
                HSV.w = RGB.w;
                return HSV;
            }
            
            float4 Hue(float H)
            {
                float R = abs(H * 6 - 3) - 1;
                float G = 2 - abs(H * 6 - 2);
                float B = 2 - abs(H * 6 - 4);
                float3 colSat = saturate(float3(R,G,B));
                return float4(colSat.x, colSat.y, colSat.z, 1);
            }

            float4 HSVtoRGB(in float4 HSV)
            {
                float4 col = ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
                col.w = HSV.w;
                return col;
            }
                
            uint _UltraTimeManipulationUseOpacity; 

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
                //this sucks, literally who cares
                if(_UltraTimeManipulationUseOpacity != 0)
                {
                    float4 col = tex2D(_MainTex, input.uv);
                    float opacity = col.a;
                    //float colBrightness = (col.r + col.g + col.b) / 3;
                    col = _UltraTimeManipulationTargetTint; //(col * (1 - _UltraTimeManipulationColorChangeAmount)) + (_UltraTimeManipulationTargetTint * colBrightness * _UltraTimeManipulationColorChangeAmount);
                    col.a = opacity * _UltraTimeManipulationColorChangeAmount;

                    //col = pow(col.rgba, 1.0 / _UltraTimeManipulationGammaAdjust); //gamma
                    float random1 = 0.5 + 0.5 * cos(_UltraTimeManipulationNoiseFrequency * dot(input.uv, float2(23.14069263277926, 2.665144142690225))); //0 to 1
                    col.a = col.a * (1 - random1 * _UltraTimeManipulationNoiseIntensity); //don't know what to call this but its cool

                    float random2 = getRandom(input.uv, 0); //dunno is this good enough
                    col.a = col.a * (1 - random2 * _UltraTimeManipulationGrainIntensity); //grain

                    float random3 = getRandom(input.uv, 1); 
                    float4 colHSV = RGBtoHSV(col);
                    colHSV.x = colHSV.x + _UltraTimeManipulationColorGrainIntensity / 2 - random3 * _UltraTimeManipulationColorGrainIntensity;
                    col = HSVtoRGB(colHSV);

                    return col;
                }
                else
                {
                    float4 col = tex2D(_MainTex, input.uv);
                    float opacity = col.a;

                    float endColorChangeAmount = _UltraTimeManipulationColorChangeAmount;

                    float random1 = 0.5 + 0.5 * cos(_UltraTimeManipulationNoiseFrequency * dot(input.uv, float2(23.14069263277926, 2.665144142690225))); //0 to 1
                    endColorChangeAmount = endColorChangeAmount * (1 - random1 * _UltraTimeManipulationNoiseIntensity); //don't know what to call this but its cool

                    float random2 = getRandom(input.uv, 0); //dunno is this good enough
                    endColorChangeAmount = endColorChangeAmount * (1 - random2 * _UltraTimeManipulationGrainIntensity); //grain

                    col = _UltraTimeManipulationTargetTint * endColorChangeAmount + col * (1 - endColorChangeAmount);

                    float random3 = getRandom(input.uv, 1); 
                    float4 colHSV = RGBtoHSV(col);
                    colHSV.x = colHSV.x + _UltraTimeManipulationColorGrainIntensity / 2 - random3 * _UltraTimeManipulationColorGrainIntensity;
                    col = HSVtoRGB(colHSV);

                    col.a = opacity;

                    return col;
                }
			}

			ENDHLSL
		}
	}
}