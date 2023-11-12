Shader "Custom/TextOutlighting"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]



		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID

				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _ClipRect;
			uniform float4 _OutlightingColor;
			uniform float _OutlightingIntensity;
			uniform float _BlurRadius;
			fixed4 _TextureSampleAdd;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord.xy = IN.texcoord;
				
				OUT.color = IN.color;

				return OUT;
			}

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;

			float GetGaussianDistribution(float x, float rho){
				float g = 1.0f / sqrt(2.0f * 3.141592654f * rho * rho);
				return g * exp(-(x * x) / (2 * rho * rho));
			}

			float GetGaussBlurAlpha(float2 uv){
				float cellX = _MainTex_TexelSize.x;
				float cellY = _MainTex_TexelSize.y;
				float rho = (float)cellX / 3.0;

				float weightTotal = 0;
				weightTotal += GetGaussianDistribution(-1 * cellX, rho);
				weightTotal += GetGaussianDistribution(0 * cellX, rho);
				weightTotal += GetGaussianDistribution(1 * cellX, rho);

				float alphaTmp = 0;
				float weight = GetGaussianDistribution(-1 * cellX, rho) / weightTotal;
				alphaTmp += tex2D(_MainTex, uv + float2(-1 * cellX, -1 * cellY)).a * weight;
				alphaTmp += tex2D(_MainTex, uv + float2(-1 * cellX, 0 * cellY)).a * weight;
				alphaTmp += tex2D(_MainTex, uv + float2(-1 * cellX, 1 * cellY)).a * weight;

				weight = GetGaussianDistribution(0 * cellX, rho) / weightTotal;
				alphaTmp += tex2D(_MainTex, uv + float2(0 * cellX, -1 * cellY)).a * weight;
				alphaTmp += tex2D(_MainTex, uv + float2(0 * cellX, 0 * cellY)).a * weight;
				alphaTmp += tex2D(_MainTex, uv + float2(0 * cellX, 1 * cellY)).a * weight;

				weight = GetGaussianDistribution(1 * cellX, rho) / weightTotal;
				alphaTmp += tex2D(_MainTex, uv + float2(1 * cellX, -1 * cellY)).a * weight;
				alphaTmp += tex2D(_MainTex, uv + float2(1 * cellX, 0 * cellY)).a * weight;
				alphaTmp += tex2D(_MainTex, uv + float2(1 * cellX, 1 * cellY)).a * weight;

				return alphaTmp;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				//²ÉÑù
				fixed4 result = (tex2D(_MainTex,IN.texcoord) + _TextureSampleAdd) * IN.color;
				float accAlpha = GetGaussBlurAlpha(IN.texcoord);

				fixed4 col = lerp(result,_OutlightingColor, 1 - result.a);
				col.a = col.a * (col.a + 1) * pow(accAlpha,_OutlightingIntensity);
				col.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				#ifdef UNITY_UI_ALPHACLIP
				clip (col.a - 0.001);
				#endif

				return col;
			}
		ENDCG
		}
	}
}
