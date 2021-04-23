using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class ProductCharacterData
	{
		
		[JsonConstructor]
		public ProductCharacterData(string productId, string iapProductId, PurchaseMethod purchaseMethod, int price, int amount, string img, string searchName, int characterCode)
		{
			this.productId = productId;
			this.iapProductId = iapProductId;
			this.purchaseMethod = purchaseMethod;
			this.price = price;
			this.amount = amount;
			this.img = img;
			this.searchName = searchName;
			this.characterCode = characterCode;
			this.product = new Product(productId, iapProductId, GoodsType.CHARACTER, characterCode.ToString(), purchaseMethod, price, amount, PurchaseCondition.ONLY_ONCE);
		}

		
		public readonly string productId;

		
		public readonly string iapProductId;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly PurchaseMethod purchaseMethod;

		
		public readonly int price;

		
		public readonly int amount;

		
		public readonly string img;

		
		public readonly string searchName;

		
		public readonly int characterCode;

		
		public readonly Product product;
	}
}
