using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TwitchDropsSlot : BaseControl
	{
		private Button getButton;


		private string productId;


		private Text productInfo;


		private string rewardId;


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
			productId = product.productId;
			rewardId = product.rewardId;
			title.text = Ln.Get(product.title);
			productInfo.text = Ln.Get(product.contents);
		}


		private void OnClickGetButton()
		{
			ShopTwitchDropsService.RequestDropsReward(productId, rewardId);
		}
	}
}