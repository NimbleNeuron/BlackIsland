using System;
using System.Collections;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Blis.Client
{
	public class EmotionEquipSlot : MonoBehaviour
	{
		[FormerlySerializedAs("collider")] [SerializeField]
		private BoxCollider2D m_collider = default;


		[SerializeField] private ClickerModule clickerModule = default;


		private GameObject cloneIcon;


		private EmotionIconData data;


		private Action<EmotionIconData, Vector2, EmotionPlateType, bool> equipCallback;


		private Transform iconParent;


		private Image imgIcon;


		private EmotionPlateType type;


		private Action<EmotionPlateType, Vector2> unEquipCallback;


		public BoxCollider2D Collider => m_collider;


		public EmotionPlateType Type => type;


		public EmotionIconData Data => data;


		private void Awake()
		{
			Bind();
			AddEvent();
		}


		private void Bind()
		{
			imgIcon = GameUtil.Bind<Image>(gameObject, "Icon");
		}


		private void AddEvent()
		{
			clickerModule.Init(OnPointerClick, OnBeginDrag, OnEndDrag, null);
		}


		public void SetIconParent(Transform parent)
		{
			iconParent = parent;
		}


		public void SetAction(Action<EmotionIconData, Vector2, EmotionPlateType, bool> equipCallback,
			Action<EmotionPlateType, Vector2> unEquipCallback)
		{
			this.equipCallback = equipCallback;
			this.unEquipCallback = unEquipCallback;
		}


		public void SetInfo(EmotionPlateType type)
		{
			this.type = type;
		}


		public void SetSlot(int emotionCode)
		{
			data = GameDB.emotionIcon.GetEmotionIconData(emotionCode);
			if (data != null)
			{
				imgIcon.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetEmotionSprite(data.sprite);
				imgIcon.gameObject.SetActive(true);
				return;
			}

			imgIcon.gameObject.SetActive(false);
		}


		private void OnPointerClick(PointerEventData eventData)
		{
			if (data == null)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
			{
				eventData.clickCount = 0;
				unEquipCallback(type, Vector2.zero);
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				unEquipCallback(type, Vector2.zero);
			}
		}


		private void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				return;
			}

			if (data == null)
			{
				return;
			}

			Singleton<SoundControl>.inst.PlayUISound("oui_emotion_v1");
			DestroyIcon();
			cloneIcon = Instantiate<GameObject>(imgIcon.gameObject, iconParent);
			imgIcon.enabled = false;
			StartCoroutine(UpdatePosition());
		}


		private void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				return;
			}

			if (data == null || cloneIcon == null)
			{
				return;
			}

			equipCallback(data, cloneIcon.transform.position, type, false);
			unEquipCallback(type, cloneIcon.transform.position);
			imgIcon.enabled = true;
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


		private IEnumerator UpdatePosition()
		{
			if (cloneIcon != null)
			{
				cloneIcon.transform.eulerAngles = Vector3.zero;
			}

			while (cloneIcon != null)
			{
				cloneIcon.transform.position = Input.mousePosition;
				yield return null;
			}
		}
	}
}