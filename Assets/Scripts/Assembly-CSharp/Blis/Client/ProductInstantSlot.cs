using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ProductInstantSlot : BaseControl
	{
		private const float focusScale = 1f;


		private const float defaultScale = 0.96f;


		private readonly Color colorHolding = new Color(0.988f, 0.686f, 0.353f, 1f);


		private readonly Color colorLack = new Color(1f, 0.243f, 0.243f, 1f);


		private GameObject discount;


		private LnText discountValue;


		private LnText free;


		private GameObject hoverBuyEnableLock;


		private GameObject hoverBuyImpossibleLock;


		private GameObject hoverFocus;


		private bool isInitialized;


		private LnText lackOfAsset;


		private LnText np;


		private Image productImg;


		private ShopProduct productInstantData;


		private LnText productName;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Init();
		}


		public void Init()
		{
			if (isInitialized)
			{
				return;
			}

			isInitialized = true;
			hoverFocus = transform.FindRecursively("Hover_Focus").gameObject;
			hoverBuyEnableLock = transform.FindRecursively("Hover_BuyEnableLock").gameObject;
			hoverBuyImpossibleLock = transform.FindRecursively("Hover_BuyImpossibleLock").gameObject;
			discount = transform.FindRecursively("Discount").gameObject;
			productImg = GameUtil.Bind<Image>(gameObject, "Mask/Image");
			discountValue = GameUtil.Bind<LnText>(gameObject, "Discount/TXT_Discount");
			productName = GameUtil.Bind<LnText>(gameObject, "ProductName/TXT_Name");
			np = GameUtil.Bind<LnText>(gameObject, "Value/TXT_NP");
			lackOfAsset = GameUtil.Bind<LnText>(gameObject, "TXT_State");
			free = GameUtil.Bind<LnText>(gameObject, "TXT_Free");
			np.transform.localScale = Vector3.zero;
			lackOfAsset.transform.localScale = Vector3.zero;
			free.transform.localScale = Vector3.zero;
		}


		public void SetSlot(ShopProduct data)
		{
			if (data == null)
			{
				return;
			}

			productInstantData = data;
			np.text = StringUtil.AssetToString(data.price);
			productName.text = Ln.Get("Product/Name/" + data.productId);
			productImg.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(data.img);
			RefreshInfo();
		}


		public void RefreshInfo()
		{
			DisableHoverState();
			if (Lobby.inst.User.HaveFreeNicknameChange)
			{
				np.transform.localScale = Vector3.zero;
				lackOfAsset.transform.localScale = Vector3.zero;
				free.transform.localScale = Vector3.one;
				return;
			}

			np.transform.localScale = Vector3.one;
			lackOfAsset.transform.localScale = Vector3.zero;
			free.transform.localScale = Vector3.zero;
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Open();
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.SetNicknameChangeProduct(productInstantData);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			transform.localScale = new Vector3(1f, 1f, 1f);
			EnableHoverState();
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
			DisableHoverState();
		}


		private void EnableHoverState()
		{
			hoverFocus.transform.localScale = Vector3.one;
			if (!Lobby.inst.User.HaveFreeNicknameChange && Lobby.inst.User.np < productInstantData.price)
			{
				hoverBuyImpossibleLock.transform.localScale = Vector3.one;
				np.transform.localScale = Vector3.zero;
				lackOfAsset.transform.localScale = Vector3.one;
				free.transform.localScale = Vector3.zero;
			}
		}


		private void DisableHoverState()
		{
			hoverFocus.transform.localScale = Vector3.zero;
			hoverBuyImpossibleLock.transform.localScale = Vector3.zero;
			lackOfAsset.transform.localScale = Vector3.zero;
			if (Lobby.inst.User.HaveFreeNicknameChange)
			{
				free.transform.localScale = Vector3.one;
				np.transform.localScale = Vector3.zero;
				return;
			}

			free.transform.localScale = Vector3.zero;
			np.transform.localScale = Vector3.one;
		}
	}
}