Shader "Gerson/MotionBlurWithDepthTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize("Blur Size",Float)=1.0
    }
    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        sampler2D _CameraDepthTexture; //相机深度贴图
        float4x4 _PreViousViewProijectMatrix;
        float4x4 _CurViewProjectInverseMatrix;
        half _BlurSize;

        struct v2f
        {
            half2 uv : TEXCOORD0;
            half2 uv_depth:TEXCOORD1;
            float4 pos : SV_POSITION;
        };


        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
            o.uv_depth = v.texcoord;

            #if UNITY_UV_STARTS_AT_TOP//兼容D3D的像素坐标 左上角为0
            if (_MainTex_TexelSize.y < 0)
                o.uv_depth.y = 1 - o.uv_depth.y;

            #endif

            return o;
        }


        fixed4 frag(v2f i) : SV_Target
        {
            float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth); //对深度贴图进行采样
            float4 H = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, d * 2 - 1, 1); //固定写法 构建像素NDC坐标
            // Transform by the view-projection inverse.
            float4 D = mul(_CurViewProjectInverseMatrix, H);
            // Divide by w to get the world position. 
            float4 worldPos = D / D.w;


            float4 currentPos=H;

            float4 preViousPos=mul(_PreViousViewProijectMatrix,worldPos);
            preViousPos/=preViousPos.w;

            float2 velocity=(currentPos.xy-preViousPos.xy)/2.0f;

            float2 uv=i.uv;
            float4 c=tex2D(_MainTex,uv);
            uv+=velocity*_BlurSize;

            for (int it=1;it<3;it++,uv+=velocity*_BlurSize)
            {
               	float4 currentColor = tex2D(_MainTex, uv);
				c += currentColor; 
            }
            c/=3;
            
            return fixed4(c.rgb,1.0);
        }
        ENDCG

        Pass
        {
            
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            ENDCG
        }
    }
    Fallback Off
}