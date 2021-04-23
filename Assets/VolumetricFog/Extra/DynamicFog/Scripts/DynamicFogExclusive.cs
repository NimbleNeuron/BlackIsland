using UnityEngine;
using UnityEngine.XR;
#if UNITY_EDITOR

#endif

namespace DynamicFogAndMist
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [HelpURL("http://kronnect.com/taptapgo")]
    [ImageEffectAllowedInSceneView]
    public class DynamicFogExclusive : DynamicFogBase
    {
        RenderTexture rt;

        [Range(0.1f, 2f)]
        public float renderScale = 1f;

        private void OnPreRender()
        {
            if (fogMat == null || _alpha == 0 || currentCamera == null) return;

            if (XRSettings.enabled)
            {
                RenderTextureDescriptor rtDesc = XRSettings.eyeTextureDesc;
                rtDesc.width = (int)(rtDesc.width * renderScale);
                rtDesc.height = (int)(rtDesc.height * renderScale);
                rt = RenderTexture.GetTemporary(rtDesc);
            }
            else
            {
                int w = (int)(currentCamera.pixelWidth * renderScale);
                int h = (int)(currentCamera.pixelHeight * renderScale);
                rt = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32);
                rt.antiAliasing = 1;
            }
            rt.wrapMode = TextureWrapMode.Clamp;
            currentCamera.targetTexture = rt;
        }

        private void OnPostRender()
        {
            if (fogMat == null || _alpha == 0 || currentCamera == null)
                return;

            if (shouldUpdateMaterialProperties)
            {
                shouldUpdateMaterialProperties = false;
                UpdateMaterialPropertiesNow();
            }

            if (currentCamera.orthographic)
            {
                if (!matOrtho)
                    ResetMaterial();
                fogMat.SetVector("_ClipDir", currentCamera.transform.forward);
            }
            else
            {
                if (matOrtho)
                    ResetMaterial();
            }

            if (_useSinglePassStereoRenderingMatrix && UnityEngine.XR.XRSettings.enabled)
            {
                fogMat.SetMatrix("_ClipToWorld", currentCamera.cameraToWorldMatrix);
            }
            else
            {
                fogMat.SetMatrix("_ClipToWorld", currentCamera.cameraToWorldMatrix * currentCamera.projectionMatrix.inverse);
            }
            currentCamera.targetTexture = null;
            Graphics.Blit(rt, null as RenderTexture, fogMat);
            RenderTexture.ReleaseTemporary(rt);

        }

    }

}