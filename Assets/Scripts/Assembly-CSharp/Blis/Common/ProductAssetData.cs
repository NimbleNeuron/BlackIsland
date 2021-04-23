using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ProductAssetData
	{
		public readonly int amount;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly AssetType assetType;


		public readonly string iapProductId;


		public readonly string img;


		public readonly int price;


		public readonly Product product;


		public readonly string productId;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly PurchaseCondition PurchaseCondition;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly PurchaseMethod purchaseMethod;

		[JsonConstructor]
		public ProductAssetData(string productId, string iapProductId, PurchaseMethod purchaseMethod, int price,
			int amount, AssetType assetType, string img, PurchaseCondition purchaseCondition)
		{
			this.productId = productId;
			this.iapProductId = iapProductId;
			this.purchaseMethod = purchaseMethod;
			this.price = price;
			this.amount = amount;
			this.assetType = assetType;
			this.img = img;
			PurchaseCondition = purchaseCondition;
			product = new Product(productId, iapProductId, GoodsType.ASSET, assetType.ToString(), purchaseMethod, price,
				amount, purchaseCondition);
		}
	}
}