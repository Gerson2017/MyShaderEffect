using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RobertsEdgeEffect : BasePostEffect
{
  public Shader MEdgeShader;

  [Range(0,1.0f)]
  public float MEdgeOnly = 0f;

  public Color MEdgeColor = Color.black;
  public Color BackGroundColor = Color.white;
  public float MSampleDistance = 1.0f;//越大 描边越宽
  public float MSensitivityDepth=1.0f;//影响相邻深度值和法线值相差多少
  public float SensititityNormals = 1.0f;
  
  public Material _edgeMat;

  public Material MEdgeMaterial
  {
    get
    {
      _edgeMat = checkShaderAndCreateMaterial(MEdgeShader, _edgeMat);
      return _edgeMat;
    }
  }


  private void OnEnable()
  {
    this.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
  }


  [ImageEffectOpaque]//在不透明的物体渲染完成后进行处理
  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (MEdgeMaterial)
    {
      MEdgeMaterial.SetFloat("_EdgeOnly",MEdgeOnly);
      MEdgeMaterial.SetColor("_EdgeColor",MEdgeColor);
      MEdgeMaterial.SetColor("_BackGroundColor",BackGroundColor);
      MEdgeMaterial.SetFloat("_SampleDistance",MSampleDistance);
      MEdgeMaterial.SetVector("_Sensitivity",new Vector4(SensititityNormals,MSensitivityDepth,0,0));
      
      Graphics.Blit(source,destination,MEdgeMaterial);
    }
    else
    {
      Graphics.Blit(source,destination);
    }
  }
}
