Shader "Orbital Path" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 0.15)
		_A ("Semimajor Axis", Float) = 100
		[PowerSlider(3)] _E ("Eccentricity", Range(0, 1)) = 0
	}
	SubShader {
		Tags {
			"RenderType" = "Transparent"
			"RenderQueue" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#define PI 3.1415926535

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _A;
			float _E;

			fixed4 _Color;

			v2f vert(appdata_base v) {
				v2f o;
				float theta = v.vertex.x * PI * 2;
				float sin_t = sin(theta);
				float cos_t = cos(theta);
				float r =
					(_A * (1 - (_E * _E)))
					/ (1 + (_E * cos_t));
				o.vertex = UnityObjectToClipPos(
					float4(
						r * sin_t,
						0,
						r * cos_t,
						1
					)
				);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				return _Color;
			}
			ENDCG
		}
	}
}
