Shader "Gerson/BrightnessSaturatiionContrast"
{
    //大家注意 Properties 里面写的变量主要是为了能够在面板显示
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Brightness ("Brightness", Float) = 1
        _Saturation ("Saturation ", Float) = 1
        _Contrast ("Contrast", Float) = 1
    }
    SubShader
    {
        Pass
        {

            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            //大家注意 这里重新写一遍是在shader里面声明变量 这里的变量名要和properties对应上
            half _Brightness;
            half _Saturation;
            half _Contrast;

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); //
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 renderTex = tex2D(_MainTex, i.uv); //获取对原屏幕的图像采样

                //1. 调整亮度
                fixed3 finalColor = renderTex.rgb * _Brightness;
                //2.调整饱和度
                fixed luminance = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
                fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
                finalColor = lerp(luminanceColor, finalColor, _Saturation); //使用插值将颜色在当前颜色和上一步颜色中进行调整

                //3.调整对比度
                fixed3 aveColor = fixed3(0.5, 0.5, 0.5);
                finalColor = lerp(aveColor, finalColor, _Contrast); //

                return fixed4(finalColor, renderTex.a);
            }
            ENDCG
        }
    } FallBack Off
}