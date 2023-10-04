Shader "Gerson/MotionBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurAmount("Blur Amount",Float)=1.0
    }
    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        fixed _BlurAmount;

        struct v2f
        {
            half2 uv : TEXCOORD0;
            float4 pos : SV_POSITION;
        };


        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;

            return o;
        }

        fixed4 fragRGB(v2f i) : SV_Target
        {
            return fixed4(tex2D(_MainTex, i.uv).rgb, _BlurAmount);
        }

	half4 fragA (v2f i) : SV_Target {
			return tex2D(_MainTex, i.uv);
		}
        ENDCG


        ZTest Always Cull Off ZWrite Off
        
 

        Pass
        {
            //使用贴图的透明值 缓存中的透明值使用1减去题图的透明值
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragRGB
            ENDCG
        }

        Pass
        {
            //使用贴图的透明值 缓存中的透明值使用0
            Blend One Zero
            ColorMask A

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragA
            ENDCG
        }
    }
    Fallback Off
}