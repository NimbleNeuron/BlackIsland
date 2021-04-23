using System.Collections.Generic;
using Ara;
using BIFog;
using BIOutline;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class CharacterRenderer : MonoBehaviour
	{
		private readonly List<AraTrail> araTrails = new List<AraTrail>();
		private readonly List<AraTrail> effectAraTrails = new List<AraTrail>();
		private readonly List<Light> effectLightList = new List<Light>();
		private readonly List<Renderer> effectRenderList = new List<Renderer>();
		private readonly List<Light> lightList = new List<Light>();
		private readonly List<MaterialSwitcher> materialSwitchers = new List<MaterialSwitcher>();
		private readonly List<Renderer> renderList = new List<Renderer>();

		private readonly List<SelectionRenderer> selectionRenderers = new List<SelectionRenderer>();
		private readonly List<Light> tempEffectLightList = new List<Light>();
		private readonly List<Renderer> tempEffectRenderList = new List<Renderer>();
		private readonly List<AraTrail> tempEffectTailList = new List<AraTrail>();

		private bool isRefreshMaterial;
		private bool isRefreshRebuild;
		private bool isRefreshRender;
		private bool isRendering;

		private MaterialSwitchType materialSwitchTypeBitFlag;
		private bool nextEnableRenderer;

		private MaterialSwitchType nextMaterialSwitchTypeBitFlag;
		private bool rendererOff = true;
		private Transform target;

		public bool IsRendering => isRendering;

		private void LateUpdate()
		{
			if (target == null)
			{
				return;
			}

			if (rendererOff)
			{
				rendererOff = false;
				return;
			}

			if (isRefreshRebuild)
			{
				Rebuild();
			}

			if (isRefreshMaterial)
			{
				SetMaterialInternal();
			}

			if (isRefreshRender)
			{
				EnableRendererInternal();
			}
		}


		public void Clear()
		{
			target = null;
			renderList.Clear();
			lightList.Clear();
			selectionRenderers.Clear();
			araTrails.Clear();
			materialSwitchers.Clear();
			effectRenderList.Clear();
			effectLightList.Clear();
			effectAraTrails.Clear();
			isRendering = false;
			rendererOff = true;
			isRefreshRender = false;
			isRefreshRebuild = false;
			isRefreshMaterial = false;
			nextEnableRenderer = false;
		}


		public void Init(GameObject targetObject)
		{
			target = targetObject.transform;
			isRefreshRebuild = false;
			isRefreshRender = false;
			isRendering = false;
			nextEnableRenderer = false;
			isRefreshMaterial = false;
			materialSwitchTypeBitFlag = MaterialSwitchType.Original;
			nextMaterialSwitchTypeBitFlag = MaterialSwitchType.Original;
			rendererOff = true;
			Rebuild();
			SetMaterialInternal();
			EnableRendererInternal();
		}


		public void RebuildAndRedraw()
		{
			if (target == null)
			{
				return;
			}

			isRefreshRebuild = true;
			isRefreshMaterial = true;
			isRefreshRender = true;
		}


		public void SetMaterial(MaterialSwitchType switchType, bool isAdd)
		{
			isRefreshMaterial = true;
			if (switchType == MaterialSwitchType.Original)
			{
				nextMaterialSwitchTypeBitFlag = MaterialSwitchType.Original;
				return;
			}

			if (isAdd)
			{
				nextMaterialSwitchTypeBitFlag = nextMaterialSwitchTypeBitFlag.AddFlag(switchType);
				return;
			}

			nextMaterialSwitchTypeBitFlag = nextMaterialSwitchTypeBitFlag.RemoveFlag(switchType);
		}


		private void SetMaterialInternal()
		{
			isRefreshMaterial = false;
			materialSwitchTypeBitFlag = nextMaterialSwitchTypeBitFlag;
			if (materialSwitchTypeBitFlag == MaterialSwitchType.Original)
			{
				for (int i = materialSwitchers.Count - 1; i >= 0; i--)
				{
					if (materialSwitchers[i] != null)
					{
						materialSwitchers[i].Switch(MaterialSwitchType.Original);
					}
					else
					{
						materialSwitchers.RemoveAt(i);
					}
				}

				return;
			}

			MaterialSwitchType targetType;
			if (materialSwitchTypeBitFlag.HasFlag(MaterialSwitchType.Stealth))
			{
				targetType = MaterialSwitchType.Stealth;
			}
			else if (materialSwitchTypeBitFlag.HasFlag(MaterialSwitchType.InBush))
			{
				targetType = MaterialSwitchType.InBush;
			}
			else
			{
				targetType = MaterialSwitchType.Original;
			}

			for (int j = materialSwitchers.Count - 1; j >= 0; j--)
			{
				if (materialSwitchers[j] != null)
				{
					materialSwitchers[j].Switch(targetType);
				}
				else
				{
					materialSwitchers.RemoveAt(j);
				}
			}
		}


		public void EnableRenderer(bool enable)
		{
			if (target == null)
			{
				return;
			}

			isRefreshRender = true;
			nextEnableRenderer = enable;
		}


		private void EnableRendererInternal()
		{
			EnableRenderList(renderList, nextEnableRenderer);
			EnableRenderList<SelectionRenderer>(selectionRenderers, nextEnableRenderer);
			EnableRenderList<AraTrail>(araTrails, nextEnableRenderer);
			EnableRenderList<Light>(lightList, nextEnableRenderer);
			EnableRenderList(effectRenderList, nextEnableRenderer);
			EnableRenderList<Light>(effectLightList, nextEnableRenderer);
			EnableRenderList<AraTrail>(effectAraTrails, nextEnableRenderer);
			isRefreshRender = false;
			isRendering = nextEnableRenderer;
		}


		private void EnableRenderList(List<Renderer> list, bool enable)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] != null)
				{
					list[i].enabled = enable;
				}
				else
				{
					list.RemoveAt(i);
				}
			}
		}


		private void EnableRenderList<T>(List<T> list, bool enable) where T : Behaviour
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] != null)
				{
					list[i].enabled = enable;
				}
				else
				{
					list.RemoveAt(i);
				}
			}
		}


		private void Rebuild()
		{
			if (target != null)
			{
				target.GetComponentsInChildren<Renderer>(true, renderList);
				for (int i = renderList.Count - 1; i >= 0; i--)
				{
					if (renderList[i] is ParticleSystemRenderer &&
					    !(renderList[i].GetComponent<IgnoreCharacterRenderer>() == null) &&
					    !(renderList[i].GetComponentInParent<FogHiderOnCollider>() == null))
					{
						renderList.RemoveAt(i);
					}
				}

				target.GetComponentsInChildren<MaterialSwitcher>(true, materialSwitchers);
				RemoveFogHiderOnCollider<MaterialSwitcher>(materialSwitchers);
				GetComponentsInChildren<SelectionRenderer>(true, selectionRenderers);
				RemoveFogHiderOnCollider<SelectionRenderer>(selectionRenderers);
				target.GetComponentsInChildren<AraTrail>(true, araTrails);
				RemoveFogHiderOnCollider<AraTrail>(araTrails);
				target.GetComponentsInChildren<Light>(true, lightList);
				RemoveFogHiderOnCollider<Light>(lightList);
				isRefreshRebuild = false;
			}
		}


		private void RemoveFogHiderOnCollider<T>(List<T> list) where T : Behaviour
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (!(list[i].GetComponentInParent<FogHiderOnCollider>() == null))
				{
					list.RemoveAt(i);
				}
			}
		}


		private void RemoveFogHiderOnCollider(List<Renderer> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (!(list[i].GetComponentInParent<FogHiderOnCollider>() == null))
				{
					list.RemoveAt(i);
				}
			}
		}


		public void AddChildEffect(GameObject go)
		{
			if (go == null)
			{
				return;
			}

			go.GetComponentsInChildren<Renderer>(true, tempEffectRenderList);
			RemoveFogHiderOnCollider(tempEffectRenderList);
			if (!isRendering)
			{
				foreach (Renderer renderer in tempEffectRenderList)
				{
					renderer.enabled = false;
				}
			}

			effectRenderList.AddRange(tempEffectRenderList);
			go.GetComponentsInChildren<Light>(true, tempEffectLightList);
			RemoveFogHiderOnCollider<Light>(tempEffectLightList);
			if (!isRendering)
			{
				foreach (Light light in tempEffectLightList)
				{
					light.enabled = false;
				}
			}

			effectLightList.AddRange(tempEffectLightList);
			go.GetComponentsInChildren<AraTrail>(true, tempEffectTailList);
			RemoveFogHiderOnCollider<AraTrail>(tempEffectTailList);
			if (!isRendering)
			{
				foreach (AraTrail araTrail in tempEffectTailList)
				{
					araTrail.enabled = false;
				}
			}

			effectAraTrails.AddRange(tempEffectTailList);
		}


		public bool IsViewRendering()
		{
			if (renderList == null)
			{
				return false;
			}

			foreach (Renderer renderer in renderList)
			{
				if (!(renderer == null) && renderer.isVisible && renderer.enabled)
				{
					return true;
				}
			}

			return false;
		}
	}
}