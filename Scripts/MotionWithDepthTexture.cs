using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionWithDepthTexture : BasePostEffect
{
    public Shader MotionDepthTexureShader;
    
    [Range(0,1.0f)]
    public float MBlurSize=0.5f;

    //定义一个矩阵变量来保存上一帧的相机视角*投影矩阵
    private Matrix4x4 _preViousViewProijectMatrix;
    
    private Material _material;

    public Material MotionDepthMat
    {
        get
        {
            _material = checkShaderAndCreateMaterial(MotionDepthTexureShader, _material);
            return _material;
        }
    }


    private Camera _camera;

    public Camera MCamera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();

            return _camera;
        }
    }

    private void OnEnable()
    {
        MCamera.depthTextureMode |= DepthTextureMode.Depth;
        _preViousViewProijectMatrix = MCamera.projectionMatrix * MCamera.worldToCameraMatrix;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (MotionDepthMat)
        {
            MotionDepthMat.SetFloat("_BlurSize",MBlurSize);
            MotionDepthMat.SetMatrix("_PreViousViewProijectMatrix",_preViousViewProijectMatrix);
            Matrix4x4 curViewProjectMatrix = MCamera.projectionMatrix * MCamera.worldToCameraMatrix;
            //获取逆矩阵
            Matrix4x4 curViewProjectInverseMatrix = curViewProjectMatrix.inverse;
            MotionDepthMat.SetMatrix("_CurViewProjectInverseMatrix",curViewProjectInverseMatrix);
            _preViousViewProijectMatrix = curViewProjectMatrix;
            
            Graphics.Blit(source,destination,MotionDepthMat);
            
        }
        else
        {
            Graphics.Blit(source,destination);
        }
    }
}