Shader "Gerson/EdgeDetection"
{
    //大家注意 Properties 里面写的变量主要是为了能够在面板显示
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _EdgesOnly("Edge Only",Float)=1.0
        _EdgesMutiple("Edge Only",Range(0,1))=1.0
        _EdgeColor("Edge Color",Color )=(0,0,0,1)
        _BackGroundColor("BackGround Color",Color)=(1,1,1,1)
    }
    SubShader
    {
        //不进行深度测试 不进行深度写入  相当于当前Pass只负责输出 不对现有效果产生任何影响
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragSobel

            #include "UnityCG.cginc"


            struct v2f
            {
                half2 uv[9] : TEXCOORD0; //定义了维度为9的纹理数组，使用sobel算子采样需要9个相邻的纹理域坐标
                float4 pos : SV_POSITION;
            };

            //写在CGPROGRAM 内的参数才是真正会对渲染产生影响的参数
            sampler2D _MainTex;
            uniform half4 _MainTex_TexelSize; //xxx_TexelSize是获取xxx纹理的每个文素大小
            fixed _EdgesOnly;
            fixed4 _EdgeColor;
            fixed4 _BackGroundColor;
            fixed _EdgesMutiple;

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                //这里是使用sobel算子采样的固定写法
                half2 uv = v.texcoord;
                o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
                o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
                o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
                o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
                o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);
                o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
                o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
                o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
                o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);

                return o;
            }


            fixed luminance(fixed4 color)
            {
                return 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
            }

            half Sobel(v2f i)
            {
                const half Gx[9] = {
                    -1, 0, 1,
                    -2, 0, 2,
                    -1, 0, 1
                };
                const half Gy[9] = {
                    -1, -2, -1,
                    0, 0, 0,
                    1, 2, 1
                };

                half texColor;
                half edgeX = 0;
                half edgeY = 0;
                for (int it = 0; it < 9; it++)
                {
                    texColor = luminance(tex2D(_MainTex, i.uv[it]));
                    edgeX += texColor * Gx[it];
                    edgeY += texColor * Gy[it];
                }

                half edge = 1 - abs(edgeX) - abs(edgeY);

                return edge;
            }


            //我去 Sobel方法需要定义在fragment之前
            fixed4 fragSobel(v2f i) : SV_Target
            {
                half edge = Sobel(i);
                fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[4]), edge*_EdgesMutiple);
                fixed4 OnlyEdgeColor = lerp(_EdgeColor, _BackGroundColor, edge*_EdgesMutiple);
                return lerp(withEdgeColor, OnlyEdgeColor, _EdgesOnly);
            }
            ENDCG
        }
    }FallBack Off
}