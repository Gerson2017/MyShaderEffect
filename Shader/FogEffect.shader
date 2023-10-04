Shader "Gerson/FogEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogDensity("Fog Density",Float)=1.0
        _FogColor("Fog Color",Color)=(1,1,1,1)
        _FogStart("Fog Start",Float)=0.0
        _FogEnd("Fog End",Float)=1.0
    }
    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"
        float4x4 _FrustumCornersRay;
        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        sampler2D _CameraDepthTexture;
        half _FogDensity;
        fixed4 _FogColor;
        float _FogStart;
        float _FogEnd;


        struct v2f
        {
            half2 uv : TEXCOORD0;
            float4 pos : SV_POSITION;
            half2 uv_depth:TEXCOORD1;
            float4 interpolatedRay : TEXCOORD2;
        };


        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
            o.uv_depth = v.texcoord;
            o.uv_depth = v.texcoord;

            #if  UNITY_UV_STARTS_AT_TOP //通常用于判断D3D平台，在开启抗锯齿的时候，图片采样可能会用到
            if (_MainTex_TexelSize.y < 0) //部分平台的纹理坐标原点不一致
                o.uv_depth.y = 1 - o.uv_depth.y;
            #endif

            int index = 0;
            if (v.texcoord.x < 0.5 && v.texcoord.y < 0.5)
            {
                index = 0;
            }
            else if (v.texcoord.x > 0.5 && v.texcoord.y < 0.5)
            {
                index = 1;
            }
            else if (v.texcoord.x > 0.5 && v.texcoord.y > 0.5)
            {
                index = 2;
            }
            else
            {
                index = 3;
            }

            #if UNITY_UV_STARTS_AT_TOP
            if (_MainTex_TexelSize.y < 0)
                index = 3 - index;
            #endif

            o.interpolatedRay = _FrustumCornersRay[index];
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            //SAMPLE_DEPTH_TEXTURE 对深度纹理进行采样  LinearEyeDepth 得到视角空间下的线性深度值
            float linearDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth));
            //得到当前像素在世界空间的坐标位置
            float3 worldPos = _WorldSpaceCameraPos + linearDepth * i.interpolatedRay.xyz;

            float fogDensity = (_FogEnd - worldPos.y) / (_FogEnd - _FogStart);
            fogDensity = saturate(fogDensity * _FogDensity);

            fixed4 finalColor = tex2D(_MainTex, i.uv);
            finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, fogDensity);

            return finalColor;
        }
        ENDCG

        Pass
        {
            
            ZTest Always Cull Off ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    
    Fallback Off
}