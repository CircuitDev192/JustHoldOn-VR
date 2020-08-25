Shader "Custom/StencilMask"
{
	Properties
	{
		//_Mask("Culling Mask", 2D) = "white" {}
		//_Cutoff("Alpha cutoff", Range(0,1)) = 0.1
	}

	SubShader
	{
		Tags {"Queue"="Geometry-100" "RenderType"="Opaque" }
		LOD 100

		ColorMask 0

		ZWrite off

		//Blend SrcAlpha OneMinusSrcAlpha
		//AlphaTest GEqual[_Cutoff]

		Pass
		{
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
			}

			//SetTexture[_Mask]{ combine texture }
			//SetTexture[_MainTex]{ combine texture, previous

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return 0;
			}
			ENDCG
		}
	}
}
