namespace Blis.Common
{
	public class Product
	{
		public readonly int amount;


		public readonly Goods goods;


		public readonly string iapProductId;


		public readonly int price;


		public readonly string productId;


		public readonly PurchaseCondition purchaseCondition;


		public readonly PurchaseMethod purchaseMethod;


		public Product(string productId, string iapProductId, GoodsType goodsType, string subType,
			PurchaseMethod purchaseMethod, int price, int amount, PurchaseCondition purchaseCondition)
		{
			this.productId = productId;
			this.iapProductId = iapProductId;
			this.purchaseMethod = purchaseMethod;
			this.price = price;
			this.amount = amount;
			this.purchaseCondition = purchaseCondition;
			goods = new Goods(goodsType, subType, amount);
		}
	}
}