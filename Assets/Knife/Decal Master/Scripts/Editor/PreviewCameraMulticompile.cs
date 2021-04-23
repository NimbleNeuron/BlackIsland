using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Knife.Tools
{
    [InitializeOnLoad]
    public class PreviewCameraMulticompile
    {
        static CommandBuffer bufferEnable;
        static CommandBuffer bufferDisable;
        static string bufferNameEnable = "preview multicompile setup buffer enable";
        static string bufferNameDisable = "preview multicompile setup buffer disable";


        static PreviewCameraMulticompile()
        {
            Camera.onPreRender -= OnCameraPreRender;
            Camera.onPreRender += OnCameraPreRender;

            bufferEnable = new CommandBuffer();
            bufferEnable.name = bufferNameEnable;
            bufferEnable.EnableShaderKeyword("PREVIEWCAMERA");

            bufferDisable = new CommandBuffer();
            bufferDisable.name = bufferNameDisable;
            bufferDisable.DisableShaderKeyword("PREVIEWCAMERA");
        }

        private static void OnCameraPreRender(Camera cam)
        {
            if (cam.cameraType == CameraType.Preview)
            {
                cam.RemoveAllCommandBuffers();
                cam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, bufferEnable);
                cam.AddCommandBuffer(CameraEvent.AfterEverything, bufferDisable);
            }
        }
        
    }
}