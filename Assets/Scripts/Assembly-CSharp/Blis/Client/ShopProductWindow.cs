using System;
using System.Collections.Generic;
using Blis.Client.UI.Module;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShopProductWindow : BaseWindow
	{
		public delegate void BuySuccessCallback(object code = null);


		public delegate void BuySuccessRouteSlotCallback(object obj = null);


		public delegate void NoMoneyOpenShopCallback();


		private const int MinSkinSlotCount = 5;


		private const int IgnoreSkinSlotCount = 2;


		[SerializeField] private CharacterSelectSkinSlot characterSelectSkinSlot = default;


		private readonly Color32 commentColor = new Color32(236, 232, 199, byte.MaxValue);


		private readonly Color32 lackColor = new Color32(byte.MaxValue, 68, 54, byte.MaxValue);


		public Action assetShopCloseAction;


		private Button btnBuyACoin;


		private Button btnBuyCash;


		private Button btnBuyNP;


		public BuySuccessCallback buySuccessCallback;


		public BuySuccessRouteSlotCallback buySuccessRouteSlotCallback;


		private int characterCode;


		private Button characterInfoButton;


		private Button clearNicknameBtn;


		private bool clickedInfoBtn;


		private Image imageNP;


		private Image imgDisableACoin;


		private Image imgDisableNP;


		private bool isAvailableACoin;


		private bool isAvailableCash;


		private bool isAvailableNP;


		private VerticalLayoutGroup nicknameChange;


		private Transform nickNameInfo;


		private InputField nicknameInputField;


		public NoMoneyOpenShopCallback noMoneyOpenShopCallback;


		private bool noticeAgree;


		private Toggle noticeToggle;


		private object product;


		private Image productImage;


		private Text productName;


		private Image productRouteCharImage;


		private GameObject productRouteImage;


		private LnUnderlineText purchaseNotice;


		private int scrollIndex;


		private int selectSlotIndex;


		private RectTransform skinContent;


		private int skinCount;


		private Transform skinInfo;


		private Button skinInfoButton;


		private LnText skinInfoContent;


		private GameObject skinList;


		private Button skinNextButton;


		private Button skinPreButton;


		private ScrollRect skinScrollRect;


		private CharacterSelectSkinSlot[] skinSlots;


		private LnText textFree;


		private Text txtComment;


		private Text txtPriceACoin;


		private Text txtPriceCash;


		private Text txtPriceNP;


		private Text txtUserACoin;


		private Text txtUserNP;


		private bool validNicknameLength;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			txtUserNP = GameUtil.Bind<Text>(gameObject, "Money/NP/Text");
			txtUserACoin = GameUtil.Bind<Text>(gameObject, "Money/AP/Text");
			productImage = GameUtil.Bind<Image>(gameObject, "Product/ProductImage");
			productRouteImage = transform.FindRecursively("ProductRouteImage").gameObject;
			productRouteCharImage = GameUtil.Bind<Image>(gameObject, "Product/ProductRouteImage/Character");
			productName = GameUtil.Bind<Text>(gameObject, "Product/ProductName");
			txtComment = GameUtil.Bind<Text>(gameObject, "PurchaseText");
			purchaseNotice = GameUtil.Bind<LnUnderlineText>(gameObject, "PurchaseNotice");
			noticeToggle = GameUtil.Bind<Toggle>(gameObject, "NoticeToggle");
			skinList = transform.FindRecursively("SkinList").gameObject;
			skinPreButton = GameUtil.Bind<Button>(skinList, "Btn_Left");
			skinNextButton = GameUtil.Bind<Button>(skinList, "Btn_Right");
			skinScrollRect = GameUtil.Bind<ScrollRect>(skinList, "SkinScrollRect");
			skinContent = GameUtil.Bind<RectTransform>(skinList, "SkinScrollRect/ViewPort/Content");
			skinInfo = GameUtil.Bind<Transform>(gameObject, "ProductInfo/Skin");
			skinInfoContent = GameUtil.Bind<LnText>(gameObject, "ProductInfo/Skin/TXT_Info");
			btnBuyNP = GameUtil.Bind<Button>(gameObject, "Buttons/BtnBuyNP");
			btnBuyACoin = GameUtil.Bind<Button>(gameObject, "Buttons/BtnBuyACoin");
			btnBuyCash = GameUtil.Bind<Button>(gameObject, "Buttons/BtnBuyCash");
			imageNP = GameUtil.Bind<Image>(btnBuyNP.gameObject, "Icon");
			textFree = GameUtil.Bind<LnText>(btnBuyNP.gameObject, "FreeText");
			txtPriceNP = GameUtil.Bind<Text>(btnBuyNP.gameObject, "Text");
			imgDisableNP = GameUtil.Bind<Image>(btnBuyNP.gameObject, "Disable");
			txtPriceACoin = GameUtil.Bind<Text>(btnBuyACoin.gameObject, "Text");
			imgDisableACoin = GameUtil.Bind<Image>(btnBuyACoin.gameObject, "Disable");
			txtPriceCash = GameUtil.Bind<Text>(btnBuyCash.gameObject, "Text");
			characterInfoButton = GameUtil.Bind<Button>(gameObject, "BtnGoChaInfo");
			skinInfoButton = GameUtil.Bind<Button>(gameObject, "BtnSkinPreview");
			nickNameInfo = GameUtil.Bind<Transform>(gameObject, "ProductInfo/NickName");
			nicknameChange = GameUtil.Bind<VerticalLayoutGroup>(gameObject, "NicknameChange");
			nicknameInputField = GameUtil.Bind<InputField>(gameObject, "NicknameChange/InputField");
			clearNicknameBtn = GameUtil.Bind<Button>(gameObject, "NicknameChange/InputField/BtnDelete");
			noticeToggle.onValueChanged.AddListener(OnToggleValueChange);
			nicknameInputField.onValueChanged.AddListener(OnInputValueChange);
			clearNicknameBtn.onClick.AddListener(OnClickClearNickname);
			skinPreButton.onClick.AddListener(OnClickPreSkin);
			skinNextButton.onClick.AddListener(OnClickNextSkin);
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab == LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.PauseLobbySound();
			}

			SetDefaultSetting();
		}


		private void SetDefaultSetting()
		{
			isAvailableNP = false;
			isAvailableACoin = false;
			isAvailableCash = false;
			productImage.sprite = null;
			productName.text = "";
			txtComment.text = "";
			txtComment.color = commentColor;
			noticeToggle.isOn = false;
			txtUserNP.text = StringUtil.AssetToUnitString(Lobby.inst.User.np);
			txtUserACoin.text = StringUtil.AssetToUnitString(Lobby.inst.User.aCoin);
			purchaseNotice.SetUnderline(Ln.GetCurrentLanguage() == SupportLanguage.Korean);
			purchaseNotice.raycastTarget = Ln.GetCurrentLanguage() == SupportLanguage.Korean;
			validNicknameLength = true;
			SetDefaultObject();
		}


		public void SetProduct(ShopProduct shopData)
		{
			if (shopData.shopType == ShopType.NP)
			{
				product = shopData;
				productImage.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(shopData.img);
				productName.text = Ln.Get("Product/Name/" + shopData.productId);
				txtComment.text = Ln.Format("{0} 개를 구입하겠습니까?", Ln.Get("Product/Name/" + shopData.productId));
				string assetPriceBtnMoney = "";
				if (shopData.purchaseType == PurchaseType.IAP)
				{
					isAvailableCash = true;
					assetPriceBtnMoney = StringUtil.GetLocalizedPrice(Lobby.inst.User.Currency, shopData.price);
				}
				else if (shopData.purchaseType == PurchaseType.ACOIN)
				{
					isAvailableACoin = true;
					assetPriceBtnMoney = StringUtil.AssetToString(shopData.price);
				}
				else if (shopData.purchaseType == PurchaseType.NP)
				{
					isAvailableNP = true;
					assetPriceBtnMoney = StringUtil.AssetToString(shopData.price);
				}

				SetAssetProductObject();
				SetAssetPriceBtnMoney(assetPriceBtnMoney);
				return;
			}

			if (shopData.shopType == ShopType.CHARACTER)
			{
				product = shopData;
				isAvailableNP = shopData.PriceNP != -1;
				isAvailableACoin = shopData.PriceACoin != -1;
				productImage.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(shopData.img);
				productName.text = Ln.Get(LnType.Character_Name, shopData.code.ToString());
				txtComment.text = Ln.Get("해당 실험체를 영입하시겠습니까?");
				SetCharacterProductObject();
				SetPriceBtnCharacter(shopData.PriceNP, shopData.PriceACoin);
				return;
			}

			if (shopData.shopType == ShopType.SKIN)
			{
				product = shopData;
				bool skinProductObject = Lobby.inst.IsHaveSkin(shopData.code);
				CharacterSkinData skinData = GameDB.character.GetSkinData(shopData.code);
				characterCode = skinData.characterCode;
				skinInfoContent.text = LnUtil.GetSkinDesc(skinData.code);
				SetSkinProductObject(skinProductObject);
				SetPriceBtnSkin(shopData.PriceNP, shopData.PriceACoin);
				SetSkinList(shopData.code);
				return;
			}

			if (shopData.shopType == ShopType.WEAPON_ROUTE)
			{
				product = shopData;
				characterCode = shopData.code;
				isAvailableNP = shopData.PriceNP != -1;
				isAvailableACoin = shopData.PriceACoin != -1;
				productRouteCharImage.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(shopData.code);
				productName.text = Ln.Format("루트 슬롯({0})", Ln.Get(LnType.Character_Name, shopData.code.ToString()));
				txtComment.text = Ln.Get("해당 상품을 구매하시겠습니까?");
				SetRouteSlotProductObject();
				SetPriceBtnRouteSlot(shopData.PriceNP, shopData.PriceACoin);
			}
		}


		public void SetProduct(WeaponRouteSlotShop weaponRouteSlotShop)
		{
			product = weaponRouteSlotShop;
			isAvailableNP = weaponRouteSlotShop.np != -1;
			isAvailableACoin = weaponRouteSlotShop.aCoin != -1;
			characterCode = weaponRouteSlotShop.characterCode;
			productRouteCharImage.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(
					weaponRouteSlotShop.characterCode);
			productName.text = Ln.Format("루트 슬롯({0})",
				Ln.Get(LnType.Character_Name, weaponRouteSlotShop.characterCode.ToString()));
			txtComment.text = Ln.Get("해당 상품을 구매하시겠습니까?");
			SetRouteSlotProductObject();
			SetPriceBtnRouteSlot(weaponRouteSlotShop.np, weaponRouteSlotShop.aCoin);
		}


		public void SetNicknameChangeProduct(ShopProduct data)
		{
			product = data;
			validNicknameLength = false;
			productImage.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(data.img);
			productName.text = Ln.Get("Product/Name/" + data.productId);
			OnClickClearNickname();
			SetNicknameChangeProductObject();
			SetChangeNicknamePrice(data.price);
		}


		private void SetDefaultObject()
		{
			skinList.SetActive(false);
			skinPreButton.gameObject.SetActive(false);
			skinNextButton.gameObject.SetActive(false);
			productImage.gameObject.SetActive(false);
			productRouteImage.SetActive(false);
			purchaseNotice.gameObject.SetActive(false);
			noticeToggle.gameObject.SetActive(false);
			btnBuyNP.gameObject.SetActive(false);
			btnBuyACoin.gameObject.SetActive(false);
			btnBuyCash.gameObject.SetActive(false);
			characterInfoButton.gameObject.SetActive(false);
			skinInfoButton.gameObject.SetActive(false);
			txtComment.gameObject.SetActive(false);
			nicknameChange.gameObject.SetActive(false);
			nickNameInfo.gameObject.SetActive(false);
			skinInfo.gameObject.SetActive(false);
			textFree.gameObject.SetActive(false);
		}


		private void SetNicknameChangeProductObject()
		{
			productImage.gameObject.SetActive(true);
			purchaseNotice.gameObject.SetActive(true);
			noticeToggle.gameObject.SetActive(true);
			btnBuyNP.gameObject.SetActive(true);
			nicknameChange.gameObject.SetActive(true);
			nickNameInfo.gameObject.SetActive(true);
			skinInfo.gameObject.SetActive(false);
		}


		private void SetCharacterProductObject()
		{
			productImage.gameObject.SetActive(true);
			purchaseNotice.gameObject.SetActive(true);
			noticeToggle.gameObject.SetActive(true);
			btnBuyNP.gameObject.SetActive(isAvailableNP);
			btnBuyACoin.gameObject.SetActive(isAvailableACoin);
			characterInfoButton.gameObject.SetActive(true);
			txtComment.gameObject.SetActive(true);
		}


		private void SetSkinProductObject(bool isHaveSkin)
		{
			skinList.SetActive(true);
			productImage.gameObject.SetActive(true);
			purchaseNotice.gameObject.SetActive(!isHaveSkin);
			noticeToggle.gameObject.SetActive(!isHaveSkin);
			btnBuyNP.gameObject.SetActive(!isHaveSkin && isAvailableNP);
			btnBuyACoin.gameObject.SetActive(!isHaveSkin && isAvailableACoin);
			skinInfoButton.gameObject.SetActive(true);
			txtComment.gameObject.SetActive(true);
			skinInfo.gameObject.SetActive(true);
		}


		private void SetAssetProductObject()
		{
			productImage.gameObject.SetActive(true);
			btnBuyNP.gameObject.SetActive(isAvailableNP);
			btnBuyACoin.gameObject.SetActive(isAvailableACoin);
			btnBuyCash.gameObject.SetActive(isAvailableCash);
			txtComment.gameObject.SetActive(true);
		}


		private void SetRouteSlotProductObject()
		{
			productRouteImage.SetActive(true);
			purchaseNotice.gameObject.SetActive(true);
			noticeToggle.gameObject.SetActive(true);
			btnBuyNP.gameObject.SetActive(isAvailableNP);
			btnBuyACoin.gameObject.SetActive(isAvailableACoin);
			txtComment.gameObject.SetActive(true);
		}


		private void SetAssetPriceBtnMoney(string price)
		{
			SetHighlightNPBtn(noticeAgree);
			SetHighlightACoinBtn(noticeAgree);
			txtPriceCash.text = price;
			txtPriceNP.gameObject.SetActive(true);
			imageNP.gameObject.SetActive(true);
		}


		private void SetPriceBtnCharacter(int charNP, int charACoin)
		{
			SetHighlightNPBtn(noticeAgree);
			SetHighlightACoinBtn(noticeAgree);
			SetColorNPBtnText(charNP);
			SetColorACoinBtnText(charACoin);
			txtPriceNP.text = StringUtil.AssetToString(charNP);
			txtPriceACoin.text = StringUtil.AssetToString(charACoin);
			txtPriceNP.gameObject.SetActive(true);
			imageNP.gameObject.SetActive(true);
		}


		private void SetPriceBtnSkin(int charNP, int charACoin)
		{
			SetHighlightNPBtn(noticeAgree);
			SetHighlightACoinBtn(noticeAgree);
			SetColorNPBtnText(charNP);
			SetColorACoinBtnText(charACoin);
			txtPriceNP.text = StringUtil.AssetToString(charNP);
			txtPriceACoin.text = StringUtil.AssetToString(charACoin);
			txtPriceNP.gameObject.SetActive(true);
			imageNP.gameObject.SetActive(true);
		}


		private void SetPriceBtnRouteSlot(int charNP, int charACoin)
		{
			SetHighlightNPBtn(noticeAgree);
			SetHighlightACoinBtn(noticeAgree);
			SetColorNPBtnText(charNP);
			SetColorACoinBtnText(charACoin);
			txtPriceNP.text = StringUtil.AssetToString(charNP);
			txtPriceACoin.text = StringUtil.AssetToString(charACoin);
			txtPriceNP.gameObject.SetActive(true);
			imageNP.gameObject.SetActive(true);
		}


		private void SetChangeNicknamePrice(int np)
		{
			if (Lobby.inst.User.HaveFreeNicknameChange)
			{
				txtPriceNP.color = Color.white;
				txtPriceNP.gameObject.SetActive(false);
				imageNP.gameObject.SetActive(false);
				textFree.gameObject.SetActive(true);
			}
			else
			{
				txtPriceNP.color = Lobby.inst.User.np >= np ? Color.white : (Color) lackColor;
				txtPriceNP.text = StringUtil.AssetToString(np);
				txtPriceNP.gameObject.SetActive(true);
				imageNP.gameObject.SetActive(true);
				textFree.gameObject.SetActive(false);
			}

			SetHighlightNPBtn(noticeAgree);

			// co: dotPeek
			// if (Lobby.inst.User.HaveFreeNicknameChange)
			// {
			// 	this.txtPriceNP.color = Color.white;
			// 	this.txtPriceNP.gameObject.SetActive(false);
			// 	this.imageNP.gameObject.SetActive(false);
			// 	this.textFree.gameObject.SetActive(true);
			// }
			// else
			// {
			// 	this.txtPriceNP.color = ((Lobby.inst.User.np >= np) ? Color.white : this.lackColor);
			// 	this.txtPriceNP.text = StringUtil.AssetToString(np);
			// 	this.txtPriceNP.gameObject.SetActive(true);
			// 	this.imageNP.gameObject.SetActive(true);
			// 	this.textFree.gameObject.SetActive(false);
			// }
			// this.SetHighlightNPBtn(this.noticeAgree);
		}


		private void SetHighlightNPBtn(bool highlight)
		{
			btnBuyNP.enabled = highlight;
			imgDisableNP.gameObject.SetActive(!highlight);
		}


		private void SetColorNPBtnText(int charNP)
		{
			if (Lobby.inst.User.np >= charNP)
			{
				txtPriceNP.color = Color.white;
				return;
			}

			txtPriceNP.color = lackColor;
		}


		private void SetHighlightACoinBtn(bool highlight)
		{
			btnBuyACoin.enabled = highlight;
			imgDisableACoin.gameObject.SetActive(!highlight);
		}


		private void SetColorACoinBtnText(int charACoin)
		{
			if (Lobby.inst.User.aCoin >= charACoin)
			{
				txtPriceACoin.color = Color.white;
				return;
			}

			txtPriceACoin.color = lackColor;
		}


		private void SetSkinList(int skinCode)
		{
			List<CharacterSkinData> skinDataList = GameDB.character.GetSkinDataList(characterCode);
			int childCount = skinContent.childCount;
			int num = Mathf.Max(0, skinDataList.Count - childCount);
			for (int i = 0; i < num; i++)
			{
				Instantiate<CharacterSelectSkinSlot>(characterSelectSkinSlot, skinContent);
			}

			skinCount = skinDataList.Count;
			skinSlots = skinContent.GetComponentsInChildren<CharacterSelectSkinSlot>(true);
			for (int j = 0; j < skinSlots.Length; j++)
			{
				if (j < skinDataList.Count)
				{
					bool flag = Lobby.inst.IsHaveSkin(skinDataList[j].code);
					bool flag2 = skinDataList[j].purchaseType == SkinPurchaseType.SHOP;
					skinSlots[j].gameObject.SetActive(true);
					skinSlots[j].SetSlot(skinDataList[j], j);
					skinSlots[j].SetReleaseableLock(!flag && flag2);
					skinSlots[j].SetButtonInteractable(true);
					skinSlots[j].SetNotReleaseableLock(!flag && !flag2);
					skinSlots[j].selectCallback = SelectSlot;
					if (skinDataList[j].code == skinCode)
					{
						SelectSlot(j);
					}
				}
				else
				{
					skinSlots[j].gameObject.SetActive(false);
				}
			}
		}


		private void SelectSlot(int slotNumber)
		{
			CharacterSkinData characterSkinData = GameDB.character.GetSkinDataList(characterCode)[slotNumber];
			bool flag = Lobby.inst.IsHaveSkin(characterSkinData.code);
			ShopProduct shopSkin = ShopProductService.GetShopSkin(characterSkinData.code);
			if (shopSkin != null)
			{
				isAvailableNP = shopSkin.PriceNP != -1;
				isAvailableACoin = shopSkin.PriceACoin != -1;
				productImage.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(shopSkin.img);
				productName.text = LnUtil.GetSkinName(shopSkin.code);
				skinInfoContent.text = LnUtil.GetSkinDesc(characterSkinData.code);
				txtComment.text = flag ? Ln.Get("보유 중") : Ln.Get("해당 상품을 구매하시겠습니까?");
				txtComment.color = flag ? lackColor : commentColor;
				SetSkinProductObject(flag);
				SetPriceBtnSkin(shopSkin.PriceNP, shopSkin.PriceACoin);
			}
			else
			{
				productImage.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterFullSprite(characterSkinData.characterCode,
						characterSkinData.index);
				productName.text = LnUtil.GetSkinName(characterSkinData.code);
				skinInfoContent.text = LnUtil.GetSkinDesc(characterSkinData.code);
				txtComment.color = lackColor;
				switch (characterSkinData.purchaseType)
				{
					case SkinPurchaseType.FREE:
						txtComment.text = flag ? Ln.Get("보유 중") : Ln.Get("기본으로 지급되는 스킨");
						goto IL_222;
					case SkinPurchaseType.EVENT_REWARD:
						txtComment.text = flag ? Ln.Get("보유 중") : Ln.Get("스킨을 이벤트로 획득 가능");
						goto IL_222;
					case SkinPurchaseType.BATTLE_PASS:
						txtComment.text = flag ? Ln.Get("보유 중") : Ln.Get("스킨을 배틀패스 구매 가능");
						goto IL_222;
				}

				txtComment.text = flag ? Ln.Get("보유 중") : Ln.Get("해당 스킨은 현재 구매할 수 없습니다.");
				IL_222:
				SetSkinProductObject(true);
			}

			if (skinSlots.Length > slotNumber)
			{
				skinSlots[selectSlotIndex].OnSelectSlot(false);
				selectSlotIndex = slotNumber;
				skinSlots[selectSlotIndex].OnSelectSlot(true);
				scrollIndex = slotNumber;
				SetScrollRect(slotNumber);
			}
		}


		private void SetScrollRect(int slotNumber)
		{
			if (slotNumber <= 2)
			{
				scrollIndex = 2;
				skinScrollRect.horizontalNormalizedPosition = 0f;
				skinPreButton.gameObject.SetActive(false);
				skinNextButton.gameObject.SetActive(skinCount > 5);
				return;
			}

			if (skinSlots.Length - 2 <= slotNumber + 1)
			{
				scrollIndex = skinSlots.Length - 2 - 1;
				skinScrollRect.horizontalNormalizedPosition = 1f;
				skinPreButton.gameObject.SetActive(skinCount > 5);
				skinNextButton.gameObject.SetActive(false);
				return;
			}

			skinScrollRect.horizontalNormalizedPosition = (slotNumber - 2) / (skinSlots.Length - 4f - 1f);
			skinPreButton.gameObject.SetActive(skinCount > 5);
			skinNextButton.gameObject.SetActive(skinCount > 5);
		}


		private void OnClickNextSkin()
		{
			scrollIndex++;
			if (scrollIndex > skinSlots.Length)
			{
				scrollIndex = skinSlots.Length - 1;
			}

			SetScrollRect(scrollIndex);
		}


		private void OnClickPreSkin()
		{
			scrollIndex--;
			if (scrollIndex <= 0)
			{
				scrollIndex = 0;
			}

			SetScrollRect(scrollIndex);
		}


		private void OnClickItem(string productId)
		{
			if (MonoBehaviourInstance<LobbyService>.inst.LobbyState != LobbyState.Ready)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중에는 이용할 수 없습니다"), new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인")
				});
				return;
			}

			ShopProduct shopProduct;
			if ((shopProduct = product as ShopProduct) != null)
			{
				if (shopProduct.purchaseType == PurchaseType.IAP)
				{
					MonoBehaviourInstance<Lobby_InAppPurchase>.inst.PurchaseItem(productId);
					return;
				}

				RequestDelegate.request<ProductApi.PurchaseResult>(ProductApi.purchase(productId),
					delegate(RequestDelegateError err, ProductApi.PurchaseResult res)
					{
						if (err != null)
						{
							MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("ServerError/" + err.message), Close,
								new Popup.Button
								{
									text = Ln.Get("확인")
								});
							return;
						}

						for (int i = 0; i < res.itemContainer.items.Count; i++)
						{
							if (res.itemContainer.items[i].type == LobbyApi.ItemType.CHARACTER)
							{
								Lobby.inst.AddCharacter(res.itemContainer.items[i].itemCode);
								BuySuccessCallback buySuccessCallback = this.buySuccessCallback;
								if (buySuccessCallback != null)
								{
									buySuccessCallback(res.itemContainer.items[i].itemCode);
								}
							}
							else if (res.itemContainer.items[i].type == LobbyApi.ItemType.SKIN)
							{
								Lobby.inst.AddSkin(res.itemContainer.items[i].itemCode);
								BuySuccessCallback buySuccessCallback2 = buySuccessCallback;
								if (buySuccessCallback2 != null)
								{
									buySuccessCallback2(res.itemContainer.items[i].itemCode);
								}
							}
						}

						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("구매가 완료되었습니다."), Close, new Popup.Button
						{
							text = Ln.Get("확인")
						});
					});
			}
		}


		private void OnClickRouteSlotItem(string productId)
		{
			if (MonoBehaviourInstance<LobbyService>.inst.LobbyState != LobbyState.Ready)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중에는 이용할 수 없습니다"), new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인")
				});
				return;
			}

			RequestDelegate.request<NullResponse>(ProductApi.BuyWeaponRouteSlot(productId, characterCode),
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("ServerError/" + err.message), Close,
							new Popup.Button
							{
								text = Ln.Get("확인")
							});
						return;
					}

					ShopProduct obj;
					WeaponRouteSlotShop obj2;
					if ((obj = product as ShopProduct) != null)
					{
						BuySuccessRouteSlotCallback buySuccessRouteSlotCallback = this.buySuccessRouteSlotCallback;
						if (buySuccessRouteSlotCallback != null)
						{
							buySuccessRouteSlotCallback(obj);
						}
					}
					else if ((obj2 = product as WeaponRouteSlotShop) != null)
					{
						BuySuccessRouteSlotCallback buySuccessRouteSlotCallback2 = buySuccessRouteSlotCallback;
						if (buySuccessRouteSlotCallback2 != null)
						{
							buySuccessRouteSlotCallback2(obj2);
						}
					}

					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("구매가 완료되었습니다."), Close, new Popup.Button
					{
						text = Ln.Get("확인")
					});
				});
		}


		public void ClickedNP()
		{
			ShopProduct shopProduct;
			WeaponRouteSlotShop weaponRouteSlotShop;
			if ((shopProduct = product as ShopProduct) != null)
			{
				if (shopProduct.shopType == ShopType.NICKNAME_CHANGE)
				{
					OnClickConfirmNickname();
					return;
				}

				if (shopProduct.shopType == ShopType.CHARACTER || shopProduct.shopType == ShopType.SKIN)
				{
					if (Lobby.inst.User.np < shopProduct.PriceNP)
					{
						noMoneyOpenShopCallback();
						Close();
						return;
					}

					OnClickItem(shopProduct.ProductNPId);
				}
				else if (shopProduct.shopType == ShopType.WEAPON_ROUTE)
				{
					if (Lobby.inst.User.np < shopProduct.PriceNP)
					{
						noMoneyOpenShopCallback();
						Close();
						return;
					}

					OnClickRouteSlotItem(shopProduct.ProductNPId);
				}
			}
			else if ((weaponRouteSlotShop = product as WeaponRouteSlotShop) != null)
			{
				if (Lobby.inst.User.np < weaponRouteSlotShop.np)
				{
					noMoneyOpenShopCallback();
					Close();
					return;
				}

				OnClickRouteSlotItem(weaponRouteSlotShop.npProductId);
			}
		}


		public void ClickedACoin()
		{
			ShopProduct shopProduct;
			WeaponRouteSlotShop weaponRouteSlotShop;
			if ((shopProduct = product as ShopProduct) != null)
			{
				if (shopProduct.shopType == ShopType.CHARACTER)
				{
					if (Lobby.inst.User.aCoin < shopProduct.PriceACoin)
					{
						noMoneyOpenShopCallback();
						Close();
						return;
					}

					OnClickItem(shopProduct.ProductACoinId);
				}
				else if (shopProduct.shopType == ShopType.WEAPON_ROUTE)
				{
					if (Lobby.inst.User.aCoin < shopProduct.PriceACoin)
					{
						noMoneyOpenShopCallback();
						Close();
						return;
					}

					OnClickRouteSlotItem(shopProduct.ProductACoinId);
				}
			}
			else if ((weaponRouteSlotShop = product as WeaponRouteSlotShop) != null)
			{
				if (Lobby.inst.User.aCoin < weaponRouteSlotShop.aCoin)
				{
					noMoneyOpenShopCallback();
					Close();
					return;
				}

				OnClickRouteSlotItem(weaponRouteSlotShop.aCoinProductId);
			}
		}


		public void ClickedCash()
		{
			ShopProduct shopProduct = (ShopProduct) product;
			OnClickItem(shopProduct.productId);
		}


		public void ClickedNotice() { }


		public void OnClickShowCharacterInfo()
		{
			ShopProduct shopProduct = (ShopProduct) product;
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnClickShowCharacterInfo(shopProduct.code);
			clickedInfoBtn = true;
			Close();
		}


		public void OnClickShowSkinInfo()
		{
			ShopProduct shopProduct = (ShopProduct) product;
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnClickShowSkinInfo(shopProduct.code);
			clickedInfoBtn = true;
			Close();
		}


		private void OnToggleValueChange(bool isOn)
		{
			noticeAgree = isOn;
			SetHighlightNPBtn(noticeAgree && validNicknameLength);
			SetHighlightACoinBtn(noticeAgree && validNicknameLength);
		}


		protected override void OnClose()
		{
			base.OnClose();
			if (!clickedInfoBtn && MonoBehaviourInstance<LobbyUI>.inst.CurrentTab == LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.PlayLobbySound();
			}

			if (product is ShopProduct)
			{
				Action action = assetShopCloseAction;
				if (action != null)
				{
					action();
				}
			}

			clickedInfoBtn = false;
		}


		private void OnInputValueChange(string value)
		{
			validNicknameLength = StringUtil.ValidateNicknameLength(value);
			SetHighlightNPBtn(noticeAgree && validNicknameLength);
		}


		public void OnClickConfirmNickname()
		{
			if (MonoBehaviourInstance<LobbyService>.inst.LobbyState != LobbyState.Ready)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중 닉네임 변경 불가"), new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인")
				});
			}
			else
			{
				ShopProduct product = (ShopProduct) this.product;
				if (!Lobby.inst.User.HaveFreeNicknameChange && Lobby.inst.User.np < product.PriceNP)
				{
					NoMoneyOpenShopCallback openShopCallback = noMoneyOpenShopCallback;
					if (openShopCallback != null)
					{
						openShopCallback();
					}

					Close();
				}
				else if (!StringUtil.ValidateNicknameLength(nicknameInputField.text))
				{
					MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 길이 에러"));
				}
				else if (!StringUtil.IsVaildStr(nicknameInputField.text))
				{
					MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임에 특수 문자 사용 불가"));
				}
				else if (SingletonMonoBehaviour<SwearWordManager>.inst.IsSwearWordNickName(nicknameInputField.text)
				)
				{
					MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 사용 불가 단어"));
				}
				else if (GameDB.bot.IsBotName(nicknameInputField.text))
				{
					MonoBehaviourInstance<Popup>.inst.Error(Ln.Get(string.Format("{0}",
						ErrorType.UnavailableNickname)));
				}
				else
				{
					string nickname = nicknameInputField.text;
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("{0}(으)로 하시겠습니까?", nickname), new Popup.Button
					{
						type = Popup.ButtonType.Confirm,
						text = Ln.Get("확인"),
						callback = (Action) (() => MonoBehaviourInstance<LobbyService>.inst.PurchaseChangeNickname(
							nickname, (restErrorType, message, res) =>
							{
								if (restErrorType != RestErrorType.SUCCESS)
								{
									MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + message));
								}
								else
								{
									Lobby.inst.User.UseFreeNicknameChange();
									BuySuccessCallback buySuccessCallback = this.buySuccessCallback;
									if (buySuccessCallback != null)
									{
										buySuccessCallback();
									}

									nicknameInputField.text = "";
									MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("닉네임 변경 완료"), new Popup.Button
									{
										type = Popup.ButtonType.Confirm,
										text = Ln.Get("확인"),
										callback = Close
									});
								}
							}))
					}, new Popup.Button
					{
						type = Popup.ButtonType.Cancel,
						text = Ln.Get("취소")
					});
				}
			}

			// co: dotPeek
			// if (MonoBehaviourInstance<LobbyService>.inst.LobbyState != LobbyState.Ready)
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중 닉네임 변경 불가"), new Popup.Button[]
			// 	{
			// 		new Popup.Button
			// 		{
			// 			type = Popup.ButtonType.Confirm,
			// 			text = Ln.Get("확인")
			// 		}
			// 	});
			// 	return;
			// }
			// ShopProduct shopProduct = (ShopProduct)this.product;
			// if (!Lobby.inst.User.HaveFreeNicknameChange && Lobby.inst.User.np < shopProduct.PriceNP)
			// {
			// 	ShopProductWindow.NoMoneyOpenShopCallback noMoneyOpenShopCallback = this.noMoneyOpenShopCallback;
			// 	if (noMoneyOpenShopCallback != null)
			// 	{
			// 		noMoneyOpenShopCallback();
			// 	}
			// 	this.Close();
			// 	return;
			// }
			// if (!StringUtil.ValidateNicknameLength(this.nicknameInputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 길이 에러"), null);
			// 	return;
			// }
			// if (!StringUtil.IsVaildStr(this.nicknameInputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임에 특수 문자 사용 불가"), null);
			// 	return;
			// }
			// if (SingletonMonoBehaviour<SwearWordManager>.inst.IsSwearWordNickName(this.nicknameInputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("닉네임 사용 불가 단어"), null);
			// 	return;
			// }
			// if (GameDB.bot.IsBotName(this.nicknameInputField.text))
			// {
			// 	MonoBehaviourInstance<Popup>.inst.Error(Ln.Get(string.Format("{0}", ErrorType.UnavailableNickname)), null);
			// 	return;
			// }
			// string nickname = this.nicknameInputField.text;
			// Action<RestErrorType, string, string> <>9__1;
			// MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("{0}(으)로 하시겠습니까?", nickname), new Popup.Button[]
			// {
			// 	new Popup.Button
			// 	{
			// 		type = Popup.ButtonType.Confirm,
			// 		text = Ln.Get("확인"),
			// 		callback = delegate()
			// 		{
			// 			LobbyService inst = MonoBehaviourInstance<LobbyService>.inst;
			// 			string nickname = nickname;
			// 			Action<RestErrorType, string, string> callback;
			// 			if ((callback = <>9__1) == null)
			// 			{
			// 				callback = (<>9__1 = delegate(RestErrorType restErrorType, string message, string res)
			// 				{
			// 					if (restErrorType != RestErrorType.SUCCESS)
			// 					{
			// 						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + message), null);
			// 						return;
			// 					}
			// 					Lobby.inst.User.UseFreeNicknameChange();
			// 					ShopProductWindow.BuySuccessCallback buySuccessCallback = this.buySuccessCallback;
			// 					if (buySuccessCallback != null)
			// 					{
			// 						buySuccessCallback(null);
			// 					}
			// 					this.nicknameInputField.text = "";
			// 					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("닉네임 변경 완료"), new Popup.Button[]
			// 					{
			// 						new Popup.Button
			// 						{
			// 							type = Popup.ButtonType.Confirm,
			// 							text = Ln.Get("확인"),
			// 							callback = new Action(this.Close)
			// 						}
			// 					});
			// 				});
			// 			}
			// 			inst.PurchaseChangeNickname(nickname, callback);
			// 		}
			// 	},
			// 	new Popup.Button
			// 	{
			// 		type = Popup.ButtonType.Cancel,
			// 		text = Ln.Get("취소")
			// 	}
			// });
		}


		private void OnClickClearNickname()
		{
			nicknameInputField.text = "";
		}
	}
}