using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class MinimapUI : BaseControl
	{
		[SerializeField] private RawImage mapShade = default;


		private Material shadeMat;


		private UIMap uiMap;


		public UIMap UIMap => uiMap;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			DestroyImmediate(shadeMat);
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			uiMap = GameUtil.Bind<UIMap>(gameObject, "Mask/Map");
			shadeMat = new Material(Shader.Find("Hidden/BIFogShade"));
			Shader.SetGlobalMatrix("mapToWorldMatrix", GameUIUtility.mapToWorldMatrix);
			Rect worldMapRect = GameUIUtility.worldMapRect;
			shadeMat.SetVector("worldMapRect",
				new Vector4(worldMapRect.x, worldMapRect.y, worldMapRect.width, worldMapRect.height));
			mapShade.material = shadeMat;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			uiMap.Init(MonoBehaviourInstance<ClientService>.inst.CurrentLevel);
			uiMap.InitBoundary(new Vector2(-145f, 141f), new Vector2(145f, -141f));
			if (Singleton<LocalSetting>.inst.setting.miniMapZoomOut)
			{
				ZoomOut();
			}
			else
			{
				ZoomIn();
			}

			MonoBehaviourInstance<MobaCamera>.inst.OnCameraModeChange += uiMap.UpdateCameraFrame;
		}


		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			uiMap.OnClickDownMiniMap(eventData);
		}


		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			uiMap.OnClickUp(eventData);
		}


		public void ZoomIn()
		{
			uiMap.ZoomIn();
		}


		public void ZoomOut()
		{
			uiMap.ZoomOut();
		}
	}
}