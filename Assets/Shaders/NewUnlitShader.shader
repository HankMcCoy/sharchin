Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float screenDepth = DecodeFloatRG(tex2d(_CameraDepthNormalsTexture, i.screenuv).zw);
                float diff = screenDepth - i.depth;
                float intersect = 0;

                if(diff > 0)
                    intersect = 1 - smoothstep(0, _ProjectionParams.w * 0.5f, diff);

                //fixed4 col = _Color * _Color.a + intersect;

                fixed4 col = fixed4(lerp(_Color.rgb, fixed3(1,1,1), pow(intersect,4)),1);

                return col;
            }

            v2f vert(appdata v){
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

                o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) / 2;
                o.screenuv.y = 1 - o.screenuv.y;
                o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z *_ProjectionParams.w;

                return o;
            }
		}
	}
}
