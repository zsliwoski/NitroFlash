Shader "Unlit/ScreenFaceShader"
{
	Properties
	{
		_OverlayTex("Texture", 2D) = "white" {}
		_PrimaryColor ("BG Color", Color) = (1,1,1,1)
		_OverlayColor ("Overlay Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _OverlayTex;
			float4 _OverlayTex_ST;
			fixed4 _PrimaryColor;
			fixed4 _OverlayColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _OverlayTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 bgCol = _PrimaryColor;
				fixed4 overlayMask = tex2D(_OverlayTex,i.uv);
				fixed4 overlayCol = _OverlayColor;
				fixed4 finalCol = {
					lerp(bgCol.r,overlayCol.r,overlayMask.r),
					lerp(bgCol.g,overlayCol.g,overlayMask.g),
					lerp(bgCol.b,overlayCol.b,overlayMask.b),
					1.0f
					};
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, finalCol);
				return finalCol;
			}
			ENDCG
		}
	}
}
