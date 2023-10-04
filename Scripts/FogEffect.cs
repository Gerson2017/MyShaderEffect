using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogEffect : BasePostEffect
{
   public Shader MFogShader;

   [Range(0.0f,3.0f)]
   public float MFogDensity = 1.0f;
   public Color MFogColor;

   public float MFogStart;
   public float MFogEnd=2.0f;
   

   private Material _fogMat;
   
   public Material MFogMat
   {
      get
      {
         _fogMat = checkShaderAndCreateMaterial(MFogShader, _fogMat);
         return _fogMat;
      }
   }



   private Camera _camera;

   public Camera MCamera
   {
      get
      {
         if (_camera == null)
         {
            _camera = this.GetComponent<Camera>();
         }

         return _camera;
      }
   }


   private Transform _cameraTs;

   public Transform MCameraTransform
   {
      get
      {
         if (!_cameraTs)
         {
            _cameraTs = MCamera.transform;
         }

         return _cameraTs;
      }
   }


   private void OnEnable()
   {
      MCamera.depthTextureMode |= DepthTextureMode.Depth;
   }


   private void OnRenderImage(RenderTexture source, RenderTexture destination)
   {
      if (MFogMat)
      {
         Matrix4x4 frustCorners= Matrix4x4.identity;//首先计算近剪裁面的四个角对应的向量，并使用矩阵存储
         float fov = MCamera.fieldOfView;
         float near=MCamera.nearClipPlane;
         float aspect = MCamera.aspect;


         float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);//中心点高度
         Vector3 toRight = MCameraTransform.right * halfHeight * aspect;
         Vector3 toTop = MCameraTransform.up * halfHeight;
         Vector3 toLeft = MCameraTransform.forward * near +toTop- toRight;


         float scale = toLeft.magnitude / near;
         
         toLeft.Normalize();
         toLeft *= scale;
         
         
         Vector3 topRight = MCameraTransform.forward * near + toRight + toTop;
         topRight.Normalize();
         topRight *= scale;

         Vector3 bottomLeft = MCameraTransform.forward * near - toTop - toRight;
         bottomLeft.Normalize();
         bottomLeft *= scale;

         Vector3 bottomRight = MCameraTransform.forward * near + toRight - toTop;
         bottomRight.Normalize();
         bottomRight *= scale;

         //使用矩阵存储
         frustCorners.SetRow(0,bottomLeft);
         frustCorners.SetRow(1,bottomRight);
         frustCorners.SetRow(2,topRight);
         frustCorners.SetRow(3,toLeft);
         
         MFogMat.SetMatrix("_FrustumCornersRay", frustCorners);
         MFogMat.SetFloat("_FogDensity", MFogDensity);
         MFogMat.SetColor("_FogColor", MFogColor);
         MFogMat.SetFloat("_FogStart", MFogStart);
         MFogMat.SetFloat("_FogEnd", MFogEnd);

         Graphics.Blit (source, destination, MFogMat);
         
      }
      else
      {

         Graphics.Blit (source, destination);
      }
   }
}
