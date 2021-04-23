using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ProductCharacterSlot : BaseControl
	{
		public delegate void BuySuccessCallback(int code);


		public delegate void NoMoneyOpenShopCallback();


		private const float focusScale = 1f;


		private const float defaultScale = 0.96f;


		[SerializeField] private GameObject hover_Focus = default;


		[SerializeField] private GameObject hover_BuyEnableLock = default;


		[SerializeField] private GameObject hover_BuyImpossibleLock = default;


		[SerializeField] private GameObject obj_Discount = default;


		[SerializeField] private Text txt_DiscountValue = default;


		[SerializeField] private Image img = default;


		[SerializeField] private Text txt_Name = default;


		[SerializeField] private Text txt_NP = default;


		[SerializeField] private Text txt_ACOIN = default;


		[SerializeField] private Text txt_State = default;


		private readonly Color colorHolding = new Color(0.988f, 0.686f, 0.353f, 1f);


		private readonly Color colorLack = new Color(1f, 0.243f, 0.243f, 1f);


		public BuySuccessCallback buySuccessCallback;


		private bool isHaveCharacter;


		public NoMoneyOpenShopCallback noMoneyOpenShopCallback;


		private ScrollRect scrollRect;


		private ShopProduct shopCharacterData;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			scrollRect = GameUtil.Bind<ScrollRect>(MonoBehaviourInstance<LobbyUI>.inst.gameObject,
				"LobbyHUD/ShopTab/ShopCharacter/ProductScrollView");
			OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnBeginDrag(eventData);
			};
			OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnEndDrag(eventData);
			};
			OnDragEvent += delegate(BaseControl control, PointerEventData eventData) { scrollRect.OnDrag(eventData); };
			OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				scrollRect.OnScroll(eventData);
			};
		}


		public void SetSlot(ShopProduct data, int discount)
		{
			shopCharacterData = data;
			isHaveCharacter = Lobby.inst.IsHaveCharacter(data.code);
			txt_NP.text = StringUtil.AssetToString(data.PriceNP);
			txt_ACOIN.text = StringUtil.AssetToString(data.PriceACoin);
			SetCharacterName();
			SetDiscount(discount);
			SetHoverState(false);
			SetSprite();
		}


		public void SetCharacterName()
		{
			txt_Name.text = Ln.Get(LnType.Character_Name, shopCharacterData.code.ToString());
		}


		private void SetDiscount(int discount)
		{
			if (discount <= 0)
			{
				obj_Discount.SetActive(false);
				return;
			}

			obj_Discount.SetActive(true);
			txt_DiscountValue.text = string.Format("-{0}%", discount);
		}


		private void SetSprite()
		{
			img.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(shopCharacterData.img);
		}


		private void SetHoverState(bool hoverEnter)
		{
			if (hoverEnter)
			{
				if (isHaveCharacter)
				{
					hover_Focus.SetActive(true);
					hover_BuyEnableLock.SetActive(false);
					hover_BuyImpossibleLock.SetActive(false);
					SetStateTextHolding();
					return;
				}

				if (shopCharacterData.purchaseType == PurchaseType.TUTORIAL)
				{
					hover_Focus.SetActive(false);
					hover_BuyEnableLock.SetActive(false);
					hover_BuyImpossibleLock.SetActive(true);
					SetStateTextTutorial();
					return;
				}

				if (Lobby.inst.User.np >= shopCharacterData.PriceNP ||
				    Lobby.inst.User.aCoin >= shopCharacterData.PriceACoin)
				{
					hover_Focus.SetActive(true);
					hover_BuyEnableLock.SetActive(true);
					hover_BuyImpossibleLock.SetActive(false);
					SetStateTextAvailable();
					return;
				}

				hover_Focus.SetActive(false);
				hover_BuyEnableLock.SetActive(false);
				hover_BuyImpossibleLock.SetActive(true);
				SetStateTextLack();
			}
			else
			{
				hover_Focus.SetActive(false);
				hover_BuyEnableLock.SetActive(false);
				hover_BuyImpossibleLock.SetActive(false);
				if (isHaveCharacter)
				{
					SetStateTextHolding();
					return;
				}

				if (shopCharacterData.purchaseType == PurchaseType.TUTORIAL)
				{
					SetStateTextTutorial();
					return;
				}

				SetStateTextAvailable();
			}
		}


		private void SetStateTextAvailable()
		{
			txt_State.gameObject.SetActive(false);
			txt_NP.gameObject.SetActive(true);
			txt_ACOIN.gameObject.SetActive(true);
		}


		private void SetStateTextLack()
		{
			txt_State.gameObject.SetActive(true);
			txt_State.text = Ln.Get("재화 부족");
			txt_State.color = colorLack;
			txt_NP.gameObject.SetActive(false);
			txt_ACOIN.gameObject.SetActive(false);
		}


		private void SetStateTextHolding()
		{
			txt_State.gameObject.SetActive(true);
			txt_State.text = Ln.Get("보유 중");
			txt_State.color = colorHolding;
			txt_NP.gameObject.SetActive(false);
			txt_ACOIN.gameObject.SetActive(false);
		}


		private void SetStateTextTutorial()
		{
			txt_State.gameObject.SetActive(true);
			txt_State.text = Ln.Get("튜토리얼 보상");
			txt_State.color = colorHolding;
			txt_NP.gameObject.SetActive(false);
			txt_ACOIN.gameObject.SetActive(false);
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (isHaveCharacter)
			{
				return;
			}

			if (shopCharacterData.purchaseType == PurchaseType.TUTORIAL)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("튜토리얼 보상으로 획득가능합니다."), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Open();
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.SetProduct(shopCharacterData);
			ShopProductWindow shopProductWindow = MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow;
			shopProductWindow.buySuccessCallback = (ShopProductWindow.BuySuccessCallback) Delegate.Combine(
				shopProductWindow.buySuccessCallback,
				new ShopProductWindow.BuySuccessCallback(delegate(object code) { buySuccessCallback((int) code); }));
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.noMoneyOpenShopCallback = delegate
			{
				noMoneyOpenShopCallback();
			};
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			transform.localScale = new Vector3(1f, 1f, 1f);
			SetHoverState(true);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
			SetHoverState(false);
		}
	}
}