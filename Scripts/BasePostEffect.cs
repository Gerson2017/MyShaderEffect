using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BasePostEffect : MonoBehaviour
{
    private void Start()
    {
        CheckResource();
    }


    protected void CheckResource()
    {
        if (!CheckSupport())
        {
            NotSupported();
        }
    }

    protected bool CheckSupport()
    {
        if (!SystemInfo.supportsImageEffects|| !SystemInfo.supportsRenderTextures  )
        {
            Debug.LogWarning("this platform does not support image effects or rendertextures");
            return false;

        }

        return true;
    }


    protected void NotSupported()
    {
        enabled = false;
    }


    protected Material checkShaderAndCreateMaterial(Shader shader,Material material)
    {
        if (!shader)
            return null;
        if (shader.isSupported&& material&&material.shader==shader)
            return material;
        if (!shader.isSupported)
            return null;
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        if (material)
            return material;

        return null;

    }
    
}
