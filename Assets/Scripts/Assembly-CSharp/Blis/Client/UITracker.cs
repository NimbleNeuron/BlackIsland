using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class UITracker : MonoBehaviour
	{
		public enum Order
		{
			OtherParent,

			MineParent,

			None
		}


		private readonly List<ItemFloatingUI> inSightUI = new List<ItemFloatingUI>();


		private readonly List<UIName> uiNames = new List<UIName>();


		private Canvas canvasEmotion;


		private Canvas canvasName;


		private Canvas canvasStatusBar;


		private FloatingUIPool floatingUIPool;


		private TrackerGrid grid;


		private Transform mineEmotionParent;


		private Transform mineNameParent;


		private Transform mineStatusBarParent;


		public List<ItemData> needItems;


		private Transform otherEmotionParent;


		private Transform otherNameParent;


		private Transform otherStatusBarParent;

		private void Awake()
		{
			floatingUIPool = GameUtil.Bind<FloatingUIPool>(gameObject, "Pool");
			grid = new TrackerGrid();
			floatingUIPool.InitPool();
			if (MonoBehaviourInstance<MainCameraEvent>.inst != null)
			{
				MonoBehaviourInstance<MainCameraEvent>.inst.OnLatePreRenderEvent += UpdateGrid;
			}

			canvasName = GameUtil.Bind<Canvas>(gameObject, "CanvasName");
			canvasStatusBar = GameUtil.Bind<Canvas>(gameObject, "CanvasStatusBar");
			otherNameParent = transform.Find("CanvasName/OtherNameParent");
			otherStatusBarParent = transform.Find("CanvasStatusBar/OtherStatusBarParent");
			mineNameParent = transform.Find("CanvasName/MineNameParent");
			mineStatusBarParent = transform.Find("CanvasStatusBar/MineStatusBarParent");
			canvasEmotion = GameUtil.Bind<Canvas>(gameObject, "CanvasEmotion");
			otherEmotionParent = GameUtil.Bind<RectTransform>(canvasEmotion.gameObject, "OtherEmotionParent");
			mineEmotionParent = GameUtil.Bind<RectTransform>(canvasEmotion.gameObject, "MineEmotionParent");
		}


		private void OnDestroy()
		{
			if (MonoBehaviourInstance<MainCameraEvent>.inst != null)
			{
				MonoBehaviourInstance<MainCameraEvent>.inst.OnLatePreRenderEvent -= UpdateGrid;
			}
		}


		public void AddGrid(BaseTrackUI baseTrackUi)
		{
			grid.AddGridItem(baseTrackUi);
		}


		private void UpdateGrid()
		{
			grid.UpdateGrid();
		}


		public T Alloc<T>(Order order = Order.OtherParent) where T : BaseTrackUI
		{
			Transform parent;
			if (order == Order.None)
			{
				parent = transform;
			}
			else if (order == Order.MineParent)
			{
				parent = typeof(T) == typeof(UIName) ? mineNameParent : mineStatusBarParent;
			}
			else
			{
				parent = typeof(T) == typeof(UIName) ? otherNameParent : otherStatusBarParent;
			}

			T component = floatingUIPool.Pop<T>(parent).GetComponent<T>();
			component.ResetUI();
			return component.GetComponent<T>();
		}


		public void Free<T>(T worldUI) where T : BaseTrackUI
		{
			if (worldUI != null)
			{
				grid.RemoveGridItem(worldUI);
				floatingUIPool.Push<T>(worldUI);
			}
		}


		public void ChangeEnableTrackerUIName()
		{
			canvasName.enabled = !canvasName.enabled;
		}


		public GameObject CreateEmotionPrefab(string prefabName, Order order = Order.OtherParent)
		{
			Transform parent = order == Order.MineParent ? mineEmotionParent : otherEmotionParent;
			return Instantiate<GameObject>(SingletonMonoBehaviour<ResourceManager>.inst.LoadEmotionPrefab(prefabName),
				transform.localPosition, Quaternion.identity, parent);
		}


		public void ChangeEnableTrackerUIStatusBar()
		{
			canvasStatusBar.enabled = !canvasStatusBar.enabled;
		}


		public void ConveyNeedItems(List<ItemData> needItems)
		{
			this.needItems = needItems;
			OnUpdateDropItemName(null);
		}


		public void OnUpdateDropItemName(ItemFloatingUI itemFloatingUI)
		{
			inSightUI.Clear();
			otherNameParent.GetComponentsInChildren<UIName>(uiNames);
			foreach (UIName uiname in uiNames)
			{
				ItemFloatingUI component = uiname.GetTrackingTarget().GetComponent<ItemFloatingUI>();
				if (component != null)
				{
					inSightUI.Add(component);
				}
			}

			if (itemFloatingUI != null)
			{
				inSightUI.Add(itemFloatingUI);
			}

			foreach (ItemFloatingUI itemFloatingUI2 in inSightUI)
			{
				if (needItems != null && needItems.Contains(itemFloatingUI2.Item.ItemData))
				{
					itemFloatingUI2.ShowNeedMark(true);
				}
				else
				{
					itemFloatingUI2.ShowNeedMark(false);
				}
			}
		}
	}
}