using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ProductMoneySlot : BaseControl
	{
		private const float focusScale = 1f;


		private const float defaultScale = 0.96f;


		[SerializeField] private GameObject hover_Focus = default;


		[SerializeField] private GameObject obj_Discount = default;


		[SerializeField] private Text txt_DiscountValue = default;


		[SerializeField] private Image img = default;


		[SerializeField] private Text txt_Name = default;


		[SerializeField] private Text txt_Price = default;


		private ShopProduct assetData = default;

		public void SetSlot(ShopProduct product, int discount)
		{
			assetData = product;
			txt_Name.text = Ln.Get(LnType.Product_Name, assetData.productId);
			txt_Price.text = StringUtil.GetLocalizedPrice(Lobby.inst.User.Currency, assetData.price);
			SetDiscount(discount);
			SetSprite(assetData.img);
			hover_Focus.SetActive(false);
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


		private void SetSprite(string sName)
		{
			img.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(sName);
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Open();
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.SetProduct(assetData);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			hover_Focus.SetActive(true);
			transform.localScale = new Vector3(1f, 1f, 1f);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			hover_Focus.SetActive(false);
			transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
		}
	}
}