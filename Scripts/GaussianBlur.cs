using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianBlur : BasePostEffect
{

    [Range(0,4)]
    public int MIterations=3;

    [Range(0.2f,3.0f)]
    public float MBlurSpread = 0.6f;

    [Range(1,8)]
    public int MDownSample = 2;
    [Range(1f,8f)]
    public int BlurSize = 1;

    
    public Shader MGaussianBlurShader;
    private Material _gaussianMat;

    public Material GuassianMat
    {
        get
        {
            _gaussianMat = checkShaderAndCreateMaterial(MGaussianBlurShader, _gaussianMat);
            return _gaussianMat;
        }
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GuassianMat)
        {
            int width = source.width;
            int height = source.height;
            
            GuassianMat.SetFloat("_BlurSize",BlurSize);
            //获取临时图像用来做模糊
            RenderTexture buffer = RenderTexture.GetTemporary(width, height, 0); //这里使用的是和屏幕一样大小的缓存，性能消耗较大
            
            //渲染垂直pass
            Graphics.Blit(source,buffer,GuassianMat,0);//buffer被用来存储第一个pass处理过后的图像
            //渲染水平pass
            Graphics.Blit(buffer,destination,GuassianMat,1);
            
            RenderTexture.ReleaseTemporary(buffer);
            
        }

            Graphics.Blit(source,destination,GuassianMat);
        
    }
}
