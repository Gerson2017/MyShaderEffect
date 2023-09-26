using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection : BasePostEffect
{
   
    public Shader MEdgeDetectShader;
    private Material _edgeDetectMat;

    [Range(0.0f,1f)]
    public float MEdgesOnly = 0.0f;
    [Range(0.0f,1f)]
    public float MEdgesMutiple = 1;
    public Color MEdgeColor = Color.black;
    
    public Color MBackGroundColor= Color.white;

    public Material EdgeDetectMat
    {
        get
        {
            _edgeDetectMat = checkShaderAndCreateMaterial(MEdgeDetectShader, _edgeDetectMat);
            return _edgeDetectMat;
        }
    }
    
    
    //这里对相机的采样进效果后处理 重写OnRenderImage方法

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //这里需要调用一下 Graphics.ilt方法 才会对屏幕图像进行处理 否则会把圆通直接显示到屏幕上
        if (EdgeDetectMat)
        {
            EdgeDetectMat.SetFloat("_EdgesOnly",MEdgesOnly);
            EdgeDetectMat.SetFloat("_EdgesMutiple",MEdgesMutiple);
            EdgeDetectMat.SetColor("_EdgeColor",MEdgeColor);
            EdgeDetectMat.SetColor("_BackGroundColor",MBackGroundColor);
            Graphics.Blit(source,destination,EdgeDetectMat);
        }
        else
        {
            Graphics.Blit(source,destination);
        }
        
        
    }
}
