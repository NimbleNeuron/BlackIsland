using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIFavoritesItem : BaseUI, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler,
		IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
	{
		[SerializeField] private ItemDataSlot slot = default;


		[SerializeField] private Text label = default;


		[SerializeField] private Image hoverEffect = default;


		private CanvasGroup canvasGroup = default;


		private bool interactionLock = default;


		private RectTransform parentCache = default;


		private int siblingIndex = default;


		public void OnBeginDrag(PointerEventData eventData)
		{
			if (interactionLock)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide(slot.GetParentWindow());
				DraggingUI = this;
				BaseWindow parentWindow = slot.GetParentWindow();
				if (parentWindow != null)
				{
					parentWindow.Open();
				}

				parentCache = (RectTransform) transform.parent;
				if (MonoBehaviourInstance<GameUI>.inst != null)
				{
					transform.SetParent(MonoBehaviourInstance<GameUI>.inst.OverlayUI);
				}
				else if (MonoBehaviourInstance<LobbyUI>.inst != null)
				{
					transform.SetParent(MonoBehaviourInstance<LobbyUI>.inst.OverlayUI);
				}

				canvasGroup.blocksRaycasts = false;
				canvasGroup.interactable = false;
			}
		}


		public void OnDrag(PointerEventData eventData)
		{
			if (interactionLock)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (DraggingUI == null)
				{
					return;
				}

				transform.Translate(eventData.delta);
			}
		}


		public void OnEndDrag(PointerEventData eventData)
		{
			if (interactionLock)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				transform.SetParent(parentCache);
				transform.SetSiblingIndex(siblingIndex);
				DraggingUI = null;
				GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
				if (gameObject == null ||
				    gameObject != null && !gameObject.transform.FindParentRecursively(parentCache.name))
				{
					OnRemoveItem(this);
				}

				parentCache = null;
				canvasGroup.blocksRaycasts = true;
				canvasGroup.interactable = true;
			}
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.dragging)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				OnClick(slot.GetItemData());
				EnableFrameEffect(true);
			}

			if (interactionLock)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
			{
				eventData.clickCount = 0;
				RequestRemove();
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				RequestRemove();
			}
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			slot.OnPointerEnter(eventData);
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			slot.OnPointerExit(eventData);
		}

		
		
		public event Action<UIFavoritesItem> OnRemoveItem = delegate { };


		
		
		public event Action<ItemData> OnClick = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			hoverEffect.gameObject.SetActive(false);
			siblingIndex = transform.GetSiblingIndex();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
		}


		public void SetItemData(ItemData itemData)
		{
			slot.SetItemData(itemData);
			slot.SetSlotType(SlotType.None);
			slot.SetSprite(itemData.GetSprite());
			slot.SetBackground(itemData.GetGradeSprite());
			label.text = LnUtil.GetItemName(itemData.code);
			label.color = itemData.itemGrade.GetColor();
		}


		public ItemData GetItemData()
		{
			return slot.GetItemData();
		}


		public void Clear()
		{
			slot.ResetSlot();
			label.text = null;
		}


		public void InteractionLock(bool enable)
		{
			interactionLock = enable;
		}


		public void EnableFrameEffect(bool enable)
		{
			hoverEffect.gameObject.SetActive(enable);
		}


		private void RequestRemove()
		{
			hoverEffect.gameObject.SetActive(false);
			OnRemoveItem(this);
		}
	}
}