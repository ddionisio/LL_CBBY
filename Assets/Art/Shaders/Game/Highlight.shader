Shader "Game/Highlight" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	_Color ("Color", Color) = (1,1,1,1)
	_StartOpaque("Start Opaque", Float) = 0
	_EndOpaque("End Opaque", Float) = 1
	_Speed ("Speed", Float) = 1
  }
  SubShader {
    Tags { "RenderType" = "Opaque" }
	Lighting Off Fog { Mode Off }
    ColorMask RGB

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _Color;
		fixed _StartOpaque;
		fixed _EndOpaque;
		fixed _Speed;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR0;
			fixed4 colorH : COLOR1;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color;
			o.colorH = fixed4(_Color.rgb, lerp(_StartOpaque, _EndOpaque, sin(_Speed*_Time.y)));
			o.texcoord = v.texcoord;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
			
			return fixed4(lerp(col.rgb, i.colorH.rgb, i.colorH.a), col.a);
		}

		ENDCG
	}
  }
  Fallback "Diffuse"
}
