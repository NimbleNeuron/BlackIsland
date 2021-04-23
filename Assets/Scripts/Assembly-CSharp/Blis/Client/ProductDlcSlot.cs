using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ProductDlcSlot : BaseControl
	{
		private Button getButton;


		private string productId;


		private Text productInfo;


		private Text title;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			title = GameUtil.Bind<Text>(gameObject, "ProductTitle");
			productInfo = GameUtil.Bind<Text>(gameObject, "ProductInfo");
			getButton = GameUtil.Bind<Button>(gameObject, "BtnGet/Bg");
			getButton.onClick.AddListener(delegate { OnClickGetButton(); });
		}


		public void SetSlot(PurchasedDLC product)
		{
			if (title == null)
			{
				title = GameUtil.Bind<Text>(gameObject, "ProductTitle");
			}

			if (productInfo == null)
			{
				productInfo = GameUtil.Bind<Text>(gameObject, "ProductInfo");
			}

			if (product == null)
			{
				productId = string.Empty;
				title.text = string.Empty;
				productInfo.text = string.Empty;
				return;
			}

			productId = product.productId;
			title.text = Ln.Get(product.title);
			productInfo.text = Ln.Get(product.contents);
		}


		private void OnClickGetButton()
		{
			ShopProductService.RequestDlcReward(productId);
		}
	}
}