using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomEffect : BasePostEffect
{
    
    // Blur iterations - larger number means more blur.
    [Range(0, 4)]
    public int Miterations = 3;
	
    // Blur spread for each iteration - larger value means more blur
    [Range(0.2f, 3.0f)]
    public float MblurSpread = 0.6f;

    [Range(1, 8)]
    public int MdownSample = 2;

    [Range(0.0f, 4.0f)]
    public float MluminanceThreshold = 0.6f;
    
    public Shader MbloomShader;
    private Material _bloomMaterial = null;
    public Material BloomMaterial {  
        get {
            _bloomMaterial = checkShaderAndCreateMaterial(MbloomShader, _bloomMaterial);
            return _bloomMaterial;
        }  
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (BloomMaterial)
        {
            BloomMaterial.SetFloat("_LuminanceThreshold",MluminanceThreshold);

            int downWidth = source.width / MdownSample;
            int downHeight = source.height / MdownSample;

            RenderTexture downTextureBuffer = RenderTexture.GetTemporary(downWidth, downHeight, 0);//获取降低像素比后的采样
            //使用pass0处理图像
            Graphics.Blit(source,downTextureBuffer,BloomMaterial,0);
            downTextureBuffer.filterMode = FilterMode.Bilinear;

            for (int i = 0; i < Miterations; i++)//开始效果迭代
            {
                BloomMaterial.SetFloat("_BlurSize",MblurSpread);
                RenderTexture downTextureBuffer1 = RenderTexture.GetTemporary(downWidth, downHeight, 0);//获取降低像素比后的采样
                
                //使用pass1处理图像
                Graphics.Blit(downTextureBuffer,downTextureBuffer1,BloomMaterial,1);
                //处理完成后 释放downTextureBuffer
                RenderTexture.ReleaseTemporary(downTextureBuffer);
                downTextureBuffer = downTextureBuffer1;//保存处理后的图像信息 准备下次迭代使用
                
                downTextureBuffer1 = RenderTexture.GetTemporary(downWidth, downHeight, 0);//获取降低像素比后的采样
                //使用pass2处理图像
                Graphics.Blit(downTextureBuffer,downTextureBuffer1,BloomMaterial,2);
                RenderTexture.ReleaseTemporary(downTextureBuffer);
                downTextureBuffer = downTextureBuffer1;

            }
            BloomMaterial.SetTexture("_Bloom",downTextureBuffer);//将模糊后的图像传给shader
            
            Graphics.Blit(source,destination,BloomMaterial,3);
            RenderTexture.ReleaseTemporary(downTextureBuffer);
        }
        else
        {
            Graphics.Blit(source,destination);  
        }
    }
}
