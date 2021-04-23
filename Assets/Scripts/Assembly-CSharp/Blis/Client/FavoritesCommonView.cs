using System;
using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class FavoritesCommonView : BaseUI, ILnEventHander
	{
		[SerializeField] private Image weaponType = default;


		[SerializeField] private Image characterImage = default;


		[SerializeField] private Text characterName = default;


		[SerializeField] private InputFieldExtension favoritesName = default;


		[SerializeField] private List<UIFavoritesItem> favoritesItems = default;


		[SerializeField] private UIDropRect itemDropRect = default;


		[SerializeField] private Transform itemBlankList = default;


		[SerializeField] private Transform guideText = default;


		[SerializeField] private GameObject statOpen = default;


		[SerializeField] private GameObject statClose = default;


		[SerializeField] private RouteFilterUI routeFilterUI = default;


		[SerializeField] private GameObject favoriteTotalValue = default;


		[SerializeField] private Button edit = default;


		[SerializeField] private Button save = default;


		[SerializeField] private Button next = default;


		[SerializeField] private Button back = default;


		[SerializeField] private Button discard = default;


		[SerializeField] private Button copy = default;


		[SerializeField] private Button bringClose = default;


		private FavoriteEditorViewStyle editorViewStyle;


		private Favorite favorite = default;


		private string prevTitle = "";


		private bool statButtonOpen = true;


		public void OnLnDataChange()
		{
			RenderView();
		}

		
		
		public event Action<Favorite> OnEdited = delegate { };


		
		
		public event Action OnRequestSave = delegate { };


		
		
		public event Action OnRequestDiscard = delegate { };


		
		
		public event Action OnRequestNextStep = delegate { };


		
		
		public event Action OnRequestPreStep = delegate { };


		
		
		public event Action<Favorite> OnRequestCopy = delegate { };


		
		
		public event Action OnRequestBringClose = delegate { };


		
		
		public event Action<bool, bool> OnChangeState = delegate { };


		
		
		public event Action<ItemData> OnClickFavItem = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			save.onClick.AddListener(delegate { OnRequestSave(); });
			discard.onClick.AddListener(delegate { OnRequestDiscard(); });
			next.onClick.AddListener(delegate { OnRequestNextStep(); });
			back.onClick.AddListener(delegate { OnRequestPreStep(); });
			copy.onClick.AddListener(delegate { OnRequestCopy(favorite); });
			bringClose.onClick.AddListener(delegate { OnRequestBringClose(); });
			favoritesName.onEndEdit.AddListener(RenameFavorites);
			itemDropRect.OnDropHandler += OnDropItem;
			for (int i = 0; i < favoritesItems.Count; i++)
			{
				favoritesItems[i].OnRemoveItem += RemoveFavItem;
				favoritesItems[i].OnClick += SelectFavItem;
			}

			routeFilterUI.changeFilterType += ChangeFilterType;
		}


		public void Load(Favorite favorite)
		{
			this.favorite = favorite;
			prevTitle = favorite.title;
			RenderView();
			UISystem.Action(new UpdateFavoriteCommonViewItems
			{
				items = favoritesItems
			});
		}


		public void SetBringModeButtons(FavoriteEditorViewStyle editorViewStyle)
		{
			this.editorViewStyle = editorViewStyle;
			favoritesName.enabled = false;
			edit.gameObject.SetActive(false);
			save.gameObject.SetActive(false);
			next.gameObject.SetActive(false);
			back.gameObject.SetActive(false);
			discard.gameObject.SetActive(false);
			copy.gameObject.SetActive(true);
			bringClose.gameObject.SetActive(true);
			routeFilterUI.Show(false);
		}


		public void SetCheckModeButtons(FavoriteEditorViewStyle editorViewStyle)
		{
			this.editorViewStyle = editorViewStyle;
			favoritesName.enabled = false;
			edit.gameObject.SetActive(false);
			save.gameObject.SetActive(false);
			discard.GetComponent<RectTransform>().sizeDelta = new Vector2(374f, 38f);
			foreach (UIFavoritesItem uifavoritesItem in favoritesItems)
			{
				uifavoritesItem.InteractionLock(true);
			}

			routeFilterUI.IgnoreClickEvent(true);
		}


		public void ResetUI()
		{
			editorViewStyle = FavoriteEditorViewStyle.NORMAL;
			favoritesName.enabled = true;
			edit.gameObject.SetActive(true);
			save.gameObject.SetActive(true);
			next.gameObject.SetActive(true);
			back.gameObject.SetActive(false);
			discard.gameObject.SetActive(true);
			discard.GetComponent<RectTransform>().sizeDelta = new Vector2(182f, 38f);
			copy.gameObject.SetActive(false);
			bringClose.gameObject.SetActive(false);
			routeFilterUI.Show(true);
			routeFilterUI.IgnoreClickEvent(false);
		}


		private void ChangeFilterType(RouteFilterType routeFilterType)
		{
			favorite.routeFilterType = routeFilterType;
			OnChangeState(true, favorite.share);
		}


		public void EnableSaveButton(bool enable)
		{
			if (editorViewStyle == FavoriteEditorViewStyle.ROUTECHECK)
			{
				return;
			}

			save.gameObject.SetActive(enable);
		}


		public void EnableNextButton(bool enable)
		{
			next.gameObject.SetActive(enable);
		}


		public void EnablePreButton(bool enable)
		{
			back.gameObject.SetActive(enable);
		}


		public void DisableFrameEffects()
		{
			foreach (UIFavoritesItem uifavoritesItem in favoritesItems)
			{
				uifavoritesItem.EnableFrameEffect(false);
			}
		}


		private void RenderView()
		{
			for (int i = 0; i < favoritesItems.Count; i++)
			{
				UIFavoritesItem uifavoritesItem = favoritesItems[i];
				uifavoritesItem.gameObject.name = i.ToString();
				uifavoritesItem.Clear();
				uifavoritesItem.gameObject.SetActive(false);
			}

			int index = 0;
			if (favorite != null)
			{
				int num = 0;
				while (num < favorite.weaponCodes.Count && num < favoritesItems.Count)
				{
					int code = favorite.weaponCodes[num];
					ItemData itemData = GameDB.item.FindItemByCode(code);
					if (itemData != null)
					{
						favoritesItems[index].SetItemData(itemData);
						favoritesItems[index++].gameObject.SetActive(true);
					}

					num++;
				}

				favoritesName.text = favorite.title;
				characterName.text = Ln.Get(string.Format("Character/Name/{0}", favorite.characterCode));
				characterImage.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterScoreSprite(favorite.characterCode);
				weaponType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetWeaponMasterySprite(favorite.weaponType);
				itemBlankList.gameObject.SetActive(favorite.weaponCodes.Count > 0);
				guideText.gameObject.SetActive(favorite.weaponCodes.Count <= 0);
				routeFilterUI.ChangeFilterType(favorite.routeFilterType);
			}
		}


		public void OnDropItem(BaseUI dropObject, PointerEventData eventData)
		{
			if (dropObject is ItemDataSlot)
			{
				ItemDataSlot itemDataSlot = (ItemDataSlot) DraggingUI;
				AddFavItem(itemDataSlot.GetItemData().code);
				return;
			}

			if (dropObject is UIFavoritesItem)
			{
				UIFavoritesItem uifavoritesItem = (UIFavoritesItem) dropObject;
				float num = itemDropRect.rectTransform.position.y - eventData.position.y;
				num /= uifavoritesItem.rectTransform.rect.height;
				num += 0.3f;
				int code = uifavoritesItem.GetItemData().code;
				RemoveFavItem(uifavoritesItem);
				AddFavItemAt((int) num, code);
			}
		}


		public void AddFavItem(ItemData itemData)
		{
			if (itemData != null && favorite.weaponCodes.Count < favoritesItems.Count)
			{
				favorite.weaponCodes.Add(itemData.code);
				OnEdited(favorite);
				OnChangeState(true, favorite.share);
			}
		}


		private void AddFavItem(int itemCode)
		{
			if (favorite.weaponCodes.Count < favoritesItems.Count)
			{
				favorite.weaponCodes.Add(itemCode);
				OnEdited(favorite);
				OnChangeState(true, favorite.share);
			}
		}


		private void AddFavItemAt(int at, int itemCode)
		{
			int index = Mathf.Clamp(at, 0, favorite.weaponCodes.Count);
			favorite.weaponCodes.Insert(index, itemCode);
			OnEdited(favorite);
		}


		public void RenameFavorites(string favName)
		{
			if (prevTitle == favoritesName.text)
			{
				return;
			}

			favorite.title = favName;
			OnEdited(favorite);
			OnChangeState(true, favorite.share);
		}


		private void SelectFavItem(ItemData itemdata)
		{
			OnClickFavItem(itemdata);
		}


		private void RemoveFavItem(UIFavoritesItem item)
		{
			if (editorViewStyle >= FavoriteEditorViewStyle.ROUTEBRING)
			{
				return;
			}

			if (favorite.weaponCodes.Count > 0)
			{
				int index = favoritesItems.IndexOf(item);
				favorite.weaponCodes.RemoveAt(index);
				OnEdited(favorite);
				OnChangeState(true, favorite.share);
			}
		}


		public void SetStep(FavoritesEditorStep step)
		{
			if (step == FavoritesEditorStep.Item)
			{
				favoritesItems.ForEach(delegate(UIFavoritesItem x) { x.InteractionLock(false); });
				return;
			}

			if (step != FavoritesEditorStep.Route)
			{
				throw new ArgumentOutOfRangeException("step", step, null);
			}

			favoritesItems.ForEach(delegate(UIFavoritesItem x) { x.InteractionLock(true); });
		}


		public void ClickedTitleEditButton()
		{
			favoritesName.ActivateInputField();
		}


		public void ClickedStatButton()
		{
			statButtonOpen = !statButtonOpen;
			statOpen.SetActive(!statButtonOpen);
			statClose.SetActive(statButtonOpen);
			favoriteTotalValue.SetActive(statButtonOpen);
		}


		public void SetAnchoredPosition(Vector2 position)
		{
			transform.GetComponent<RectTransform>().anchoredPosition = position;
		}
	}
}