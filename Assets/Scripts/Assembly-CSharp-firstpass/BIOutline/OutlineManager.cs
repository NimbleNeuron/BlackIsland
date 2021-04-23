using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BIOutline
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class OutlineManager : MonoBehaviour
	{
		private readonly List<OutlineMaterial> materialCache = new List<OutlineMaterial>();


		private readonly List<Outliner> outLinerList = new List<Outliner>();

		private CommandBuffer commandBuffer;


		private BoundingSphere[] cullBuffer;


		private CullingGroup cullingGroup;


		private Camera outlineCamera;


		private Material outlineEffectMaterial;


		private RenderTexture outlineMetaTexture;


		private Vector2 resolution;


		private RenderTexture temp;


		private void Awake()
		{
			commandBuffer = new CommandBuffer();
			cullingGroup = new CullingGroup();
			cullBuffer = new BoundingSphere[1000];
			Init();
			resolution = new Vector2(Screen.width, Screen.height);
		}


		private void Update()
		{
			if (!resolution.x.Equals(Screen.width) || !resolution.y.Equals(Screen.height))
			{
				resolution.x = Screen.width;
				resolution.y = Screen.height;
				OnResolution();
			}
		}


		private void OnDestroy()
		{
			CullingGroup cullingGroup = this.cullingGroup;
			if (cullingGroup != null)
			{
				cullingGroup.Dispose();
			}

			this.cullingGroup = null;
			DestroyImmediate(outlineEffectMaterial);
			foreach (OutlineMaterial outlineMaterial in materialCache)
			{
				DestroyImmediate(outlineMaterial.mat);
			}

			materialCache.Clear();
		}


		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			UpdateCullingBuffer();
			if (outlineCamera.pixelHeight != Camera.main.pixelHeight ||
			    outlineCamera.pixelWidth != Camera.main.pixelWidth ||
			    Math.Abs(outlineCamera.fieldOfView - Camera.main.fieldOfView) > Mathf.Epsilon)
			{
				UpdateOutlineCamera(outlineCamera);
			}

			if (temp == null)
			{
				temp = RenderTexture.GetTemporary(outlineCamera.pixelWidth, outlineCamera.pixelHeight, 0,
					RenderTextureFormat.ARGB32);
			}

			if (outlineMetaTexture == null)
			{
				outlineMetaTexture = RenderTexture.GetTemporary(outlineCamera.pixelWidth, outlineCamera.pixelHeight, 0,
					RenderTextureFormat.ARGB32);
			}

			Graphics.SetRenderTarget(outlineMetaTexture);
			GL.Clear(true, true, Color.clear);
			GL.LoadProjectionMatrix(outlineCamera.projectionMatrix);
			commandBuffer.Clear();
			RenderMeta(OutlineType.HoverLine);
			Graphics.ExecuteCommandBuffer(commandBuffer);
			outlineEffectMaterial.SetTexture("_MetaTexture", outlineMetaTexture);
			outlineEffectMaterial.SetFloat("_LineThickness", 0.001f);
			Graphics.Blit(src, temp, outlineEffectMaterial);
			Graphics.SetRenderTarget(outlineMetaTexture);
			GL.Clear(true, true, Color.clear);
			GL.LoadProjectionMatrix(outlineCamera.projectionMatrix);
			commandBuffer.Clear();
			RenderMeta(OutlineType.InSight);
			RenderMeta(OutlineType.SelectLine);
			Graphics.ExecuteCommandBuffer(commandBuffer);
			outlineEffectMaterial.SetTexture("_MetaTexture", outlineMetaTexture);
			outlineEffectMaterial.SetFloat("_LineThickness", 0.0004f);
			Graphics.Blit(temp, dest, outlineEffectMaterial);
			temp.Release();
			outlineMetaTexture.Release();
		}


		private void Init()
		{
			outlineCamera = CreateOutlineCamera();
			outlineEffectMaterial = new Material(Shader.Find("Hidden/OutlineEffectShader"));
			cullingGroup.targetCamera = Camera.main;
			cullingGroup.SetBoundingSpheres(cullBuffer);
		}


		private Camera CreateOutlineCamera()
		{
			Camera component = GetComponent<Camera>();
			if (component != Camera.main)
			{
				enabled = false;
				throw new Exception("OutlineManager Can Attach in MainCamera");
			}

			Camera camera = new GameObject
			{
				transform =
				{
					parent = component.transform,
					position = Vector3.zero,
					localRotation = Quaternion.identity,
					localScale = Vector3.one
				}
			}.AddComponent<Camera>();
			UpdateOutlineCamera(camera);
			return camera;
		}


		private void UpdateOutlineCamera(Camera cam)
		{
			cam.CopyFrom(Camera.main);
			cam.backgroundColor = Color.clear;
			cam.useOcclusionCulling = false;
			cam.gameObject.name = "Outline Cam";
			cam.renderingPath = RenderingPath.Forward;
			cam.cullingMask = 0;
			cam.enabled = false;
			cam.allowMSAA = false;
		}


		private void OnResolution()
		{
			if (outlineCamera != null)
			{
				Destroy(outlineCamera.gameObject);
			}

			outlineCamera = CreateOutlineCamera();
			RenderTexture.ReleaseTemporary(temp);
			RenderTexture.ReleaseTemporary(outlineMetaTexture);
		}


		public void RegisterOutliner(Outliner outliner)
		{
			if (!outLinerList.Contains(outliner))
			{
				outLinerList.Add(outliner);
			}
		}


		public void UnregisterOutliner(Outliner outliner)
		{
			if (outLinerList.Contains(outliner))
			{
				outLinerList.Remove(outliner);
			}
		}


		private void UpdateCullingBuffer()
		{
			for (int i = 0; i < outLinerList.Count; i++)
			{
				cullBuffer[i].radius = 1f;
				cullBuffer[i].position = outLinerList[i].Transform.position;
			}

			cullingGroup.SetBoundingSphereCount(outLinerList.Count);
		}


		private void RenderMeta(OutlineType outlineType)
		{
			for (int i = 0; i < outLinerList.Count; i++)
			{
				if (outLinerList[i].outlineType == outlineType && cullingGroup.IsVisible(i))
				{
					List<OutlineDrawCall> drawCalls = outLinerList[i].drawCalls;
					Material cachedMaterial = GetCachedMaterial(outLinerList[i].color);
					for (int j = 0; j < drawCalls.Count; j++)
					{
						for (int k = 0; k < drawCalls[j].meshCount; k++)
						{
							if (drawCalls[j].render != null)
							{
								commandBuffer.DrawRenderer(drawCalls[j].render, cachedMaterial, k, 0);
							}
						}
					}
				}
			}
		}


		private Material GetCachedMaterial(Color color)
		{
			for (int i = 0; i < materialCache.Count; i++)
			{
				if (materialCache[i].color == color)
				{
					return materialCache[i].mat;
				}
			}

			OutlineMaterial outlineMaterial = new OutlineMaterial(color);
			materialCache.Add(outlineMaterial);
			return outlineMaterial.mat;
		}
	}
}