using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectSkinSlot : BaseControl
	{
		public delegate void SelectCallback(int code);


		private Button button;


		private Image focusImgGrade;


		private Image focusImgSkin;


		private GameObject focusNotReleaseableLock;


		private GameObject focusReleaseableLock;


		private Image imgGrade;


		private Image imgSkin;


		private bool isFocus;


		private GameObject notReleaseableLock;


		private Transform objFocus;


		private GameObject releaseableLock;


		private ScrollRect scrollRect;


		private GameObject select;


		public SelectCallback selectCallback;


		private int slotIndex;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			button = GameUtil.Bind<Button>(gameObject, "SkinButton");
			imgSkin = GameUtil.Bind<Image>(button.gameObject, "Portrait");
			imgGrade = GameUtil.Bind<Image>(button.gameObject, "Tier");
			select = transform.Find("SkinButton/Select").gameObject;
			releaseableLock = transform.Find("SkinButton/SlotState/LockOpen").gameObject;
			notReleaseableLock = transform.Find("SkinButton/SlotState/Lock").gameObject;
			objFocus = GameUtil.Bind<Transform>(button.gameObject, "OBJ_Focus");
			focusImgSkin = GameUtil.Bind<Image>(objFocus.gameObject, "Portrait");
			focusImgGrade = GameUtil.Bind<Image>(objFocus.gameObject, "Tier");
			focusReleaseableLock = objFocus.Find("SlotState/LockOpen").gameObject;
			focusNotReleaseableLock = objFocus.Find("SlotState/Lock").gameObject;
			OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnBeginDrag(eventData);
			};
			OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnEndDrag(eventData);
			};
			OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnDrag(eventData);
			};
			OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				ScrollRect scrollRect = this.scrollRect;
				if (scrollRect == null)
				{
					return;
				}

				scrollRect.OnScroll(eventData);
			};
			button.onClick.AddListener(OnClickSlot);
		}


		public void SetScrollRect(ScrollRect scrollRect)
		{
			this.scrollRect = scrollRect;
		}


		public void SetSlot(CharacterSkinData data, int index)
		{
			slotIndex = index;
			imgSkin.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(data.characterCode,
					data.index);
			imgGrade.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterSkinGradeSprite(data.grade);
			focusImgSkin.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(data.characterCode,
					data.index);
			focusImgGrade.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterSkinGradeSprite(data.grade);
		}


		public void SetReleaseableLock(bool isActive)
		{
			releaseableLock.SetActive(isActive);
			focusReleaseableLock.SetActive(isActive);
		}


		public void SetNotReleaseableLock(bool isActive)
		{
			notReleaseableLock.SetActive(isActive);
			focusNotReleaseableLock.SetActive(isActive);
		}


		public void SetButtonInteractable(bool interactable)
		{
			button.interactable = interactable;
		}


		public void SetFocus(bool isFocus)
		{
			this.isFocus = isFocus;
		}


		public void OnSelectSlot(bool isActive)
		{
			select.SetActive(isActive);
		}


		private void OnClickSlot()
		{
			SelectCallback selectCallback = this.selectCallback;
			if (selectCallback == null)
			{
				return;
			}

			selectCallback(slotIndex);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (isFocus)
			{
				objFocus.gameObject.SetActive(true);
			}
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			if (isFocus)
			{
				objFocus.gameObject.SetActive(false);
			}
		}
	}
}