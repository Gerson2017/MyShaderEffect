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


    // private void OnRenderImage(RenderTexture source, RenderTexture destination)
    // {
    //     if (GuassianMat)
    //     {
    //         int width = source.width;
    //         int height = source.height;
    //         
    //         GuassianMat.SetFloat("_BlurSize",BlurSize);
    //         //获取临时图像用来做模糊
    //         RenderTexture buffer = RenderTexture.GetTemporary(width, height, 0); //这里使用的是和屏幕一样大小的缓存，性能消耗较大
    //         
    //         //渲染垂直pass
    //         Graphics.Blit(source,buffer,GuassianMat,0);//buffer被用来存储第一个pass处理过后的图像
    //         //渲染水平pass
    //         Graphics.Blit(buffer,destination,GuassianMat,1);
    //         
    //         RenderTexture.ReleaseTemporary(buffer);
    //         
    //     }
    //
    //         Graphics.Blit(source,destination,GuassianMat);
    //     
    // }
    //
    
    /// 2nd edition: scale the render texture
//	void OnRenderImage (RenderTexture src, RenderTexture dest) {
//		if (material != null) {
//			int rtW = src.width/downSample;
//			int rtH = src.height/downSample;
//			RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
//			buffer.filterMode = FilterMode.Bilinear;
//
//			// Render the vertical pass
//			Graphics.Blit(src, buffer, material, 0);
//			// Render the horizontal pass
//			Graphics.Blit(buffer, dest, material, 1);
//
//			RenderTexture.ReleaseTemporary(buffer);
//		} else {
//			Graphics.Blit(src, dest);
//		}
//	}

    /// 3rd edition: use iterations for larger blur
    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (GuassianMat != null) {
            int rtW = src.width/MDownSample;
            int rtH = src.height/MDownSample;

            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer0);

            for (int i = 0; i < MIterations; i++) {
                GuassianMat.SetFloat("_BlurSize", 1.0f + i * MBlurSpread);

                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the vertical pass
                Graphics.Blit(buffer0, buffer1, GuassianMat, 0);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the horizontal pass
                Graphics.Blit(buffer0, buffer1, GuassianMat, 1);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            Graphics.Blit(buffer0, dest);
            RenderTexture.ReleaseTemporary(buffer0);
        } else {
            Graphics.Blit(src, dest);
        }
    }
    
}
