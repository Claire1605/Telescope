Shader "Custom/VectorMultiColourSwap"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_LightColour("Light Colour", Color) = (1,1,1,1)
		_LightRedValue("Light Red Value", Float) = 0.75
		_MidColour("Mid Colour", Color) = (1,1,1,1)
		_MidRedValue("Mid Red Value", Float) = 0.5
		_ShadeColour("Shade Colour", Color) = (1,1,1,1)
		_ShadeRedValue("Shade Red Value", Float) = 0.25
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex VectorVert
				#pragma fragment SpriteFrag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

				float _ColourOverride;
				float _OverlayBlend;
				sampler2D _OverlayTex;
				fixed4 _LightColour;
				float _LightRedValue;
				fixed4 _MidColour;
				float _MidRedValue;
				fixed4 _ShadeColour;
				float _ShadeRedValue;

				v2f VectorVert(appdata_t IN)
				{
					v2f OUT;

					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

					OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
					OUT.vertex = UnityObjectToClipPos(OUT.vertex);
					OUT.texcoord = IN.texcoord;

					#ifdef UNITY_COLORSPACE_GAMMA
					fixed4 color = IN.color;
					#else
					fixed4 color = fixed4(GammaToLinearSpace(IN.color.rgb), IN.color.a);
					#endif

					float useLightValue = max(0, sign(color.r - _LightRedValue));
					float useMidValue = max(0, sign(color.r - _MidRedValue) * (1 - useLightValue));
					float useShadeValue = max(0, sign(color.r - _ShadeRedValue) * (1 - useMidValue) * (1 - useLightValue));

					OUT.color = useLightValue * _LightColour + useMidValue * _MidColour + useShadeValue * _ShadeColour;

					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}
			ENDCG
			}
		}
}
