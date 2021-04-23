using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ProductInstantData
	{
		public readonly int amount;


		public readonly string iapProductId;


		public readonly string img;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly InstantType instantType;


		public readonly int price;


		public readonly Product product;


		public readonly string productId;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly PurchaseMethod purchaseMethod;

		[JsonConstructor]
		public ProductInstantData(string productId, string iapProductId, PurchaseMethod purchaseMethod, int price,
			int amount, InstantType instantType, string img)
		{
			this.productId = productId;
			this.iapProductId = iapProductId;
			this.purchaseMethod = purchaseMethod;
			this.price = price;
			this.amount = amount;
			this.instantType = instantType;
			this.img = img;
			product = new Product(productId, iapProductId, GoodsType.INSTANT, instantType.ToString(), purchaseMethod,
				price, amount, PurchaseCondition.NONE);
		}
	}
}