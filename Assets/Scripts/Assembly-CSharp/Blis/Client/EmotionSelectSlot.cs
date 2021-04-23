using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class EmotionSelectSlot : BaseControl
	{
		private ClickerModule clickerModule;


		private GameObject cloneIcon;


		private EmotionIconData data;


		private Action<EmotionIconData, Vector2, EmotionPlateType, bool> equipCallback;


		private Transform iconParent;


		private Image imgIcon;


		private bool isHasEmotion;


		private GameObject lockIcon;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Bind();
			AddEvent();
		}


		private void Bind()
		{
			imgIcon = GameUtil.Bind<Image>(gameObject, "Bg/Icon");
			lockIcon = GameUtil.Bind<RectTransform>(gameObject, "Lock").gameObject;
			clickerModule = GameUtil.Bind<ClickerModule>(gameObject, "Bg");
		}


		private void AddEvent()
		{
			clickerModule.Init(OnPointerClick, OnBeginDrag, OnEndDrag, null);
		}


		public void SetIconParent(Transform parent)
		{
			iconParent = parent;
		}


		public void SetEquipAction(Action<EmotionIconData, Vector2, EmotionPlateType, bool> equipCallback)
		{
			this.equipCallback = equipCallback;
		}


		public void SetSlot(EmotionIconData data)
		{
			this.data = data;
			isHasEmotion = data.purchaseType == EmotionPurchaseType.FREE;
			if (!isHasEmotion && Lobby.inst != null)
			{
				isHasEmotion = Lobby.inst.IsHasEmotion(data.code);
			}

			imgIcon.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetEmotionSprite(data.sprite);
			lockIcon.SetActive(!isHasEmotion);
		}


		private IEnumerator UpdatePosition()
		{
			while (cloneIcon != null)
			{
				cloneIcon.transform.position = Input.mousePosition;
				yield return null;
			}
		}


		public void OnPointerEnter()
		{
			MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(data.name));
			Vector2 vector = transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(15f, 50f);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		public void OnPointerExit()
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}


		private new void OnPointerClick(PointerEventData eventData)
		{
			if (!isHasEmotion)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
			{
				eventData.clickCount = 0;
				equipCallback(data, Vector3.zero, EmotionPlateType.None, true);
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				equipCallback(data, Vector3.zero, EmotionPlateType.None, true);
			}
		}


		private new void OnBeginDrag(PointerEventData eventData)
		{
			if (!isHasEmotion)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				return;
			}

			Singleton<SoundControl>.inst.PlayUISound("oui_emotion_v1");
			DestroyIcon();
			cloneIcon = Instantiate<GameObject>(imgIcon.gameObject, iconParent);
			StartCoroutine(UpdatePosition());
		}


		private new void OnEndDrag(PointerEventData eventData)
		{
			if (!isHasEmotion)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				return;
			}

			if (cloneIcon == null)
			{
				return;
			}

			equipCallback(data, cloneIcon.transform.position, EmotionPlateType.None, false);
			DestroyIcon();
		}


		private void DestroyIcon()
		{
			if (cloneIcon != null)
			{
				Destroy(cloneIcon);
				cloneIcon = null;
			}
		}
	}
}