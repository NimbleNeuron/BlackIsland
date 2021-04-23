using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class Slot : BaseControl
	{
		protected Image background;
		protected UICooldown cooldown;
		private ISlotEventListener eventListener;
		protected Image image;
		protected bool isDraggable;
		private bool isDragging;
		private bool isItemBox;
		protected bool? isLock;
		private Image lockImg;
		private Transform mask;
		protected LnText stack;
		private Coroutine updateDrag;
		protected bool useTooltip;
		public bool UseTooltip => useTooltip;
		public bool IsDraggable => isDraggable;

		public UICooldown Cooldown => cooldown;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			useTooltip = true;
			isDraggable = false;
			isDragging = false;
			mask = GameUtil.Bind<Transform>(gameObject, "Mask");
			image = GameUtil.Bind<Image>(gameObject, "Mask/Img");
			background = GameUtil.Bind<Image>(gameObject, "Background");
			lockImg = GameUtil.Bind<Image>(gameObject, "Lock");
			stack = GameUtil.Bind<LnText>(gameObject, "Stack");
			cooldown = GetComponentInChildren<UICooldown>();
		}


		public virtual void ResetSlot()
		{
			if (image != null)
			{
				image.sprite = null;
				image.color = Color.clear;
			}

			if (background != null)
			{
				background.sprite = null;
				background.color = Color.clear;
			}

			if (stack != null)
			{
				stack.text = "";
			}

			SetLock(false);
		}


		public virtual void SetSprite(Sprite sprite)
		{
			if (image != null)
			{
				image.sprite = sprite;
				image.color = sprite == null ? Color.clear : Color.white;
			}
		}


		public void SetSpriteColor(Color color)
		{
			if (image.sprite != null)
			{
				image.color = color;
			}
		}


		public void SetBackground(Sprite sprite)
		{
			if (background != null)
			{
				background.sprite = sprite;
				background.color = sprite == null ? Color.clear : Color.white;
			}
		}


		public virtual void SetLock(bool isLock)
		{
			this.isLock = isLock;
			if (lockImg != null)
			{
				lockImg.enabled = isLock;
			}
		}


		public void SetStackText(string stackText)
		{
			if (stack != null)
			{
				stack.text = stackText;
			}
		}


		public void SetUseTooltip(bool usable)
		{
			useTooltip = usable;
		}


		public void SetDraggable(bool enable)
		{
			isDraggable = enable;
		}


		public void SetItemBoxSlot(bool enable)
		{
			isItemBox = enable;
		}


		public void SetEventListener(ISlotEventListener eventListener)
		{
			this.eventListener = eventListener;
		}


		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			if (isItemBox)
			{
				OnSlotClickEvent(eventData, true);
			}
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (eventData.dragging)
			{
				return;
			}

			if (isItemBox)
			{
				return;
			}

			OnSlotClickEvent(eventData, false);
		}


		private void OnSlotClickEvent(PointerEventData eventData, bool isPointerDownEvent)
		{
			PointerEventData.InputButton button = eventData.button;
			if (button != PointerEventData.InputButton.Left)
			{
				if (button != PointerEventData.InputButton.Right)
				{
					return;
				}

				ISlotEventListener slotEventListener = eventListener;
				if (slotEventListener == null)
				{
					return;
				}

				slotEventListener.OnSlotRightClick(this);
			}
			else if (eventData.clickCount == 1 || isPointerDownEvent)
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					if (MonoBehaviourInstance<GameUI>.inst == null)
					{
						return;
					}

					ItemDataSlot itemDataSlot = this as ItemDataSlot;
					if (itemDataSlot == null)
					{
						return;
					}

					ItemData itemData = null;
					if (itemDataSlot != null)
					{
						itemData = itemDataSlot.GetItemData();
						if (itemData == null)
						{
							return;
						}
					}

					MonoBehaviourInstance<GameUI>.inst.Events.RequestAddItemGuide(itemData);
				}
				else if (Input.GetKey(KeyCode.LeftAlt))
				{
					if (MonoBehaviourInstance<GameUI>.inst == null)
					{
						return;
					}

					ItemSlot itemSlot = this as ItemSlot;
					ItemDataSlot itemDataSlot2 = this as ItemDataSlot;
					if (itemSlot != null)
					{
						Item item = itemSlot.GetItem();
						if (item == null)
						{
							return;
						}

						SlotType slotType = itemSlot.GetSlotType();
						SystemChatType type = SystemChatType.PingFindItem;
						if (slotType == SlotType.ScoreBoard)
						{
							type = SystemChatType.PingPickItem;
						}
						else if (slotType == SlotType.Equipment || slotType == SlotType.Inventory)
						{
							type = SystemChatType.PingInventoryItem;
						}

						ReqItemPing packet = new ReqItemPing
						{
							itemId = item.itemCode,
							type = type
						};
						MonoBehaviourInstance<GameClient>.inst.Request(packet);
					}
					else
					{
						if (!(itemDataSlot2 != null))
						{
							return;
						}

						ItemData itemData2 = itemDataSlot2.GetItemData();
						if (itemData2 == null)
						{
							return;
						}

						SlotType slotType2 = itemDataSlot2.GetSlotType();
						SystemChatType type2 = SystemChatType.PingPickItem;
						if (slotType2 == SlotType.Navigation)
						{
							type2 = SystemChatType.PingSearchItem;
						}
						else if (slotType2 == SlotType.ShortCut)
						{
							type2 = SystemChatType.PingPickItem;
						}

						ReqItemPing packet2 = new ReqItemPing
						{
							itemId = itemData2.code,
							type = type2
						};
						MonoBehaviourInstance<GameClient>.inst.Request(packet2);
					}
				}
				else
				{
					ISlotEventListener slotEventListener2 = eventListener;
					if (slotEventListener2 == null)
					{
						return;
					}

					slotEventListener2.OnSlotLeftClick(this);
				}
			}
			else if (eventData.clickCount == 2)
			{
				eventData.clickCount = 0;
				ISlotEventListener slotEventListener3 = eventListener;
				if (slotEventListener3 == null)
				{
					return;
				}

				slotEventListener3.OnSlotDoubleClick(this);
			}
		}


		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (!isDraggable)
				{
					return;
				}

				MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
				DraggingUI = this;
				isDragging = true;
				BaseWindow parentWindow = GetParentWindow();
				if (parentWindow != null)
				{
					parentWindow.Open();
				}

				if (MonoBehaviourInstance<GameUI>.inst != null)
				{
					image.transform.SetParent(MonoBehaviourInstance<GameUI>.inst.OverlayUI);
				}
				else if (MonoBehaviourInstance<LobbyUI>.inst != null)
				{
					image.transform.SetParent(MonoBehaviourInstance<LobbyUI>.inst.OverlayUI);
				}

				StopCoroutineUpdateDrag();
				updateDrag = this.StartThrowingCoroutine(UpdateDrag(),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][OnBeginDrag] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}


		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (!isDraggable)
				{
					return;
				}

				DraggingUI = null;
				isDragging = false;
				image.transform.SetParent(mask);
				image.rectTransform.anchoredPosition = Vector2.zero;
				if (eventData.hovered == null || eventData.hovered.Count <= 0)
				{
					ISlotEventListener slotEventListener = eventListener;
					if (slotEventListener != null)
					{
						slotEventListener.OnThrowItem(this);
					}
				}

				StopCoroutineUpdateDrag();
			}
		}


		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (!isDraggable || DraggingUI == null)
				{
					return;
				}

				image.transform.position = eventData.position;
			}
		}


		public override void OnDrop(PointerEventData eventData)
		{
			base.OnDrop(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				ISlotEventListener slotEventListener = eventListener;
				if (slotEventListener == null)
				{
					return;
				}

				slotEventListener.OnDropItem(this, DraggingUI);
			}
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			ISlotEventListener slotEventListener = eventListener;
			if (slotEventListener == null)
			{
				return;
			}

			slotEventListener.OnPointerEnter(this);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			ISlotEventListener slotEventListener = eventListener;
			if (slotEventListener != null)
			{
				slotEventListener.OnPointerExit(this);
			}

			MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
		}


		private IEnumerator UpdateDrag()
		{
			while (isDragging && isDraggable && DraggingUI != null)
			{
				if (!Input.GetMouseButton(0))
				{
					OnEndDrag(new PointerEventData(EventSystem.current));
					break;
				}

				yield return new WaitForEndOfFrame();
			}
		}


		private void StopCoroutineUpdateDrag()
		{
			if (updateDrag != null)
			{
				StopCoroutine(updateDrag);
				updateDrag = null;
			}
		}
	}
}