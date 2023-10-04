using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur : BasePostEffect
{


    [Range(0,0.9f)]
    public float MBlurAmount = 0.5f;

    private RenderTexture _accumulationTexture;
    
    
    
    public Shader MMotionBlurShader;
    private Material _motionBlurMat;

    public Material MMotionBlurMat
    {
        get
        {
            _motionBlurMat = checkShaderAndCreateMaterial(MMotionBlurShader, _motionBlurMat);
            return _motionBlurMat;
        }
    }


    private void OnDisable()
    {
        DestroyImmediate(_accumulationTexture);
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (MMotionBlurMat)
        {
            if (_accumulationTexture==null||_accumulationTexture.width!=source.width||_accumulationTexture.height!=source.height)
            {
                DestroyImmediate(_accumulationTexture);
                _accumulationTexture = new RenderTexture(source.width,source.height,0);
                _accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source,_accumulationTexture);
            }
            // We are accumulating motion over frames without clear/discard
            // by design, so silence any performance warnings from Unity
            _accumulationTexture.MarkRestoreExpected();
            MMotionBlurMat.SetFloat("_BlurAmount",1.0f-MBlurAmount);
            Graphics.Blit(source,_accumulationTexture,MMotionBlurMat);
            Graphics.Blit(_accumulationTexture,destination);
        }
        else
        {
            Graphics.Blit(source,destination);
        }
    }
}
