// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SimpleOutlineShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (1,1,1,0.5)
		_OutlineWidth ("Outline Width", range(1.0,2.0)) = 1.1
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200

         Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			Cull front
			Fog { Mode off }
			
			CGPROGRAM  		
			#pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      		
      #include "UnityCG.cginc"
			
			half4 _OutlineColor;
			float _OutlineWidth;
			
			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_base v) {
				v.vertex.x *= _OutlineWidth;
				v.vertex.y *= _OutlineWidth;
				v.vertex.z *= _OutlineWidth;
				
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o; 
			} 
			
			half4 frag(v2f i) : COLOR { 
				return _OutlineColor;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}