using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class BaseUI : UIBehaviour
	{
		public static BaseUI DraggingUI;


		private BaseUI cacheParentUI;


		private RectTransform cacheRectTransform;


		private bool isOnAwakeUICalled;


		private bool isOnStartUICalled;


		public RectTransform rectTransform {
			get
			{
				if (cacheRectTransform == null)
				{
					cacheRectTransform = GetComponent<RectTransform>();
				}

				return cacheRectTransform;
			}
		}


		protected override void Awake()
		{
			base.Awake();
			if (UISceneContext.currentSceneState >= UISceneContext.SceneState.UIAwaked)
			{
				AwakeUI();
			}
		}


		protected override void Start()
		{
			base.Start();
			if (UISceneContext.currentSceneState >= UISceneContext.SceneState.UIStarted)
			{
				StartUI();
			}
		}


		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			cacheParentUI = null;
		}


		public BaseUI GetParentUI()
		{
			if (cacheParentUI == null && transform.parent != null)
			{
				cacheParentUI = transform.parent.GetComponentInParent<BaseUI>();
			}

			return cacheParentUI;
		}


		public void AwakeUI()
		{
			if (!isOnAwakeUICalled)
			{
				isOnAwakeUICalled = true;
				OnAwakeUI();
			}
		}


		public void StartUI()
		{
			if (!isOnStartUICalled)
			{
				isOnStartUICalled = true;
				OnStartUI();
			}
		}


		protected virtual void OnAwakeUI() { }


		protected virtual void OnStartUI() { }


		protected bool IsPointerOverUIElement(string moduleName)
		{
			return !(EventSystem.current == null) && EventSystem.current.IsPointerOverGameObject() &&
			       IsPointerOverUIElement(GetEventSystemRaycastResults(), moduleName);
		}


		private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, string moduleName)
		{
			for (int i = 0; i < eventSystemRaysastResults.Count; i++)
			{
				if (eventSystemRaysastResults[i].module.name.Equals(moduleName))
				{
					return true;
				}
			}

			return false;
		}


		private List<RaycastResult> GetEventSystemRaycastResults()
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			return list;
		}
	}
}