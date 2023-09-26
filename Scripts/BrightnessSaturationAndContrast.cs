using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSaturationAndContrast : BasePostEffect
{
    [Range(0, 3.0f)] public float MBrightness;
    [Range(0.0f, 3.0f)] public float MSaturation;
    [Range(0.0f, 3.0f)] public float MContrast;


    public Shader MBrigSatConShader;
    private Material _briSatConMat;

    public Material BriSatConMat
    {
        get
        {
            _briSatConMat = checkShaderAndCreateMaterial(MBrigSatConShader, _briSatConMat);
            return _briSatConMat;
        }
    }
    
    
    
    //这里对相机的采样进效果后处理 重写OnRenderImage方法

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //这里需要调用一下 Graphics.ilt方法 才会对屏幕图像进行处理 否则会把圆通直接显示到屏幕上
        if (BriSatConMat)
        {
            BriSatConMat.SetFloat("_Brightness",MBrightness);
            BriSatConMat.SetFloat("_Saturation",MSaturation);
            BriSatConMat.SetFloat("_Contrast",MContrast);
            Graphics.Blit(source,destination,BriSatConMat);
        }
        else
        {
            
            Graphics.Blit(source,destination);
        }
       
    }
}