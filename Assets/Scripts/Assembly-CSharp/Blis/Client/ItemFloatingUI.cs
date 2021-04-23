using BIFog;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ItemFloatingUI : MonoBehaviour, ISightEventHandler, IHoverble
	{
		private Item item;


		private UIName uiName;


		public Item Item => item;


		private void OnEnable()
		{
			if (!SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
			{
				return;
			}

			if (MonoBehaviourInstance<GameUI>.inst != null)
			{
				uiName = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIName>();
				uiName.SetColor(Color.white);
				uiName.SetIsItemUIName(true);
			}

			UpdateName();
			if (MonoBehaviourInstance<GameInput>.inst != null)
			{
				MonoBehaviourInstance<GameInput>.inst.OnKeyPressed += OnKeyPress;
				MonoBehaviourInstance<GameInput>.inst.OnKeyRelease += OnKeyRelease;
				if (MonoBehaviourInstance<GameInput>.inst.IsKeyHoldEvent(GameInputEvent.ShowObjectText))
				{
					OnKeyPress(GameInputEvent.ShowObjectText, Vector3.zero);
					return;
				}

				OnKeyRelease(GameInputEvent.ShowObjectText, Vector3.zero);
			}
		}


		private void OnDisable()
		{
			if (!SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
			{
				return;
			}

			GameUI inst = MonoBehaviourInstance<GameUI>.inst;
			if (uiName != null && inst != null)
			{
				uiName.SetColor(Color.white);
				uiName.SetIsItemUIName(false);
				inst.UITracker.Free<UIName>(uiName);
				uiName = null;
			}

			GameInput inst2 = MonoBehaviourInstance<GameInput>.inst;
			if (inst2 != null)
			{
				inst2.OnKeyPressed -= OnKeyPress;
				inst2.OnKeyRelease -= OnKeyRelease;
			}
		}


		public void HoverOn()
		{
			if (item != null && uiName != null)
			{
				uiName.ShowRolloverBg(true);
			}
		}


		public void HoverOff()
		{
			if (uiName != null)
			{
				uiName.ShowRolloverBg(false);
			}
		}


		public void OnSight()
		{
			enabled = true;
			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateDropItemName(this);
		}


		public void OnHide()
		{
			enabled = false;
		}


		Transform ISightEventHandler.transform => base.transform;


		public void ShowNeedMark(bool enable)
		{
			if (uiName != null)
			{
				uiName.ShowNeedMark(enable);
			}
		}


		public void Init(Item item)
		{
			this.item = item;
			UpdateName();
			OnHide();
		}


		private void UpdateName()
		{
			if (uiName != null && item != null)
			{
				uiName.SetName(LnUtil.GetItemName(item.ItemData.code));
				uiName.SetColor(item.ItemData.itemGrade.GetColor());
				uiName.ShowBg(true);
				uiName.SetTrackingTarget(base.transform);
				uiName.SetTrackingOffset(Vector3.up);
			}
		}


		private void OnKeyPress(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (inputEvent == GameInputEvent.ShowObjectText && uiName != null)
			{
				uiName.SetLeftClickCallback(delegate
				{
					LocalObject component = base.transform.GetComponent<LocalObject>();
					if (component != null)
					{
						SingletonMonoBehaviour<PlayerController>.inst.OnSelectEvent(component, component.GetPosition(),
							component.GetPosition(), true);
					}
				});
				uiName.SetRightClickCallback(delegate
				{
					LocalObject component = base.transform.GetComponent<LocalObject>();
					if (component != null)
					{
						SingletonMonoBehaviour<PlayerController>.inst.OnMoveEvent(false, component,
							component.GetPosition(), true);
					}
				});
				uiName.EnableClick(true);
			}
		}


		private void OnKeyRelease(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (inputEvent == GameInputEvent.ShowObjectText && uiName != null)
			{
				uiName.EnableClick(false);
			}
		}
	}
}